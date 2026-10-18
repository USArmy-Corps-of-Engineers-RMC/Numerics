"""Copy arrays between NumPy and .NET in bulk, preserving every bit.

Each non-empty transfer is one ``System.Runtime.InteropServices.Marshal.Copy`` call
over a contiguous NumPy buffer, never element-by-element indexing, so values keep their
exact bit patterns (NaN payloads, signed zeros, infinities and subnormals included) and
large arrays copy at memory speed.

Two-dimensional ``double[,]`` arrays, which Numerics returns from samplers such as
``LatinHypercube.Random`` and takes as covariance matrices, cross through a flat
``double[]`` and ``System.Buffer.BlockCopy``: a .NET rectangular array is stored in row
major order, the same layout as a C-order NumPy array, so element ``[i, j]`` maps
exactly.

Space-time blocks cross as a flat ``System.Single[]`` in C order together with explicit
``(steps, rows, columns)`` dimensions: ``flat[(t * rows + r) * columns + c]`` equals
``block[t, r, c]``.
"""

from __future__ import annotations

import math
from typing import Any

import numpy as np

from rmc_numerics.runtime import initialize

_INT32_MIN = -(2**31)
_INT32_MAX = 2**31 - 1


def _clr() -> tuple[Any, Any]:
    """Return the (System, Marshal) handles after guaranteeing runtime initialization."""
    initialize()
    import System  # noqa: PLC0415 - resolvable only after runtime initialization
    from System.Runtime.InteropServices import Marshal  # noqa: PLC0415

    return System, Marshal


def to_net_doubles(values: Any) -> Any:
    """Copy a one-dimensional array into a new ``System.Double[]``, preserving every bit.

    The input is converted with ``numpy.ascontiguousarray(values, dtype=numpy.float64)``
    first. That conversion is exact for float32 values and for integers up to 2**53 in
    magnitude, and it turns a scalar into a one-element array. Starts the .NET runtime if
    it is not already running.

    Parameters
    ----------
    values : array_like
        Values to copy, one-dimensional after conversion.

    Returns
    -------
    System.Double[]
        A new .NET array holding the float64 values.

    Raises
    ------
    ValueError
        If ``values`` is not one-dimensional after conversion.
    """
    System, Marshal = _clr()
    array = np.ascontiguousarray(values, dtype=np.float64)
    if array.ndim == 0:
        array = array.reshape(1)
    if array.ndim != 1:
        raise ValueError(f"Expected a 1-D array; got shape {array.shape}.")
    target = System.Array.CreateInstance(System.Double, array.size)
    if array.size:
        Marshal.Copy(System.IntPtr(array.ctypes.data), target, 0, array.size)
    return target


def to_numpy_doubles(values: Any) -> np.ndarray:
    """Copy a ``System.Double[]`` into a new float64 NumPy array, preserving every bit.

    The result is always a copy, so it never aliases .NET state.

    Parameters
    ----------
    values : System.Double[]
        .NET array to copy.

    Returns
    -------
    numpy.ndarray
        A new one-dimensional float64 array with ``values.Length`` elements.
    """
    System, Marshal = _clr()
    length = int(values.Length)
    out = np.empty(length, dtype=np.float64)
    if length:
        Marshal.Copy(values, 0, System.IntPtr(out.ctypes.data), length)
    return out


def to_net_matrix(values: Any) -> Any:
    """Copy a two-dimensional array into a new ``System.Double[,]``, preserving every bit.

    Parameters
    ----------
    values : array_like
        Values to copy, two-dimensional after conversion to C-order float64.

    Returns
    -------
    System.Double[,]
        A new rectangular .NET array with the same shape.

    Raises
    ------
    ValueError
        If ``values`` is not two-dimensional after conversion.
    """
    System, _ = _clr()
    array = np.ascontiguousarray(values, dtype=np.float64)
    if array.ndim != 2:
        raise ValueError(f"Expected a 2-D array; got shape {array.shape}.")
    rows, cols = (int(dimension) for dimension in array.shape)
    target = System.Array.CreateInstance(System.Double, rows, cols)
    if array.size:
        flat = to_net_doubles(array.ravel())
        System.Buffer.BlockCopy(flat, 0, target, 0, array.size * 8)
    return target


def to_numpy_matrix(values: Any) -> np.ndarray:
    """Copy a ``System.Double[,]`` into a new two-dimensional float64 array, preserving every bit.

    Parameters
    ----------
    values : System.Double[,]
        Rectangular .NET array to copy.

    Returns
    -------
    numpy.ndarray
        A new C-order float64 array of shape ``(values.GetLength(0), values.GetLength(1))``.

    Raises
    ------
    ValueError
        If ``values`` is not a rank-2 array.
    """
    System, _ = _clr()
    if int(values.Rank) != 2:
        raise ValueError(f"Expected a rank-2 .NET array; got rank {int(values.Rank)}.")
    rows, cols = int(values.GetLength(0)), int(values.GetLength(1))
    size = rows * cols
    if size == 0:
        return np.empty((rows, cols), dtype=np.float64)
    flat = System.Array.CreateInstance(System.Double, size)
    System.Buffer.BlockCopy(values, 0, flat, 0, size * 8)
    return to_numpy_doubles(flat).reshape((rows, cols))


def _int32_range_error(low: object, high: object) -> OverflowError:
    """Return the error for values from ``low`` to ``high`` that leave the int32 range."""
    return OverflowError(f"Values must lie in the Int32 range [{_INT32_MIN}, {_INT32_MAX}]; got values from {low} to "
                         f"{high}.")


def _int32_array(values: Any) -> np.ndarray:
    """Convert values to a contiguous int32 array, refusing any value outside the int32 range."""
    source = np.asarray(values)
    kind = source.dtype.kind
    if kind == "O":
        items = source.ravel().tolist()
        if not all(isinstance(item, (int, np.integer)) for item in items):
            raise TypeError("Values must be booleans, integers, or floating-point numbers to cross as Int32; got an "
                            "object array holding another type.")
        if items and (min(items) < _INT32_MIN or max(items) > _INT32_MAX):
            raise _int32_range_error(min(items), max(items))
        return np.ascontiguousarray(np.array(items, dtype=np.int64).reshape(source.shape), dtype=np.int32)
    if kind not in "biuf":
        raise TypeError(f"Values must be booleans, integers, or floating-point numbers to cross as Int32; got dtype "
                        f"{source.dtype}.")
    if kind in "iuf" and source.size:
        if kind == "f":
            truncated = np.trunc(source)
            low, high = float(truncated.min()), float(truncated.max())
            if not (math.isfinite(low) and math.isfinite(high)):
                raise OverflowError(f"Values must lie in the Int32 range [{_INT32_MIN}, {_INT32_MAX}]; "
                                    "got a value that is not finite.")
        else:
            low, high = int(source.min()), int(source.max())
        if low < _INT32_MIN or high > _INT32_MAX:
            raise _int32_range_error(low, high)
    return np.ascontiguousarray(source, dtype=np.int32)


def to_net_ints(values: Any) -> Any:
    """Copy a one-dimensional array into a new ``System.Int32[]``, preserving every bit.

    A value outside the int32 range raises ``OverflowError`` instead of wrapping,
    whether it arrives as a Python integer, a wider NumPy integer, or a floating-point
    value. Floating-point values inside the range truncate toward zero. Booleans cross
    as 0 and 1; complex, date, time, text and other values are refused.

    Parameters
    ----------
    values : array_like
        Integer values to copy, one-dimensional after conversion.

    Returns
    -------
    System.Int32[]
        A new .NET array holding the int32 values.

    Raises
    ------
    OverflowError
        If a value lies outside the int32 range or is not finite; raised before the
        runtime starts.
    TypeError
        If the values are not booleans, integers, or floating-point numbers.
    ValueError
        If ``values`` is not one-dimensional after conversion.
    """
    array = _int32_array(values)
    System, Marshal = _clr()
    if array.ndim != 1:
        raise ValueError(f"Expected a 1-D array; got shape {array.shape}.")
    target = System.Array.CreateInstance(System.Int32, array.size)
    if array.size:
        Marshal.Copy(System.IntPtr(array.ctypes.data), target, 0, array.size)
    return target


def to_numpy_ints(values: Any) -> np.ndarray:
    """Copy a ``System.Int32[]`` into a new int32 NumPy array, preserving every bit.

    Parameters
    ----------
    values : System.Int32[]
        .NET array to copy.

    Returns
    -------
    numpy.ndarray
        A new one-dimensional int32 array with ``values.Length`` elements.
    """
    System, Marshal = _clr()
    length = int(values.Length)
    out = np.empty(length, dtype=np.int32)
    if length:
        Marshal.Copy(values, 0, System.IntPtr(out.ctypes.data), length)
    return out


def to_net_singles(block: Any) -> tuple[Any, tuple[int, int, int]]:
    """Flatten a ``(steps, rows, columns)`` block into a ``System.Single[]`` and its shape.

    The block is converted with ``numpy.ascontiguousarray(block, dtype=numpy.float32)``
    first, which rounds float64 values to the nearest float32, and is then copied in C
    order, so ``flat[(t * rows + r) * columns + c]`` equals ``block[t, r, c]``.

    Parameters
    ----------
    block : array_like
        Block with shape ``(steps, rows, columns)``.

    Returns
    -------
    flat : System.Single[]
        A new .NET array holding the ``steps * rows * columns`` float32 values.
    dims : tuple of int
        The block shape ``(steps, rows, columns)``.

    Raises
    ------
    ValueError
        If ``block`` is not three-dimensional after conversion.
    """
    System, Marshal = _clr()
    array = np.ascontiguousarray(block, dtype=np.float32)
    if array.ndim != 3:
        raise ValueError(f"Expected a (steps, rows, cols) block; got shape {array.shape}.")
    steps, rows, cols = (int(dimension) for dimension in array.shape)
    target = System.Array.CreateInstance(System.Single, array.size)
    if array.size:
        Marshal.Copy(System.IntPtr(array.ctypes.data), target, 0, array.size)
    return target, (steps, rows, cols)


def to_numpy_singles(values: Any, dims: tuple[int, int, int]) -> np.ndarray:
    """Copy a flat ``System.Single[]`` back into a ``(steps, rows, columns)`` float32 block.

    Parameters
    ----------
    values : System.Single[]
        Flat C-order .NET array, as produced by :func:`to_net_singles`.
    dims : tuple of int
        Block shape ``(steps, rows, columns)``.

    Returns
    -------
    numpy.ndarray
        A new float32 array with shape ``dims``, preserving every bit.

    Raises
    ------
    ValueError
        If ``values.Length`` differs from ``steps * rows * columns``.
    """
    System, Marshal = _clr()
    steps, rows, cols = (int(dimension) for dimension in dims)
    length = int(values.Length)
    if length != steps * rows * cols:
        raise ValueError(f"Flat length {length} does not match dims {dims}.")
    out = np.empty(length, dtype=np.float32)
    if length:
        Marshal.Copy(values, 0, System.IntPtr(out.ctypes.data), length)
    return out.reshape((steps, rows, cols))
