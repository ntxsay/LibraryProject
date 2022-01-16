using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Publishers;
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
    public class LivrePublicationVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [JsonIgnore]
        public long Id { get; set; }
        
        [JsonIgnore]
        public LivreVM Parent { get; set; }

        private DateTimeOffset? _DateParution;
        public DateTimeOffset? DateParution
        {
            get => _DateParution;
            set
            {
                if (_DateParution != value)
                {
                    _DateParution = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsJourParutionKnow;
        public bool IsJourParutionKnow
        {
            get => _IsJourParutionKnow;
            set
            {
                if (_IsJourParutionKnow != value)
                {
                    _IsJourParutionKnow = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsMoisParutionKnow;
        public bool IsMoisParutionKnow
        {
            get => _IsMoisParutionKnow;
            set
            {
                if (_IsMoisParutionKnow != value)
                {
                    _IsMoisParutionKnow = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsJourParutionVisible = true;
        public bool IsJourParutionVisible
        {
            get => _IsJourParutionVisible;
            set
            {
                if (_IsJourParutionVisible != value)
                {
                    _IsJourParutionVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsMoisParutionVisible = true;
        public bool IsMoisParutionVisible
        {
            get => _IsMoisParutionVisible;
            set
            {
                if (_IsMoisParutionVisible != value)
                {
                    _IsMoisParutionVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<CollectionVM> _Collections = new ObservableCollection<CollectionVM>();
        public ObservableCollection<CollectionVM> Collections
        {
            get => _Collections;
            set
            {
                if (_Collections != value)
                {
                    _Collections = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<PublisherVM> _Editeurs = new ObservableCollection<PublisherVM>();
        public ObservableCollection<PublisherVM> Editeurs
        {
            get => _Editeurs;
            set
            {
                if (_Editeurs != value)
                {
                    _Editeurs = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<string> _Langues = new ObservableCollection<string>();
        public ObservableCollection<string> Langues
        {
            get => _Langues;
            set
            {
                if (_Langues != value)
                {
                    _Langues = value;
                    OnPropertyChanged();
                }
            }
        }

        [JsonIgnore]
        public readonly IEnumerable<string> MoneyList = Code.Helpers.CultureHelpers.Money.MoneyList();

        private ObservableCollection<PriceVM> _Prices = new ObservableCollection<PriceVM>();
        public ObservableCollection<PriceVM> Prices
        {
            get => _Prices;
            set
            {
                if (_Prices != value)
                {
                    _Prices = value;
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
