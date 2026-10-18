"""Uncertainty analysis from ``Numerics.Distributions``: ensembles and the parametric bootstrap.

:class:`UncertaintyAnalysisResults` wraps the ensemble summaries (Type-7 percentile
bands, the expected-probability curve, the mode curve); :class:`BootstrapAnalysis`
wraps the parametric resample-and-refit bootstrap of one distribution.
"""

from __future__ import annotations

from collections.abc import Sequence
from typing import Any

import numpy as np

from rmc_numerics._util import namespace, net_enum
from rmc_numerics.distributions import UnivariateDistribution, wrap
from rmc_numerics.enums import EstimationMethod
from rmc_numerics.marshal import to_net_doubles, to_numpy_doubles, to_numpy_matrix


def _nd() -> Any:
    """Return the ``Numerics.Distributions`` namespace after starting the runtime."""
    return namespace("Numerics.Distributions")


def _members(distributions: Sequence[UnivariateDistribution | None]) -> Any:
    """Build a ``UnivariateDistributionBase[]``; None entries stay null (Numerics treats them as missing)."""
    nd = _nd()
    import System  # noqa: PLC0415

    return System.Array[nd.UnivariateDistributionBase]([None if d is None else d.net for d in distributions])


class UncertaintyAnalysisResults:
    """Ensemble summaries (``Numerics.Distributions.UncertaintyAnalysisResults``).

    Construct empty and call the ``process_*`` members, or use :meth:`from_ensemble`
    for the full constructor. Probabilities are non-exceedance probabilities.

    Parameters
    ----------
    net : Numerics.Distributions.UncertaintyAnalysisResults, optional
        An existing .NET results object to wrap; None, the default, creates an empty one.

    Attributes
    ----------
    net : Numerics.Distributions.UncertaintyAnalysisResults
        The wrapped .NET object.
    """

    def __init__(self, net: Any = None) -> None:
        self.net = _nd().UncertaintyAnalysisResults() if net is None else net

    @classmethod
    def from_ensemble(cls, parent: UnivariateDistribution, sampled: Sequence[UnivariateDistribution | None],
                      probabilities: Any, alpha: float = 0.1, min_probability: float = 0.001,
                      max_probability: float = 1 - 1e-9,
                      record_parameter_sets: bool = False) -> UncertaintyAnalysisResults:
        """Run the full constructor: mode curve, confidence intervals and mean curve.

        Parameters
        ----------
        parent : UnivariateDistribution
            The point-estimate distribution (mode curve).
        sampled : sequence of UnivariateDistribution
            Ensemble members; None members are missing.
        probabilities : array_like
            Non-exceedance probabilities at which to summarize.
        alpha : float, optional
            Two-sided band level; the band is [α/2, 1 − α/2]; default 0.1.
        min_probability, max_probability : float, optional
            Range of the mean-curve value grid.
        record_parameter_sets : bool, optional
            Store the members' parameter sets; default False.

        Returns
        -------
        UncertaintyAnalysisResults
            The populated results.
        """
        net = _nd().UncertaintyAnalysisResults(parent.net, _members(sampled), to_net_doubles(probabilities),
                                               float(alpha), float(min_probability), float(max_probability),
                                               bool(record_parameter_sets))
        return cls(net)

    def process_mode_curve(self, parent: UnivariateDistribution, probabilities: Any) -> np.ndarray:
        """Compute the mode curve ``parent.InverseCDF(probabilities)`` (``ProcessModeCurve``).

        Parameters
        ----------
        parent : UnivariateDistribution
            The point-estimate distribution.
        probabilities : array_like
            Non-exceedance probabilities.

        Returns
        -------
        numpy.ndarray
            The mode curve.
        """
        self.net.ProcessModeCurve(parent.net, to_net_doubles(probabilities))
        return self.mode_curve

    def process_confidence_intervals(self, sampled: Sequence[UnivariateDistribution | None], probabilities: Any,
                                     alpha: float = 0.1) -> np.ndarray:
        """Compute the Type-7 percentile band of member quantiles (``ProcessConfidenceIntervals``).

        Non-finite member quantiles are dropped before the percentiles.

        Parameters
        ----------
        sampled : sequence of UnivariateDistribution
            Ensemble members; None members are missing.
        probabilities : array_like
            Non-exceedance probabilities.
        alpha : float, optional
            Two-sided band level; default 0.1.

        Returns
        -------
        numpy.ndarray
            Array of shape ``(len(probabilities), 2)``: lower and upper limits.
        """
        self.net.ProcessConfidenceIntervals(_members(sampled), to_net_doubles(probabilities), float(alpha))
        return self.confidence_intervals

    def process_mean_curve(self, sampled: Sequence[UnivariateDistribution | None], probabilities: Any,
                           min_probability: float = 0.001, max_probability: float = 1 - 1e-9) -> np.ndarray:
        """Compute the expected-probability curve E[F] (``ProcessMeanCurve``).

        Parameters
        ----------
        sampled : sequence of UnivariateDistribution
            Ensemble members; None members are missing.
        probabilities : array_like
            Non-exceedance probabilities at which the curve is read.
        min_probability, max_probability : float, optional
            Range of the value grid.

        Returns
        -------
        numpy.ndarray
            The mean curve.
        """
        self.net.ProcessMeanCurve(_members(sampled), to_net_doubles(probabilities), float(min_probability),
                                  float(max_probability))
        return self.mean_curve

    @property
    def confidence_intervals(self) -> np.ndarray | None:
        """The band, shape ``(n, 2)``."""
        ci = self.net.ConfidenceIntervals
        return None if ci is None else to_numpy_matrix(ci)

    @property
    def mean_curve(self) -> np.ndarray | None:
        """The expected-probability curve."""
        curve = self.net.MeanCurve
        return None if curve is None else to_numpy_doubles(curve)

    @property
    def mode_curve(self) -> np.ndarray | None:
        """The mode curve."""
        curve = self.net.ModeCurve
        return None if curve is None else to_numpy_doubles(curve)


class BootstrapAnalysis:
    """Parametric bootstrap of one distribution (``Numerics.Distributions.BootstrapAnalysis``).

    Parameters
    ----------
    distribution : UnivariateDistribution
        The parent distribution; it must support bootstrapping (``IBootstrappable``).
    method : EstimationMethod or str
        Refit method for each replicate.
    sample_size : int
        Size of each resampled data set.
    replications : int, optional
        Number of replicates; default 10,000.
    seed : int, optional
        Seed; default 12345.

    Attributes
    ----------
    net : Numerics.Distributions.BootstrapAnalysis
        The wrapped .NET object.
    """

    def __init__(self, distribution: UnivariateDistribution, method: EstimationMethod | str, sample_size: int,
                 replications: int = 10000, seed: int = 12345) -> None:
        nd = _nd()
        self.net = nd.BootstrapAnalysis(distribution.net, net_enum(nd.ParameterEstimationMethod, method),
                                        int(sample_size), int(replications), int(seed))
        self._distributions: Any = None

    @property
    def failed_replications(self) -> int:
        """Replicates whose refit failed (set after :meth:`distributions`)."""
        return int(self.net.FailedReplications)

    def _net_distributions(self) -> Any:
        """Return the cached .NET replicate array, computing it once."""
        if self._distributions is None:
            self._distributions = self.net.Distributions()
        return self._distributions

    def distributions(self) -> list[UnivariateDistribution | None]:
        """Run (once) and return the bootstrap replicate distributions (``Distributions``).

        Returns
        -------
        list of UnivariateDistribution or None
            One fitted distribution per replicate; None where the refit failed.
        """
        return [None if d is None else wrap(d) for d in self._net_distributions()]

    def quantiles(self, probabilities: Any) -> np.ndarray:
        """Return replicate quantiles (``Quantiles``).

        Parameters
        ----------
        probabilities : array_like
            Non-exceedance probabilities.

        Returns
        -------
        numpy.ndarray
            Array of shape ``(replications, len(probabilities))``.
        """
        return to_numpy_matrix(self.net.Quantiles(to_net_doubles(probabilities), self._net_distributions()))

    def percentile_quantile_ci(self, probabilities: Any, alpha: float = 0.1) -> np.ndarray:
        """Return percentile confidence intervals of quantiles (``PercentileQuantileCI``).

        Parameters
        ----------
        probabilities : array_like
            Non-exceedance probabilities.
        alpha : float, optional
            Two-sided level; default 0.1.

        Returns
        -------
        numpy.ndarray
            Array of shape ``(len(probabilities), 2)``.
        """
        return to_numpy_matrix(self.net.PercentileQuantileCI(to_net_doubles(probabilities), float(alpha),
                                                             self._net_distributions()))

    def expected_probabilities(self, quantiles: Any) -> np.ndarray:
        """Return the mean replicate CDF at given values (``ExpectedProbabilities``).

        Parameters
        ----------
        quantiles : array_like
            Values at which the replicate CDFs are averaged.

        Returns
        -------
        numpy.ndarray
            Expected non-exceedance probabilities.
        """
        return to_numpy_doubles(self.net.ExpectedProbabilities(to_net_doubles(quantiles), self._net_distributions()))

    def estimate(self, probabilities: Any, alpha: float = 0.1,
                 record_parameter_sets: bool = True) -> UncertaintyAnalysisResults:
        """Summarize the bootstrap ensemble (``Estimate``).

        Parameters
        ----------
        probabilities : array_like
            Non-exceedance probabilities.
        alpha : float, optional
            Two-sided level; default 0.1.
        record_parameter_sets : bool, optional
            Store the replicate parameter sets; default True.

        Returns
        -------
        UncertaintyAnalysisResults
            Mode curve, band and mean curve.
        """
        return UncertaintyAnalysisResults(self.net.Estimate(to_net_doubles(probabilities), float(alpha),
                                                            self._net_distributions(), bool(record_parameter_sets)))


__all__ = ["BootstrapAnalysis", "UncertaintyAnalysisResults"]
