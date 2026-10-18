"""The multivariate normal distribution from ``Numerics.Distributions``."""

from __future__ import annotations

from typing import Any

import numpy as np

from rmc_numerics._util import namespace
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


__all__ = ["MultivariateNormal"]
