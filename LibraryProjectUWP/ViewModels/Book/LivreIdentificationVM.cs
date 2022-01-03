using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Book
{
    public class LivreIdentificationVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public long Id { get; set; }
        public LivreVM Parent { get; set; }
        
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

        private string _ASIN;
        /// <summary>
        /// L'ASIN est un numéro international (amazon) normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string ASIN
        {
            get => _ASIN;
            set
            {
                if (_ASIN != value)
                {
                    _ASIN = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ISSN;
        /// <summary>
        /// L'ISSN est un numéro international  normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string ISSN
        {
            get => _ISSN;
            set
            {
                if (_ISSN != value)
                {
                    _ISSN = value;
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

        private string _ISBN10;
        /// <summary>
        /// L'ISBN-10 est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string ISBN10
        {
            get => _ISBN10;
            set
            {
                if (_ISBN10 != value)
                {
                    _ISBN10 = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ISBN13;
        /// <summary>
        /// L'ISBN-13 est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string ISBN13
        {
            get => _ISBN13;
            set
            {
                if (_ISBN13 != value)
                {
                    _ISBN13 = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _CodeBarre;
        /// <summary>
        /// L'ISBN est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string CodeBarre
        {
            get => _CodeBarre;
            set
            {
                if (_CodeBarre != value)
                {
                    _CodeBarre = value;
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
