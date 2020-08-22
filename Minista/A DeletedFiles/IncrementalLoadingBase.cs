using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Minista
{
    public abstract class IncrementalLoadingBase : IList, ISupportIncrementalLoading, INotifyCollectionChanged, INotifyPropertyChanged, IItemsRangeInfo
    {
        #region IList
        public List<object> ToList()
        {
            return _storage;
        }

        public int Add(object value)
        {
            _storage.Add(value);
            NotifyOfInsertedItems((_storage.Count - 1), 1);
            return 1;
        }

        public void Clear()
        {
            _storage.Clear();
        }

        public bool Contains(object value)
        {
            return _storage.Contains(value);
        }

        public int IndexOf(object value)
        {
            return _storage.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            _storage.Remove(value);
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get
            {
                return _storage[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)_storage).CopyTo(array, index);
        }

        public int Count
        {
            get { return _storage.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public IEnumerator GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        #endregion

        #region ISupportIncrementalLoading

        public bool HasMoreItems
        {
            get; set;
        }

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }

            IsBusy = true;
            try
            {
                return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));

            }
            catch(Exception ex)
            {
                ex.PrintException("LoadMore");
                return null;
            }
        }

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private methods

        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                var items = await LoadMoreItemsOverrideAsync(c, count);
                var baseIndex = _storage.Count;

                _storage.AddRange(items);

                // Now notify of the new items
                NotifyOfInsertedItems(baseIndex, items.Count);

                return new LoadMoreItemsResult { Count = (uint)items.Count };
            }
            finally
            {
                IsBusy = false;
            }
        }

        void NotifyOfInsertedItems(int baseIndex, int count)
        {
            if (CollectionChanged == null)
            {
                return;
            }
            try
            {
                var list = _storage;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            $"pekh {i + baseIndex}     {count}         11111111111111".PrintDebug();
                            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list[i + baseIndex], i + baseIndex);
                            $"pekh {i + baseIndex}              22222222222222".PrintDebug();
                            CollectionChanged(this, args);
                        }
                        catch (Exception ex)
                        {
                            $" {count} | {i} | {baseIndex} | {(i + baseIndex)} | {list.Count}".PrintDebug();
                            ex.PrintException();

                        }
                    }
                }
                catch (Exception ex)
                {
                    $"FOREACH {count} | {list.Count}".PrintDebug();
                    ex.PrintException();

                }
            } catch (Exception ex)
            {
                $"list = _storage".PrintDebug();
                ex.PrintException();

            }
        }

        #endregion

        #region Overridable methods

        protected abstract Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count);
        protected abstract bool HasMoreItemsOverride();

        #endregion

        #region State

        List<object> _storage = new List<object>();
        bool _isbusy = false;
        public bool IsBusy { get => _isbusy; set { _isbusy = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsBusy")); } }
        #endregion

        #region IItemsRangeInfo
        private int _firstvisibleitemindex = 0;
        private int _lastvisibleitemindex = 0;
        public int FirstVisibleItemIndex
        {
            get => _firstvisibleitemindex;
            set { _firstvisibleitemindex = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FirstVisibleItemIndex")); }
        }
        public int LastVisibleItemIndex
        {
            get => _lastvisibleitemindex;
            set { _lastvisibleitemindex = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LastVisibleItemIndex")); }
        }
        public event EventHandler<object> FirstVisibleItemChanged;
        public event EventHandler<object> LastVisibleItemChanged;
        public event EventHandler<object> FirstDisappearedItemChanged;
        public event EventHandler<object> LastDisappearedItemChanged;
        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            if (visibleRange.FirstIndex == FirstVisibleItemIndex) return;
            var _tempfirst = FirstVisibleItemIndex;
            var _templast = LastVisibleItemIndex;
            //if (FirstVisibleItemIndex > visibleRange.FirstIndex)
            //{
            //    //Scrolled Up
            //}
            //else
            //{
            //    //Scrolled Down
            //}
            FirstVisibleItemChanged?.Invoke(this, _storage[visibleRange.FirstIndex]);
            FirstVisibleItemIndex = visibleRange.FirstIndex;
            LastVisibleItemChanged?.Invoke(this, _storage[visibleRange.FirstIndex]);
            LastVisibleItemIndex = visibleRange.LastIndex;

            FirstDisappearedItemChanged?.Invoke(this, _storage[_tempfirst]);
            LastDisappearedItemChanged?.Invoke(this, _storage[_templast]);
        }

        public void Dispose()
        {
            throw new Exception("Dispose not supported");
        }
        #endregion
    }
}
