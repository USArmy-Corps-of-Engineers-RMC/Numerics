"""Sampling designs from ``Numerics.Sampling``: stratification, Latin hypercube, Mersenne Twister.

Stratification bins come back as a :class:`StratificationBins` record of NumPy arrays
holding the .NET bin list (``.net``), which can be passed back to Numerics members that
take ``List<StratificationBin>``.
"""

from __future__ import annotations

from dataclasses import dataclass, field
from typing import Any

import numpy as np

from rmc_numerics._util import namespace, net_enum
from rmc_numerics.enums import ImportanceDistribution
from rmc_numerics.marshal import to_numpy_doubles, to_numpy_ints, to_numpy_matrix


def _sampling() -> Any:
    """Return the ``Numerics.Sampling`` namespace after starting the runtime."""
    return namespace("Numerics.Sampling")


@dataclass(frozen=True)
class StratificationBin:
    """One stratification bin as Numerics holds it (``StratificationBin``).

    Parameters
    ----------
    lower_bound : float
        Lower edge.
    upper_bound : float
        Upper edge.
    weight : float
        Weight (probability mass); Numerics sets the bin width for value bins built
        without one.
    midpoint : float
        ``Midpoint`` as Numerics computes it, (lower + upper) / 2.
    """

    lower_bound: float
    upper_bound: float
    weight: float
    midpoint: float

    def contains(self, x: float) -> bool:
        """Return whether ``lower_bound <= x < upper_bound``.

        Parameters
        ----------
        x : float
            The value.

        Returns
        -------
        bool
            True when the value lies in the half-open bin.
        """
        return self.lower_bound <= x < self.upper_bound


@dataclass(frozen=True)
class StratificationBins:
    """Stratification bins as arrays.

    Parameters
    ----------
    lower : numpy.ndarray
        Lower bound of each bin.
    upper : numpy.ndarray
        Upper bound of each bin.
    weight : numpy.ndarray
        Weight (probability mass) of each bin.
    net : System.Collections.Generic.List[Numerics.Sampling.StratificationBin]
        The .NET bin list the arrays were read from.
    """

    lower: np.ndarray
    upper: np.ndarray
    weight: np.ndarray
    net: Any = field(repr=False, compare=False)

    @property
    def midpoint(self) -> np.ndarray:
        """Bin midpoints, (lower + upper) / 2 as Numerics computes them."""
        return np.array([float(b.Midpoint) for b in self.net], dtype=np.float64)

    def __len__(self) -> int:
        return int(self.lower.size)

    @property
    def bins(self) -> tuple[StratificationBin, ...]:
        """The bins as :class:`StratificationBin` records, in Numerics order."""
        return tuple(StratificationBin(float(b.LowerBound), float(b.UpperBound), float(b.Weight), float(b.Midpoint))
                     for b in self.net)

    @classmethod
    def from_net(cls, bins: Any) -> StratificationBins:
        """Read a .NET ``List<StratificationBin>``.

        Parameters
        ----------
        bins : System.Collections.Generic.List[Numerics.Sampling.StratificationBin]
            The .NET bins.

        Returns
        -------
        StratificationBins
            The arrays and the original list.
        """
        lower = np.array([float(b.LowerBound) for b in bins], dtype=np.float64)
        upper = np.array([float(b.UpperBound) for b in bins], dtype=np.float64)
        weight = np.array([float(b.Weight) for b in bins], dtype=np.float64)
        return cls(lower, upper, weight, bins)


def _options(lower: float, upper: float, number_of_bins: int, is_probability: bool) -> Any:
    """Build ``StratificationOptions``."""
    return _sampling().StratificationOptions(float(lower), float(upper), int(number_of_bins), bool(is_probability))


def stratify_x_values(lower: float, upper: float, number_of_bins: int,
                      is_logarithmic: bool = False) -> StratificationBins:
    """Stratify a value range into equal-width bins (``Stratify.XValues``).

    Parameters
    ----------
    lower, upper : float
        Range to stratify.
    number_of_bins : int
        Number of bins.
    is_logarithmic : bool, optional
        Equal widths in log10 space; default False.

    Returns
    -------
    StratificationBins
        The bins; Numerics leaves their weights unset (−1) for value bins.
    """
    s = _sampling()
    return StratificationBins.from_net(s.Stratify.XValues(_options(lower, upper, number_of_bins, False),
                                                          bool(is_logarithmic)))


def stratify_x_values_many(options: Any, is_logarithmic: bool = False) -> StratificationBins:
    """Stratify several value ranges into one bin list (``Stratify.XValues(List<StratificationOptions>)``).

    Parameters
    ----------
    options : sequence of (float, float, int)
        One ``(lower, upper, number_of_bins)`` triple per range.
    is_logarithmic : bool, optional
        Equal widths in log10 space; default False.

    Returns
    -------
    StratificationBins
        The bins of every range, in the order Numerics returns them.
    """
    s = _sampling()
    from System.Collections.Generic import List  # noqa: PLC0415 - the runtime is started by _sampling

    items = List[s.StratificationOptions]()
    for lower, upper, number_of_bins in options:
        items.Add(_options(lower, upper, number_of_bins, False))
    return StratificationBins.from_net(s.Stratify.XValues(items, bool(is_logarithmic)))


def stratify_probabilities(lower: float, upper: float, number_of_bins: int,
                           importance: ImportanceDistribution | str = ImportanceDistribution.UNIFORM,
                           is_exhaustive: bool = True) -> StratificationBins:
    """Stratify a non-exceedance probability range (``Stratify.Probabilities``).

    Parameters
    ----------
    lower, upper : float
        Probability range in [0, 1].
    number_of_bins : int
        Number of bins.
    importance : ImportanceDistribution or str, optional
        Space in which the bins are equally wide; default uniform.
    is_exhaustive : bool, optional
        Whether the end bins absorb the remaining probability; default True.

    Returns
    -------
    StratificationBins
        The bins.
    """
    s = _sampling()
    kind = net_enum(s.Stratify.ImportanceDistribution, importance)
    return StratificationBins.from_net(s.Stratify.Probabilities(_options(lower, upper, number_of_bins, True), kind,
                                                                bool(is_exhaustive)))


def stratify_exceedance_probabilities(lower: float, upper: float, number_of_bins: int,
                                      importance: ImportanceDistribution | str = ImportanceDistribution.UNIFORM,
                                      is_exhaustive: bool = True) -> StratificationBins:
    """Stratify an exceedance probability range (``Stratify.ExceedanceProbabilities``).

    Parameters
    ----------
    lower, upper : float
        Exceedance probability range in [0, 1].
    number_of_bins : int
        Number of bins.
    importance : ImportanceDistribution or str, optional
        Space in which the bins are equally wide; default uniform.
    is_exhaustive : bool, optional
        Whether the end bins absorb the remaining probability; default True.

    Returns
    -------
    StratificationBins
        The bins.
    """
    s = _sampling()
    kind = net_enum(s.Stratify.ImportanceDistribution, importance)
    return StratificationBins.from_net(s.Stratify.ExceedanceProbabilities(
        _options(lower, upper, number_of_bins, True), kind, bool(is_exhaustive)))


def latin_hypercube(sample_size: int, dimension: int = 1, seed: int = -1, median: bool = False) -> np.ndarray:
    """Draw a Latin hypercube design on the unit cube (``LatinHypercube.Random``/``Median``).

    Parameters
    ----------
    sample_size : int
        Number of rows.
    dimension : int, optional
        Number of columns; default 1.
    seed : int, optional
        Seed; zero or negative seeds from the clock (not reproducible).
    median : bool, optional
        Use bin midpoints instead of uniform positions within bins; default False.

    Returns
    -------
    numpy.ndarray
        Array of shape ``(sample_size, dimension)``.
    """
    lhs = _sampling().LatinHypercube
    method = lhs.Median if median else lhs.Random
    return to_numpy_matrix(method(int(sample_size), int(dimension), int(seed)))


class MersenneTwister:
    """The Numerics Mersenne Twister (``Numerics.Sampling.MersenneTwister``).

    Exposed for cross-checks of NumPy re-implementations against the .NET stream.

    Parameters
    ----------
    seed : int
        Seed.

    Attributes
    ----------
    net : Numerics.Sampling.MersenneTwister
        The wrapped .NET generator.
    """

    def __init__(self, seed: int) -> None:
        self.net = _sampling().MersenneTwister(int(seed))

    def next_double(self) -> float:
        """Return the next double in [0, 1) (``NextDouble``).

        Returns
        -------
        float
            The draw.
        """
        return float(self.net.NextDouble())

    def next_doubles(self, length: int) -> np.ndarray:
        """Return the next doubles (``ExtensionMethods.NextDoubles``).

        Parameters
        ----------
        length : int
            Number of draws.

        Returns
        -------
        numpy.ndarray
            The draws.
        """
        root = namespace("Numerics")
        return to_numpy_doubles(root.ExtensionMethods.NextDoubles(self.net, int(length)))

    def next_integers(self, length: int) -> np.ndarray:
        """Return the next non-negative integers (``ExtensionMethods.NextIntegers``).

        Parameters
        ----------
        length : int
            Number of draws.

        Returns
        -------
        numpy.ndarray
            The draws as int32.
        """
        root = namespace("Numerics")
        return to_numpy_ints(root.ExtensionMethods.NextIntegers(self.net, int(length)))

    def gen_rand_int32(self) -> int:
        """Return the next raw 32-bit output (``GenRandInt32``).

        Returns
        -------
        int
            An unsigned 32-bit integer.
        """
        return int(self.net.GenRandInt32())


__all__ = [
    "MersenneTwister", "StratificationBin", "StratificationBins", "latin_hypercube",
    "stratify_exceedance_probabilities", "stratify_probabilities", "stratify_x_values", "stratify_x_values_many",
]
