"""Map the exceptions Numerics throws onto the Python exceptions callers expect.

pythonnet raises a .NET exception as a Python exception of the .NET type, which ``except ValueError``
does not catch. ``net_errors`` re-raises the argument and arithmetic families as their Python
counterparts, keeping the .NET message and chaining the .NET exception as the cause:

=====================================================  ========================
.NET exception                                          Python exception
=====================================================  ========================
``ArgumentException`` (``ArgumentOutOfRangeException``)  ``ValueError``
``ArithmeticException`` (``OverflowException``)          ``ArithmeticError``
``NotImplementedException``                              ``NotImplementedError``
``InvalidOperationException``                            ``RuntimeError``
=====================================================  ========================

Any other exception, .NET or Python, propagates unchanged.
"""

from __future__ import annotations

from collections.abc import Iterator
from contextlib import contextmanager

from rmc_numerics._util import namespace


def translate(error: BaseException) -> BaseException | None:
    """Return the Python exception for a .NET argument or arithmetic exception.

    Parameters
    ----------
    error : BaseException
        The raised exception.

    Returns
    -------
    BaseException or None
        A new ``ValueError``, ``ArithmeticError``, ``NotImplementedError`` or ``RuntimeError``
        carrying the .NET message, or None when ``error`` is not one of those .NET families.
    """
    system = namespace("System")
    message = str(getattr(error, "Message", error))
    if isinstance(error, system.ArgumentException):
        return ValueError(message)
    if isinstance(error, system.ArithmeticException):
        return ArithmeticError(message)
    if isinstance(error, system.NotImplementedException):
        return NotImplementedError(message)
    if isinstance(error, system.InvalidOperationException):
        return RuntimeError(message)
    return None


@contextmanager
def net_errors() -> Iterator[None]:
    """Re-raise .NET argument and arithmetic exceptions as their Python counterparts.

    Yields
    ------
    None
        Nothing; the body runs inside the translation.

    Examples
    --------
    >>> from rmc_numerics.distributions import Normal
    >>> from rmc_numerics.errors import net_errors
    >>> try:
    ...     with net_errors():
    ...         Normal().inverse_cdf(1.5)
    ... except ValueError as error:
    ...     print("refused")
    refused
    """
    try:
        yield
    except Exception as error:
        translated = translate(error)
        if translated is None:
            raise
        raise translated from error


__all__ = ["net_errors", "translate"]
