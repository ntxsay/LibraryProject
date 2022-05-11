using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.Code.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media;
using System.Collections.ObjectModel;

namespace LibraryProjectUWP.ViewModels.Book
{
    public class LivrePretVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public long Id { get; set; }
        public long IdBook { get; set; } = -1;
        public string BookTitle { get; set; }
        public long IdBookExemplary { get; set; }
        public long NoExemplary { get; set; }
        public LivreVM Livre { get; set; }

        [JsonIgnore]
        public readonly IEnumerable<string> EtatList = LibraryHelpers.Book.EtatModelList;

        private SolidColorBrush _EventColor;
        public SolidColorBrush EventColor
        {
            get => _EventColor;
            set
            {
                if (_EventColor != value)
                {
                    _EventColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private LivreExemplaryVM _Exemplary;
        public LivreExemplaryVM Exemplary
        {
            get => _Exemplary;
            set
            {
                if (_Exemplary != value)
                {
                    _Exemplary = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTimeOffset _DatePret = DateTime.Now;
        public DateTimeOffset DatePret
        {
            get => _DatePret;
            set
            {
                if (_DatePret != value)
                {
                    _DatePret = value;
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

        private TimeSpan? _TimePret;
        public TimeSpan? TimePret
        {
            get => _TimePret;
            set
            {
                if (_TimePret != value)
                {
                    _TimePret = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTimeOffset? _DateRemise = null;
        public DateTimeOffset? DateRemise
        {
            get => _DateRemise;
            set
            {
                if (_DateRemise != value)
                {
                    _DateRemise = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _DateRealRemise = null;
        public DateTime? DateRealRemise
        {
            get => _DateRealRemise;
            set
            {
                if (_DateRealRemise != value)
                {
                    _DateRealRemise = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan? _TimeRemise;
        public TimeSpan? TimeRemise
        {
            get => _TimeRemise;
            set
            {
                if (_TimeRemise != value)
                {
                    _TimeRemise = value;
                    OnPropertyChanged();
                }
            }
        }

        private ContactVM _Emprunteur;
        public ContactVM Emprunteur
        {
            get => _Emprunteur;
            set
            {
                if (_Emprunteur != value)
                {
                    _Emprunteur = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _IdEmprunteur;
        public long? IdEmprunteur
        {
            get => _IdEmprunteur;
            set
            {
                if (_IdEmprunteur != value)
                {
                    _IdEmprunteur = value;
                    OnPropertyChanged();
                }
            }
        }

        private LivreEtatVM _EtatAvantPret = new LivreEtatVM()
        {
            TypeVerification = Code.BookTypeVerification.AvantPret,
        };
        public LivreEtatVM EtatAvantPret
        {
            get => _EtatAvantPret;
            set
            {
                if (_EtatAvantPret != value)
                {
                    _EtatAvantPret = value;
                    OnPropertyChanged();
                }
            }
        }

        private LivreEtatVM _EtatApresPret = new LivreEtatVM()
        {
            TypeVerification = Code.BookTypeVerification.ApresPret,
        };

        public LivreEtatVM EtatApresPret
        {
            get => _EtatApresPret;
            set
            {
                if (_EtatApresPret != value)
                {
                    _EtatApresPret = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PretStatus()
        {
            try
            {
                if (DateRealRemise.HasValue)
                {
                    return "Exemplaires retournés";
                }
                else
                {
                    var compareDatePret = DatePret.DateTime.CompareDate(DateTime.Now);
                    if (compareDatePret == DateCompare.DateInferieur)
                    {
                        return "Prêt à venir";
                    }
                    else
                    {
                        if (!DateRemise.HasValue)
                        {
                            return "Durée du prêt indéterminée";
                        }
                        else
                        {
                            var compare = DateRemise.Value.DateTime.CompareDate(DateTime.Now);
                            switch (compare)
                            {
                                case DateCompare.DateSuperieur:
                                    return "Prêt en cours";
                                case DateCompare.DateEgal:
                                    return "Prêt en cours";
                                case DateCompare.DateInferieur:
                                    return "En attente du retour";
                                case DateCompare.Unknow:
                                    return "Status inconnu";
                                default:
                                    return "Status inconnu";
                            }
                        }

                    }
                }

            }
            catch (Exception)
            {
                return "Status inconnu";
            }
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LivrePretDayCellVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private int _Day;
        public int Day
        {
            get => _Day;
            set
            {
                if (_Day != value)
                {
                    _Day = value;
                    OnPropertyChanged();
                }
            }
        }

        private SolidColorBrush _DayColor;
        public SolidColorBrush DayColor
        {
            get => _DayColor;
            set
            {
                if (_DayColor != value)
                {
                    _DayColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _Date;
        public DateTime Date
        {
            get => _Date;
            set
            {
                if (_Date != value)
                {
                    _Date = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<LivrePretVM> _Prets = new ObservableCollection<LivrePretVM>();
        public ObservableCollection<LivrePretVM> Prets
        {
            get => _Prets;
            set
            {
                if (_Prets != value)
                {
                    _Prets = value;
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
