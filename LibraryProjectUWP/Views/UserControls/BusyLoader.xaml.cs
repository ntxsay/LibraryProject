using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.ViewModels.General;
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

namespace LibraryProjectUWP.Views.UserControls
{
    public sealed partial class BusyLoader : Grid
    {
        public BusyLoaderParametersVM Parameters { get; private set; }
        public BusyLoader(BusyLoaderParametersVM parameters) 
        {
            this.InitializeComponent();
            Parameters = parameters;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Parameters != null && Parameters.Callback != null)
                {
                    Parameters.Callback();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
                                                                typeof(BusyLoader), new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BusyLoader parent && e.NewValue is string title)
            {
                if (!title.IsStringNullOrEmptyOrWhiteSpace())
                {
                    parent.TbcTitle.Text = title.Trim();
                }
            }
        }
    }
}
