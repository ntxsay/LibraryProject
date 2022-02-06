using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.UserControls.TitleBar
{
    public sealed partial class TitleBarLibraryName : UserControl
    {
        private BibliothequeVM ViewModel { get; set; }
        public TitleBarLibraryName()
        {
            this.InitializeComponent();
        }

        public TitleBarLibraryName(BibliothequeVM _viewModel)
        {
            this.InitializeComponent();
            ViewModel = _viewModel;
        }
    }
}
