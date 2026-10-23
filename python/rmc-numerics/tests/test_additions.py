"""Wrappers over the Numerics members added in 2.3.0.

Each test needs the member in the loaded ``Numerics.dll``; against an assembly built before the
member existed the test is skipped with the member named, so the suite still runs on an
earlier interim build.
"""

from __future__ import annotations

import importlib

import numpy as np
import pytest

from rmc_numerics import distributions, statistics
from rmc_numerics.runtime import initialize


def _require(namespace: str, type_name: str, member: str, *parameters: str) -> None:
    """Skip the calling test when the loaded assembly lacks ``type_name.member``.

    With ``parameters`` (full .NET type names), the overload with exactly those parameter types
    is required.
    """
    initialize()
    import clr  # noqa: PLC0415
    import System  # noqa: PLC0415

    owner = clr.GetClrType(getattr(importlib.import_module(namespace), type_name))
    if parameters:
        types = System.Array[System.Type]([System.Type.GetType(name) for name in parameters])
        found = owner.GetMethod(member, types) is not None
    else:
        found = len(owner.GetMember(member)) > 0
    if not found:
        signature = f"({', '.join(parameters)})" if parameters else ""
        pytest.skip(f"the loaded Numerics.dll has no {type_name}.{member}{signature}")


def test_column_percentiles_equal_the_per_column_percentile_bitwise() -> None:
    # Arrange
    _require("Numerics.Data.Statistics", "Statistics", "ColumnPercentiles")
    rng = np.random.default_rng(7)
    data = 100.0 * np.exp(rng.standard_normal((300, 25)))
    data[rng.random(data.shape) < 0.05] = np.nan
    data[3, 4] = np.inf
    data[:, 7] = np.nan
    k = np.array([0.05, 0.25, 0.5, 0.75, 0.95])
    # Act
    table, excluded = statistics.column_percentiles(data, k)
    median, _ = statistics.column_percentiles(data, 0.5)
    # Assert
    assert table.shape == (5, 25) and excluded.dtype == np.int32
    for column in range(data.shape[1]):
        values = data[:, column]
        finite = values[np.isfinite(values)]
        assert excluded[column] == values.size - finite.size
        if finite.size == 0:
            assert np.all(np.isnan(table[:, column]))
            continue
        reference = statistics.percentile(finite, k)
        np.testing.assert_array_equal(table[:, column].view(np.uint64), reference.view(np.uint64))
    np.testing.assert_array_equal(median, table[2])


def test_generalized_pareto_fit_known_location_matches_the_reference_maximum() -> None:
    # Arrange
    _require("Numerics.Distributions", "GeneralizedPareto", "MLE",
             "System.Collections.Generic.IList`1[System.Double]", "System.Double")
    excesses = np.array([1.2, 0.4, 3.9, 2.2, 0.15, 5.6, 1.05, 0.8, 7.3, 2.9, 0.33, 1.7, 4.4, 0.62, 2.5, 11.2, 0.09,
                         1.33, 3.1, 0.95])
    # Act
    fitted = distributions.GeneralizedPareto.fit_known_location(excesses + 10.0, 10.0)
    # Assert (the reference maximizes the same likelihood with SciPy at xatol 1e-12)
    location, scale, shape = fitted.parameters
    assert location == 10.0
    assert scale == pytest.approx(2.388710825197821, rel=1e-3)
    assert shape == pytest.approx(-0.07721168825622311, abs=1e-3)
    assert fitted.log_likelihood(excesses + 10.0) == pytest.approx(-38.959310020261206, abs=1e-6)


def test_weighted_plotting_positions_reduce_to_the_unweighted_formula() -> None:
    # Arrange
    _require("Numerics.Data.Statistics", "PlottingPositions", "Function",
             "System.Collections.Generic.IList`1[System.Double]", "System.Double")
    weights = np.array([0.5, 2.0, 0.0, 1.25, 3.0])
    # Act
    positions = statistics.weighted_plotting_positions(weights, 0.44)
    unit = statistics.weighted_plotting_positions(np.ones(40), 0.44)
    # Assert
    cumulative = np.cumsum(weights)
    total = cumulative[-1]
    expected = (cumulative - 0.44 * weights) / (total + (1.0 - 2.0 * 0.44) * (total / weights.size))
    np.testing.assert_allclose(positions, expected, rtol=1e-15, atol=0.0)
    np.testing.assert_array_equal(unit.view(np.uint64), statistics.plotting_positions(40, 0.44).view(np.uint64))


def test_wilson_score_interval_known_values_and_operation_order() -> None:
    # Arrange
    _require("Numerics.Data.Statistics", "Statistics", "WilsonScoreInterval")
    import math

    # Act
    lower, upper = statistics.wilson_score_interval(5, 20, 0.9)
    # Assert (SciPy binomtest(5, 20).proportion_ci(0.9, method="wilson"))
    assert lower == pytest.approx(0.12737716852035774, abs=1e-12)
    assert upper == pytest.approx(0.4322017501601245, abs=1e-12)
    z = float(distributions.Normal(0.0, 1.0).inverse_cdf(0.5 + 0.8 / 2.0))
    p_hat = 12.5 / 37.25
    z2n = z * z / 37.25
    denominator = 1.0 + z2n
    center = (p_hat + z2n / 2.0) / denominator
    half = z * math.sqrt(p_hat * (1.0 - p_hat) / 37.25 + z2n / (4.0 * 37.25)) / denominator
    assert statistics.wilson_score_interval(12.5, 37.25, 0.8) == (max(0.0, center - half), min(1.0, center + half))
    assert statistics.wilson_score_interval(0, 10, 0.95)[0] == pytest.approx(0.0, abs=1e-15)


def test_weibull_plotting_position_percentile_agrees_with_numpy_type_6() -> None:
    # Arrange
    _require("Numerics.Data.Statistics", "Statistics", "Percentile",
             "System.Collections.Generic.IList`1[System.Double]", "System.Double",
             "Numerics.Data.Statistics.PlottingPositions+PlottingPostionType, Numerics", "System.Boolean")
    rng = np.random.default_rng(11)
    data = rng.gamma(2.0, 30.0, 47)
    k = np.linspace(1 / 48, 47 / 48, 200)
    # Act
    weibull = statistics.plotting_position_percentile(data, k, "weibull")
    scalar = statistics.plotting_position_percentile(data, 0.9, "weibull")
    hazen = statistics.plotting_position_percentile([3, 1, 4, 1, 5, 9, 2, 6], 0.9, "hazen")
    # Assert: NumPy forms the rank as n q + q - 1, Numerics as (n + 1) q, so the two agree to rounding.
    np.testing.assert_allclose(weibull, np.quantile(data, k, method="weibull"), rtol=1e-13, atol=0.0)
    assert scalar == statistics.plotting_position_percentile(data, [0.9], "weibull")[0]
    assert scalar == pytest.approx(np.quantile(data, 0.9, method="weibull"), rel=1e-13)
    assert hazen == pytest.approx(8.1, abs=1e-12)


def test_weighted_mean_curve_reproduces_the_unweighted_curve_at_equal_weights() -> None:
    # Arrange
    _require("Numerics.Distributions", "UncertaintyAnalysisResults", "ProcessMeanCurve",
             "Numerics.Distributions.UnivariateDistributionBase[], Numerics", "System.Double[]", "System.Double[]",
             "System.Double", "System.Double")
    from rmc_numerics.bootstrap import UncertaintyAnalysisResults

    members = [distributions.GeneralizedExtremeValue(100.0, 20.0, -0.1),
               distributions.GeneralizedExtremeValue(110.0, 25.0, 0.0),
               distributions.GeneralizedExtremeValue(95.0, 15.0, 0.1)]
    p = [0.01, 0.5, 0.99]
    # Act
    plain = UncertaintyAnalysisResults().process_mean_curve(members, p)
    equal = UncertaintyAnalysisResults().process_mean_curve(members, p, weights=[0.2, 0.2, 0.2])
    weighted = UncertaintyAnalysisResults().process_mean_curve(members, p, weights=[0.6, 0.3, 0.1])
    # Assert
    np.testing.assert_array_equal(equal.view(np.uint64), plain.view(np.uint64))
    assert not np.array_equal(weighted, plain)


def test_the_added_wrappers_refuse_invalid_input_as_value_errors() -> None:
    # Arrange
    _require("Numerics.Data.Statistics", "Statistics", "WilsonScoreInterval")
    from rmc_numerics.bootstrap import UncertaintyAnalysisResults
    from rmc_numerics.errors import net_errors

    members = [distributions.Normal(0.0, 1.0), distributions.Normal(1.0, 1.0)]
    calls = [
        lambda: statistics.column_percentiles(np.ones((3, 2)), [1.5]),
        lambda: distributions.GeneralizedPareto.fit_known_location([1.0, 2.0, -1.0], 0.0),
        lambda: statistics.weighted_plotting_positions([1.0, -1.0], 0.5),
        lambda: statistics.weighted_plotting_positions([1.0, 1.0], 0.75),
        lambda: statistics.wilson_score_interval(11, 10, 0.9),
        lambda: statistics.plotting_position_percentile([1.0, 2.0], 1.5, "weibull"),
        lambda: UncertaintyAnalysisResults().process_mean_curve(members, [0.5], weights=[1.0, -1.0]),
    ]
    # Act / Assert
    for call in calls:
        with pytest.raises(ValueError), net_errors():
            call()
