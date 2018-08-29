using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAccountLocker.Model
{
    [Serializable]
    public class BetterObservableCollection<T> : ObservableCollection<T>
    {
        public BetterObservableCollection() : base()
        {
        }
        public BetterObservableCollection(IEnumerable<T> collection) : base(collection)
        {
        }
        public BetterObservableCollection(List<T> list) : base(list)
        {
        }
        public void Reset(IEnumerable<T> range)
        {
            this.Items.Clear();

            AddRange(range);
        }
        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                Items.Add(item);
            }
            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
