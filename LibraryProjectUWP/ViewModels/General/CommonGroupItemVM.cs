using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.General
{
    public class CommonGroupItemVM<T> : INotifyPropertyChanged where T : class
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private int _CountItems;
        public int CountItems
        {
            get => this._CountItems;
            set
            {
                if (this._CountItems != value)
                {
                    this._CountItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private IEnumerable<T> _ViewModelList;
        public IEnumerable<T> ViewModelList
        {
            get => this._ViewModelList;
            set
            {
                if (this._ViewModelList != value)
                {
                    this._ViewModelList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
