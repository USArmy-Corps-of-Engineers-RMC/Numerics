/**
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from Numerics.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* **/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Numerics.Data
{

    /// <summary>
    /// An abstract series class, which is a collection of series ordinates.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class Series<TIndex, TValue> : IList<SeriesOrdinate<TIndex, TValue>>, INotifyCollectionChanged
    {
        /// <summary>
        /// Internal list.
        /// </summary>
        protected List<SeriesOrdinate<TIndex, TValue>> _seriesOrdinates = new List<SeriesOrdinate<TIndex, TValue>>();
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Get and sets the element at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set</param>
        public SeriesOrdinate<TIndex, TValue> this[int index]
        {
            get { return _seriesOrdinates[index]; }
            set
            {
                if (_seriesOrdinates[index] != value)
                {
                    var oldvalue = _seriesOrdinates[index];
                    _seriesOrdinates[index] = value;
                    if (SuppressCollectionChanged == false)
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldvalue));               
                }
            }
        }

        /// <summary>
        /// Suppress collection changed events from firing.
        /// </summary>
        public bool SuppressCollectionChanged { get; set; } = false;

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count => _seriesOrdinates.Count;

        /// <summary>
        /// Determines if the collection is read only. 
        /// </summary>
        public virtual bool IsReadOnly => false;

        /// <summary>
        /// Add element to the collection.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public virtual void Add(SeriesOrdinate<TIndex, TValue> item)
        {
            _seriesOrdinates.Add(item);
            if (SuppressCollectionChanged == false)
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _seriesOrdinates.Count - 1));
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="item">Item to insert.</param>
        public virtual void Insert(int index, SeriesOrdinate<TIndex, TValue> item)
        {
            _seriesOrdinates.Insert(index, item);
            if (SuppressCollectionChanged == false)
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Removes the first occurrence of the specified object.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>True if the occurrence is successfully removed. False otherwise.</returns>
        public virtual bool Remove(SeriesOrdinate<TIndex, TValue> item)
        {
            var index = IndexOf(item);
            if (_seriesOrdinates.Remove(item) == true)
            {
                if (SuppressCollectionChanged == false)
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public virtual void RemoveAt(int index)
        {
            Remove(_seriesOrdinates[index]);
        }

        /// <summary>
        /// Remove all elements from the collection.
        /// </summary>
        public void Clear()
        {
            SuppressCollectionChanged = true;
            for (int i = _seriesOrdinates.Count - 1; i >= 0; i--)
                Remove(_seriesOrdinates[i]);
            SuppressCollectionChanged = false;
            RaiseCollectionChangedReset();
        }

        /// <summary>
        /// Determines whether an element is in the collection.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>True if the element is in the collection. False otherwise/</returns>
        public bool Contains(SeriesOrdinate<TIndex, TValue> item)
        {
            return _seriesOrdinates.Contains(item);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the copied elements.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        public void CopyTo(SeriesOrdinate<TIndex, TValue>[] array, int arrayIndex)
        {
            _seriesOrdinates.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        public int IndexOf(SeriesOrdinate<TIndex, TValue> item)
        {
            return _seriesOrdinates.IndexOf(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public IEnumerator<SeriesOrdinate<TIndex, TValue>> GetEnumerator()
        {
            return _seriesOrdinates.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Raise the collection changed reset event.
        /// </summary>
        public void RaiseCollectionChangedReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Returns the list of series values as a list.
        /// </summary>
        public List<TValue> ValuesToList()
        {
            return _seriesOrdinates.Select(x => x.Value).ToList();
        }

        /// <summary>
        /// Returns the list of series values as an array.
        /// </summary>
        public TValue[] ValuesToArray()
        {
            return _seriesOrdinates.Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// Returns the list of series indexes as a list.
        /// </summary>
        public List<TIndex> IndexesToList()
        {
            return _seriesOrdinates.Select(x => x.Index).ToList();
        }

        /// <summary>
        /// Returns the list of series indexes as an array.
        /// </summary>
        public TIndex[] IndexesToArray()
        {
            return _seriesOrdinates.Select(x => x.Index).ToArray();
        }

        
    }
}
