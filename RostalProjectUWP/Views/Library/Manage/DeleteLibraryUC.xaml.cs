using RostalProjectUWP.Code;
using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.Code.Services.Logging;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace RostalProjectUWP.Views.Library.Manage
{
    public sealed partial class DeleteLibraryUC : UserControl
    {
        
        public DeleteLibraryUCVM ViewModelPage { get; set; } = new DeleteLibraryUCVM();

        public delegate void CancelModificationEventHandler(DeleteLibraryUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void DeleteLibraryWithSaveEventHandler(DeleteLibraryUC sender, ExecuteRequestedEventArgs e);
        public event DeleteLibraryWithSaveEventHandler DeleteLibraryWithSaveRequested;

        public delegate void DeleteLibraryWithOutSaveEventHandler(DeleteLibraryUC sender, ExecuteRequestedEventArgs e);
        public event DeleteLibraryWithOutSaveEventHandler DeleteLibraryWithOutSaveRequested;

        public DeleteLibraryUC()
        {
            this.InitializeComponent();
        }

        public DeleteLibraryUC(BibliothequeVM viewModel)
        {
            this.InitializeComponent();
            ViewModelPage.ViewModel = viewModel;
            ViewModelPage.Header = $"Supprimer la bibliothèque";
        }


        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CancelModificationRequested != null)
                {
                    CancelModificationRequested = null;
                }

                if (DeleteLibraryWithOutSaveRequested != null)
                {
                    DeleteLibraryWithOutSaveRequested = null;
                }

                if (DeleteLibraryWithSaveRequested != null)
                {
                    DeleteLibraryWithSaveRequested = null;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DeleteItemWithOutSaveXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                DeleteLibraryWithOutSaveRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DeleteItemWithSaveXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                DeleteLibraryWithSaveRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
    }

    public class DeleteLibraryUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private BibliothequeVM _ViewModel;
        public BibliothequeVM ViewModel
        {
            get => this._ViewModel;
            set
            {
                if (this._ViewModel != value)
                {
                    this._ViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Header;
        public string Header
        {
            get => this._Header;
            set
            {
                if (this._Header != value)
                {
                    this._Header = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get => this._ErrorMessage;
            set
            {
                if (this._ErrorMessage != value)
                {
                    this._ErrorMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
