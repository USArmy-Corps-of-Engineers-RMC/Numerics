"""Python enums mirroring the Numerics enums the wrappers accept.

Every wrapper argument typed with one of these enums also accepts the member value or
the .NET member name as a string, matched case-insensitively with underscores ignored.
"""

from __future__ import annotations

import enum


class EstimationMethod(enum.Enum):
    """Parameter estimation methods (``Numerics.Distributions.ParameterEstimationMethod``)."""

    MAXIMUM_LIKELIHOOD = "MaximumLikelihood"
    METHOD_OF_MOMENTS = "MethodOfMoments"
    METHOD_OF_LINEAR_MOMENTS = "MethodOfLinearMoments"
    METHOD_OF_PERCENTILES = "MethodOfPercentiles"


class SortOrder(enum.Enum):
    """Sort orders (``Numerics.Data.SortOrder``)."""

    ASCENDING = "Ascending"
    DESCENDING = "Descending"
    NONE = "None"


class Transform(enum.Enum):
    """Interpolation transforms (``Numerics.Data.Transform``)."""

    NONE = "None"
    LOGARITHMIC = "Logarithmic"
    NORMAL_Z = "NormalZ"


class Extrapolation(enum.Enum):
    """Extrapolation sides (``Numerics.Data.ExtrapolationSides``)."""

    NONE = "None"
    BELOW = "Below"
    ABOVE = "Above"
    BOTH = "Both"


class PlottingPosition(enum.Enum):
    """Plotting-position formulas (``PlottingPositions.PlottingPostionType``)."""

    WEIBULL = "Weibull"
    MEDIAN = "Median"
    BLOM = "Blom"
    CUNNANE = "Cunnane"
    GRINGORTEN = "Gringorten"
    HAZEN = "Hazen"
    KAPLAN_MEIER = "KaplanMeier"


class ImportanceDistribution(enum.Enum):
    """Stratification importance distributions (``Stratify.ImportanceDistribution``)."""

    GUMBEL = "Gumbel"
    NORMAL = "Normal"
    UNIFORM = "Uniform"
    LOG10_UNIFORM = "Log10Uniform"


class WeightType(enum.Enum):
    """Weight interpretations for weighted statistics (``Numerics.Data.Statistics.WeightType``)."""

    FREQUENCY = "Frequency"
    RELIABILITY = "Reliability"
