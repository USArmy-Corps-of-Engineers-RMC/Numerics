"""Stage the pinned Numerics.dll and build the rmc-numerics wheel.

The wheel is pure Python plus ``Numerics.dll`` and ``rmc_numerics.runtimeconfig.json``
under ``rmc_numerics/_bin``, so one ``py3-none-any`` wheel serves Linux and Windows; the
.NET 10 runtime is its only platform requirement. ``Numerics.dll`` is taken byte for byte
from ``lib/net10.0`` of the published ``RMC.Numerics`` NuGet package whose version equals
the distribution version (or, with ``--dll``, from a locally built assembly), and its SHA-256
must equal ``NUMERICS_SHA256`` in ``src/rmc_numerics/_version.py``.

The package is found, in order: ``--nupkg PATH``; the NuGet global packages folder
(``NUGET_PACKAGES`` or ``~/.nuget/packages``), where ``dotnet restore`` of any project
referencing ``RMC.Numerics`` leaves it; with ``--download``, the nuget.org flat
container; with ``--dll PATH``, that file (the interim locally built assembly named by
``NUMERICS_SOURCE_COMMIT``). The staged assembly stays in ``_bin`` (gitignored) so a source
checkout can run the tests against it.

Usage, from ``python/rmc-numerics``::

    python scripts/build_wheel.py [--nupkg PATH | --dll PATH] [--download] [--stage-only] [--out DIR]

Needs the ``build`` package (``pip install build``) unless ``--stage-only``. Prints the
wheel's path and SHA-256 on its last lines.
"""

from __future__ import annotations

import argparse
import hashlib
import io
import os
import pathlib
import re
import subprocess
import sys
import urllib.request
import zipfile

PACKAGE = pathlib.Path(__file__).resolve().parents[1]
BIN = PACKAGE / "src" / "rmc_numerics" / "_bin"
VERSION_FILE = PACKAGE / "src" / "rmc_numerics" / "_version.py"
ASSEMBLY_ENTRY = "lib/net10.0/Numerics.dll"
RUNTIME_CONFIG = "rmc_numerics.runtimeconfig.json"
FLAT_CONTAINER = "https://api.nuget.org/v3-flatcontainer/rmc.numerics/{v}/rmc.numerics.{v}.nupkg"


def _pins() -> tuple[str, str, str]:
    """Read (distribution version, NuGet version, SHA-256) from _version.py without importing it."""
    text = VERSION_FILE.read_text(encoding="utf-8")

    def value(name: str) -> str:
        match = re.search(rf'^{name} = "([^"]+)"', text, re.MULTILINE)
        if match is None:
            raise SystemExit(f"{VERSION_FILE} lacks {name}.")
        return match.group(1)

    return value("__version__"), value("NUMERICS_VERSION"), value("NUMERICS_SHA256")


def pep440(numerics_version: str) -> str:
    """Return the PEP 440 form of an RMC.Numerics version: ``2.3.0`` stays, ``2.3.0-dev.1`` is ``2.3.0.dev1``."""
    match = re.fullmatch(r"(\d+\.\d+\.\d+)(?:-dev\.(\d+))?", numerics_version)
    if match is None:
        raise SystemExit(f"Unsupported RMC.Numerics version {numerics_version!r} (expected X.Y.Z or X.Y.Z-dev.N).")
    return match.group(1) + (f".dev{match.group(2)}" if match.group(2) else "")


def _package_bytes(version: str, nupkg: pathlib.Path | None, download: bool) -> tuple[bytes, str]:
    """Return the .nupkg bytes and a description of where they came from."""
    if nupkg is not None:
        return nupkg.read_bytes(), str(nupkg)
    root = pathlib.Path(os.environ.get("NUGET_PACKAGES") or pathlib.Path.home() / ".nuget" / "packages")
    cached = root / "rmc.numerics" / version / f"rmc.numerics.{version}.nupkg"
    if cached.is_file():
        return cached.read_bytes(), str(cached)
    if not download:
        raise SystemExit(f"RMC.Numerics {version} is not in {root}; pass --nupkg, restore a project that references "
                         "it, or pass --download.")
    url = FLAT_CONTAINER.format(v=version)
    with urllib.request.urlopen(url, timeout=120) as response:  # noqa: S310 - fixed https nuget.org URL
        return response.read(), url


def stage(nupkg: pathlib.Path | None, download: bool, dll: pathlib.Path | None = None) -> str:
    """Stage Numerics.dll into _bin from a package (or a local build with ``dll``), verify its SHA-256, return it."""
    dist_version, nuget_version, pinned = _pins()
    if dist_version.split("+")[0].split(".post")[0] != pep440(nuget_version):
        raise SystemExit(f"Distribution version {dist_version} does not match the Numerics version {nuget_version}.")
    if dll is not None:
        assembly, source = dll.read_bytes(), str(dll)
    else:
        payload, source = _package_bytes(nuget_version, nupkg, download)
        with zipfile.ZipFile(io.BytesIO(payload)) as archive:
            assembly = archive.read(ASSEMBLY_ENTRY)
    digest = hashlib.sha256(assembly).hexdigest()
    if digest != pinned:
        raise SystemExit(f"Numerics.dll from {source} has SHA-256 {digest}; _version.py pins {pinned}.")
    if not (BIN / RUNTIME_CONFIG).is_file():
        raise SystemExit(f"{BIN / RUNTIME_CONFIG} is missing; it is tracked in the repository.")
    (BIN / "Numerics.dll").write_bytes(assembly)
    print(f"staged Numerics.dll {digest} from {source}")
    return digest


def build(out: pathlib.Path) -> pathlib.Path:
    """Build the wheel, verify its tag and contents, and return its path."""
    out.mkdir(parents=True, exist_ok=True)
    subprocess.run([sys.executable, "-m", "build", "--wheel", "--outdir", str(out), str(PACKAGE)], check=True)
    wheels = sorted(out.glob("rmc_numerics-*.whl"), key=lambda p: p.stat().st_mtime)
    if not wheels:
        raise SystemExit(f"no rmc_numerics wheel in {out}")
    wheel = wheels[-1]
    if not wheel.name.endswith("-py3-none-any.whl"):
        raise SystemExit(f"{wheel.name} is not a py3-none-any wheel.")
    _, _, pinned = _pins()
    with zipfile.ZipFile(wheel) as archive:
        names = set(archive.namelist())
        for name in ("Numerics.dll", RUNTIME_CONFIG):
            if f"rmc_numerics/_bin/{name}" not in names:
                raise SystemExit(f"{wheel.name} lacks rmc_numerics/_bin/{name}.")
        packed = hashlib.sha256(archive.read("rmc_numerics/_bin/Numerics.dll")).hexdigest()
    if packed != pinned:
        raise SystemExit(f"The wheel's Numerics.dll has SHA-256 {packed}; expected {pinned}.")
    return wheel


def main(argv: list[str]) -> int:
    """Stage the assembly, optionally build the wheel, and print its path and digest."""
    parser = argparse.ArgumentParser(description=__doc__.split("\n", 1)[0])
    parser.add_argument("--nupkg", type=pathlib.Path, help="the RMC.Numerics .nupkg to take Numerics.dll from")
    parser.add_argument("--dll", type=pathlib.Path, help="a locally built Numerics.dll to stage instead of a package's")
    parser.add_argument("--download", action="store_true", help="download the package from nuget.org if not cached")
    parser.add_argument("--stage-only", action="store_true", help="stage _bin/Numerics.dll and stop")
    parser.add_argument("--out", type=pathlib.Path, default=PACKAGE / "dist", help="where the wheel is written")
    args = parser.parse_args(argv)
    if args.dll is not None and args.nupkg is not None:
        parser.error("--dll and --nupkg are exclusive")
    stage(args.nupkg, args.download, args.dll)
    if args.stage_only:
        return 0
    wheel = build(args.out.resolve())
    print(wheel)
    print(f"{hashlib.sha256(wheel.read_bytes()).hexdigest()}  {wheel.name}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
