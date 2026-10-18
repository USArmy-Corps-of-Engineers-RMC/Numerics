"""Root finding and accurate elementary functions from Numerics.

``brent_solve`` and ``brent_bracket`` call ``Numerics.Mathematics.RootFinding.Brent``
with the Python callable wrapped as a ``System.Func<double, double>``; ``expm1`` and
``log1p`` call ``Numerics.Tools``. Numerics has no array overload of ``Tools.Expm1`` or
``Tools.Log1p``, so arrays are evaluated element by element with the Numerics routine.
"""

from __future__ import annotations

from collections.abc import Callable
from typing import Any

from rmc_numerics._util import loop, namespace


def _func(f: Callable[[float], float]) -> Any:
    """Wrap a Python callable as ``System.Func[Double, Double]``."""
    namespace("Numerics")
    import System  # noqa: PLC0415

    return System.Func[System.Double, System.Double](lambda x: float(f(float(x))))


def brent_solve(f: Callable[[float], float], lower: float, upper: float, tolerance: float = 1e-8,
                max_iterations: int = 1000, report_failure: bool = True) -> float:
    """Find a root of ``f`` in a sign-changing bracket (``Brent.Solve``).

    Parameters
    ----------
    f : callable
        Function of one float.
    lower, upper : float
        Bracket with f(lower)·f(upper) ≤ 0.
    tolerance : float, optional
        Absolute tolerance; default 1e-8.
    max_iterations : int, optional
        Iteration limit; default 1000.
    report_failure : bool, optional
        Raise when the solver fails (the Numerics default); default True.

    Returns
    -------
    float
        The root.

    Raises
    ------
    System.ArgumentOutOfRangeException
        If the bracket does not change sign (raised by Numerics).
    """
    brent = namespace("Numerics.Mathematics.RootFinding").Brent
    return float(brent.Solve(_func(f), float(lower), float(upper), float(tolerance), int(max_iterations),
                             bool(report_failure)))


def brent_bracket(f: Callable[[float], float], lower: float, upper: float,
                  max_iterations: int = 10) -> tuple[bool, float, float, float, float]:
    """Expand a range outward until it brackets a sign change (``Brent.Bracket``).

    Parameters
    ----------
    f : callable
        Function of one float.
    lower, upper : float
        Starting range.
    max_iterations : int, optional
        Expansion limit; default 10.

    Returns
    -------
    tuple
        ``(found, lower, upper, f_lower, f_upper)``.
    """
    brent = namespace("Numerics.Mathematics.RootFinding").Brent
    found, lo, hi, f1, f2 = brent.Bracket(_func(f), float(lower), float(upper), 0.0, 0.0, int(max_iterations))
    return bool(found), float(lo), float(hi), float(f1), float(f2)


def expm1(x: Any) -> Any:
    """Return exp(x) − 1 accurately near zero (``Tools.Expm1``).

    Parameters
    ----------
    x : float or array_like
        Arguments.

    Returns
    -------
    float or numpy.ndarray
        The values exp(x) − 1, shaped like ``x``.
    """
    return loop(x, namespace("Numerics").Tools.Expm1)


def log1p(x: Any) -> Any:
    """Return log(1 + x) accurately near zero (``Tools.Log1p``).

    Parameters
    ----------
    x : float or array_like
        Arguments.

    Returns
    -------
    float or numpy.ndarray
        The values log(1 + x), shaped like ``x``.
    """
    return loop(x, namespace("Numerics").Tools.Log1p)


__all__ = ["brent_bracket", "brent_solve", "expm1", "log1p"]
