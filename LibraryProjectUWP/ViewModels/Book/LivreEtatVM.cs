using LibraryProjectUWP.Code;
using LibraryProjectUWP.ViewModels.Collection;
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
    public class LivreEtatVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [JsonIgnore]
        public long Id { get; set; }

        [JsonIgnore]
        public long IdBookExemplary { get; set; }


        [JsonIgnore]
        public LivreVM Parent { get; set; }

        private string _Etat;
        public string Etat
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

        private DateTime _DateVerification = DateTime.UtcNow;
        public DateTime DateVerification
        {
            get => _DateVerification;
            set
            {
                if (_DateVerification != value)
                {
                    _DateVerification = value;
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

        private BookTypeVerification _TypeVerification;
        public BookTypeVerification TypeVerification
        {
            get => _TypeVerification;
            set
            {
                if (_TypeVerification != value)
                {
                    _TypeVerification = value;
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
