"""Descriptive statistics from ``Numerics.Data.Statistics``.

Thin calls into ``Statistics`` (sequential sums, Type-7 and weighted percentiles),
``RunningStatistics`` (streaming moments) and ``PlottingPositions``.
"""

from __future__ import annotations

from typing import Any

import numpy as np

from rmc_numerics._util import namespace, net_enum
from rmc_numerics.enums import PlottingPosition, WeightType
from rmc_numerics.marshal import to_net_doubles, to_numpy_doubles


def _stats() -> Any:
    """Return the ``Numerics.Data.Statistics`` namespace after starting the runtime."""
    return namespace("Numerics.Data.Statistics")


def mean(data: Any, weights: Any = None) -> float:
    """Return the mean through ``Statistics.Mean`` (sequential summation).

    Parameters
    ----------
    data : array_like
        Values.
    weights : array_like, optional
        Weights for ``Statistics.Mean(IList, IList)``.

    Returns
    -------
    float
        The (weighted) mean.
    """
    stats = _stats()
    if weights is None:
        return float(stats.Statistics.Mean(to_net_doubles(data)))
    return float(stats.Statistics.Mean(to_net_doubles(data), to_net_doubles(weights)))


def variance(data: Any, weights: Any = None, weight_type: WeightType | str = WeightType.FREQUENCY) -> float:
    """Return the sample variance through ``Statistics.Variance``.

    Parameters
    ----------
    data : array_like
        Values.
    weights : array_like, optional
        Weights; when given ``weight_type`` selects frequency or reliability weights.
    weight_type : WeightType or str, optional
        Weight interpretation; default frequency.

    Returns
    -------
    float
        The variance.
    """
    stats = _stats()
    if weights is None:
        return float(stats.Statistics.Variance(to_net_doubles(data)))
    return float(stats.Statistics.Variance(to_net_doubles(data), to_net_doubles(weights),
                                           net_enum(stats.WeightType, weight_type)))


def standard_deviation(data: Any, weights: Any = None, weight_type: WeightType | str = WeightType.FREQUENCY) -> float:
    """Return the sample standard deviation through ``Statistics.StandardDeviation``.

    Parameters
    ----------
    data : array_like
        Values.
    weights : array_like, optional
        Weights; when given ``weight_type`` selects frequency or reliability weights.
    weight_type : WeightType or str, optional
        Weight interpretation; default frequency.

    Returns
    -------
    float
        The standard deviation.
    """
    stats = _stats()
    if weights is None:
        return float(stats.Statistics.StandardDeviation(to_net_doubles(data)))
    return float(stats.Statistics.StandardDeviation(to_net_doubles(data), to_net_doubles(weights),
                                                    net_enum(stats.WeightType, weight_type)))


def percentile(data: Any, k: Any, weights: Any = None, data_is_sorted: bool = False) -> Any:
    """Return percentiles through ``Statistics.Percentile``.

    Unweighted percentiles are Type 7 (position (n − 1)k). Weighted percentiles use
    ``Statistics.Percentile(IList, k, IList weights, bool)``, which reduces to Type 7 at
    unit weights.

    Parameters
    ----------
    data : array_like
        Values.
    k : float or array_like
        Percentiles as fractions in [0, 1].
    weights : array_like, optional
        Weights, one per value.
    data_is_sorted : bool, optional
        Whether ``data`` is already ascending; default False.

    Returns
    -------
    float or numpy.ndarray
        Percentile values, shaped like ``k``.
    """
    s = _stats().Statistics
    values = to_net_doubles(data)
    w = None if weights is None else to_net_doubles(weights)
    karr = np.asarray(k, dtype=np.float64)
    if karr.ndim == 0:
        if w is None:
            return float(s.Percentile(values, float(karr), bool(data_is_sorted)))
        return float(s.Percentile(values, float(karr), w, bool(data_is_sorted)))
    ks = to_net_doubles(karr.ravel())
    result = s.Percentile(values, ks, bool(data_is_sorted)) if w is None else s.Percentile(values, ks, w,
                                                                                         bool(data_is_sorted))
    return to_numpy_doubles(result).reshape(karr.shape)


def plotting_positions(n: int, kind: PlottingPosition | str | float = PlottingPosition.WEIBULL) -> np.ndarray:
    """Return plotting-position probabilities through ``PlottingPositions.Function``.

    Parameters
    ----------
    n : int
        Sample size.
    kind : PlottingPosition, str or float, optional
        A named formula, or the plotting-position parameter α of (i − α)/(n + 1 − 2α).

    Returns
    -------
    numpy.ndarray
        The n ascending probabilities.
    """
    pp = _stats().PlottingPositions
    if isinstance(kind, (int, float)) and not isinstance(kind, bool):
        return to_numpy_doubles(pp.Function(int(n), float(kind)))
    return to_numpy_doubles(pp.Function(int(n), net_enum(pp.PlottingPostionType, kind)))


class RunningStatistics:
    """Streaming moments (``Numerics.Data.Statistics.RunningStatistics``).

    Parameters
    ----------
    values : array_like, optional
        Initial values to push.

    Attributes
    ----------
    net : Numerics.Data.Statistics.RunningStatistics
        The wrapped .NET object.
    """

    def __init__(self, values: Any = None) -> None:
        stats = _stats()
        self.net = stats.RunningStatistics() if values is None else stats.RunningStatistics(to_net_doubles(values))

    def push(self, values: Any) -> None:
        """Push one value or an array (``Push(double)`` / ``Push(IList<double>)``).

        Parameters
        ----------
        values : float or array_like
            Values to add.
        """
        array = np.asarray(values, dtype=np.float64)
        if array.ndim == 0:
            self.net.Push(float(array))
        else:
            self.net.Push(to_net_doubles(array.ravel()))

    @classmethod
    def combine(cls, a: RunningStatistics, b: RunningStatistics) -> RunningStatistics:
        """Merge two accumulators (``RunningStatistics.Combine``).

        Parameters
        ----------
        a, b : RunningStatistics
            Accumulators.

        Returns
        -------
        RunningStatistics
            The combined accumulator.
        """
        out = cls.__new__(cls)
        out.net = _stats().RunningStatistics.Combine(a.net, b.net)
        return out

    @property
    def count(self) -> int:
        """Number of values pushed."""
        return int(self.net.Count)

    @property
    def minimum(self) -> float:
        """Smallest value."""
        return float(self.net.Minimum)

    @property
    def maximum(self) -> float:
        """Largest value."""
        return float(self.net.Maximum)

    @property
    def mean(self) -> float:
        """Mean."""
        return float(self.net.Mean)

    @property
    def variance(self) -> float:
        """Sample variance."""
        return float(self.net.Variance)

    @property
    def standard_deviation(self) -> float:
        """Sample standard deviation."""
        return float(self.net.StandardDeviation)

    @property
    def skewness(self) -> float:
        """Sample skewness."""
        return float(self.net.Skewness)

    @property
    def kurtosis(self) -> float:
        """Sample kurtosis."""
        return float(self.net.Kurtosis)


__all__ = ["RunningStatistics", "mean", "percentile", "plotting_positions", "standard_deviation", "variance"]
