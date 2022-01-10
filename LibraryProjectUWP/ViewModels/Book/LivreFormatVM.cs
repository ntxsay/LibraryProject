using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Book
{
    public class LivreFormatVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [JsonIgnore]
        public long Id { get; set; }
        
        [JsonIgnore]
        public LivreVM Parent { get; set; }

        public  IEnumerable<string> FormatModelList => new List<string>()
        {
            "Relié",
            "Broché",
            "Cartonné",
            "Poche",
            "Audio",
            "Ebook",
        };

        private string _Format;
        public string Format
        {
            get => _Format;
            set
            {
                if (_Format != value)
                {
                    _Format = value;
                    OnPropertyChanged();
                }
            }
        }

        private short _NbOfPages;
        public short NbOfPages
        {
            get => _NbOfPages;
            set
            {
                if (_NbOfPages != value)
                {
                    _NbOfPages = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _Hauteur;
        public double Hauteur
        {
            get => _Hauteur;
            set
            {
                if (_Hauteur != value)
                {
                    _Hauteur = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _Largeur;
        public double Largeur
        {
            get => _Largeur;
            set
            {
                if (_Largeur != value)
                {
                    _Largeur = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _Epaisseur;
        public double Epaisseur
        {
            get => _Epaisseur;
            set
            {
                if (_Epaisseur != value)
                {
                    _Epaisseur = value;
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
