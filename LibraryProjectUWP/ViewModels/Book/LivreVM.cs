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

        [Obsolete]
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

        private ObservableCollection<ContactVM> _Auteurs = new ObservableCollection<ContactVM>();
        public ObservableCollection<ContactVM> Auteurs
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
        [Obsolete]
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

        private ObservableCollection<LivreEtatVM> _EtatLivre = new ObservableCollection<LivreEtatVM>();
        public ObservableCollection<LivreEtatVM> EtatLivre
        {
            get => _EtatLivre;
            set
            {
                if (_EtatLivre != value)
                {
                    _EtatLivre = value;
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

        private ObservableCollection<LivreExemplaryVM> _Exemplaires = new ObservableCollection<LivreExemplaryVM>();
        public ObservableCollection<LivreExemplaryVM> Exemplaires
        {
            get => _Exemplaires;
            set
            {
                if (_Exemplaires != value)
                {
                    _Exemplaires = value;
                    OnPropertyChanged();
                }
            }
        }

        public PretLivreVM Pret { get; set; }
        public LivreDescriptionVM Description { get; set; } = new LivreDescriptionVM();
        public LivreClassificationAgeVM ClassificationAge { get; set; } = new LivreClassificationAgeVM();
        public LivreIdentificationVM Identification { get; set; } = new LivreIdentificationVM();
        public LivreFormatVM Format { get; set; } = new LivreFormatVM();
        public LivrePublicationVM Publication { get; set; } = new LivrePublicationVM();

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class LivreVMDisplayMode
    {
        public int Id { get; set; }
        public LivreVM Livre { get; set; }
        public ContactVM Emprunteur { get; set; }
        public DateTimeOffset DatePret { get; set; }
        public DateTimeOffset? DateRemise { get; set; }
        public string ObservationAv { get; set; }
        public string ObservationAp { get; set; }
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
