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
