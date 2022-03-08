using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Book
{
    public class LivrePretGroupVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public enum GroupBy
        {
            None,
            GroupName,
            DateAjout,
            DateAcquisition,
            Pret,
        }

        public enum SortBy
        {
            Name,
            //DateAjout,
            DatePret,
            DateRemise,
        }

        public enum OrderBy
        {
            Croissant,
            DCroissant
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LivrePretVMCastVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string _GroupName;
        public string GroupName
        {
            get => _GroupName;
            set
            {
                if (_GroupName != value)
                {
                    _GroupName = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<LivrePretVM> _Items = new ObservableCollection<LivrePretVM>();
        public ObservableCollection<LivrePretVM> Items
        {
            get => _Items;
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                    OnPropertyChanged();
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
