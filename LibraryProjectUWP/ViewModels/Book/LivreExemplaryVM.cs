using LibraryProjectUWP.Code.Helpers;
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
        public long? IdContactSource { get; set; }


        [JsonIgnore]
        public LivreVM Parent { get; set; }

        private string _Source = LibraryHelpers.Book.Entry.Achat;
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

        private string _NoGroup;
        public string NoGroup
        {
            get => _NoGroup;
            set
            {
                if (_NoGroup != value)
                {
                    _NoGroup = value;
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

        private bool _IsExemplarySeparated;
        public bool IsExemplarySeparated
        {
            get => _IsExemplarySeparated;
            set
            {
                if (_IsExemplarySeparated != value)
                {
                    _IsExemplarySeparated = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _NbExemplaire = 1;
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

        private DateTimeOffset _DateAjout = DateTime.UtcNow;
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

        private string _DateAcquisition;
        public string DateAcquisition
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

        private string _DayAcquisition;
        
        [JsonIgnore]
        public string DayAcquisition
        {
            get => this._DayAcquisition;
            set
            {
                if (this._DayAcquisition != value)
                {
                    this._DayAcquisition = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _MonthAcquisition;
        
        [JsonIgnore]
        public string MonthAcquisition
        {
            get => this._MonthAcquisition;
            set
            {
                if (this._MonthAcquisition != value)
                {
                    this._MonthAcquisition = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _YearAcquisition;
        
        [JsonIgnore]
        public string YearAcquisition
        {
            get => this._YearAcquisition;
            set
            {
                if (this._YearAcquisition != value)
                {
                    this._YearAcquisition = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _DayRemise;
        [JsonIgnore]
        public string DayRemise
        {
            get => this._DayRemise;
            set
            {
                if (this._DayRemise != value)
                {
                    this._DayRemise = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _MonthRemise;
        [JsonIgnore]
        public string MonthRemise
        {
            get => this._MonthRemise;
            set
            {
                if (this._MonthRemise != value)
                {
                    this._MonthRemise = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _YearRemise;
        [JsonIgnore]
        public string YearRemise
        {
            get => this._YearRemise;
            set
            {
                if (this._YearRemise != value)
                {
                    this._YearRemise = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsJourAcquisitionKnow;
        [Obsolete]
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
        [Obsolete]
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
        [Obsolete]
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
        [Obsolete]
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

        private bool _IsPriceUnavailable;
        public bool IsPriceUnavailable
        {
            get => _IsPriceUnavailable;
            set
            {
                if (_IsPriceUnavailable != value)
                {
                    _IsPriceUnavailable = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<LivreEtatVM> _Etats = new ObservableCollection<LivreEtatVM>();
        public ObservableCollection<LivreEtatVM> Etats
        {
            get => _Etats;
            set
            {
                if (_Etats != value)
                {
                    _Etats = value;
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
