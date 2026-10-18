"""The loader: fresh-process start, the clr-first refusal, discovery and the load identity."""

from __future__ import annotations

import subprocess
import sys
import textwrap
from pathlib import Path

import pytest

import rmc_numerics
from rmc_numerics import runtime
from rmc_numerics.identity import file_sha256

SRC = Path(rmc_numerics.__file__).resolve().parents[1]
"""The import root of the package under test (the source tree or an installed wheel)."""


def _run(code: str) -> subprocess.CompletedProcess[str]:
    """Run code in a fresh interpreter with the package source importable."""
    env_code = f"import sys; sys.path.insert(0, {str(SRC)!r})\n" + textwrap.dedent(code)
    return subprocess.run([sys.executable, "-c", env_code], capture_output=True, text=True, timeout=300)


def test_fresh_process_loads_coreclr_and_numerics() -> None:
    # Arrange / Act
    result = _run("""
        import sys
        import rmc_numerics
        assert "clr" not in sys.modules, "importing the package must not start the runtime"
        assert not rmc_numerics.is_initialized()
        rmc_numerics.initialize()
        print(rmc_numerics.get_runtime_info().kind)
        print(rmc_numerics.numerics_identity().version)
    """)
    # Assert
    assert result.returncode == 0, result.stderr
    kind, version = result.stdout.split()
    assert kind == "CoreCLR"
    assert version == rmc_numerics.NUMERICS_ASSEMBLY_VERSION


def test_bare_import_clr_first_is_refused() -> None:
    # Arrange / Act: a bare 'import clr' picks the default runtime (.NET Framework on Windows)
    result = _run("""
        try:
            import clr  # noqa: F401
        except Exception as exc:
            print("NOCLR", type(exc).__name__)
            raise SystemExit(0)
        import rmc_numerics
        try:
            rmc_numerics.initialize()
        except rmc_numerics.RuntimeLoadError as exc:
            print("REFUSED", exc)
        else:
            print("LOADED", rmc_numerics.get_runtime_info().kind)
    """)
    # Assert
    assert result.returncode == 0, result.stderr
    if result.stdout.startswith("NOCLR"):
        pytest.skip(f"no default CLR to import on this platform ({result.stdout.strip()})")
    assert result.stdout.startswith("REFUSED"), result.stdout
    assert "import clr" in result.stdout


def test_explicit_directory_that_is_missing_is_an_error(tmp_path: Path) -> None:
    # Act / Assert
    with pytest.raises(rmc_numerics.RuntimeLoadError, match="not a directory"):
        runtime._resolve_dll_dir(tmp_path / "absent")
    with pytest.raises(rmc_numerics.RuntimeLoadError, match="does not hold"):
        runtime._resolve_dll_dir(tmp_path)


def test_environment_variable_directory_wins_over_packaged(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    # Arrange
    (tmp_path / "Numerics.dll").write_bytes(b"")
    monkeypatch.setenv(runtime.ENV_DLL_DIR, str(tmp_path))
    # Act / Assert
    assert runtime._resolve_dll_dir(None) == tmp_path.resolve()
    assert runtime._resolve_runtime_config(tmp_path).name == runtime.RUNTIME_CONFIG_FILE


def test_dotnet_root_that_is_missing_is_an_error(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    # Arrange
    monkeypatch.setenv("DOTNET_ROOT", str(tmp_path / "absent"))
    # Act / Assert
    with pytest.raises(rmc_numerics.RuntimeLoadError, match="DOTNET_ROOT"):
        runtime._resolve_dotnet_root()


def test_add_assembly_refuses_a_second_numerics_and_missing_files(tmp_path: Path) -> None:
    # Arrange
    other = tmp_path / "Numerics.dll"
    other.write_bytes(b"")
    # Act / Assert
    with pytest.raises(rmc_numerics.RuntimeLoadError, match="one Numerics assembly"):
        rmc_numerics.add_assembly(other)
    with pytest.raises(rmc_numerics.RuntimeLoadError, match="does not exist"):
        rmc_numerics.add_assembly(tmp_path / "Hydrologics.dll")


def test_initialize_is_memoized() -> None:
    # Act
    first = rmc_numerics.initialize()
    second = rmc_numerics.initialize("ignored after the first call")
    # Assert
    assert first == second == rmc_numerics.runtime_dir()


def test_load_identity_reports_version_and_file_digest() -> None:
    # Act
    identity = rmc_numerics.numerics_identity()
    # Assert
    assert identity.version == rmc_numerics.NUMERICS_ASSEMBLY_VERSION
    assert identity.informational_version.startswith(rmc_numerics.NUMERICS_VERSION)
    assert identity.location.resolve().parent == rmc_numerics.runtime_dir().resolve()
    assert identity.sha256 == file_sha256(identity.location)


def test_packaged_assembly_is_the_pinned_nuget_bytes() -> None:
    # Arrange
    identity = rmc_numerics.numerics_identity()
    if identity.location.resolve().parent != runtime._PACKAGED_BIN.resolve():
        pytest.skip("Numerics.dll was loaded from an override directory, not the packaged _bin")
    # Act / Assert
    assert identity.sha256 == rmc_numerics.NUMERICS_SHA256
    assert identity.is_packaged
    assert rmc_numerics.assert_numerics_identity() == identity
    with pytest.raises(rmc_numerics.RuntimeLoadError, match="SHA-256"):
        rmc_numerics.assert_numerics_identity(sha256="0" * 64)
    with pytest.raises(rmc_numerics.RuntimeLoadError, match="version"):
        rmc_numerics.assert_numerics_identity(version="0.0.0.0")


def test_distribution_version_equals_numerics_version() -> None:
    # Assert
    assert rmc_numerics.__version__.split(".post")[0] == rmc_numerics.NUMERICS_VERSION
    assert rmc_numerics.NUMERICS_ASSEMBLY_VERSION.startswith(rmc_numerics.NUMERICS_VERSION)
