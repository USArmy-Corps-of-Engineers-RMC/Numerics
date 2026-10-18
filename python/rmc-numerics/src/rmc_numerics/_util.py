"""Shared helpers for the typed wrappers: enum mapping, array dispatch and unwrapping."""

from __future__ import annotations

import enum
from collections.abc import Callable
from typing import Any

import numpy as np

from rmc_numerics.marshal import to_net_doubles, to_numpy_doubles
from rmc_numerics.runtime import initialize


def unwrap(obj: Any) -> Any:
    """Return the concrete .NET object behind a pythonnet interface or base-class wrapper.

    pythonnet 3 wraps return values at their declared type; ``__implementation__``
    exposes the runtime type so members of the concrete class are reachable.
    """
    return getattr(obj, "__implementation__", obj)


def net_enum(net_type: Any, value: Any) -> Any:
    """Map a Python enum member or a name to a member of a .NET enum.

    Names match case-insensitively and ignore underscores, so ``"normal_z"``,
    ``"NormalZ"`` and ``Transform.NORMAL_Z`` all select ``Transform.NormalZ``.

    Raises
    ------
    ValueError
        If the name matches no member of ``net_type``.
    """
    if isinstance(value, enum.Enum):
        value = value.value
    if not isinstance(value, str):
        return value
    import clr  # noqa: PLC0415
    import System  # noqa: PLC0415 - the runtime is started by the caller

    key = value.replace("_", "").lower()
    clr_type = clr.GetClrType(net_type)
    for name in System.Enum.GetNames(clr_type):
        if str(name).lower() == key:
            return getattr(net_type, str(name))
    names = ", ".join(str(name) for name in System.Enum.GetNames(clr_type))
    raise ValueError(f"'{value}' is not a member of {clr_type.Name} ({names}).")


def evaluate(values: Any, scalar: Callable[[float], float], vector: Callable[[Any], Any]) -> Any:
    """Evaluate a Numerics member on a scalar or on an array through its array overload.

    A scalar input calls ``scalar`` and returns a Python float. Any other input is
    flattened to float64, passed to ``vector`` as one ``System.Double[]`` (the
    ``IList<double>`` overload), and the result is reshaped to the input's shape.
    """
    array = np.asarray(values, dtype=np.float64)
    if array.ndim == 0:
        return float(scalar(float(array)))
    result = to_numpy_doubles(vector(to_net_doubles(array.ravel())))
    return result.reshape(array.shape)


def loop(values: Any, scalar: Callable[[float], float]) -> Any:
    """Evaluate a scalar-only Numerics member element by element.

    Used only where Numerics has no array overload; the arithmetic stays in Numerics.
    """
    array = np.asarray(values, dtype=np.float64)
    if array.ndim == 0:
        return float(scalar(float(array)))
    out = np.empty(array.size, dtype=np.float64)
    for index, value in enumerate(array.ravel()):
        out[index] = scalar(float(value))
    return out.reshape(array.shape)


def bind(member: Any, arity: int = 1) -> Any:
    """Return the overload of a .NET method group that takes ``arity`` doubles.

    pythonnet resolves an overloaded method group on every call, which costs several times
    the call itself for the cheap scalar members (``InverseCDF(double)`` beside
    ``InverseCDF(IList<double>)``); binding the overload once makes repeated scalar calls
    about six times faster. The arithmetic is unchanged: the bound overload is the member
    the group would have resolved to. A group without that overload is returned as it is.
    """
    import System  # noqa: PLC0415 - the runtime is started by the caller

    try:
        return member.Overloads[(System.Double,) * arity] if arity > 1 else member.Overloads[System.Double]
    except (KeyError, TypeError, AttributeError):
        return member


def namespace(name: str) -> Any:
    """Start the runtime and import a .NET namespace by dotted name."""
    initialize()
    import importlib  # noqa: PLC0415

    return importlib.import_module(name)
