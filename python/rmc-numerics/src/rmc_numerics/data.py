"""Paired data and interpolation from ``Numerics.Data``.

:class:`OrderedPairedData` wraps ``Numerics.Data.OrderedPairedData`` and :class:`Linear`
wraps ``Numerics.Data.Linear``. Lookups on arrays cross once into the ``IList<double>``
overloads (``GetYFromX(IList, Transform, Transform, ExtrapolationSides)``,
``GetXFromY(...)``, ``Interpolater.Interpolate(IList)``), so the interpolation
arithmetic is Numerics' own.
"""

from __future__ import annotations

from typing import Any

import numpy as np

from rmc_numerics._util import evaluate, namespace, net_enum
from rmc_numerics.enums import Extrapolation, SortOrder, Transform
from rmc_numerics.marshal import to_net_doubles


def _data() -> Any:
    """Return the ``Numerics.Data`` namespace after starting the runtime."""
    return namespace("Numerics.Data")


class OrderedPairedData:
    """Monotone (x, y) pairs with transformed linear lookups in both directions.

    Parameters
    ----------
    x, y : array_like
        Paired values of equal length.
    strict_x : bool, optional
        Whether x must be strictly monotone; default True.
    x_order : SortOrder or str, optional
        Order of x; default ascending.
    strict_y : bool, optional
        Whether y must be strictly monotone; default True.
    y_order : SortOrder or str, optional
        Order of y; default ascending.

    Attributes
    ----------
    net : Numerics.Data.OrderedPairedData
        The wrapped .NET object.
    """

    def __init__(self, x: Any, y: Any, strict_x: bool = True, x_order: SortOrder | str = SortOrder.ASCENDING,
                 strict_y: bool = True, y_order: SortOrder | str = SortOrder.ASCENDING) -> None:
        data = _data()
        self.net = data.OrderedPairedData(to_net_doubles(x), to_net_doubles(y), bool(strict_x),
                                          net_enum(data.SortOrder, x_order), bool(strict_y),
                                          net_enum(data.SortOrder, y_order))

    def __len__(self) -> int:
        return int(self.net.Count)

    @property
    def x(self) -> np.ndarray:
        """The x values."""
        return np.array([float(o.X) for o in self.net], dtype=np.float64)

    @property
    def y(self) -> np.ndarray:
        """The y values."""
        return np.array([float(o.Y) for o in self.net], dtype=np.float64)

    @property
    def is_valid(self) -> bool:
        """Whether the pairs satisfy the configured order and strictness."""
        return bool(self.net.IsValid)

    @property
    def errors(self) -> list[str]:
        """The validation errors (``GetErrors``)."""
        return [str(message) for message in self.net.GetErrors()]

    def get_y_from_x(self, x: Any, x_transform: Transform | str = Transform.NONE,
                     y_transform: Transform | str = Transform.NONE,
                     extrapolation: Extrapolation | str = Extrapolation.NONE) -> Any:
        """Interpolate y at x (``GetYFromX``).

        Parameters
        ----------
        x : float or array_like
            Lookup values.
        x_transform, y_transform : Transform or str, optional
            Interpolation spaces; default none.
        extrapolation : Extrapolation or str, optional
            Sides on which to extend linearly; default none (endpoint hold).

        Returns
        -------
        float or numpy.ndarray
            Interpolated y, shaped like ``x``.
        """
        data = _data()
        xt, yt = net_enum(data.Transform, x_transform), net_enum(data.Transform, y_transform)
        ext = net_enum(data.ExtrapolationSides, extrapolation)
        return evaluate(x, lambda v: self.net.GetYFromX(v, xt, yt, ext),
                        lambda arr: self.net.GetYFromX(arr, xt, yt, ext))

    def get_x_from_y(self, y: Any, x_transform: Transform | str = Transform.NONE,
                     y_transform: Transform | str = Transform.NONE,
                     extrapolation: Extrapolation | str = Extrapolation.NONE) -> Any:
        """Interpolate x at y (``GetXFromY``).

        Parameters
        ----------
        y : float or array_like
            Lookup values.
        x_transform, y_transform : Transform or str, optional
            Interpolation spaces; default none.
        extrapolation : Extrapolation or str, optional
            Sides on which to extend linearly; default none (endpoint hold).

        Returns
        -------
        float or numpy.ndarray
            Interpolated x, shaped like ``y``.
        """
        data = _data()
        xt, yt = net_enum(data.Transform, x_transform), net_enum(data.Transform, y_transform)
        ext = net_enum(data.ExtrapolationSides, extrapolation)
        return evaluate(y, lambda v: self.net.GetXFromY(v, xt, yt, ext),
                        lambda arr: self.net.GetXFromY(arr, xt, yt, ext))


class Linear:
    """Linear interpolation in optionally transformed spaces (``Numerics.Data.Linear``).

    Build once and reuse: the interpolator keeps a search cursor between calls.

    Parameters
    ----------
    x, y : array_like
        Knots; x sorted in ``sort_order``.
    sort_order : SortOrder or str, optional
        Order of x; default ascending.
    x_transform, y_transform : Transform or str, optional
        Interpolation spaces; default none.

    Attributes
    ----------
    net : Numerics.Data.Linear
        The wrapped .NET object.
    """

    def __init__(self, x: Any, y: Any, sort_order: SortOrder | str = SortOrder.ASCENDING,
                 x_transform: Transform | str = Transform.NONE, y_transform: Transform | str = Transform.NONE) -> None:
        data = _data()
        self.net = data.Linear(to_net_doubles(x), to_net_doubles(y), net_enum(data.SortOrder, sort_order))
        self.net.XTransform = net_enum(data.Transform, x_transform)
        self.net.YTransform = net_enum(data.Transform, y_transform)

    def interpolate(self, x: Any) -> Any:
        """Interpolate at x (``Interpolate``), holding the end values outside the knots.

        Parameters
        ----------
        x : float or array_like
            Lookup values.

        Returns
        -------
        float or numpy.ndarray
            Interpolated values, shaped like ``x``.
        """
        return evaluate(x, self.net.Interpolate, self.net.Interpolate)

    __call__ = interpolate


__all__ = ["Linear", "OrderedPairedData"]
