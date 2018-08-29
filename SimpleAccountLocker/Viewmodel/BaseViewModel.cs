using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAccountLocker.Viewmodel
{
    /// <summary>
    /// Base common methods commonly used by other ViewModels.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        //event to be raised when property changed.
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Update form event when property changes value.
        /// </summary>
        /// <param name="propertyName">Property to notify</param>
        public void OnPropertyChange(string propertyName)
        {
            OnPropertyChange(new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Updates form when set value changed
        /// </summary>
        /// <param name="e">Property to notify</param>
        public void OnPropertyChange(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }
}
