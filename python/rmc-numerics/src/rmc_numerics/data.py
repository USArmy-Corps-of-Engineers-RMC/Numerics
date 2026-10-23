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

from rmc_numerics._util import bind, bind_list, evaluate, loop, namespace, net_enum
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

    def _lookup(self, name: str) -> tuple[Any, Any]:
        """Return the scalar and list overloads of ``GetYFromX`` or ``GetXFromY`` with extrapolation sides.

        Both are bound once per wrapped object: pythonnet resolves the four-overload group on every
        call otherwise, which costs more than a short lookup. The bound overloads are the members the
        group resolves to for these arguments.
        """
        cache = self.__dict__.setdefault("_bound", {})
        entry = cache.get(name)
        if entry is None or entry[0] is not self.net:
            data = _data()
            import System  # noqa: PLC0415 - the runtime is running (self.net exists)
            from System.Collections.Generic import IList  # noqa: PLC0415

            group = getattr(self.net, name)
            tail = (data.Transform, data.Transform, data.ExtrapolationSides)
            entry = cache[name] = (self.net, group.Overloads[(System.Double, *tail)],
                                   group.Overloads[(IList[System.Double], *tail)])
        return entry[1], entry[2]

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
        scalar, vector = self._lookup("GetYFromX")
        return evaluate(x, lambda v: scalar(v, xt, yt, ext), lambda arr: vector(arr, xt, yt, ext))

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
        scalar, vector = self._lookup("GetXFromY")
        return evaluate(y, lambda v: scalar(v, xt, yt, ext), lambda arr: vector(arr, xt, yt, ext))


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
        self._arguments = (np.array(x, dtype=np.float64), np.array(y, dtype=np.float64), sort_order, x_transform,
                           y_transform)
        self.net = data.Linear(to_net_doubles(x), to_net_doubles(y), net_enum(data.SortOrder, sort_order))
        self.net.XTransform = net_enum(data.Transform, x_transform)
        self.net.YTransform = net_enum(data.Transform, y_transform)
        self._interpolate = bind(self.net.Interpolate)
        self._interpolate_list = bind_list(self.net.Interpolate)
        self._extrapolate = bind(self.net.Extrapolate)

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
        return evaluate(x, self._interpolate, self._interpolate_list)

    def extrapolate(self, x: Any) -> Any:
        """Evaluate the end segments' straight lines beyond the knots (``Extrapolate``).

        Inside the knots this equals :meth:`interpolate`; outside it extends the first or
        last segment in the transformed spaces instead of holding the end values. Numerics
        has no array overload of ``Extrapolate``, so arrays are evaluated element by element.

        Parameters
        ----------
        x : float or array_like
            Lookup values.

        Returns
        -------
        float or numpy.ndarray
            Values, shaped like ``x``.
        """
        return loop(x, self._extrapolate)

    __call__ = interpolate

    def __deepcopy__(self, memo: dict) -> Linear:
        """Return an independent interpolator over the same knots (its own .NET object and search cursor)."""
        return Linear(*self._arguments)


__all__ = ["Linear", "OrderedPairedData"]
