# rmc-numerics

RMC.Numerics for Python. The wheel ships the `Numerics.dll` assembly from the published
`RMC.Numerics` NuGet package, byte for byte, together with:

- the single CoreCLR loader (`rmc_numerics.runtime`): one .NET runtime and one `Numerics`
  assembly per Python process, with `add_assembly` for dependent assemblies such as
  `Hydrologics.dll` or `RMC.BestFit.dll`;
- bit-exact bulk marshaling between NumPy and .NET arrays (`rmc_numerics.marshal`);
- the load identity (`rmc_numerics.numerics_identity`): the loaded assembly's version and
  file SHA-256;
- thin typed wrappers over Numerics: univariate distributions (GEV, GPD, Gumbel, Normal,
  base-10 `LogNormal`, `LnNormal`, Pearson III, log-Pearson III, Kappa-4, Mixture, Beta,
  Poisson, Empirical incl. survival orientation), the multivariate normal, the Dirichlet
  (moments, density and `GenerateRandomValues` draws), bivariate copulas, `OrderedPairedData` and `Linear`, `Statistics` and `RunningStatistics`,
  plotting positions and L-moments, stratification and Latin hypercube designs, the Mersenne
  Twister, Brent root finding and `BrentSearch`, `Tools.Expm1`/`Log1p`, the incomplete beta
  function, `Matrix` products, `MatrixRegularization` and the Cholesky factor, link
  functions, `UncertaintyAnalysisResults` and `BootstrapAnalysis`;
- per-parameter-set members for ensembles held as parameter rows: one quantile per row
  (`parameter_set_quantiles`), one validity flag per row (`parameter_set_validity`) and one
  distribution per row as a .NET array the ensemble summaries take directly
  (`parameter_set_distributions`), with the scalar overloads bound once per row; and the
  quantile matrix of many probabilities under many rows with their validity flags in one
  call into `UnivariateDistributionBase.ParameterSetQuantiles` (`parameter_set_quantile_table`,
  RMC.Numerics 2.3 and later).

The wrappers do no numerical work in Python: array inputs cross once into the Numerics
array overloads (for example `UnivariateDistributionBase.InverseCDF(IList<double>)`), and
random generation uses the Numerics generators.

The distribution version equals the RMC.Numerics version of the bundled assembly.

## Requirements

- CPython 3.10 to 3.14 on Linux or Windows.
- The .NET 10 runtime (`Microsoft.NETCore.App` 10.0). Set `DOTNET_ROOT` when it is not
  installed in the default location.

## Use

```python
import numpy as np
from rmc_numerics import distributions

gev = distributions.GeneralizedExtremeValue(location=10.0, scale=2.0, shape=-0.1)
gev.inverse_cdf(np.array([0.5, 0.99, 0.999]))   # one call into InverseCDF(IList<double>)

survival = distributions.Empirical.survival([100.0, 200.0, 300.0], [0.5, 0.1, 0.01])
survival.cdf(200.0)                              # 1 - S(200) = 0.9
```

Import `rmc_numerics` before anything else that uses pythonnet. A bare `import clr` before
the loader runs starts the legacy .NET Framework on Windows, which cannot be undone in that
process; the loader refuses to continue in that state.

The assembly directory is, in order: `rmc_numerics.initialize(dll_dir)`, the
`RMC_NUMERICS_DLL_DIR` environment variable, then the packaged `_bin`.

## Interim: a locally built Numerics.dll (until RMC.Numerics 2.3.0 ships)

This branch pins one locally built `Numerics.dll` instead of a NuGet package assembly:
`_version.py` names its source commit (`NUMERICS_SOURCE_COMMIT`), its SHA-256, the
informational version `2.3.0-dev.1` and the assembly version `2.2.0.0`, which stays at the
2.2.0 value so consumer pins do not move. Build it once, on Windows, from a clean committed tree:

```bash
dotnet build Numerics/Numerics.csproj -c Release -f net10.0 -p:Version=2.3.0-dev.1 -p:ContinuousIntegrationBuild=true
python scripts/build_wheel.py --stage-only --dll ../../Numerics/bin/Release/net10.0/Numerics.dll
```

`ContinuousIntegrationBuild` maps source paths, so any checkout of that commit builds the same
bytes on Windows; a Linux build differs (the embedded PDB), so Linux and Docker use the
Windows-built file. `--download` and the `PythonWheel.yml` CI job cannot find a `2.3.0-dev.1`
package: a branch carrying these pins (including a local `main` it is merged into) must not be
pushed until the pins move to the published 2.3.0 package.

## Build and test

From `python/rmc-numerics`:

```bash
python scripts/build_wheel.py --stage-only       # stage _bin/Numerics.dll from the NuGet package
python -m pip install -e ".[test]"
python -m pytest
python scripts/build_wheel.py                    # build dist/rmc_numerics-<version>-py3-none-any.whl
```

The script takes `Numerics.dll` from `lib/net10.0` of the `RMC.Numerics` package in the
NuGet global packages folder (or `--nupkg PATH`, or `--download` from nuget.org) and refuses
any file whose SHA-256 differs from `NUMERICS_SHA256` in `src/rmc_numerics/_version.py`.
To test against a locally built assembly, set `RMC_NUMERICS_DLL_DIR` to
`Numerics/bin/Release/net10.0`.
