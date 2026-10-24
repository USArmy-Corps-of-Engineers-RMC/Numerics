"""Version of the rmc-numerics distribution and the identity of the Numerics assembly it ships.

Interim (until RMC.Numerics 2.3.0 is published): the wheel ships one locally built ``Numerics.dll``
from the release branch instead of a NuGet package assembly. It was built once, on Windows, from a
clean committed tree with::

    dotnet build Numerics/Numerics.csproj -c Release -f net10.0 -p:Version=2.3.0-dev.2
        -p:ContinuousIntegrationBuild=true

(.NET SDK 10.0.400 on its bundled runtime 10.0.11), and is staged with ``scripts/build_wheel.py --stage-only --dll PATH``. Its
assembly version stays 2.2.0.0, so no consumer pin moves in the interim; the SHA-256 and the
informational version tell it apart. At the release every value below moves to the published
2.3.0 package and ``NUMERICS_SOURCE_COMMIT`` becomes empty.
"""

__version__ = "2.3.0.dev2"
"""The distribution version; it equals the RMC.Numerics version of the bundled assembly (PEP 440 form)."""

NUMERICS_VERSION = "2.3.0-dev.2"
"""The RMC.Numerics version of the bundled ``Numerics.dll`` (a NuGet package version, or the local build's)."""

NUMERICS_ASSEMBLY_VERSION = "2.2.0.0"
"""The assembly version recorded in the bundled ``Numerics.dll`` (2.2.0.0 for the interim local build)."""

NUMERICS_SHA256 = "209e7dbdc65f8ebe30f8254eb385d45c1dbc60bb562bea5d572d0399f5d4e921"
"""SHA-256 of the bundled ``Numerics.dll`` (the interim local build of ``NUMERICS_SOURCE_COMMIT``)."""

NUMERICS_SOURCE_COMMIT = "2660f1a589cc5460953885e08a990be39d6f27a9"
"""The Numerics commit a locally built ``Numerics.dll`` was built from; empty for a NuGet package assembly."""

NUMERICS_BUILD_COMMAND = ("dotnet build Numerics/Numerics.csproj -c Release -f net10.0 -p:Version=2.3.0-dev.2 "
                          "-p:ContinuousIntegrationBuild=true (Windows, .NET SDK 10.0.400 on its bundled runtime 10.0.11)")
"""How a locally built ``Numerics.dll`` was built (receipts record it); empty for a NuGet package assembly."""
