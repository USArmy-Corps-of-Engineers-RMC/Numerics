"""Multivariate distributions from ``Numerics.Distributions``: the multivariate normal and the Dirichlet."""

from __future__ import annotations

from typing import Any

import numpy as np

from rmc_numerics._util import namespace, unwrap
from rmc_numerics.marshal import (
    to_net_doubles,
    to_net_matrix,
    to_numpy_doubles,
    to_numpy_matrix,
)


class MultivariateNormal:
    """Multivariate normal distribution (``Numerics.Distributions.MultivariateNormal``).

    Parameters
    ----------
    mean : array_like
        Mean vector μ of length d.
    covariance : array_like, optional
        Covariance matrix Σ of shape (d, d); default the identity.

    Attributes
    ----------
    net : Numerics.Distributions.MultivariateNormal
        The wrapped .NET object.
    """

    def __init__(self, mean: Any, covariance: Any = None) -> None:
        nd = namespace("Numerics.Distributions")
        if covariance is None:
            self.net = nd.MultivariateNormal(to_net_doubles(mean))
        else:
            self.net = nd.MultivariateNormal(to_net_doubles(mean), to_net_matrix(covariance))

    @property
    def dimension(self) -> int:
        """The dimension d."""
        return int(self.net.Dimension)

    @property
    def mean(self) -> np.ndarray:
        """The mean vector."""
        return to_numpy_doubles(self.net.Mean)

    @property
    def covariance(self) -> np.ndarray:
        """The covariance matrix."""
        return to_numpy_matrix(self.net.Covariance)

    def pdf(self, x: Any) -> float:
        """Return the density at one point (``PDF(double[])``).

        Parameters
        ----------
        x : array_like
            Point of length d.

        Returns
        -------
        float
            The density.
        """
        return float(self.net.PDF(to_net_doubles(x)))

    def log_pdf(self, x: Any) -> float:
        """Return the log-density at one point (``LogPDF(double[])``).

        Parameters
        ----------
        x : array_like
            Point of length d.

        Returns
        -------
        float
            The log-density.
        """
        return float(self.net.LogPDF(to_net_doubles(x)))

    def cdf(self, x: Any) -> float:
        """Return the joint CDF at one point (``CDF(double[])``).

        Parameters
        ----------
        x : array_like
            Point of length d.

        Returns
        -------
        float
            P(X ≤ x).
        """
        return float(self.net.CDF(to_net_doubles(x)))

    def inverse_cdf(self, probabilities: Any) -> np.ndarray:
        """Map d marginal probabilities to a correlated point (``InverseCDF(double[])``).

        Parameters
        ----------
        probabilities : array_like
            Probabilities of length d; each maps to a standard normal deviate, and the
            deviates are correlated as A·z + μ.

        Returns
        -------
        numpy.ndarray
            The point of length d.
        """
        return to_numpy_doubles(self.net.InverseCDF(to_net_doubles(probabilities)))

    def generate_random_values(self, sample_size: int, seed: int = -1) -> np.ndarray:
        """Draw a Monte Carlo sample (``GenerateRandomValues``).

        Parameters
        ----------
        sample_size : int
            Number of points.
        seed : int, optional
            Seed; zero or negative seeds from the clock (not reproducible).

        Returns
        -------
        numpy.ndarray
            Array of shape ``(sample_size, d)``.
        """
        return to_numpy_matrix(self.net.GenerateRandomValues(int(sample_size), int(seed)))

    def latin_hypercube_random_values(self, sample_size: int, seed: int) -> np.ndarray:
        """Draw a Latin hypercube sample (``LatinHypercubeRandomValues``).

        Parameters
        ----------
        sample_size : int
            Number of points.
        seed : int
            Seed.

        Returns
        -------
        numpy.ndarray
            Array of shape ``(sample_size, d)``.
        """
        return to_numpy_matrix(self.net.LatinHypercubeRandomValues(int(sample_size), int(seed)))


class Dirichlet:
    """Dirichlet distribution on the probability simplex (``Numerics.Distributions.Dirichlet``).

    Parameters
    ----------
    alpha : array_like
        Concentration parameters α of length K ≥ 2, each positive and finite. Numerics refuses
        anything else with ``ArgumentOutOfRangeException`` (``ValueError`` inside
        :func:`rmc_numerics.errors.net_errors`).

    Attributes
    ----------
    net : Numerics.Distributions.Dirichlet
        The wrapped .NET object.

    Notes
    -----
    Every value comes from Numerics; nothing is computed in Python. Draws use the gamma
    representation inside ``Dirichlet.GenerateRandomValues``: K independent
    Gamma(αᵢ, 1) inverse-CDF variates from one seeded Mersenne Twister, normalized to sum
    to one. The sample crosses back as one ``double[,]`` block copy, so a draw is bit for bit
    the ``double[,]`` the .NET call returns.
    """

    def __init__(self, alpha: Any) -> None:
        nd = namespace("Numerics.Distributions")
        self.net = nd.Dirichlet(to_net_doubles(alpha))
        self._bind()

    @classmethod
    def symmetric(cls, dimension: int, alpha: float) -> Dirichlet:
        """Build the symmetric Dirichlet with every αᵢ = ``alpha`` (``Dirichlet(int, double)``).

        Parameters
        ----------
        dimension : int
            Number of components K ≥ 2.
        alpha : float
            Common positive concentration parameter.

        Returns
        -------
        Dirichlet
            The wrapped distribution.
        """
        nd = namespace("Numerics.Distributions")
        instance = cls.__new__(cls)
        instance.net = nd.Dirichlet(int(dimension), float(alpha))
        instance._bind()
        return instance

    @classmethod
    def from_net(cls, net: Any) -> Dirichlet:
        """Wrap an existing ``Numerics.Distributions.Dirichlet`` without copying it.

        Parameters
        ----------
        net : Numerics.Distributions.Dirichlet
            The .NET object; interface and base-class wrappers are unwrapped.

        Returns
        -------
        Dirichlet
            A wrapper sharing ``net``.
        """
        instance = cls.__new__(cls)
        instance.net = unwrap(net)
        instance._bind()
        return instance

    def _bind(self) -> None:
        """Bind ``GenerateRandomValues(int, int)`` once, so calls skip overload resolution."""
        import System  # noqa: PLC0415 - the runtime is started by the constructor

        self._generate = self.net.GenerateRandomValues.Overloads[System.Int32, System.Int32]

    @property
    def dimension(self) -> int:
        """The number of components K."""
        return int(self.net.Dimension)

    @property
    def alpha(self) -> np.ndarray:
        """The concentration parameters α (a copy)."""
        return to_numpy_doubles(self.net.Alpha)

    @property
    def alpha_sum(self) -> float:
        """The concentration sum α₀ = Σαᵢ."""
        return float(self.net.AlphaSum)

    @property
    def parameters_valid(self) -> bool:
        """Whether every αᵢ is positive and finite."""
        return bool(self.net.ParametersValid)

    @property
    def mean(self) -> np.ndarray:
        """The mean vector αᵢ/α₀."""
        return to_numpy_doubles(self.net.Mean)

    @property
    def variance(self) -> np.ndarray:
        """The marginal variances αᵢ(α₀ − αᵢ)/(α₀²(α₀ + 1))."""
        return to_numpy_doubles(self.net.Variance)

    @property
    def mode(self) -> np.ndarray:
        """The mode (αᵢ − 1)/(α₀ − K); Numerics refuses it unless every αᵢ > 1."""
        return to_numpy_doubles(self.net.Mode)

    def covariance(self, i: int, j: int) -> float:
        """Return one covariance entry (``Covariance(int, int)``).

        Parameters
        ----------
        i : int
            Row component index.
        j : int
            Column component index.

        Returns
        -------
        float
            Cov(Xᵢ, Xⱼ).
        """
        return float(self.net.Covariance(int(i), int(j)))

    def covariance_matrix(self) -> np.ndarray:
        """Return the full covariance matrix (``CovarianceMatrix()``).

        Returns
        -------
        numpy.ndarray
            Array of shape ``(K, K)``.
        """
        return to_numpy_matrix(self.net.CovarianceMatrix())

    def pdf(self, x: Any) -> float:
        """Return the density at one point of the simplex (``PDF(double[])``).

        Parameters
        ----------
        x : array_like
            Point of length K with positive components summing to one.

        Returns
        -------
        float
            The density; zero off the simplex.
        """
        return float(self.net.PDF(to_net_doubles(x)))

    def log_pdf(self, x: Any) -> float:
        """Return the log-density at one point of the simplex (``LogPDF(double[])``).

        Parameters
        ----------
        x : array_like
            Point of length K with positive components summing to one.

        Returns
        -------
        float
            The log-density; negative infinity off the simplex.
        """
        return float(self.net.LogPDF(to_net_doubles(x)))

    def generate_random_values(self, sample_size: int, seed: int = -1) -> np.ndarray:
        """Draw a Monte Carlo sample (``GenerateRandomValues(int, int)``).

        Parameters
        ----------
        sample_size : int
            Number of points; zero returns an empty ``(0, K)`` array.
        seed : int, optional
            Mersenne Twister seed; zero or negative seeds from the clock (not reproducible).

        Returns
        -------
        numpy.ndarray
            Array of shape ``(sample_size, K)``; each row lies on the simplex.
        """
        return to_numpy_matrix(self._generate(int(sample_size), int(seed)))

    @staticmethod
    def log_multivariate_beta(alpha: Any) -> float:
        """Return log B(α) = Σ log Γ(αᵢ) − log Γ(Σαᵢ) (``Dirichlet.LogMultivariateBeta``).

        Parameters
        ----------
        alpha : array_like
            Concentration parameters.

        Returns
        -------
        float
            The log of the multivariate beta function.
        """
        nd = namespace("Numerics.Distributions")
        return float(nd.Dirichlet.LogMultivariateBeta(to_net_doubles(alpha)))


__all__ = ["Dirichlet", "MultivariateNormal"]
