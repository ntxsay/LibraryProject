using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Contact;
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
    public  class LivreVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public long Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public long CountOpening { get; set; }
        private short _NbExactExemplaire;
        public short NbExactExemplaire
        {
            get => _NbExactExemplaire;
            set
            {
                if (_NbExactExemplaire != value)
                {
                    _NbExactExemplaire = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _MainTitle;
        public string MainTitle
        {
            get => _MainTitle;
            set
            {
                if (_MainTitle != value)
                {
                    _MainTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<string> _TitresOeuvre = new ObservableCollection<string>();
        public ObservableCollection<string> TitresOeuvre
        {
            get => _TitresOeuvre;
            set
            {
                if (_TitresOeuvre != value)
                {
                    _TitresOeuvre = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<AuthorVM> _Auteurs = new ObservableCollection<AuthorVM>();
        public ObservableCollection<AuthorVM> Auteurs
        {
            get => _Auteurs;
            set
            {
                if (_Auteurs != value)
                {
                    _Auteurs = value;
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

        private string _Resume;
        public string Resume
        {
            get => _Resume;
            set
            {
                if (_Resume != value)
                {
                    _Resume = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Notes;
        public string Notes
        {
            get => _Notes;
            set
            {
                if (_Notes != value)
                {
                    _Notes = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _AnneeParution = DateTime.Now.Year;
        public int AnneeParution
        {
            get => _AnneeParution;
            set
            {
                if (_AnneeParution != value)
                {
                    _AnneeParution = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private DateTime _DateAjout = DateTime.UtcNow;
        public DateTime DateAjout
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

        private DateTimeOffset? _DateAjoutUser = DateTime.UtcNow;
        public DateTimeOffset? DateAjoutUser
        {
            get => _DateAjoutUser;
            set
            {
                if (_DateAjoutUser != value)
                {
                    _DateAjoutUser = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _DateEdition;
        public DateTime? DateEdition
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

        private string _JaquettePath = EsLibrary.LibraryDefaultJaquette;

        [JsonIgnore]
        public string JaquettePath
        {
            get => this._JaquettePath;
            set
            {
                if (this._JaquettePath != value)
                {
                    this._JaquettePath = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public List<BibliothequeVM> Bibliotheques { get; set; }
        private ObservableCollection<CategorieLivreVM> _Categories = new ObservableCollection<CategorieLivreVM>();
        public ObservableCollection<CategorieLivreVM> Categories
        {
            get => _Categories;
            set
            {
                if (_Categories != value)
                {
                    _Categories = value;
                    OnPropertyChanged();
                }
            }
        }
        public PretLivreVM Pret { get; set; }
        public LivreIdentificationVM Identification { get; set; } = new LivreIdentificationVM();

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class PretLivreVM
    {
        public int Id { get; set; }
        public LivreVM Livre { get; set; }
        public ContactVM Emprunteur { get; set; }
        public DateTimeOffset DatePret { get; set; }
        public DateTimeOffset? DateRemise { get; set; }
        public string ObservationAv { get; set; }
        public string ObservationAp { get; set; }
    }
}
