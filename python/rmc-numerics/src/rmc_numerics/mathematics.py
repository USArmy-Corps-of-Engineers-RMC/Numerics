"""Root finding, optimization, special functions, matrices and link functions from Numerics.

``brent_solve`` and ``brent_bracket`` call ``Numerics.Mathematics.RootFinding.Brent``
and ``brent_search`` calls ``Numerics.Mathematics.Optimization.BrentSearch``, with the
Python callable wrapped as a ``System.Func<double, double>``; ``expm1`` and ``log1p`` call
``Numerics.Tools``; ``beta_incomplete`` and ``beta_incomplete_inverse`` call
``Numerics.Mathematics.SpecialFunctions.Beta``. Numerics has no array overload of these
scalar functions, so arrays are evaluated element by element with the Numerics routine.
``matrix_product`` and ``make_symmetric_positive_definite`` run on ``Numerics`` matrices,
and :class:`LinkController` wraps the parameter link functions.
"""

from __future__ import annotations

from collections.abc import Callable, Sequence
from dataclasses import dataclass
from typing import Any

import numpy as np

from rmc_numerics._util import bind, loop, namespace, net_enum
from rmc_numerics.marshal import to_net_doubles, to_net_matrix, to_numpy_doubles, to_numpy_matrix


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


@dataclass(frozen=True)
class BrentSearchResult:
    """The outcome of a one-dimensional Brent search.

    Parameters
    ----------
    x : float
        The best parameter (``BestParameterSet.Values[0]``).
    fitness : float
        The objective at ``x`` (``BestParameterSet.Fitness``; negated by Numerics for a
        maximization, so it is reported here as the objective value itself).
    iterations : int
        Iterations used.
    function_evaluations : int
        Objective evaluations used.
    status : str
        The ``OptimizationStatus`` member name.
    """

    x: float
    fitness: float
    iterations: int
    function_evaluations: int
    status: str


def brent_search(f: Callable[[float], float], lower: float, upper: float, *, maximize: bool = False,
                 relative_tolerance: float | None = None, absolute_tolerance: float | None = None,
                 max_iterations: int | None = None, report_failure: bool | None = None,
                 compute_hessian: bool | None = None) -> BrentSearchResult:
    """Minimize or maximize ``f`` on a bounded interval (``BrentSearch``).

    Settings left as None keep the Numerics defaults (``RelativeTolerance`` and
    ``AbsoluteTolerance`` 1e-8, ``MaxIterations`` 10,000, ``ReportFailure`` and
    ``ComputeHessian`` true).

    Parameters
    ----------
    f : callable
        Objective of one float.
    lower, upper : float
        The search interval.
    maximize : bool, optional
        Maximize instead of minimize; default False.
    relative_tolerance, absolute_tolerance : float, optional
        Convergence tolerances.
    max_iterations : int, optional
        Iteration limit.
    report_failure : bool, optional
        Whether Numerics throws when the search fails.
    compute_hessian : bool, optional
        Whether Numerics evaluates the Hessian after convergence.

    Returns
    -------
    BrentSearchResult
        The best point, its objective value, the counts and the status.

    Raises
    ------
    System.Exception
        Whatever Numerics throws, for example when the search fails with
        ``report_failure`` set.
    """
    optimization = namespace("Numerics.Mathematics.Optimization")
    search = optimization.BrentSearch(_func(f), float(lower), float(upper))
    if relative_tolerance is not None:
        search.RelativeTolerance = float(relative_tolerance)
    if absolute_tolerance is not None:
        search.AbsoluteTolerance = float(absolute_tolerance)
    if max_iterations is not None:
        search.MaxIterations = int(max_iterations)
    if report_failure is not None:
        search.ReportFailure = bool(report_failure)
    if compute_hessian is not None:
        search.ComputeHessian = bool(compute_hessian)
    if maximize:
        search.Maximize()
    else:
        search.Minimize()
    best = search.BestParameterSet
    x = float(best.Values[0])
    return BrentSearchResult(x, float(f(x)), int(search.Iterations), int(search.FunctionEvaluations),
                             str(search.Status))


def beta_incomplete(a: Any, b: Any, x: Any) -> Any:
    """Return the regularized incomplete beta function I_x(a, b) (``Beta.Incomplete``).

    Parameters
    ----------
    a, b : float
        Shape parameters.
    x : float or array_like
        Arguments in [0, 1].

    Returns
    -------
    float or numpy.ndarray
        The values, shaped like ``x``.
    """
    member = namespace("Numerics.Mathematics.SpecialFunctions").Beta.Incomplete
    aa, bb = float(a), float(b)
    return loop(x, lambda v: member(aa, bb, v))


def beta_incomplete_inverse(a: Any, b: Any, probability: Any) -> Any:
    """Return the inverse of the regularized incomplete beta function (``Beta.IncompleteInverse``).

    Parameters
    ----------
    a, b : float
        Shape parameters.
    probability : float or array_like
        Probabilities in [0, 1].

    Returns
    -------
    float or numpy.ndarray
        The x with I_x(a, b) equal to each probability, shaped like ``probability``.
    """
    member = namespace("Numerics.Mathematics.SpecialFunctions").Beta.IncompleteInverse
    aa, bb = float(a), float(b)
    return loop(probability, lambda p: member(aa, bb, p))


def _matrix(values: Any) -> Any:
    """Build a ``Numerics.Mathematics.LinearAlgebra.Matrix`` from a 2-D array."""
    linear_algebra = namespace("Numerics.Mathematics.LinearAlgebra")
    return linear_algebra.Matrix(to_net_matrix(values))


def matrix_product(*matrices: Any) -> np.ndarray:
    """Multiply matrices left to right with ``Matrix.Multiply`` (``A * B * ...``).

    Parameters
    ----------
    *matrices : array_like
        Two or more 2-D arrays with conforming shapes.

    Returns
    -------
    numpy.ndarray
        The product.

    Raises
    ------
    ValueError
        If fewer than two matrices are given.
    """
    if len(matrices) < 2:
        raise ValueError("matrix_product needs at least two matrices.")
    result = _matrix(matrices[0])
    for operand in matrices[1:]:
        result = result.Multiply(_matrix(operand))
    return to_numpy_matrix(result.ToArray())


def cholesky(matrix: Any, relative_tolerance: float | None = None) -> np.ndarray:
    """Return the lower Cholesky factor L with A = L Lᵀ (``CholeskyDecomposition.L``).

    Parameters
    ----------
    matrix : array_like
        A symmetric positive-definite 2-D array.
    relative_tolerance : float, optional
        The relative pivot tolerance; None uses the Numerics default (2 n eps).

    Returns
    -------
    numpy.ndarray
        The lower-triangular factor.

    Raises
    ------
    System.ArgumentException
        If the matrix is not square, symmetric or positive definite (raised by Numerics).
    """
    linear_algebra = namespace("Numerics.Mathematics.LinearAlgebra")
    decomposition = (linear_algebra.CholeskyDecomposition(_matrix(matrix)) if relative_tolerance is None
                     else linear_algebra.CholeskyDecomposition(_matrix(matrix), float(relative_tolerance)))
    return to_numpy_matrix(decomposition.L.ToArray())


def make_symmetric_positive_definite(matrix: Any) -> np.ndarray:
    """Return the nearest Cholesky-accepted symmetric matrix (``MatrixRegularization.MakeSymmetricPositiveDefinite``).

    The symmetric part when its Cholesky factor exists, otherwise the symmetric part plus
    the smallest trace-scaled decade ridge whose factor exists.

    Parameters
    ----------
    matrix : array_like
        A finite square 2-D array.

    Returns
    -------
    numpy.ndarray
        The regularized matrix.

    Raises
    ------
    System.ArgumentException
        If the matrix is not square or holds non-finite entries (raised by Numerics).
    System.InvalidOperationException
        If no ridge on the ladder yields a factor (raised by Numerics).
    """
    linear_algebra = namespace("Numerics.Mathematics.LinearAlgebra")
    return to_numpy_matrix(linear_algebra.MatrixRegularization.MakeSymmetricPositiveDefinite(_matrix(matrix)).ToArray())


class LinkController:
    """Per-parameter link functions (``Numerics.Functions.LinkController``).

    Parameters
    ----------
    links : sequence of str or None
        One ``LinkFunctionType`` member name per parameter (``"identity"``, ``"log"``,
        ``"logit"``, ``"probit"``, ...), or None for a parameter without a link.

    Attributes
    ----------
    net : Numerics.Functions.LinkController
        The wrapped .NET object.
    """

    def __init__(self, links: Sequence[str | None]) -> None:
        functions = namespace("Numerics.Functions")
        import System  # noqa: PLC0415

        created = [None if name is None else functions.LinkFunctionFactory.Create(
            net_enum(functions.LinkFunctionType, name)) for name in links]
        self.net = functions.LinkController(System.Array[functions.ILinkFunction](created))

    def link(self, x: Any) -> np.ndarray:
        """Map parameters to the link space (``Link``).

        Parameters
        ----------
        x : array_like
            Parameters, one per link.

        Returns
        -------
        numpy.ndarray
            The linked values.
        """
        return to_numpy_doubles(self.net.Link(to_net_doubles(x)))

    def inverse_link(self, eta: Any) -> np.ndarray:
        """Map link-space values back to parameters (``InverseLink``).

        Parameters
        ----------
        eta : array_like
            Link-space values, one per link.

        Returns
        -------
        numpy.ndarray
            The parameters.
        """
        return to_numpy_doubles(self.net.InverseLink(to_net_doubles(eta)))

    def link_jacobian(self, x: Any) -> np.ndarray:
        """Return the diagonal Jacobian d(eta)/d(x) at the parameters (``LinkJacobian``).

        Parameters
        ----------
        x : array_like
            Parameters, one per link.

        Returns
        -------
        numpy.ndarray
            The square Jacobian matrix.
        """
        return to_numpy_matrix(self.net.LinkJacobian(to_net_doubles(x)).ToArray())


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
    return loop(x, bind(namespace("Numerics").Tools.Expm1))


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
    return loop(x, bind(namespace("Numerics").Tools.Log1p))


__all__ = [
    "BrentSearchResult", "LinkController", "beta_incomplete", "beta_incomplete_inverse", "brent_bracket",
    "brent_search", "brent_solve", "cholesky", "expm1", "log1p", "make_symmetric_positive_definite",
    "matrix_product",
]
