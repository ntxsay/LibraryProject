using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
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
    public class LivreExemplaryVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [JsonIgnore]
        public long Id { get; set; }

        [JsonIgnore]
        public long IdBook { get; set; }

        [JsonIgnore]
        public LivreVM Parent { get; set; }

        private string _Source;
        public string Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _NoExemplaire;
        public string NoExemplaire
        {
            get => _NoExemplaire;
            set
            {
                if (_NoExemplaire != value)
                {
                    _NoExemplaire = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _NbExemplaire;
        public int NbExemplaire
        {
            get => _NbExemplaire;
            set
            {
                if (_NbExemplaire != value)
                {
                    _NbExemplaire = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Observations;
        public string Observations
        {
            get => _Observations;
            set
            {
                if (_Observations != value)
                {
                    _Observations = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsVisible;
        public bool IsVisible
        {
            get => _IsVisible;
            set
            {
                if (_IsVisible != value)
                {
                    _IsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTimeOffset _DateAjout;
        public DateTimeOffset DateAjout
        {
            get => _DateAjout;
            set
            {
                if (_DateAjout != value)
                {
                    _DateAjout = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTimeOffset? _DateEdition;
        public DateTimeOffset? DateEdition
        {
            get => _DateEdition;
            set
            {
                if (_DateEdition != value)
                {
                    _DateEdition = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTimeOffset? _DateRemiseLivre;
        public DateTimeOffset? DateRemiseLivre
        {
            get => _DateRemiseLivre;
            set
            {
                if (_DateRemiseLivre != value)
                {
                    _DateRemiseLivre = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTimeOffset? _DateAcquisition;
        public DateTimeOffset? DateAcquisition
        {
            get => _DateAcquisition;
            set
            {
                if (_DateAcquisition != value)
                {
                    _DateAcquisition = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsJourAcquisitionKnow;
        public bool IsJourAcquisitionKnow
        {
            get => _IsJourAcquisitionKnow;
            set
            {
                if (_IsJourAcquisitionKnow != value)
                {
                    _IsJourAcquisitionKnow = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsMoisAcquisitionKnow;
        public bool IsMoisAcquisitionKnow
        {
            get => _IsMoisAcquisitionKnow;
            set
            {
                if (_IsMoisAcquisitionKnow != value)
                {
                    _IsMoisAcquisitionKnow = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsJourAcquisitionVisible = true;
        public bool IsJourAcquisitionVisible
        {
            get => _IsJourAcquisitionVisible;
            set
            {
                if (_IsJourAcquisitionVisible != value)
                {
                    _IsJourAcquisitionVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsMoisAcquisitionVisible = true;
        public bool IsMoisAcquisitionVisible
        {
            get => _IsMoisAcquisitionVisible;
            set
            {
                if (_IsMoisAcquisitionVisible != value)
                {
                    _IsMoisAcquisitionVisible = value;
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


        [JsonIgnore]
        public readonly IEnumerable<string> MoneyList = Code.Helpers.CultureHelpers.Money.MoneyList();

        private double _Price;
        public double Price
        {
            get => _Price;
            set
            {
                if (_Price != value)
                {
                    _Price = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _DeviceName;
        public string DeviceName
        {
            get => _DeviceName;
            set
            {
                if (_DeviceName != value)
                {
                    _DeviceName = value;
                    OnPropertyChanged();
                }
            }
        }

        private LivreEtatVM _Etat = new LivreEtatVM();
        public LivreEtatVM Etat
        {
            get => _Etat;
            set
            {
                if (_Etat != value)
                {
                    _Etat = value;
                    OnPropertyChanged();
                }
            }
        }

        private ContactVM _ContactSource;
        public ContactVM ContactSource
        {
            get => _ContactSource;
            set
            {
                if (_ContactSource != value)
                {
                    _ContactSource = value;
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
