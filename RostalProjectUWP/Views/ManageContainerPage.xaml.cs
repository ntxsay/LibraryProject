﻿using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ManageContainerPage : Page
    {
        private ManageBookParametersVM BookParameters { get; set; }
        private ManageLibraryParametersVM LibraryParameters { get; set; }
        public ManageContainerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is ManageLibraryParametersVM libraryParameters)
            {
                LibraryParameters = libraryParameters;
                NavigateToView(typeof(ManageLibraryPage), new ManageLibraryParametersVM() { ParentPage = this, ViewModelList = libraryParameters.ViewModelList, EditMode = libraryParameters.EditMode });
            }
            else if (e.Parameter is ManageBookParametersVM bookParameters)
            {
                BookParameters = bookParameters;
                NavigateToView(typeof(ManageBookPage), new ManageBookParametersVM() { ParentPage = this, ViewModel = bookParameters.ViewModel, EditMode = bookParameters.EditMode });
            }
        }

        public void NavigateToView(Type page, object parameters)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _ = FrameContainer.Navigate(page, parameters, new EntranceNavigationTransitionInfo());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
