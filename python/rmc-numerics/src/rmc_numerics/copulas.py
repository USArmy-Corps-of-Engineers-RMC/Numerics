"""Bivariate copulas from ``Numerics.Distributions.Copulas``.

Numerics evaluates copulas on scalar (u, v) pairs only; there are no array overloads.
The wrappers evaluate paired arrays element by element with the copula's own members
and use ``GenerateRandomValues`` for bulk sampling.
"""

from __future__ import annotations

from typing import Any

import numpy as np

from rmc_numerics._util import namespace, unwrap
from rmc_numerics.distributions import UnivariateDistribution
from rmc_numerics.marshal import to_net_doubles, to_numpy_doubles, to_numpy_matrix


def _nc() -> Any:
    """Return the ``Numerics.Distributions.Copulas`` namespace after starting the runtime."""
    return namespace("Numerics.Distributions.Copulas")


def _pairwise(u: Any, v: Any, copula: Any, name: str) -> Any:
    """Evaluate a (u, v) copula member over broadcast arrays.

    A scalar pair calls the ``(double, double)`` overload. Arrays cross once into the
    ``(IList<double>, IList<double>)`` overload of ``BivariateCopula`` (RMC.Numerics 2.3), or pair
    by pair on an older assembly; every value is the scalar member's. Both overloads are bound
    once per copula object.
    """
    import System  # noqa: PLC0415 - the runtime is running (the copula exists)
    from System.Collections.Generic import IList  # noqa: PLC0415

    cache = copula.__dict__.setdefault("_bound", {})
    entry = cache.get(name)
    if entry is None or entry[0] is not copula.net:
        group = getattr(copula.net, name)
        scalar = group.Overloads[System.Double, System.Double]
        try:
            vector = group.Overloads[IList[System.Double], IList[System.Double]]
        except (KeyError, TypeError):
            vector = None
        entry = cache[name] = (copula.net, scalar, vector)
    _, scalar, vector = entry
    uu, vv = np.broadcast_arrays(np.asarray(u, dtype=np.float64), np.asarray(v, dtype=np.float64))
    if uu.ndim == 0:
        return float(scalar(float(uu), float(vv)))
    if vector is not None:
        return to_numpy_doubles(vector(to_net_doubles(uu.ravel()), to_net_doubles(vv.ravel()))).reshape(uu.shape)
    out = np.empty(uu.size, dtype=np.float64)
    for index, (a, b) in enumerate(zip(uu.ravel(), vv.ravel(), strict=True)):
        out[index] = scalar(float(a), float(b))
    return out.reshape(uu.shape)


class BivariateCopula:
    """A wrapped ``Numerics.Distributions.Copulas.BivariateCopula``.

    Parameters
    ----------
    net : Numerics.Distributions.Copulas.BivariateCopula
        The .NET copula.

    Attributes
    ----------
    net : Numerics.Distributions.Copulas.BivariateCopula
        The wrapped .NET object.
    """

    def __init__(self, net: Any) -> None:
        self.net = unwrap(net)

    @property
    def theta(self) -> float:
        """The dependence parameter θ (ρ for the normal and Student-t copulas); settable."""
        return float(self.net.Theta)

    @theta.setter
    def theta(self, value: float) -> None:
        self.net.Theta = float(value)

    def set_marginals(self, x: UnivariateDistribution | None, y: UnivariateDistribution | None) -> None:
        """Set the marginal distributions used by sampling and the likelihoods.

        Parameters
        ----------
        x, y : UnivariateDistribution or None
            Marginals of the first and second variable.
        """
        self.net.MarginalDistributionX = None if x is None else x.net
        self.net.MarginalDistributionY = None if y is None else y.net

    def pdf(self, u: Any, v: Any) -> Any:
        """Return the copula density c(u, v).

        Parameters
        ----------
        u, v : float or array_like
            Uniform margins, broadcast together.

        Returns
        -------
        float or numpy.ndarray
            Densities.
        """
        return _pairwise(u, v, self, "PDF")

    def cdf(self, u: Any, v: Any) -> Any:
        """Return the copula C(u, v).

        Parameters
        ----------
        u, v : float or array_like
            Uniform margins, broadcast together.

        Returns
        -------
        float or numpy.ndarray
            Joint non-exceedance probabilities.
        """
        return _pairwise(u, v, self, "CDF")

    def conditional_cdf(self, u: Any, v: Any) -> Any:
        """Return the conditional CDF ``ConditionalCDF(u, v)``.

        Parameters
        ----------
        u, v : float or array_like
            Uniform margins, broadcast together.

        Returns
        -------
        float or numpy.ndarray
            Conditional probabilities.
        """
        return _pairwise(u, v, self, "ConditionalCDF")

    def inverse_conditional_cdf(self, u: Any, t: Any) -> Any:
        """Return the inverse conditional CDF ``InverseConditionalCDF(u, t)``.

        Parameters
        ----------
        u : float or array_like
            Conditioning margin.
        t : float or array_like
            Conditional probability.

        Returns
        -------
        float or numpy.ndarray
            The second margin v.
        """
        return _pairwise(u, t, self, "InverseConditionalCDF")

    def inverse_cdf(self, u: float, v: float) -> np.ndarray:
        """Map two independent uniforms to a dependent pair (``InverseCDF(u, v)``).

        Parameters
        ----------
        u, v : float
            Independent uniforms.

        Returns
        -------
        numpy.ndarray
            The dependent uniform pair.
        """
        return to_numpy_doubles(self.net.InverseCDF(float(u), float(v)))

    def generate_random_values(self, sample_size: int, seed: int = -1) -> np.ndarray:
        """Draw a sample (``GenerateRandomValues``; marginal values when both marginals are set).

        Parameters
        ----------
        sample_size : int
            Number of pairs.
        seed : int, optional
            Seed; zero or negative seeds from the clock (not reproducible).

        Returns
        -------
        numpy.ndarray
            Array of shape ``(sample_size, 2)``.
        """
        return to_numpy_matrix(self.net.GenerateRandomValues(int(sample_size), int(seed)))


class NormalCopula(BivariateCopula):
    """Gaussian copula.

    Parameters
    ----------
    rho : float
        Correlation ρ.
    """

    def __init__(self, rho: float) -> None:
        super().__init__(_nc().NormalCopula(float(rho)))


class StudentTCopula(BivariateCopula):
    """Student-t copula.

    Parameters
    ----------
    rho : float
        Correlation ρ.
    degrees_of_freedom : float
        Degrees of freedom ν.
    """

    def __init__(self, rho: float, degrees_of_freedom: float) -> None:
        super().__init__(_nc().StudentTCopula(float(rho), float(degrees_of_freedom)))


class ClaytonCopula(BivariateCopula):
    """Clayton copula.

    Parameters
    ----------
    theta : float
        Dependence parameter θ.
    """

    def __init__(self, theta: float) -> None:
        super().__init__(_nc().ClaytonCopula(float(theta)))


class GumbelCopula(BivariateCopula):
    """Gumbel copula.

    Parameters
    ----------
    theta : float
        Dependence parameter θ ≥ 1.
    """

    def __init__(self, theta: float) -> None:
        super().__init__(_nc().GumbelCopula(float(theta)))


class FrankCopula(BivariateCopula):
    """Frank copula.

    Parameters
    ----------
    theta : float
        Dependence parameter θ.
    """

    def __init__(self, theta: float) -> None:
        super().__init__(_nc().FrankCopula(float(theta)))


class JoeCopula(BivariateCopula):
    """Joe copula.

    Parameters
    ----------
    theta : float
        Dependence parameter θ ≥ 1.
    """

    def __init__(self, theta: float) -> None:
        super().__init__(_nc().JoeCopula(float(theta)))


class AMHCopula(BivariateCopula):
    """Ali-Mikhail-Haq copula.

    Parameters
    ----------
    theta : float
        Dependence parameter θ in [−1, 1).
    """

    def __init__(self, theta: float) -> None:
        super().__init__(_nc().AMHCopula(float(theta)))


class IndependenceCopula(BivariateCopula):
    """Independence copula C(u, v) = u·v."""

    def __init__(self) -> None:
        super().__init__(_nc().IndependenceCopula())


__all__ = [
    "AMHCopula", "BivariateCopula", "ClaytonCopula", "FrankCopula", "GumbelCopula", "IndependenceCopula",
    "JoeCopula", "NormalCopula", "StudentTCopula",
]
