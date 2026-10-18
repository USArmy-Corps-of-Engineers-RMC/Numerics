"""Univariate distributions from ``Numerics.Distributions``.

Every class wraps one .NET distribution object (``.net``). Evaluation members accept a
scalar or any array: a scalar calls the scalar member, an array crosses once as a
``System.Double[]`` into the ``UnivariateDistributionBase`` array overloads
(``PDF(IList<double>)``, ``CDF(IList<double>)``, ``InverseCDF(IList<double>)``), and
random generation uses ``GenerateRandomValues(n, seed)``, so no numerical work happens
in Python. Parameters follow the Numerics conventions (for example the generalized
extreme value shape is Hosking's kappa, and ``LogNormal`` is base 10 while ``LnNormal``
is natural-log with real-space mean and standard deviation).
"""

from __future__ import annotations

from collections.abc import Sequence
from typing import Any

import numpy as np

from rmc_numerics._util import bind, evaluate, loop, namespace, net_enum, unwrap
from rmc_numerics.enums import (
    EstimationMethod,
    Extrapolation,
    PlottingPosition,
    SortOrder,
    Transform,
)
from rmc_numerics.marshal import to_net_doubles, to_numpy_doubles, to_numpy_matrix


def _nd() -> Any:
    """Return the ``Numerics.Distributions`` namespace after starting the runtime."""
    return namespace("Numerics.Distributions")


class UnivariateDistribution:
    """A wrapped ``Numerics.Distributions.UnivariateDistributionBase``.

    Parameters
    ----------
    net : Numerics.Distributions.UnivariateDistributionBase
        The .NET distribution object; interface-typed values are unwrapped to their
        concrete type.

    Attributes
    ----------
    net : Numerics.Distributions.UnivariateDistributionBase
        The wrapped .NET object, for passing back into Numerics members.
    """

    def __init__(self, net: Any) -> None:
        self.net = unwrap(net)

    def _scalar(self, name: str) -> Any:
        """Return the ``double`` overload of a scalar member, bound once per instance (see ``_util.bind``)."""
        cache = self.__dict__.setdefault("_bound", {})
        member = cache.get(name)
        if member is None:
            member = cache[name] = bind(getattr(self.net, name))
        return member

    def __repr__(self) -> str:
        return f"{type(self).__name__}({', '.join(repr(float(p)) for p in self.parameters)})"

    @property
    def display_name(self) -> str:
        """The Numerics display name."""
        return str(self.net.DisplayName)

    @property
    def parameters(self) -> np.ndarray:
        """The parameter values in Numerics order (``GetParameters``)."""
        return to_numpy_doubles(self.net.GetParameters)

    @property
    def parameters_valid(self) -> bool:
        """Whether the current parameters are valid (``ParametersValid``)."""
        return bool(self.net.ParametersValid)

    @property
    def mean(self) -> float:
        """The distribution mean."""
        return float(self.net.Mean)

    @property
    def standard_deviation(self) -> float:
        """The distribution standard deviation."""
        return float(self.net.StandardDeviation)

    @property
    def skewness(self) -> float:
        """The distribution skewness."""
        return float(self.net.Skewness)

    @property
    def kurtosis(self) -> float:
        """The distribution kurtosis."""
        return float(self.net.Kurtosis)

    @property
    def minimum(self) -> float:
        """The lower end of the support."""
        return float(self.net.Minimum)

    @property
    def maximum(self) -> float:
        """The upper end of the support."""
        return float(self.net.Maximum)

    def pdf(self, x: Any) -> Any:
        """Evaluate the probability density (or mass) function.

        Parameters
        ----------
        x : float or array_like
            Values at which to evaluate.

        Returns
        -------
        float or numpy.ndarray
            Densities, shaped like ``x``.
        """
        return evaluate(x, self._scalar("PDF"), self.net.PDF)

    def cdf(self, x: Any) -> Any:
        """Evaluate the cumulative distribution function (non-exceedance probability).

        Parameters
        ----------
        x : float or array_like
            Values at which to evaluate.

        Returns
        -------
        float or numpy.ndarray
            Non-exceedance probabilities, shaped like ``x``.
        """
        return evaluate(x, self._scalar("CDF"), self.net.CDF)

    def ccdf(self, x: Any) -> Any:
        """Evaluate the complementary CDF (exceedance probability) through ``CCDF``.

        Numerics has no array overload of ``CCDF``, so arrays are evaluated element by
        element; the arithmetic is the distribution's own ``CCDF``.

        Parameters
        ----------
        x : float or array_like
            Values at which to evaluate.

        Returns
        -------
        float or numpy.ndarray
            Exceedance probabilities, shaped like ``x``.
        """
        return loop(x, self._scalar("CCDF"))

    def inverse_cdf(self, probability: Any) -> Any:
        """Evaluate the quantile function at non-exceedance probabilities.

        Parameters
        ----------
        probability : float or array_like
            Non-exceedance probabilities in [0, 1].

        Returns
        -------
        float or numpy.ndarray
            Quantiles, shaped like ``probability``.
        """
        return evaluate(probability, self._scalar("InverseCDF"), self.net.InverseCDF)

    def log_pdf(self, x: Any) -> Any:
        """Evaluate the natural logarithm of the density through ``LogPDF``.

        Numerics has no array overload of ``LogPDF``, so arrays are evaluated element by
        element; the arithmetic is the distribution's own ``LogPDF``.

        Parameters
        ----------
        x : float or array_like
            Values at which to evaluate.

        Returns
        -------
        float or numpy.ndarray
            Log densities, shaped like ``x``.
        """
        return loop(x, self._scalar("LogPDF"))

    def log_cdf(self, x: Any) -> Any:
        """Evaluate the natural logarithm of the CDF through ``LogCDF``.

        Evaluated element by element (Numerics has no array overload of ``LogCDF``).

        Parameters
        ----------
        x : float or array_like
            Values at which to evaluate.

        Returns
        -------
        float or numpy.ndarray
            Log non-exceedance probabilities, shaped like ``x``.
        """
        return loop(x, self._scalar("LogCDF"))

    def log_ccdf(self, x: Any) -> Any:
        """Evaluate the natural logarithm of the complementary CDF through ``LogCCDF``.

        Evaluated element by element (Numerics has no array overload of ``LogCCDF``).

        Parameters
        ----------
        x : float or array_like
            Values at which to evaluate.

        Returns
        -------
        float or numpy.ndarray
            Log exceedance probabilities, shaped like ``x``.
        """
        return loop(x, self._scalar("LogCCDF"))

    @property
    def type_name(self) -> str:
        """The ``UnivariateDistributionType`` member name, for example ``"GeneralizedExtremeValue"``."""
        return str(self.net.Type)

    @property
    def parameter_names(self) -> tuple[str, ...]:
        """The parameter property names in ``GetParameters`` order (``GetParameterPropertyNames``)."""
        return tuple(str(name) for name in self.net.GetParameterPropertyNames)

    @property
    def number_of_parameters(self) -> int:
        """The number of parameters (``NumberOfParameters``)."""
        return int(self.net.NumberOfParameters)

    def set_parameters(self, parameters: Any) -> None:
        """Set the parameters in place through ``SetParameters(IList<double>)``.

        Invalid values are accepted and recorded, as in Numerics: ``parameters_valid``
        becomes False and the evaluation members raise.

        Parameters
        ----------
        parameters : array_like
            The parameters in Numerics order.
        """
        self.net.SetParameters(to_net_doubles(parameters))

    def validate_parameters(self, parameters: Any) -> str | None:
        """Return why a parameter vector is invalid for this family, or None when it is valid.

        Calls ``ValidateParameters(IList<double>, false)`` and returns the message of the
        exception Numerics builds without throwing it.

        Parameters
        ----------
        parameters : array_like
            A parameter vector in Numerics order.

        Returns
        -------
        str or None
            The Numerics message, or None for a valid vector.
        """
        error = self.net.ValidateParameters(to_net_doubles(parameters), False)
        return None if error is None else str(error.Message)

    @property
    def has_parameter_covariance(self) -> bool:
        """Whether the family implements ``IStandardError`` (an asymptotic parameter covariance)."""
        nd = _nd()
        import clr  # noqa: PLC0415

        return bool(clr.GetClrType(nd.IStandardError).IsInstanceOfType(self.net))

    def parameter_covariance(self, sample_size: int,
                             method: EstimationMethod | str = EstimationMethod.MAXIMUM_LIKELIHOOD) -> np.ndarray:
        """Return the asymptotic parameter covariance through ``IStandardError.ParameterCovariance``.

        Parameters
        ----------
        sample_size : int
            The sample size n; the covariance scales as 1/n.
        method : EstimationMethod or str, optional
            The estimation method whose covariance is returned; default maximum likelihood.

        Returns
        -------
        numpy.ndarray
            Array of shape ``(number_of_parameters, number_of_parameters)``.

        Raises
        ------
        TypeError
            If the distribution does not implement ``IStandardError``.
        """
        nd = _nd()
        if not self.has_parameter_covariance:
            raise TypeError(f"{self.display_name} exposes no parameter covariance.")
        covariance = nd.IStandardError(self.net).ParameterCovariance(int(sample_size),
                                                                     net_enum(nd.ParameterEstimationMethod, method))
        return to_numpy_matrix(covariance)

    def parameter_set_quantiles(self, parameter_sets: Any, probability: float) -> np.ndarray:
        """Return one quantile per parameter set, each from this family at that set.

        A clone of this distribution receives every row through ``SetParameters`` and
        returns ``InverseCDF(probability)``: the probe pattern a .NET caller uses for many
        parameter sets, with the scalar overloads bound once. Numerics has no member that
        takes a parameter matrix, so the rows cross one at a time.

        Parameters
        ----------
        parameter_sets : array_like
            Array of shape ``(count, number_of_parameters)``.
        probability : float
            The non-exceedance probability.

        Returns
        -------
        numpy.ndarray
            ``count`` quantiles.

        Raises
        ------
        ValueError
            If the rows do not have ``number_of_parameters`` columns.
        """
        rows = np.atleast_2d(np.asarray(parameter_sets, dtype=np.float64))
        arity = self.number_of_parameters
        if rows.shape[1] != arity:
            raise ValueError(f"Expected {arity} parameters per row; got {rows.shape[1]}.")
        probe = self.net.Clone()
        quantile = bind(probe.InverseCDF)
        p = float(probability)
        out = np.empty(rows.shape[0], dtype=np.float64)
        setter = _typed_member(probe, "SetParameters", arity) if arity > 1 else None
        for index, row in enumerate(rows.tolist()):
            if setter is not None:
                setter(probe, *row)
            else:
                probe.SetParameters(to_net_doubles(row))
            out[index] = quantile(p)
        return out

    def parameter_set_validity(self, parameter_sets: Any) -> np.ndarray:
        """Return which parameter sets this family accepts (``ValidateParameters(row, false)`` is null).

        The rows cross one at a time; a family with a ``ValidateParameters(double, ..., bool)``
        overload takes each row without an array copy.

        Parameters
        ----------
        parameter_sets : array_like
            Array of shape ``(count, number_of_parameters)``.

        Returns
        -------
        numpy.ndarray
            Boolean array of length ``count``, True where the row is valid.

        Raises
        ------
        ValueError
            If the rows do not have ``number_of_parameters`` columns.
        """
        rows = np.atleast_2d(np.asarray(parameter_sets, dtype=np.float64))
        arity = self.number_of_parameters
        if rows.shape[1] != arity:
            raise ValueError(f"Expected {arity} parameters per row; got {rows.shape[1]}.")
        out = np.empty(rows.shape[0], dtype=bool)
        typed = _typed_member(self.net, "ValidateParameters", arity, flag=True)
        for index, row in enumerate(rows.tolist()):
            if typed is not None:
                out[index] = typed(self.net, *row, False) is None
            else:
                out[index] = self.net.ValidateParameters(to_net_doubles(row), False) is None
        return out

    def parameter_set_distributions(self, parameter_sets: Any, valid: Any = None) -> Any:
        """Build one distribution of this family per parameter set, as a .NET array.

        Each member is a ``Clone`` of this distribution with ``SetParameters(row)``, so fixed
        settings that are not parameters (a ``LogNormal`` base) carry over. The result stays
        on the .NET side for the members that take ``UnivariateDistributionBase[]``
        (``UncertaintyAnalysisResults``, ``BootstrapAnalysis.Quantiles``); rows marked False
        in ``valid`` are null entries, which those members treat as missing.

        Parameters
        ----------
        parameter_sets : array_like
            Array of shape ``(count, number_of_parameters)``.
        valid : array_like of bool, optional
            Which rows to build; None builds every row.

        Returns
        -------
        System.Array[Numerics.Distributions.UnivariateDistributionBase]
            ``count`` members, null where ``valid`` is False.

        Raises
        ------
        ValueError
            If the rows do not have ``number_of_parameters`` columns or ``valid`` has another length.
        """
        rows = np.atleast_2d(np.asarray(parameter_sets, dtype=np.float64))
        arity = self.number_of_parameters
        if rows.shape[1] != arity:
            raise ValueError(f"Expected {arity} parameters per row; got {rows.shape[1]}.")
        keep = np.ones(rows.shape[0], dtype=bool) if valid is None else np.asarray(valid, dtype=bool)
        if keep.shape != (rows.shape[0],):
            raise ValueError("valid must hold one flag per row.")
        nd = _nd()
        import System  # noqa: PLC0415 - the runtime is running (self.net exists)

        members = System.Array.CreateInstance(nd.UnivariateDistributionBase, rows.shape[0])
        clone = self.net.Clone
        setter = _typed_member(self.net, "SetParameters", arity) if arity > 1 else None
        for index, row in enumerate(rows.tolist()):
            if not keep[index]:
                continue
            member = clone()
            if setter is not None:
                setter(member, *row)
            else:
                member.SetParameters(to_net_doubles(row))
            members[index] = member
        return members

    def to_xml(self) -> str:
        """Return the distribution's ``ToXElement()`` as XML text.

        Returns
        -------
        str
            The element as .NET writes it (``XElement.ToString``).
        """
        return str(self.net.ToXElement().ToString())

    def generate_random_values(self, sample_size: int, seed: int = -1) -> np.ndarray:
        """Draw a Monte Carlo sample through ``GenerateRandomValues``.

        Parameters
        ----------
        sample_size : int
            Number of values.
        seed : int, optional
            Mersenne Twister seed; zero or negative seeds from the clock (not
            reproducible).

        Returns
        -------
        numpy.ndarray
            The sample.
        """
        return to_numpy_doubles(self.net.GenerateRandomValues(int(sample_size), int(seed)))

    def log_likelihood(self, sample: Any) -> float:
        """Return the log-likelihood of a sample (``LogLikelihood(IList<double>)``).

        Parameters
        ----------
        sample : array_like
            Observations.

        Returns
        -------
        float
            The log-likelihood.
        """
        return float(self.net.LogLikelihood(to_net_doubles(sample)))

    def estimate(self, sample: Any, method: EstimationMethod | str = EstimationMethod.MAXIMUM_LIKELIHOOD) -> None:
        """Fit the parameters in place through ``IEstimation.Estimate``.

        Parameters
        ----------
        sample : array_like
            Observations.
        method : EstimationMethod or str, optional
            Estimation method; default maximum likelihood.

        Raises
        ------
        TypeError
            If the distribution does not implement ``IEstimation``.
        """
        nd = _nd()
        import clr  # noqa: PLC0415

        if not clr.GetClrType(nd.IEstimation).IsInstanceOfType(self.net):
            raise TypeError(f"{self.display_name} does not support parameter estimation.")
        nd.IEstimation(self.net).Estimate(to_net_doubles(sample), net_enum(nd.ParameterEstimationMethod, method))

    def clone(self) -> UnivariateDistribution:
        """Return a deep copy through ``Clone``.

        Returns
        -------
        UnivariateDistribution
            A wrapper of the cloned .NET object, of the same Python class when possible.
        """
        return wrap(self.net.Clone())


def _typed_member(net: Any, name: str, arity: int, flag: bool = False) -> Any:
    """Return the unbound ``name(double x arity[, bool])`` overload of ``net``'s concrete type, or None.

    Called as ``member(instance, *values)``. Binding the overload once on the type, rather than
    resolving the method group on every call, is what makes the per-row loops of
    ``parameter_set_*`` cheap; families without such an overload use the ``IList<double>`` one.
    """
    import System  # noqa: PLC0415 - the runtime is running (net exists)

    types = (System.Double,) * arity + ((System.Boolean,) if flag else ())
    try:
        return getattr(type(net), name).Overloads[types if len(types) > 1 else types[0]]
    except (AttributeError, KeyError, TypeError):
        return None


_CLASS_BY_NET_NAME: dict[str, type[UnivariateDistribution]] = {}


def wrap(net: Any) -> UnivariateDistribution:
    """Wrap a .NET univariate distribution in the matching Python class.

    Parameters
    ----------
    net : Numerics.Distributions.UnivariateDistributionBase or IUnivariateDistribution
        The .NET object.

    Returns
    -------
    UnivariateDistribution
        The specific wrapper class when one exists, otherwise the generic wrapper.
    """
    concrete = unwrap(net)
    cls = _CLASS_BY_NET_NAME.get(str(concrete.GetType().Name), UnivariateDistribution)
    instance = cls.__new__(cls)
    UnivariateDistribution.__init__(instance, concrete)
    return instance


def _register(net_name: str):
    """Register a wrapper class for a .NET class name, for :func:`wrap`."""

    def decorator(cls: type[UnivariateDistribution]) -> type[UnivariateDistribution]:
        _CLASS_BY_NET_NAME[net_name] = cls
        return cls

    return decorator


@_register("GeneralizedExtremeValue")
class GeneralizedExtremeValue(UnivariateDistribution):
    """Generalized extreme value distribution (Hosking parameterization).

    Parameters
    ----------
    location : float
        Location ξ.
    scale : float
        Scale α > 0.
    shape : float
        Shape κ (Hosking sign: κ < 0 is heavy-tailed).
    """

    def __init__(self, location: float, scale: float, shape: float) -> None:
        super().__init__(_nd().GeneralizedExtremeValue(float(location), float(scale), float(shape)))


@_register("GeneralizedPareto")
class GeneralizedPareto(UnivariateDistribution):
    """Generalized Pareto distribution (Hosking parameterization).

    Parameters
    ----------
    location : float
        Location ξ.
    scale : float
        Scale α > 0.
    shape : float
        Shape κ (Hosking sign).
    """

    def __init__(self, location: float, scale: float, shape: float) -> None:
        super().__init__(_nd().GeneralizedPareto(float(location), float(scale), float(shape)))


@_register("Gumbel")
class Gumbel(UnivariateDistribution):
    """Gumbel (extreme value type I) distribution.

    Parameters
    ----------
    location : float
        Location ξ.
    scale : float
        Scale α > 0.
    """

    def __init__(self, location: float, scale: float) -> None:
        super().__init__(_nd().Gumbel(float(location), float(scale)))


@_register("Normal")
class Normal(UnivariateDistribution):
    """Normal distribution.

    Parameters
    ----------
    mean : float, optional
        Mean μ; default 0.
    standard_deviation : float, optional
        Standard deviation σ > 0; default 1.
    """

    def __init__(self, mean: float = 0.0, standard_deviation: float = 1.0) -> None:
        super().__init__(_nd().Normal(float(mean), float(standard_deviation)))


@_register("LogNormal")
class LogNormal(UnivariateDistribution):
    """Log-normal distribution in base 10 (``Numerics.Distributions.LogNormal``).

    Parameters
    ----------
    mean_of_log : float
        Mean of log10(X).
    standard_deviation_of_log : float
        Standard deviation of log10(X).
    """

    def __init__(self, mean_of_log: float, standard_deviation_of_log: float) -> None:
        super().__init__(_nd().LogNormal(float(mean_of_log), float(standard_deviation_of_log)))


@_register("LnNormal")
class LnNormal(UnivariateDistribution):
    """Natural-log normal distribution with real-space moments (``LnNormal``).

    Parameters
    ----------
    mean : float
        Real-space mean of X.
    standard_deviation : float
        Real-space standard deviation of X.
    """

    def __init__(self, mean: float, standard_deviation: float) -> None:
        super().__init__(_nd().LnNormal(float(mean), float(standard_deviation)))

    @staticmethod
    def direct_method_of_moments(mean: float, standard_deviation: float) -> tuple[float, float]:
        """Return (mu, sigma) of ln X for real-space moments (``LnNormal.DirectMethodOfMoments``).

        Parameters
        ----------
        mean : float
            The mean of X.
        standard_deviation : float
            The standard deviation of X.

        Returns
        -------
        tuple of float
            ``(mu, sigma)`` as Numerics computes them, before the ``Sigma`` setter's clamp.
        """
        values = _nd().LnNormal.DirectMethodOfMoments(float(mean), float(standard_deviation))
        return float(values[0]), float(values[1])

    @classmethod
    def from_log_parameters(cls, mu: float, sigma: float) -> LnNormal:
        """Create the distribution from mu and sigma of ln X (the ``Mu`` and ``Sigma`` setters).

        Parameters
        ----------
        mu : float
            The mean of ln X.
        sigma : float
            The standard deviation of ln X.

        Returns
        -------
        LnNormal
            The distribution; Numerics derives the real-space moments from mu and sigma.
        """
        net = _nd().LnNormal()
        net.Mu = float(mu)
        net.Sigma = float(sigma)
        instance = cls.__new__(cls)
        UnivariateDistribution.__init__(instance, net)
        return instance


@_register("PearsonTypeIII")
class PearsonTypeIII(UnivariateDistribution):
    """Pearson type III distribution.

    Parameters
    ----------
    mean : float
        Mean μ.
    standard_deviation : float
        Standard deviation σ > 0.
    skew : float
        Skewness γ.
    """

    def __init__(self, mean: float, standard_deviation: float, skew: float) -> None:
        super().__init__(_nd().PearsonTypeIII(float(mean), float(standard_deviation), float(skew)))


@_register("LogPearsonTypeIII")
class LogPearsonTypeIII(UnivariateDistribution):
    """Log-Pearson type III distribution (base-10 logarithms).

    Parameters
    ----------
    mean_of_log : float
        Mean of log10(X).
    standard_deviation_of_log : float
        Standard deviation of log10(X).
    skew_of_log : float
        Skewness of log10(X).
    """

    def __init__(self, mean_of_log: float, standard_deviation_of_log: float, skew_of_log: float) -> None:
        super().__init__(_nd().LogPearsonTypeIII(float(mean_of_log), float(standard_deviation_of_log),
                                                 float(skew_of_log)))


@_register("KappaFour")
class KappaFour(UnivariateDistribution):
    """Four-parameter kappa distribution (Hosking).

    Parameters
    ----------
    location : float
        Location ξ.
    scale : float
        Scale α > 0.
    shape : float
        Shape κ.
    shape2 : float
        Second shape h.
    """

    def __init__(self, location: float, scale: float, shape: float, shape2: float) -> None:
        super().__init__(_nd().KappaFour(float(location), float(scale), float(shape), float(shape2)))


@_register("Uniform")
class Uniform(UnivariateDistribution):
    """Continuous uniform distribution on [minimum, maximum] (``Uniform``).

    Parameters
    ----------
    minimum : float, optional
        Lower end; default 0.
    maximum : float, optional
        Upper end; default 1.
    """

    def __init__(self, minimum: float = 0.0, maximum: float = 1.0) -> None:
        super().__init__(_nd().Uniform(float(minimum), float(maximum)))


@_register("BetaDistribution")
class Beta(UnivariateDistribution):
    """Beta distribution on [0, 1] (``BetaDistribution``).

    Parameters
    ----------
    alpha : float
        First shape α > 0.
    beta : float
        Second shape β > 0.
    """

    def __init__(self, alpha: float, beta: float) -> None:
        super().__init__(_nd().BetaDistribution(float(alpha), float(beta)))


@_register("Poisson")
class Poisson(UnivariateDistribution):
    """Poisson distribution (discrete; ``pdf`` is the probability mass).

    Parameters
    ----------
    rate : float
        Rate λ > 0.
    """

    def __init__(self, rate: float) -> None:
        super().__init__(_nd().Poisson(float(rate)))


@_register("Mixture")
class Mixture(UnivariateDistribution):
    """Finite mixture of univariate distributions.

    Parameters
    ----------
    weights : array_like
        Component weights.
    distributions : sequence of UnivariateDistribution
        Components, one per weight.
    """

    def __init__(self, weights: Any, distributions: Sequence[UnivariateDistribution]) -> None:
        nd = _nd()
        import System  # noqa: PLC0415

        components = System.Array[nd.UnivariateDistributionBase]([d.net for d in distributions])
        super().__init__(nd.Mixture(to_net_doubles(weights), components))

    @property
    def weights(self) -> np.ndarray:
        """The component weights (``Weights``)."""
        return to_numpy_doubles(self.net.Weights)

    @property
    def components(self) -> list[UnivariateDistribution]:
        """The component distributions, wrapped (``Distributions``)."""
        return [wrap(component) for component in self.net.Distributions]


def _data() -> Any:
    """Return the ``Numerics.Data`` namespace after starting the runtime."""
    return namespace("Numerics.Data")


@_register("EmpiricalDistribution")
class Empirical(UnivariateDistribution):
    """Empirical distribution defined by value and probability tables.

    With the default orders the probabilities are non-exceedance probabilities
    (strictly ascending). With ``probability_order="descending"`` the table is a
    survival function: the probabilities are exceedance probabilities S(x) (strictly
    descending), ``cdf`` returns 1 − S(x) and ``inverse_cdf(q)`` reads the exceedance
    axis at 1 − q. Interpolation is in (X, Φ⁻¹(P)) space by default
    (``probability_transform="normal_z"``) and holds the endpoints.

    Parameters
    ----------
    values : array_like
        Ascending X values (ties allowed).
    probabilities : array_like
        Probability values in [0, 1], strictly monotone in ``probability_order``.
    x_order : SortOrder or str, optional
        Sort order of ``values``; must be ascending (the default).
    probability_order : SortOrder or str, optional
        ``"ascending"`` (non-exceedance, default) or ``"descending"`` (survival).

    Notes
    -----
    Uses ``EmpiricalDistribution(IList, IList, SortOrder, SortOrder)``.
    """

    def __init__(self, values: Any, probabilities: Any, x_order: SortOrder | str = SortOrder.ASCENDING,
                 probability_order: SortOrder | str = SortOrder.ASCENDING) -> None:
        nd = _nd()
        data = _data()
        super().__init__(nd.EmpiricalDistribution(to_net_doubles(values), to_net_doubles(probabilities),
                                                  net_enum(data.SortOrder, x_order),
                                                  net_enum(data.SortOrder, probability_order)))

    @classmethod
    def survival(cls, values: Any, exceedance: Any) -> Empirical:
        """Build a survival-orientation table from exceedance probabilities.

        Parameters
        ----------
        values : array_like
            Ascending X values.
        exceedance : array_like
            Strictly descending exceedance probabilities S(x).

        Returns
        -------
        Empirical
            The distribution with ``probability_order`` descending.
        """
        return cls(values, exceedance, SortOrder.ASCENDING, SortOrder.DESCENDING)

    @classmethod
    def from_sample(cls, sample: Any,
                    plotting_position: PlottingPosition | str = PlottingPosition.WEIBULL) -> Empirical:
        """Build the distribution of a sample with plotting-position probabilities.

        Parameters
        ----------
        sample : array_like
            Observations; sorted by Numerics.
        plotting_position : PlottingPosition or str, optional
            Plotting-position formula; default Weibull.

        Returns
        -------
        Empirical
            ``EmpiricalDistribution(IList, PlottingPostionType)``.
        """
        nd = _nd()
        stats = namespace("Numerics.Data.Statistics")
        kind = net_enum(stats.PlottingPositions.PlottingPostionType, plotting_position)
        instance = cls.__new__(cls)
        UnivariateDistribution.__init__(instance, nd.EmpiricalDistribution(to_net_doubles(sample), kind))
        return instance

    @classmethod
    def from_paired_data(cls, paired_data: Any) -> Empirical:
        """Build the distribution from ordered paired data, keeping its orders and strictness.

        Parameters
        ----------
        paired_data : rmc_numerics.data.OrderedPairedData or Numerics.Data.OrderedPairedData
            X ascending; Y the probabilities.

        Returns
        -------
        Empirical
            ``EmpiricalDistribution(OrderedPairedData)``.
        """
        net = getattr(paired_data, "net", paired_data)
        instance = cls.__new__(cls)
        UnivariateDistribution.__init__(instance, _nd().EmpiricalDistribution(net))
        return instance

    @property
    def x_values(self) -> np.ndarray:
        """The X table."""
        return np.array([float(v) for v in self.net.XValues], dtype=np.float64)

    @property
    def probability_values(self) -> np.ndarray:
        """The probability table as stored (exceedance for a survival table)."""
        return np.array([float(v) for v in self.net.ProbabilityValues], dtype=np.float64)

    @property
    def probability_order(self) -> str:
        """The probability sort order: ``"Ascending"``, ``"Descending"`` or ``"None"``."""
        return str(self.net.ProbabilityOrder)

    @property
    def x_transform(self) -> str:
        """The X interpolation transform; settable with a :class:`Transform` or name."""
        return str(self.net.XTransform)

    @x_transform.setter
    def x_transform(self, value: Transform | str) -> None:
        self.net.XTransform = net_enum(_data().Transform, value)

    @property
    def probability_transform(self) -> str:
        """The probability interpolation transform (default ``NormalZ``); settable."""
        return str(self.net.ProbabilityTransform)

    @probability_transform.setter
    def probability_transform(self, value: Transform | str) -> None:
        self.net.ProbabilityTransform = net_enum(_data().Transform, value)

    @property
    def extrapolation(self) -> str:
        """The X-space extrapolation sides (default ``None``: endpoint hold); settable."""
        return str(self.net.Extrapolation)

    @extrapolation.setter
    def extrapolation(self, value: Extrapolation | str) -> None:
        self.net.Extrapolation = net_enum(_data().ExtrapolationSides, value)


def create_distribution(type_name: str, parameters: Any = None) -> UnivariateDistribution:
    """Create a distribution by type name through ``UnivariateDistributionFactory``.

    ``CreateDistribution(UnivariateDistributionType)`` builds the family at its default
    parameters; ``parameters``, when given, are then set through ``SetParameters``.
    Invalid parameters are recorded, not refused (``parameters_valid`` is False).

    Parameters
    ----------
    type_name : str
        A ``UnivariateDistributionType`` member name, for example ``"Gumbel"``; matched
        case-insensitively with underscores ignored.
    parameters : array_like, optional
        The parameters in Numerics order.

    Returns
    -------
    UnivariateDistribution
        The wrapped distribution (its specific wrapper class when one exists).

    Raises
    ------
    ValueError
        If ``type_name`` is not a member of ``UnivariateDistributionType``.
    """
    nd = _nd()
    net = nd.UnivariateDistributionFactory.CreateDistribution(net_enum(nd.UnivariateDistributionType, type_name))
    if parameters is not None:
        unwrap(net).SetParameters(to_net_doubles(parameters))
    return wrap(net)


def standard_z(probability: Any) -> Any:
    """Return standard normal quantiles through ``Normal.StandardZ``.

    Parameters
    ----------
    probability : float or array_like
        Non-exceedance probabilities; 0 and 1 map to ±``double.Epsilon`` probabilities
        as in Numerics.

    Returns
    -------
    float or numpy.ndarray
        Standard normal deviates, shaped like ``probability``.
    """
    n = _nd().Normal
    return evaluate(probability, _static("StandardZ", n), n.StandardZ)


def standard_cdf(z: Any) -> Any:
    """Return the standard normal CDF through ``Normal.StandardCDF``.

    Parameters
    ----------
    z : float or array_like
        Standard normal deviates.

    Returns
    -------
    float or numpy.ndarray
        Non-exceedance probabilities, shaped like ``z``.
    """
    n = _nd().Normal
    return evaluate(z, _static("StandardCDF", n), n.StandardCDF)


_STATIC_BOUND: dict[str, Any] = {}


def _static(name: str, owner: Any) -> Any:
    """Return a static member's ``double`` overload, bound once per process (see ``_util.bind``)."""
    member = _STATIC_BOUND.get(name)
    if member is None:
        member = _STATIC_BOUND[name] = bind(getattr(owner, name))
    return member


def standard_pdf(z: Any) -> Any:
    """Return the standard normal density through ``Normal.StandardPDF``.

    Parameters
    ----------
    z : float or array_like
        Standard normal deviates.

    Returns
    -------
    float or numpy.ndarray
        Densities, shaped like ``z``.
    """
    n = _nd().Normal
    return evaluate(z, n.StandardPDF, n.StandardPDF)


__all__ = [
    "Beta", "Empirical", "EstimationMethod", "Extrapolation", "GeneralizedExtremeValue", "GeneralizedPareto",
    "Gumbel", "KappaFour", "LnNormal", "LogNormal", "LogPearsonTypeIII", "Mixture", "Normal", "PearsonTypeIII",
    "PlottingPosition", "Poisson", "SortOrder", "Transform", "UnivariateDistribution", "standard_cdf",
    "standard_pdf", "standard_z", "wrap",
    "Uniform", "create_distribution",
]
