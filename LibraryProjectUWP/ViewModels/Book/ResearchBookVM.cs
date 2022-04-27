using LibraryProjectUWP.Code;
using Newtonsoft.Json;
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
    public class ResearchBookVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public long IdLibrary { get; set; }

        private string _Term;
        public string Term
        {
            get => _Term;
            set
            {
                if (_Term != value)
                {
                    _Term = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool? _SearchInMainTitle;
        public bool? SearchInMainTitle
        {
            get => _SearchInMainTitle;
            set
            {
                if (_SearchInMainTitle != value)
                {
                    _SearchInMainTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool? _SearchInOtherTitles;
        public bool? SearchInOtherTitles
        {
            get => _SearchInOtherTitles;
            set
            {
                if (_SearchInOtherTitles != value)
                {
                    _SearchInOtherTitles = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool? _SearchInAuthors;
        public bool? SearchInAuthors
        {
            get => _SearchInAuthors;
            set
            {
                if (_SearchInAuthors != value)
                {
                    _SearchInAuthors = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool? _SearchInCollections;
        public bool? SearchInCollections
        {
            get => _SearchInCollections;
            set
            {
                if (_SearchInCollections != value)
                {
                    _SearchInCollections = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool? _SearchInEditors;
        public bool? SearchInEditors
        {
            get => _SearchInEditors;
            set
            {
                if (_SearchInEditors != value)
                {
                    _SearchInEditors = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Code.Helpers.LibraryHelpers.Book.Search.In> _SearchIn = new ObservableCollection<Code.Helpers.LibraryHelpers.Book.Search.In>() 
        {
            Code.Helpers.LibraryHelpers.Book.Search.In.MainTitle,
            Code.Helpers.LibraryHelpers.Book.Search.In.OtherTitle
        };

       [Obsolete] 
        public ObservableCollection<Code.Helpers.LibraryHelpers.Book.Search.In> SearchIn
        {
            get => _SearchIn;
            set
            {
                if (_SearchIn != value)
                {
                    _SearchIn = value;
                    OnPropertyChanged();
                }
            }
        }

        private Code.Helpers.LibraryHelpers.Book.Search.Terms _TermParameter = Code.Helpers.LibraryHelpers.Book.Search.Terms.Contains;
        public Code.Helpers.LibraryHelpers.Book.Search.Terms TermParameter
        {
            get => _TermParameter;
            set
            {
                if (_TermParameter != value)
                {
                    _TermParameter = value;
                    OnPropertyChanged();
                }
            }
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
