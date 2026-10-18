"""Bulk marshaling keeps every bit, including NaN payloads, signed zeros and subnormals."""

from __future__ import annotations

import numpy as np
import pytest

from rmc_numerics import marshal

PATTERNS = np.array(
    [
        0x7FF8000000000001,  # quiet NaN with a payload
        0xFFF8DEADBEEF0001,  # negative quiet NaN with a payload
        0x7FF0000000000001,  # signalling NaN pattern
        0x8000000000000000,  # -0.0
        0x0000000000000000,  # +0.0
        0x0000000000000001,  # smallest subnormal
        0x000FFFFFFFFFFFFF,  # largest subnormal
        0x7FF0000000000000,  # +inf
        0xFFF0000000000000,  # -inf
        0x7FEFFFFFFFFFFFFF,  # largest finite
        0x3FB999999999999A,  # 0.1
    ],
    dtype=np.uint64,
)


def test_doubles_round_trip_bit_exact() -> None:
    # Arrange
    values = PATTERNS.view(np.float64)
    # Act
    back = marshal.to_numpy_doubles(marshal.to_net_doubles(values))
    # Assert
    np.testing.assert_array_equal(back.view(np.uint64), PATTERNS)


def test_doubles_are_bit_exact_inside_net() -> None:
    # Arrange
    values = PATTERNS.view(np.float64)
    # Act
    net = marshal.to_net_doubles(values)
    import System  # resolvable once the runtime has started

    bits = [int(System.BitConverter.DoubleToInt64Bits(net[i])) & 0xFFFFFFFFFFFFFFFF for i in range(net.Length)]
    # Assert
    assert bits == [int(p) for p in PATTERNS]


def test_empty_and_scalar_inputs() -> None:
    # Act / Assert
    assert marshal.to_numpy_doubles(marshal.to_net_doubles([])).size == 0
    assert marshal.to_numpy_doubles(marshal.to_net_doubles(2.5)).tolist() == [2.5]
    with pytest.raises(ValueError, match="1-D"):
        marshal.to_net_doubles(np.zeros((2, 2)))


def test_matrix_round_trip_bit_exact_and_row_major() -> None:
    # Arrange
    values = PATTERNS[:10].view(np.float64).reshape(2, 5)
    # Act
    net = marshal.to_net_matrix(values)
    back = marshal.to_numpy_matrix(net)
    # Assert
    assert (int(net.GetLength(0)), int(net.GetLength(1))) == (2, 5)
    assert np.float64(net[1, 3]).view(np.uint64) == values[1, 3].view(np.uint64)
    np.testing.assert_array_equal(back.view(np.uint64), values.view(np.uint64))


def test_ints_round_trip_and_range_refusal() -> None:
    # Arrange
    values = np.array([-(2**31), -1, 0, 1, 2**31 - 1], dtype=np.int64)
    # Act / Assert
    np.testing.assert_array_equal(marshal.to_numpy_ints(marshal.to_net_ints(values)), values.astype(np.int32))
    with pytest.raises(OverflowError):
        marshal.to_net_ints([2**31])
    with pytest.raises(TypeError):
        marshal.to_net_ints(np.array(["a"]))


def test_singles_block_round_trip_c_order() -> None:
    # Arrange
    block = np.arange(24, dtype=np.float32).reshape(2, 3, 4)
    block[1, 2, 3] = np.float32(np.nan)
    # Act
    flat, dims = marshal.to_net_singles(block)
    back = marshal.to_numpy_singles(flat, dims)
    # Assert
    assert dims == (2, 3, 4)
    assert float(flat[(1 * 3 + 0) * 4 + 2]) == float(block[1, 0, 2])
    np.testing.assert_array_equal(back.view(np.uint32), block.view(np.uint32))
