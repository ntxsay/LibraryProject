using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Boîte de dialogue de contenu, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookEditedCD : ContentDialog
    {
        private LivreVM ViewModel { get; set; }
        private ObservableCollection<PropertiesChangedVM> ChangedProperties { get; set; } = new ObservableCollection<PropertiesChangedVM>();
        public BookEditedCD()
        {
            this.InitializeComponent();
        }

        public BookEditedCD(LivreVM _viewModel, IEnumerable<PropertiesChangedVM> _changedProperties)
        {
            ViewModel = _viewModel;
            ChangedProperties = new ObservableCollection<PropertiesChangedVM>(_changedProperties);
            this.InitializeComponent();
        }
    }
}
