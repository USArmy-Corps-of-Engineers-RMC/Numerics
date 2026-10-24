"""Wrappers over the generalized members: the plotting-position alpha table and its weighted typed overload.

Each test needs the member in the loaded ``Numerics.dll`` and is skipped with the member named
otherwise; on an older assembly the wrappers raise ``NotImplementedError`` for the new forms and keep
every existing form working.
"""

from __future__ import annotations

import numpy as np
import pytest

from rmc_numerics import statistics


def _bits(values: object) -> np.ndarray:
    return np.ascontiguousarray(values, dtype=np.float64).view(np.uint64)


def _has(owner: object, member: str) -> bool:
    return hasattr(owner, member)


def test_plotting_position_alpha_table_and_the_weighted_named_formulas() -> None:
    # Arrange
    pp = statistics._stats().PlottingPositions
    if not _has(pp, "Alpha"):
        with pytest.raises(NotImplementedError, match="PlottingPositions.Alpha"):
            statistics.plotting_position_alpha("hazen")
        pytest.skip("the loaded Numerics.dll has no PlottingPositions.Alpha")
    rng = np.random.default_rng(23)
    weights = rng.random(80) * 3.0
    table = {"weibull": 0.0, "median": 0.3175, "blom": 0.375, "cunnane": 0.4, "gringorten": 0.44, "hazen": 0.5}
    # Act / Assert
    for name, alpha in table.items():
        assert statistics.plotting_position_alpha(name) == alpha
        typed = statistics.weighted_plotting_positions(weights, name)
        assert _bits(typed).tolist() == _bits(statistics.weighted_plotting_positions(weights, alpha)).tolist()
        assert _bits(statistics.weighted_plotting_positions(np.ones(25), name)).tolist() == _bits(
            statistics.plotting_positions(25, name)).tolist()
    # Kaplan-Meier: the cumulative weight share, by NumPy (exact for these weights)
    km = statistics.weighted_plotting_positions([1.0, 3.0, 0.0, 4.0], "kaplan_meier")
    assert km.tolist() == (np.cumsum([1.0, 3.0, 0.0, 4.0]) / 8.0).tolist()
    assert _bits(statistics.weighted_plotting_positions(np.ones(30), "kaplan_meier")).tolist() == _bits(
        statistics.plotting_positions(30, "kaplan_meier")).tolist()
    with pytest.raises(ValueError, match="not a member"):
        statistics.weighted_plotting_positions([1.0], "nope")


def test_wilson_score_interval_keeps_its_tuple_shape_on_every_assembly() -> None:
    # Arrange / Act: 2.3 returns double[]; the interim build returned a (Lower, Upper) tuple
    bounds = statistics.wilson_score_interval(3.0, 17.0, 0.9)
    # Assert: the wrapper's shape is unchanged, and the values agree with the closed form by SciPy
    from scipy import stats as sps  # noqa: PLC0415

    assert isinstance(bounds, tuple) and len(bounds) == 2 and all(type(b) is float for b in bounds)
    z = sps.norm.ppf(0.95)
    p, n = 3.0 / 17.0, 17.0
    center = (p + z * z / (2 * n)) / (1 + z * z / n)
    half = z * np.sqrt(p * (1 - p) / n + z * z / (4 * n * n)) / (1 + z * z / n)
    np.testing.assert_allclose(bounds, (center - half, center + half), rtol=1e-12)
