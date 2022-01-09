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
    public class BookGroupVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        public enum GroupBy
        {
            None,
            Letter,
            CreationYear,
            ParutionYear,
        }

        public enum SortBy
        {
            Name,
            DateCreation,
        }

        public enum OrderBy
        {
            Croissant,
            DCroissant
        }

        public bool IsSortedByName
        {
            get => this.SortedBy == SortBy.Name;
        }

        public bool IsSortedByDateDebutDiffusion
        {
            get => this.SortedBy == SortBy.DateCreation;
        }

        private SortBy _SortedBy;
        public SortBy SortedBy
        {
            get => this._SortedBy;
            set
            {
                if (this._SortedBy != value)
                {
                    this._SortedBy = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsOrderedByCroissant
        {
            get => this.OrderedBy == OrderBy.Croissant;
        }

        public bool IsOrderedByDecroissant
        {
            get => this.OrderedBy == OrderBy.DCroissant;
        }

        private OrderBy _OrderedBy;
        public OrderBy OrderedBy
        {
            get => this._OrderedBy;
            set
            {
                if (this._OrderedBy != value)
                {
                    this._OrderedBy = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsGroupedByNone
        {
            get => this.GroupedBy == GroupBy.None;
        }

        public bool IsGroupedByLetter
        {
            get => this.GroupedBy == GroupBy.Letter;
        }

        public bool IsGroupedByDateCreationYear
        {
            get => this.GroupedBy == GroupBy.CreationYear;
        }

        public bool IsGroupedByDateParutionYear
        {
            get => this.GroupedBy == GroupBy.ParutionYear;
        }

        private GroupBy _GroupedBy;
        public GroupBy GroupedBy
        {
            get => this._GroupedBy;
            set
            {
                if (this._GroupedBy != value)
                {
                    this._GroupedBy = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<IGrouping<string, LivreVM>> _Collection = new ObservableCollection<IGrouping<string, LivreVM>>();
        public ObservableCollection<IGrouping<string, LivreVM>> Collection
        {
            get => this._Collection;
            set
            {
                if (this._Collection != value)
                {
                    this._Collection = value;
                    this.OnPropertyChanged();
                    //this.OnPropertyChanged(nameof(IsGroupedByNone));
                    //this.OnPropertyChanged(nameof(IsGroupedByLetter));
                    //this.OnPropertyChanged(nameof(IsGroupedByDateCreationYear));
                    //this.OnPropertyChanged(nameof(IsGroupedByDateParutionYear));
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
