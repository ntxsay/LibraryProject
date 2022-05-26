using LibraryProjectUWP.Code;
using LibraryProjectUWP.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.General
{
    public class ResearchItemVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public Guid Guid { get; private set; } = Guid.NewGuid();

        public Type TypeObject { get; set; } = typeof(LivreVM);
        public long? IdLibrary { get; set; }

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

        private string _TermMessage;
        public string TermMessage
        {
            get => _TermMessage;
            set
            {
                if (_TermMessage != value)
                {
                    _TermMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool? _SearchInMainTitle = false;
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

        private bool? _SearchInOtherTitles = false;
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

        private bool? _SearchInAuthors = false;
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

        private bool? _SearchInCollections = false;
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

        private bool? _SearchInEditors = false;
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

        private Search.Terms _TermParameter = Search.Terms.Contains;
        public Search.Terms TermParameter
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

        private bool _IsExcluded;
        public bool IsExcluded
        {
            get => _IsExcluded;
            set
            {
                if (_IsExcluded != value)
                {
                    _IsExcluded = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsSearchFromParentResult = false;
        public bool IsSearchFromParentResult
        {
            get => _IsSearchFromParentResult;
            set
            {
                if (_IsSearchFromParentResult != value)
                {
                    _IsSearchFromParentResult = value;
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

    public class ResearchContainerVM<T> : INotifyPropertyChanged where T : class
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ResearchItemVM _CurrentSearchParameter;
        public ResearchItemVM CurrentSearchParameter
        {
            get => _CurrentSearchParameter;
            set
            {
                if (_CurrentSearchParameter != value)
                {
                    _CurrentSearchParameter = value;
                    OnPropertyChanged();
                }
            }
        }

        private IEnumerable<T> _ParentSearchedResult;
        public IEnumerable<T> ParentSearchedResult
        {
            get => _ParentSearchedResult;
            set
            {
                if (_ParentSearchedResult != value)
                {
                    _ParentSearchedResult = value;
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
