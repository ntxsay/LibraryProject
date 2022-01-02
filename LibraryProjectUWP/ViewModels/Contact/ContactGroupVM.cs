using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Contact
{
    public class ContactGroupVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public enum GroupBy
        {
            None,
            LetterNomNaissance,
            LetterPrenom,
            CreationYear,
        }

        public enum SortBy
        {
            NomNaissance,
            Prenom,
            DateCreation,
        }

        public enum OrderBy
        {
            Croissant,
            DCroissant
        }

        public bool IsSortedByNomNaissance
        {
            get => this.SortedBy == SortBy.NomNaissance;
        }

        public bool IsSortedByPrenom
        {
            get => this.SortedBy == SortBy.Prenom;
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

        public bool IsGroupedByLetterNomNaissance
        {
            get => this.GroupedBy == GroupBy.LetterNomNaissance;
        }

        public bool IsGroupedByLetterPrenom
        {
            get => this.GroupedBy == GroupBy.LetterPrenom;
        }

        public bool IsGroupedByDateCreationYear
        {
            get => this.GroupedBy == GroupBy.CreationYear;
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

        private ObservableCollection<IGrouping<string, ContactVM>> _Collection = new ObservableCollection<IGrouping<string, ContactVM>>();
        public ObservableCollection<IGrouping<string, ContactVM>> Collection
        {
            get => this._Collection;
            set
            {
                if (this._Collection != value)
                {
                    this._Collection = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(IsGroupedByNone));
                    this.OnPropertyChanged(nameof(IsGroupedByLetterNomNaissance));
                    this.OnPropertyChanged(nameof(IsGroupedByLetterPrenom));
                    this.OnPropertyChanged(nameof(IsGroupedByDateCreationYear));
                }
            }
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ContactGroupCastVM
    {
        public string GroupName { get; set; }
        public ObservableCollection<ContactVM> Items { get; set; }
    }
}
