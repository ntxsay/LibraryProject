using LibraryProjectUWP.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Book
{
    public class LivrePretVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public long Id { get; set; }
        public long IdBook { get; set; } = -1;
        public long IdBookExemplary { get; set; }
        public LivreVM Livre { get; set; }

        private DateTimeOffset _DatePret = DateTime.UtcNow;
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

        private DateTimeOffset? _DateRemise;
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

        private LivreEtatVM _EtatAvantPret = new LivreEtatVM();
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

        private LivreEtatVM _EtatApresPret = new LivreEtatVM();
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
