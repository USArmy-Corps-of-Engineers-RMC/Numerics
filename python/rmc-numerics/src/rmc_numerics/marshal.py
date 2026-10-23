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

``System.SByte[]`` and ``System.Boolean[]`` have no ``Marshal.Copy`` overload; they cross as
one byte copy into a ``System.Byte[]`` followed by one ``System.Buffer.BlockCopy``, which
reinterprets the bytes without conversion (a .NET ``Boolean`` occupies one byte, as a NumPy
``bool`` does).
"""

from __future__ import annotations

import math
from typing import Any

import numpy as np

from rmc_numerics.runtime import initialize

_INT32_MIN = -(2**31)
_INT32_MAX = 2**31 - 1
_INT64_MIN = -(2**63)
_INT64_MAX = 2**63 - 1
_INT8_MIN = -(2**7)
_INT8_MAX = 2**7 - 1


def _clr() -> tuple[Any, Any]:
    """Return the (System, Marshal) handles after guaranteeing runtime initialization."""
    initialize()
    import System  # noqa: PLC0415 - resolvable only after runtime initialization
    from System.Runtime.InteropServices import Marshal  # noqa: PLC0415

    return System, Marshal


_BOUND: dict[str, Any] = {}


def _bound() -> dict[str, Any]:
    """Return the ``Marshal.Copy`` overloads and array constructors, resolved once per process.

    pythonnet resolves an overloaded method group on every call; ``Marshal.Copy`` has more than a
    dozen overloads, and resolving them costs about twenty times the copy of a short array. Binding
    the exact overloads once keeps a small transfer in the microseconds; the copies are unchanged.
    """
    if not _BOUND:
        System, Marshal = _clr()
        pointer, length = System.IntPtr, System.Int32
        doubles, ints, singles = System.Array[System.Double], System.Array[System.Int32], System.Array[System.Single]
        longs, raw = System.Array[System.Int64], System.Array[System.Byte]
        _BOUND.update({
            "doubles": doubles, "ints": ints, "singles": singles, "pointer": pointer,
            "longs": longs, "bytes": raw, "sbytes": System.Array[System.SByte], "bools": System.Array[System.Boolean],
            "block_copy": System.Buffer.BlockCopy,
            "doubles_in": Marshal.Copy.Overloads[pointer, doubles, length, length],
            "doubles_out": Marshal.Copy.Overloads[doubles, length, pointer, length],
            "ints_in": Marshal.Copy.Overloads[pointer, ints, length, length],
            "ints_out": Marshal.Copy.Overloads[ints, length, pointer, length],
            "singles_in": Marshal.Copy.Overloads[pointer, singles, length, length],
            "singles_out": Marshal.Copy.Overloads[singles, length, pointer, length],
            "longs_in": Marshal.Copy.Overloads[pointer, longs, length, length],
            "longs_out": Marshal.Copy.Overloads[longs, length, pointer, length],
            "bytes_in": Marshal.Copy.Overloads[pointer, raw, length, length],
            "bytes_out": Marshal.Copy.Overloads[raw, length, pointer, length],
        })
    return _BOUND


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
    bound = _bound()
    array = np.ascontiguousarray(values, dtype=np.float64)
    if array.ndim == 0:
        array = array.reshape(1)
    if array.ndim != 1:
        raise ValueError(f"Expected a 1-D array; got shape {array.shape}.")
    target = bound["doubles"](array.size)
    if array.size:
        bound["doubles_in"](bound["pointer"](array.ctypes.data), target, 0, array.size)
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
    bound = _bound()
    length = int(values.Length)
    out = np.empty(length, dtype=np.float64)
    if length:
        bound["doubles_out"](values, 0, bound["pointer"](out.ctypes.data), length)
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


def _range_error(label: str, minimum: int, maximum: int, low: object, high: object) -> OverflowError:
    """Return the error for values from ``low`` to ``high`` that leave the ``label`` range."""
    return OverflowError(f"Values must lie in the {label} range [{minimum}, {maximum}]; got values from {low} to "
                         f"{high}.")


def _checked_integers(values: Any, dtype: type, label: str, minimum: int, maximum: int) -> np.ndarray:
    """Convert values to a contiguous integer array of ``dtype``, refusing any value outside its range.

    Booleans cross as 0 and 1 and floating-point values truncate toward zero; a value outside
    ``[minimum, maximum]``, or a floating-point value that is not finite, raises ``OverflowError``
    instead of wrapping. Complex, date, time, text and other values raise ``TypeError``.
    """
    source = np.asarray(values)
    kind = source.dtype.kind
    if kind == "O":
        items = source.ravel().tolist()
        if not all(isinstance(item, (int, np.integer)) for item in items):
            raise TypeError(f"Values must be booleans, integers, or floating-point numbers to cross as {label}; got "
                            "an object array holding another type.")
        if items and (min(items) < minimum or max(items) > maximum):
            raise _range_error(label, minimum, maximum, min(items), max(items))
        return np.ascontiguousarray(np.array(items, dtype=np.int64).reshape(source.shape), dtype=dtype)
    if kind not in "biuf":
        raise TypeError(f"Values must be booleans, integers, or floating-point numbers to cross as {label}; got dtype "
                        f"{source.dtype}.")
    if kind in "iuf" and source.size:
        if kind == "f":
            truncated = np.trunc(source)
            low, high = float(truncated.min()), float(truncated.max())
            if not (math.isfinite(low) and math.isfinite(high)):
                raise OverflowError(f"Values must lie in the {label} range [{minimum}, {maximum}]; "
                                    "got a value that is not finite.")
        else:
            low, high = int(source.min()), int(source.max())
        if low < minimum or high > maximum:
            raise _range_error(label, minimum, maximum, low, high)
    return np.ascontiguousarray(source, dtype=dtype)


def _int32_array(values: Any) -> np.ndarray:
    """Convert values to a contiguous int32 array, refusing any value outside the int32 range."""
    return _checked_integers(values, np.int32, "Int32", _INT32_MIN, _INT32_MAX)


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
    bound = _bound()
    if array.ndim != 1:
        raise ValueError(f"Expected a 1-D array; got shape {array.shape}.")
    target = bound["ints"](array.size)
    if array.size:
        bound["ints_in"](bound["pointer"](array.ctypes.data), target, 0, array.size)
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
    bound = _bound()
    length = int(values.Length)
    out = np.empty(length, dtype=np.int32)
    if length:
        bound["ints_out"](values, 0, bound["pointer"](out.ctypes.data), length)
    return out


def to_net_longs(values: Any) -> Any:
    """Copy a one-dimensional array into a new ``System.Int64[]``, preserving every bit.

    A value outside the int64 range raises ``OverflowError`` instead of wrapping, whether
    it arrives as a Python integer, an unsigned 64-bit NumPy integer, or a floating-point
    value. Floating-point values inside the range truncate toward zero. Booleans cross as
    0 and 1; complex, date, time, text and other values are refused.

    Parameters
    ----------
    values : array_like
        Integer values to copy, one-dimensional after conversion.

    Returns
    -------
    System.Int64[]
        A new .NET array holding the int64 values.

    Raises
    ------
    OverflowError
        If a value lies outside the int64 range or is not finite; raised before the
        runtime starts.
    TypeError
        If the values are not booleans, integers, or floating-point numbers.
    ValueError
        If ``values`` is not one-dimensional after conversion.
    """
    array = _checked_integers(values, np.int64, "Int64", _INT64_MIN, _INT64_MAX)
    bound = _bound()
    if array.ndim != 1:
        raise ValueError(f"Expected a 1-D array; got shape {array.shape}.")
    target = bound["longs"](array.size)
    if array.size:
        bound["longs_in"](bound["pointer"](array.ctypes.data), target, 0, array.size)
    return target


def to_numpy_longs(values: Any) -> np.ndarray:
    """Copy a ``System.Int64[]`` into a new int64 NumPy array, preserving every bit.

    Parameters
    ----------
    values : System.Int64[]
        .NET array to copy, for example the ticks of a ``DateTime`` series.

    Returns
    -------
    numpy.ndarray
        A new one-dimensional int64 array with ``values.Length`` elements.
    """
    bound = _bound()
    length = int(values.Length)
    out = np.empty(length, dtype=np.int64)
    if length:
        bound["longs_out"](values, 0, bound["pointer"](out.ctypes.data), length)
    return out


def _bytes_to_net(array: np.ndarray, kind: str) -> Any:
    """Copy a contiguous one-byte-per-element array into a new .NET array of ``kind`` through one byte copy."""
    bound = _bound()
    target = bound[kind](array.size)
    if array.size:
        raw = bound["bytes"](array.size)
        bound["bytes_in"](bound["pointer"](array.ctypes.data), raw, 0, array.size)
        bound["block_copy"](raw, 0, target, 0, array.size)
    return target


def _bytes_from_net(values: Any, dtype: type) -> np.ndarray:
    """Copy a one-byte-per-element .NET array into a new NumPy array of ``dtype`` through one byte copy."""
    bound = _bound()
    length = int(values.Length)
    out = np.empty(length, dtype=dtype)
    if length:
        raw = bound["bytes"](length)
        bound["block_copy"](values, 0, raw, 0, length)
        bound["bytes_out"](raw, 0, bound["pointer"](out.ctypes.data), length)
    return out


def to_net_sbytes(values: Any) -> Any:
    """Copy a one-dimensional array into a new ``System.SByte[]``, preserving every bit.

    A value outside the int8 range raises ``OverflowError`` instead of wrapping.
    Floating-point values inside the range truncate toward zero. Booleans cross as 0 and 1;
    complex, date, time, text and other values are refused.

    Parameters
    ----------
    values : array_like
        Integer values to copy, one-dimensional after conversion.

    Returns
    -------
    System.SByte[]
        A new .NET array holding the int8 values.

    Raises
    ------
    OverflowError
        If a value lies outside the int8 range or is not finite; raised before the
        runtime starts.
    TypeError
        If the values are not booleans, integers, or floating-point numbers.
    ValueError
        If ``values`` is not one-dimensional after conversion.
    """
    array = _checked_integers(values, np.int8, "SByte", _INT8_MIN, _INT8_MAX)
    if array.ndim != 1:
        raise ValueError(f"Expected a 1-D array; got shape {array.shape}.")
    return _bytes_to_net(array, "sbytes")


def to_numpy_sbytes(values: Any) -> np.ndarray:
    """Copy a ``System.SByte[]`` into a new int8 NumPy array, preserving every bit.

    Parameters
    ----------
    values : System.SByte[]
        .NET array to copy.

    Returns
    -------
    numpy.ndarray
        A new one-dimensional int8 array with ``values.Length`` elements.
    """
    return _bytes_from_net(values, np.int8)


def to_net_bools(values: Any) -> Any:
    """Copy a one-dimensional boolean array into a new ``System.Boolean[]``.

    Only boolean input is accepted: integers or floating-point values are refused rather
    than interpreted as truth values. An empty input of any type gives an empty array.

    Parameters
    ----------
    values : array_like
        Boolean values to copy, one-dimensional after conversion.

    Returns
    -------
    System.Boolean[]
        A new .NET array holding the values.

    Raises
    ------
    TypeError
        If the values are not booleans; raised before the runtime starts.
    ValueError
        If ``values`` is not one-dimensional after conversion.
    """
    source = np.asarray(values)
    if source.dtype.kind != "b" and source.size:
        raise TypeError(f"Values must be booleans to cross as Boolean; got dtype {source.dtype}.")
    array = np.ascontiguousarray(source, dtype=np.bool_)
    if array.ndim != 1:
        raise ValueError(f"Expected a 1-D array; got shape {array.shape}.")
    return _bytes_to_net(array, "bools")


def to_numpy_bools(values: Any) -> np.ndarray:
    """Copy a ``System.Boolean[]`` into a new boolean NumPy array.

    Parameters
    ----------
    values : System.Boolean[]
        .NET array to copy.

    Returns
    -------
    numpy.ndarray
        A new one-dimensional bool array with ``values.Length`` elements; any nonzero
        .NET byte reads as True.
    """
    return _bytes_from_net(values, np.uint8) != 0


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
    bound = _bound()
    target = bound["singles"](array.size)
    if array.size:
        bound["singles_in"](bound["pointer"](array.ctypes.data), target, 0, array.size)
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
        _bound()["singles_out"](values, 0, System.IntPtr(out.ctypes.data), length)
    return out.reshape((steps, rows, cols))
