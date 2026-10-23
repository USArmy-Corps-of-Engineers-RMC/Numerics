"""Wrappers over the Numerics members added in 2.3.0.

Each test needs the member in the loaded ``Numerics.dll``; against an assembly built before the
member existed the test is skipped with the member named, so the suite still runs on an
earlier interim build.
"""

from __future__ import annotations

import importlib

import numpy as np
import pytest

from rmc_numerics import statistics
from rmc_numerics.runtime import initialize


def _require(namespace: str, type_name: str, member: str) -> None:
    """Skip the calling test when the loaded assembly lacks ``type_name.member``."""
    initialize()
    owner = getattr(importlib.import_module(namespace), type_name)
    if not hasattr(owner, member):
        pytest.skip(f"the loaded Numerics.dll has no {type_name}.{member}")


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
