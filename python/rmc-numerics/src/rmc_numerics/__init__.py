"""RMC.Numerics for Python: the Numerics assembly, the single CoreCLR loader and typed wrappers.

Importing the package does not start the .NET runtime; the first wrapper call does, or
call :func:`initialize` explicitly. Submodules:

``runtime``
    The CoreCLR loader (one runtime and one ``Numerics`` assembly per process) and
    :func:`add_assembly` for dependent assemblies.
``marshal``
    Bit-exact bulk copies between NumPy and .NET arrays.
``identity``
    The version and SHA-256 of the loaded ``Numerics`` assembly.
``distributions``, ``multivariate``, ``copulas``
    Distributions; array inputs use the Numerics array overloads.
``data``, ``statistics``, ``sampling``, ``mathematics``, ``bootstrap``
    Paired data and interpolation, statistics, sampling designs, root finding and
    elementary functions, and uncertainty analysis.
``errors``
    ``net_errors``: the Numerics argument and arithmetic exceptions as Python exceptions.
"""

from rmc_numerics._version import (
    NUMERICS_ASSEMBLY_VERSION,
    NUMERICS_SHA256,
    NUMERICS_VERSION,
    __version__,
)
from rmc_numerics.identity import (
    NumericsIdentity,
    assert_numerics_identity,
    numerics_identity,
)
from rmc_numerics.runtime import (
    RuntimeLoadError,
    add_assembly,
    get_runtime_info,
    initialize,
    is_initialized,
    loaded_assemblies,
    runtime_dir,
)

__all__ = [
    "NUMERICS_ASSEMBLY_VERSION", "NUMERICS_SHA256", "NUMERICS_VERSION", "NumericsIdentity", "RuntimeLoadError",
    "__version__", "add_assembly", "assert_numerics_identity", "get_runtime_info", "initialize", "is_initialized",
    "loaded_assemblies", "numerics_identity", "runtime_dir",
]
