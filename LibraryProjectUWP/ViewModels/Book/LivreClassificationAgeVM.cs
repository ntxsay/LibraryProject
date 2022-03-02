using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.Code;
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
    public class LivreClassificationAgeVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [JsonIgnore]
        public long Id { get; set; }
        
        [JsonIgnore]
        public LivreVM Parent { get; set; }

        private ClassificationAgeType _TypeClassification = ClassificationAgeType.ToutPublic;
        public ClassificationAgeType TypeClassification
        {
            get => _TypeClassification;
            set
            {
                if (_TypeClassification != value)
                {
                    _TypeClassification = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte _ApartirDe;
        /// <summary>
        /// A partir de tel age
        /// </summary>
        public byte ApartirDe
        {
            get => _ApartirDe;
            set
            {
                if (_ApartirDe != value)
                {
                    _ApartirDe = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte _Jusqua;
        /// <summary>
        /// Jusqu'à tel age
        /// </summary>
        public byte Jusqua
        {
            get => _Jusqua;
            set
            {
                if (_Jusqua != value)
                {
                    _Jusqua = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte _DeTelAge;
        /// <summary>
        /// De tel age ... suite
        /// </summary>
        public byte DeTelAge
        {
            get => _DeTelAge;
            set
            {
                if (_DeTelAge != value)
                {
                    _DeTelAge = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte _ATelAge;
        /// <summary>
        /// ... à tel age
        /// </summary>
        public byte ATelAge
        {
            get => _ATelAge;
            set
            {
                if (_ATelAge != value)
                {
                    _ATelAge = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _StringClassification;
        public string StringClassification
        {
            get => _StringClassification;
            set
            {
                if (_StringClassification != value)
                {
                    _StringClassification = value;
                    OnPropertyChanged();
                }
            }
        }

        public string GetClassificationAge()
        {
            try
            {
                string result = string.Empty;
                switch (this.TypeClassification)
                {
                    case ClassificationAgeType.ToutPublic:
                        result =  "Tout public";
                        break;
                    case ClassificationAgeType.ApartirDe:
                        result = $"A partir de {this.ApartirDe} ans";
                        break;
                    case ClassificationAgeType.Jusqua:
                        result = $"Jusqu'à {this.Jusqua} ans";
                        break;
                    case ClassificationAgeType.DeTantATant:
                        if (this.DeTelAge == this.ATelAge)
                        {
                            result =  $"{this.DeTelAge} ans uniquement";
                        }
                        else
                        {
                            result = $"De {this.DeTelAge} à {this.ATelAge} ans";
                        }
                        break;
                    default:
                        break;
                }

                this.StringClassification = result;
                return result;
            }
            catch (Exception)
            {
                this.StringClassification = string.Empty;
                return string.Empty;
            }
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
