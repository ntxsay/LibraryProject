using RostalProjectUWP.Code;
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
    public class CategorieLivreVM// : INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public CategorieType CategorieType { get; set; }
        //public CategorieType CategorieType
        //{
        //    get => _CategorieType;
        //    set
        //    {
        //        if (_CategorieType != value)
        //        {
        //            _CategorieType = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        public ObservableCollection<CategorieLivreVM> SubCategorieLivres { get; set; } = new ObservableCollection<CategorieLivreVM>();
        //public ObservableCollection<CategorieLivreVM> SubCategorieLivres
        //{
        //    get => SubCategorieLivres;
        //    set
        //    {
        //        if (SubCategorieLivres != value)
        //        {
        //            SubCategorieLivres = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        public List<LivreVM> Livres { get; set; }

        //public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    // Raise the PropertyChanged event, passing the name of the property whose value has changed.
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }

    public class SubCategorieLivreVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<LivreVM> Livres { get; set; }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
