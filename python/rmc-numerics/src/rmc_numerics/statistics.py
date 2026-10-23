"""Descriptive statistics from ``Numerics.Data.Statistics``.

Thin calls into ``Statistics`` (sequential sums, Type-7 and weighted percentiles),
``RunningStatistics`` (streaming moments), ``PlottingPositions`` and ``Autocorrelation``.
"""

from __future__ import annotations

from typing import Any

import numpy as np

from rmc_numerics._util import namespace, net_enum
from rmc_numerics.enums import AutocorrelationType, PlottingPosition, WeightType
from rmc_numerics.marshal import to_net_doubles, to_net_matrix, to_numpy_doubles, to_numpy_ints, to_numpy_matrix


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


def plotting_position_percentile(data: Any, k: Any,
                                 kind: PlottingPosition | str = PlottingPosition.WEIBULL,
                                 data_is_sorted: bool = False) -> Any:
    """Return percentiles interpolated between order statistics at plotting positions.

    Calls ``Statistics.Percentile(IList, k, PlottingPostionType, bool)``: the i-th order
    statistic sits at its plotting position and the percentile interpolates linearly between
    them, held at the sample extremes outside the first and last positions. Weibull is
    Hyndman-Fan type 6 (NumPy ``method="weibull"``), Hazen type 5 and Kaplan-Meier type 4.

    Parameters
    ----------
    data : array_like
        Values.
    k : float or array_like
        Percentiles as fractions in [0, 1].
    kind : PlottingPosition or str, optional
        The plotting-position formula; default Weibull.
    data_is_sorted : bool, optional
        Whether ``data`` is already ascending; default False.

    Returns
    -------
    float or numpy.ndarray
        Percentile values, shaped like ``k``.
    """
    stats = _stats()
    s, kind_net = stats.Statistics, net_enum(stats.PlottingPositions.PlottingPostionType, kind)
    values = to_net_doubles(data)
    karr = np.asarray(k, dtype=np.float64)
    if karr.ndim == 0:
        return float(s.Percentile(values, float(karr), kind_net, bool(data_is_sorted)))
    result = s.Percentile(values, to_net_doubles(karr.ravel()), kind_net, bool(data_is_sorted))
    return to_numpy_doubles(result).reshape(karr.shape)


def column_percentiles(data: Any, k: Any) -> tuple[np.ndarray, np.ndarray]:
    """Return the Type 7 percentiles of each column over its finite entries (``Statistics.ColumnPercentiles``).

    Each column's finite entries go through ``Statistics.Percentile`` in one call for the
    whole matrix, so every value is bitwise the per-column ``percentile`` of the finite
    entries; NaN and infinite entries are excluded rather than sorted in.

    Parameters
    ----------
    data : array_like
        Two-dimensional samples, one row per member (for example a realization) and one
        column per variable (for example a curve node).
    k : float or array_like
        Percentiles as fractions in [0, 1].

    Returns
    -------
    percentiles : numpy.ndarray
        Shape ``(len(k), columns)`` (``(columns,)`` for a scalar ``k``); NaN for a column
        without a finite entry.
    excluded : numpy.ndarray
        The int32 number of non-finite entries excluded from each column.
    """
    s = _stats().Statistics
    karr = np.asarray(k, dtype=np.float64)
    table, excluded = s.ColumnPercentiles(to_net_matrix(data), to_net_doubles(karr.ravel()), None)
    result = to_numpy_matrix(table)
    if karr.ndim == 0:
        result = result[0]
    return result, to_numpy_ints(excluded)


def wilson_score_interval(successes: float, trials: float, confidence_level: float) -> tuple[float, float]:
    """Return the Wilson score interval of a binomial proportion (``Statistics.WilsonScoreInterval``).

    Parameters
    ----------
    successes : float
        The number of successes, in [0, ``trials``]; may be fractional.
    trials : float
        The number of trials (positive).
    confidence_level : float
        The two-sided level in (0, 1).

    Returns
    -------
    tuple of float
        The lower and upper bounds, clamped to [0, 1].
    """
    bounds = _stats().Statistics.WilsonScoreInterval(float(successes), float(trials), float(confidence_level))
    return float(bounds.Item1), float(bounds.Item2)


def linear_moments(data: Any) -> np.ndarray:
    """Return the sample L-moments through ``Statistics.LinearMoments``.

    Parameters
    ----------
    data : array_like
        Values (sorted by Numerics).

    Returns
    -------
    numpy.ndarray
        ``[l1, l2, t3, t4]``: the first two L-moments and the L-skewness and L-kurtosis
        ratios, as Numerics returns them.
    """
    return to_numpy_doubles(_stats().Statistics.LinearMoments(to_net_doubles(data)))


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


def weighted_plotting_positions(weights: Any, alpha: float) -> np.ndarray:
    """Return weighted plotting positions through ``PlottingPositions.Function(IList, double)``.

    ``F_i = (c_i - alpha w_i) / (W + (1 - 2 alpha) W / n)`` with ``c_i`` the sequential cumulative
    weight and ``W`` the total; unit weights give bitwise the positions of
    :func:`plotting_positions`.

    Parameters
    ----------
    weights : array_like
        Non-negative weights of the observations in ascending order of their values.
    alpha : float
        The plotting-position parameter in [0, 1] (0 Weibull, 0.44 Gringorten, 0.5 Hazen).

    Returns
    -------
    numpy.ndarray
        The non-decreasing positions, one per weight.
    """
    return to_numpy_doubles(_stats().PlottingPositions.Function(to_net_doubles(weights), float(alpha)))


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


def autocorrelation(data: Any, max_lag: int | None = None,
                    kind: AutocorrelationType | str = AutocorrelationType.CORRELATION) -> tuple[np.ndarray, np.ndarray]:
    """Return a sample autocorrelation function through ``Autocorrelation.Function``.

    ``COVARIANCE`` is ``R(k) = n^-1 sum_{t=k}^{n-1} (x_t - xbar)(x_{t-k} - xbar)`` (divisor ``n`` at
    every lag, the mean by ``Statistics.Mean``), ``CORRELATION`` is ``R(k) / R(0)`` and ``PARTIAL``
    is the Durbin-Levinson partial autocorrelation from lag 1.

    Parameters
    ----------
    data : array_like
        The series, in time order.
    max_lag : int, optional
        The largest lag; default Numerics' ``floor(min(10 log10(n), n - 1))``.
    kind : AutocorrelationType or str, optional
        ``CORRELATION`` (default), ``COVARIANCE`` or ``PARTIAL``.

    Returns
    -------
    lags : numpy.ndarray
        The lags (``0..max_lag``; ``1..max_lag`` for ``PARTIAL``), as integers.
    values : numpy.ndarray
        The function at those lags.

    Raises
    ------
    ValueError
        If Numerics returns no function: fewer than two values, a maximum lag below 1, or a
        zero-variance series for ``CORRELATION``.
    """
    stats = _stats()
    lag = -1 if max_lag is None else int(max_lag)
    table = stats.Autocorrelation.Function(to_net_doubles(data), lag, net_enum(stats.Autocorrelation.Type, kind))
    if table is None:
        raise ValueError("Autocorrelation.Function returned no function: the series needs at least two values, "
                         "a maximum lag of at least 1 and, for the correlation, a nonzero variance.")
    result = to_numpy_matrix(table)
    return result[:, 0].astype(np.int64), np.ascontiguousarray(result[:, 1])


__all__ = [
    "RunningStatistics", "autocorrelation", "column_percentiles", "linear_moments", "mean", "percentile",
    "plotting_position_percentile", "plotting_positions", "weighted_plotting_positions", "wilson_score_interval",
    "standard_deviation",
    "variance",
]
