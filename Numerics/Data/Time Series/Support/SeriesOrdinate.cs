using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

namespace Numerics.Data
{

    /// <summary>
    /// A series ordinate.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors: 
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public class SeriesOrdinate<TIndex, TValue> : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructs a new series ordinate.
        /// </summary>
        public SeriesOrdinate() { }

        /// <summary>
        /// Constructs a new series ordinate. 
        /// </summary>
        /// <param name="index">The ordinate index.</param>
        /// <param name="value">The ordinate value.</param>
        public SeriesOrdinate(TIndex index, TValue value)
        {
            _index = index;
            _value = value;
        }

        protected TIndex _index;
        protected TValue _value;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The index of the series ordinate.
        /// </summary>
        public virtual TIndex Index
        {
            get { return _index; }
            set
            {
                if (!EqualityComparer<TIndex>.Default.Equals(_index, value))
                {
                    _index = value;
                    RaisePropertyChanged(nameof(Index));
                }
            }
        }

        /// <summary>
        /// The value of the time-series ordinate.
        /// </summary>
        public virtual TValue Value
        {
            get { return _value; }
            set
            {
                if (!EqualityComparer<TValue>.Default.Equals(_value, value))
                {
                    _value = value;
                    RaisePropertyChanged(nameof(Value));
                }
            }
        }


        public static bool operator ==(SeriesOrdinate<TIndex, TValue> left, SeriesOrdinate<TIndex, TValue> right)
        {
            if (!EqualityComparer<TIndex>.Default.Equals(left.Index, right.Index)) return false;
            if (!EqualityComparer<TValue>.Default.Equals(left.Value, right.Value)) return false;
            return true;
        }

        public static bool operator !=(SeriesOrdinate<TIndex, TValue> left, SeriesOrdinate<TIndex, TValue> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Raise property changed event.
        /// </summary>
        /// <param name="propertyname">Name of property that changed.</param>
        protected void RaisePropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        /// <summary>
        /// Returns a copy of the series ordinate.
        /// </summary>
        public SeriesOrdinate<TIndex, TValue> Clone()
        {
            return new SeriesOrdinate<TIndex, TValue>(Index, Value);
        }



    }
}
