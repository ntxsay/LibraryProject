﻿using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Services.ES;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Contact
{
    public class ContactVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public long Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        
        private ContactType _ContactType;
        public ContactType ContactType
        {
            get => _ContactType;
            set
            {
                if (_ContactType != value)
                {
                    _ContactType = value;
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

        private string _TitreCivilite;
        public string TitreCivilite
        {
            get => _TitreCivilite;
            set
            {
                if (_TitreCivilite != value)
                {
                    _TitreCivilite = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _NomNaissance;
        public string NomNaissance
        {
            get => _NomNaissance;
            set
            {
                if (_NomNaissance != value)
                {
                    _NomNaissance = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _NomUsage = String.Empty;
        public string NomUsage
        {
            get => _NomUsage;
            set
            {
                if (_NomUsage != value)
                {
                    _NomUsage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Prenom;
        public string Prenom
        {
            get => _Prenom;
            set
            {
                if (_Prenom != value)
                {
                    _Prenom = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _AutresPrenoms = String.Empty;
        public string AutresPrenoms
        {
            get => _AutresPrenoms;
            set
            {
                if (_AutresPrenoms != value)
                {
                    _AutresPrenoms = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _AdressePostal;
        public string AdressePostal
        {
            get => _AdressePostal;
            set
            {
                if (_AdressePostal != value)
                {
                    _AdressePostal = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Ville;
        public string Ville
        {
            get => _Ville;
            set
            {
                if (_Ville != value)
                {
                    _Ville = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _CodePostal;
        public string CodePostal
        {
            get => _CodePostal;
            set
            {
                if (_CodePostal != value)
                {
                    _CodePostal = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _AdresseMail;
        public string AdresseMail
        {
            get => _AdresseMail;
            set
            {
                if (_AdresseMail != value)
                {
                    _AdresseMail = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _NoTelephone;
        public string NoTelephone
        {
            get => _NoTelephone;
            set
            {
                if (_NoTelephone != value)
                {
                    _NoTelephone = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _NoMobile;
        public string NoMobile
        {
            get => _NoMobile;
            set
            {
                if (_NoMobile != value)
                {
                    _NoMobile = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Observation;
        public string Observation
        {
            get => _Observation;
            set
            {
                if (_Observation != value)
                {
                    _Observation = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SocietyName;
        public string SocietyName
        {
            get => _SocietyName;
            set
            {
                if (_SocietyName != value)
                {
                    _SocietyName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Nationality;
        public string Nationality
        {
            get => _Nationality;
            set
            {
                if (_Nationality != value)
                {
                    _Nationality = value;
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

        private DateTimeOffset? _DateNaissance;
        public DateTimeOffset? DateNaissance
        {
            get => _DateNaissance;
            set
            {
                if (_DateNaissance != value)
                {
                    _DateNaissance = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTimeOffset? _DateDeces;
        public DateTimeOffset? DateDeces
        {
            get => _DateDeces;
            set
            {
                if (_DateDeces != value)
                {
                    _DateDeces = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _LieuNaissance;
        public string LieuNaissance
        {
            get => _LieuNaissance;
            set
            {
                if (_LieuNaissance != value)
                {
                    _LieuNaissance = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _LieuDeces;
        public string LieuDeces
        {
            get => _LieuDeces;
            set
            {
                if (_LieuDeces != value)
                {
                    _LieuDeces = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Biographie;
        public string Biographie
        {
            get => _Biographie;
            set
            {
                if (_Biographie != value)
                {
                    _Biographie = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DisplayName
        {
            get => $"{NomNaissance} {Prenom}";
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
