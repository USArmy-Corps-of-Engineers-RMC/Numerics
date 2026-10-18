"""Typed wrappers call Numerics and return the values Numerics computes."""

from __future__ import annotations

import math

import numpy as np
import pytest

from rmc_numerics import (
    bootstrap,
    copulas,
    data,
    distributions,
    mathematics,
    multivariate,
    sampling,
    statistics,
)

NUMERICS_2_2_0_SHA256 = "364d2296a6024643a8bf0f60d1eaa8f5be5511563dea0b5d572074e45c27ea48"
"""SHA-256 of Numerics.dll in the RMC.Numerics 2.2.0 NuGet package (Clone loses the probability order there)."""


def test_standard_z_known_values_and_array_overload() -> None:
    # Arrange
    p = np.array([0.5, 0.975, 0.025, 0.999])
    # Act
    z = distributions.standard_z(p)
    # Assert
    np.testing.assert_allclose(z, [0.0, 1.959963984540054, -1.959963984540054, 3.090232306167813], atol=1e-12)
    assert z.shape == p.shape
    for value, expected in zip(p, z, strict=True):
        assert distributions.standard_z(float(value)) == expected  # scalar and array paths agree bitwise
    np.testing.assert_allclose(distributions.standard_cdf(z), p, atol=1e-14)


def test_stratify_value_and_probability_bins() -> None:
    # Act
    xbins = sampling.stratify_x_values(0.0, 10.0, 5)
    pbins = sampling.stratify_probabilities(0.001, 0.999, 4, importance="uniform", is_exhaustive=False)
    # Assert
    np.testing.assert_allclose(xbins.lower, [0, 2, 4, 6, 8])
    np.testing.assert_allclose(xbins.upper, [2, 4, 6, 8, 10])
    np.testing.assert_allclose(xbins.midpoint, [1, 3, 5, 7, 9])
    assert len(pbins) == 4
    np.testing.assert_allclose(pbins.lower[0], 0.001)
    np.testing.assert_allclose(pbins.upper[-1], 0.999)
    np.testing.assert_allclose(pbins.weight, pbins.upper - pbins.lower, atol=1e-15)


def test_latin_hypercube_one_point_per_stratum_and_reproducible() -> None:
    # Act
    design = sampling.latin_hypercube(10, dimension=3, seed=12345)
    again = sampling.latin_hypercube(10, dimension=3, seed=12345)
    # Assert
    assert design.shape == (10, 3)
    np.testing.assert_array_equal(design, again)
    for column in design.T:
        assert sorted(np.floor(column * 10).astype(int).tolist()) == list(range(10))


def test_gev_quantiles_through_array_overload_match_closed_form() -> None:
    # Arrange
    xi, alpha, kappa = 10.0, 2.0, -0.1
    gev = distributions.GeneralizedExtremeValue(xi, alpha, kappa)
    p = np.array([0.01, 0.5, 0.9, 0.99, 0.999])
    # Act
    q = gev.inverse_cdf(p)
    # Assert
    np.testing.assert_allclose(q, xi + alpha / kappa * (1.0 - (-np.log(p)) ** kappa), rtol=1e-12)
    for value, expected in zip(p, q, strict=True):
        assert gev.inverse_cdf(float(value)) == expected
    np.testing.assert_allclose(gev.cdf(q), p, rtol=1e-12)
    np.testing.assert_allclose(gev.ccdf(q), 1.0 - p, rtol=1e-10)
    sample = gev.generate_random_values(100, seed=7)
    np.testing.assert_array_equal(sample, gev.generate_random_values(100, seed=7))


def test_empirical_survival_orientation_against_hand_values() -> None:
    # Arrange: exceedance probabilities S(x) strictly descending; linear interpolation in both spaces
    dist = distributions.Empirical.survival([100.0, 200.0, 300.0], [0.5, 0.1, 0.01])
    dist.probability_transform = "none"
    # Act / Assert
    assert dist.probability_order == "Descending"
    assert dist.cdf(200.0) == 1.0 - 0.1
    assert dist.cdf(150.0) == pytest.approx(1.0 - 0.3, abs=1e-15)  # S(150) = (0.5 + 0.1) / 2
    assert dist.cdf(250.0) == pytest.approx(1.0 - 0.055, abs=1e-15)
    assert dist.inverse_cdf(0.7) == pytest.approx(150.0, abs=1e-10)  # reads S = 0.3 on the exceedance axis
    assert dist.inverse_cdf(0.9) == pytest.approx(200.0, abs=1e-10)
    np.testing.assert_allclose(dist.inverse_cdf(np.array([0.7, 0.945])), [150.0, 250.0], atol=1e-10)


def test_empirical_survival_clone_keeps_orientation() -> None:
    # Arrange
    import System

    import rmc_numerics

    dist = distributions.Empirical.survival([100.0, 200.0, 300.0], [0.5, 0.1, 0.01])
    clone = dist.clone()
    # Act / Assert
    if rmc_numerics.numerics_identity().sha256 == NUMERICS_2_2_0_SHA256:
        # Numerics 2.2.0 Clone rebuilds an ascending table, so the survival clone is invalid
        # (defect G3, fixed on this branch); the pin flips when the bundled assembly moves.
        with pytest.raises(System.ArgumentOutOfRangeException):
            clone.cdf(250.0)
    else:
        assert clone.probability_order == "Descending"
        assert clone.cdf(250.0) == dist.cdf(250.0)


def test_empirical_from_sample_uses_plotting_positions() -> None:
    # Act
    dist = distributions.Empirical.from_sample([3.0, 1.0, 2.0, 4.0], "weibull")
    # Assert
    np.testing.assert_array_equal(dist.x_values, [1.0, 2.0, 3.0, 4.0])
    np.testing.assert_allclose(dist.probability_values, [0.2, 0.4, 0.6, 0.8])
    np.testing.assert_allclose(statistics.plotting_positions(4, "weibull"), [0.2, 0.4, 0.6, 0.8])
    np.testing.assert_allclose(statistics.plotting_positions(4, 0.5), [0.125, 0.375, 0.625, 0.875])


def test_uncertainty_analysis_results_on_a_tiny_ensemble() -> None:
    # Arrange: five normals with means 10..14; their medians are the means
    members = [distributions.Normal(10.0 + i, 1.0) for i in range(5)]
    uar = bootstrap.UncertaintyAnalysisResults()
    # Act
    ci = uar.process_confidence_intervals(members, [0.5], alpha=0.5)
    mean_curve = uar.process_mean_curve(members, [0.5])
    mode = uar.process_mode_curve(distributions.Normal(12.0, 1.0), [0.5])
    full = bootstrap.UncertaintyAnalysisResults.from_ensemble(distributions.Normal(12.0, 1.0), members, [0.1, 0.5])
    # Assert: Type-7 percentiles of {10, ..., 14} at 0.25 and 0.75
    np.testing.assert_allclose(ci, [[11.0, 13.0]], atol=1e-12)
    assert mean_curve[0] == pytest.approx(12.0, abs=1e-2)  # the symmetric mixture's median
    assert mode[0] == pytest.approx(12.0, abs=1e-12)
    assert full.confidence_intervals.shape == (2, 2)
    assert full.mean_curve.shape == (2,)


def test_bootstrap_analysis_smoke() -> None:
    # Arrange
    parent = distributions.Normal(100.0, 15.0)
    analysis = bootstrap.BootstrapAnalysis(parent, "method_of_moments", sample_size=50, replications=200, seed=12345)
    p = [0.1, 0.5, 0.9]
    # Act
    replicates = analysis.distributions()
    quantiles = analysis.quantiles(p)
    ci = analysis.percentile_quantile_ci(p, alpha=0.1)
    results = analysis.estimate(p, alpha=0.1)
    # Assert
    assert len(replicates) == 200
    assert isinstance(replicates[0], distributions.Normal)
    assert quantiles.shape == (200, 3)
    assert ci.shape == (3, 2) and np.all(ci[:, 0] < ci[:, 1])
    assert results.confidence_intervals.shape == (3, 2)
    assert analysis.failed_replications == 0


def test_statistics_mean_and_percentiles() -> None:
    # Arrange
    values = [4.0, 1.0, 3.0, 2.0, 5.0]
    # Act / Assert
    assert statistics.mean(values) == 3.0
    assert statistics.mean([1.0, 3.0], weights=[3.0, 1.0]) == 1.5
    assert statistics.percentile(values, 0.25) == 2.0
    np.testing.assert_allclose(statistics.percentile(values, [0.1, 0.9]), [1.4, 4.6])
    np.testing.assert_allclose(statistics.percentile(values, [0.1, 0.9], weights=np.ones(5)), [1.4, 4.6])
    assert statistics.variance(values) == pytest.approx(2.5)
    running = statistics.RunningStatistics(values)
    running.push(6.0)
    assert running.count == 6
    assert running.mean == pytest.approx(3.5)
    assert running.variance == pytest.approx(np.var([1, 2, 3, 4, 5, 6], ddof=1))
    merged = statistics.RunningStatistics.combine(statistics.RunningStatistics([1.0, 2.0]),
                                                  statistics.RunningStatistics([3.0]))
    assert merged.count == 3 and merged.mean == pytest.approx(2.0)


def test_paired_data_and_linear_interpolation() -> None:
    # Arrange
    opd = data.OrderedPairedData([0.0, 1.0, 2.0], [0.0, 10.0, 40.0])
    linear = data.Linear([1.0, 10.0, 100.0], [0.0, 1.0, 2.0], x_transform="logarithmic")
    # Act / Assert
    assert opd.is_valid and len(opd) == 3
    np.testing.assert_allclose(opd.get_y_from_x([0.5, 1.5, 5.0]), [5.0, 25.0, 40.0])
    assert opd.get_x_from_y(25.0) == pytest.approx(1.5)
    assert opd.get_y_from_x(3.0, extrapolation="both") == pytest.approx(70.0)
    np.testing.assert_allclose(linear([math.sqrt(10.0), 1000.0]), [0.5, 2.0], atol=1e-12)


def test_beta_poisson_mixture_and_elementary_functions() -> None:
    # Act / Assert
    assert distributions.Beta(2.0, 1.0).cdf(0.5) == pytest.approx(0.25, abs=1e-12)
    assert distributions.Beta(2.0, 3.0).inverse_cdf(distributions.Beta(2.0, 3.0).cdf(0.3)) == pytest.approx(0.3)
    assert distributions.Poisson(1.0).cdf(2.0) == pytest.approx(math.exp(-1.0) * 2.5, abs=1e-12)
    mixture = distributions.Mixture([0.5, 0.5], [distributions.Normal(0, 1), distributions.Normal(0, 1)])
    assert mixture.cdf(1.0) == pytest.approx(distributions.Normal(0, 1).cdf(1.0), abs=1e-12)
    assert mathematics.expm1(1e-10) == pytest.approx(1e-10 + 5e-21, rel=1e-15)
    np.testing.assert_allclose(mathematics.log1p(np.array([1e-12, 1.0])), [np.log1p(1e-12), np.log(2.0)], rtol=1e-15)


def test_other_families_construct_and_evaluate() -> None:
    # Arrange
    families = [
        distributions.GeneralizedPareto(0.0, 1.0, 0.1),
        distributions.Gumbel(10.0, 2.0),
        distributions.LogNormal(1.0, 0.2),
        distributions.LnNormal(10.0, 2.0),
        distributions.PearsonTypeIII(10.0, 2.0, 0.5),
        distributions.LogPearsonTypeIII(1.0, 0.2, 0.3),
        distributions.KappaFour(10.0, 2.0, 0.1, -0.2),
    ]
    p = np.array([0.1, 0.5, 0.9])
    for dist in families:
        # Act
        q = dist.inverse_cdf(p)
        # Assert
        assert dist.parameters_valid, dist.display_name
        np.testing.assert_allclose(dist.cdf(q), p, atol=1e-6, err_msg=dist.display_name)
    gumbel = distributions.Gumbel(10.0, 2.0)
    assert gumbel.inverse_cdf(0.5) == pytest.approx(10.0 - 2.0 * math.log(math.log(2.0)), rel=1e-12)
    # LogNormal is base 10: the median is 10**mean_of_log
    assert distributions.LogNormal(1.0, 0.2).inverse_cdf(0.5) == pytest.approx(10.0, rel=1e-12)


def test_estimate_fits_in_place() -> None:
    # Arrange
    sample = distributions.Normal(50.0, 5.0).generate_random_values(500, seed=3)
    fitted = distributions.Normal()
    # Act
    fitted.estimate(sample, "method_of_moments")
    # Assert
    assert fitted.mean == pytest.approx(float(np.mean(sample)), rel=1e-12)


def test_brent_root_and_bracket() -> None:
    # Act / Assert
    assert mathematics.brent_solve(lambda x: x * x - 2.0, 0.0, 2.0, tolerance=1e-12) == pytest.approx(math.sqrt(2.0))
    found, lo, hi, f1, f2 = mathematics.brent_bracket(lambda x: x - 5.0, 0.0, 1.0)
    assert found and lo <= 5.0 <= hi and f1 * f2 <= 0.0


def test_multivariate_normal() -> None:
    # Arrange
    mvn = multivariate.MultivariateNormal([1.0, 2.0], [[1.0, 0.5], [0.5, 2.0]])
    # Act / Assert
    assert mvn.dimension == 2
    np.testing.assert_allclose(mvn.covariance, [[1.0, 0.5], [0.5, 2.0]])
    np.testing.assert_allclose(mvn.inverse_cdf([0.5, 0.5]), [1.0, 2.0], atol=1e-12)
    sample = mvn.generate_random_values(2000, seed=11)
    assert sample.shape == (2000, 2)
    np.testing.assert_allclose(sample.mean(axis=0), [1.0, 2.0], atol=0.15)
    assert mvn.latin_hypercube_random_values(10, 5).shape == (10, 2)
    assert mvn.pdf([1.0, 2.0]) > 0.0


def test_copulas() -> None:
    # Act / Assert
    assert copulas.IndependenceCopula().cdf(0.3, 0.4) == pytest.approx(0.12, abs=1e-15)
    assert copulas.NormalCopula(0.0).cdf(0.3, 0.4) == pytest.approx(0.12, abs=1e-6)
    clayton = copulas.ClaytonCopula(2.0)
    np.testing.assert_allclose(clayton.cdf([0.3, 0.5], 0.4), [(0.3 ** -2 + 0.4 ** -2 - 1) ** -0.5,
                                                               (0.5 ** -2 + 0.4 ** -2 - 1) ** -0.5], rtol=1e-12)
    for copula in (copulas.GumbelCopula(1.5), copulas.FrankCopula(3.0), copulas.JoeCopula(1.5),
                   copulas.AMHCopula(0.3), copulas.StudentTCopula(0.5, 4.0)):
        sample = copula.generate_random_values(50, seed=5)
        assert sample.shape == (50, 2) and np.all((sample > 0) & (sample < 1))


def test_mersenne_twister_streams_agree() -> None:
    # Act
    bulk = sampling.MersenneTwister(12345).next_doubles(5)
    rng = sampling.MersenneTwister(12345)
    single = [rng.next_double() for _ in range(5)]
    # Assert
    np.testing.assert_array_equal(bulk, single)
    assert np.all((bulk >= 0.0) & (bulk < 1.0))
