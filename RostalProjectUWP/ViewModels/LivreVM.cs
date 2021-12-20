using RostalProjectUWP.Code;
using RostalProjectUWP.Code.Services.ES;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RostalProjectUWP.ViewModels
{
    public  class LivreVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public long Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();


        private string _Cotation;
        public string Cotation
        {
            get => _Cotation;
            set
            {
                if (_Cotation != value)
                {
                    _Cotation = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private string _ISBN;
        /// <summary>
        /// L'ISBN est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string ISBN
        {
            get => _ISBN;
            set
            {
                if (_ISBN != value)
                {
                    _ISBN = value;
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

        private ObservableCollection<string> _Auteurs = new ObservableCollection<string>();
        public ObservableCollection<string> Auteurs
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

        private string _Description;
        public string Description
        {
            get => _Description;
            set
            {
                if (_Description != value)
                {
                    _Description = value;
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

        private bool _IsAnneeParutionUnknow;
        public bool IsAnneeParutionUnknow
        {
            get => _IsAnneeParutionUnknow;
            set
            {
                if (_IsAnneeParutionUnknow != value)
                {
                    _IsAnneeParutionUnknow = value;
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
