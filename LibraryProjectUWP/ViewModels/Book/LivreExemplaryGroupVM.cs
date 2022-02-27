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
    public class LivreExemplaryGroupVM : INotifyPropertyChanged
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
            DateAjout,
            DateAcquisition,
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

    public class LivreExemplaryVMCastVM
    {
        public string GroupName { get; set; }
        public ObservableCollection<LivreExemplaryVM> Items { get; set; }
    }

}
