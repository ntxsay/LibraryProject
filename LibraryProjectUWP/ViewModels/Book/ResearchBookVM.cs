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

        private ObservableCollection<Search.Book.In> _SearchIn = new ObservableCollection<Search.Book.In>() { Search.Book.In.MainTitle, Search.Book.In.OtherTitle};
        public ObservableCollection<Search.Book.In> SearchIn
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

        private Search.Book.Terms _TermParameter = Search.Book.Terms.Contains;
        public Search.Book.Terms TermParameter
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
