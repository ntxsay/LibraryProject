using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using Microsoft.UI.Xaml.Controls;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Boîte de dialogue de contenu, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class ReturnBookExemplaryCD : ContentDialog
    {
        public ReturnBookExemplaryCD()
        {
            this.InitializeComponent();
        }

        public ReturnBookExemplaryCD(LivrePretVM viewModel)
        {
            ViewModelPage = new ReturnBookExemplaryCDVM()
            {
                ViewModel = viewModel
            };
            this.InitializeComponent();
        }

        public ReturnBookExemplaryCDVM ViewModelPage { get; set; }
        

        public string ResultMessage { get; set; }

        public InfoBarSeverity ResultMessageSeverity { get; set; }

        public bool IsResultMessageOpen { get; set; }

        public string ResultMessageTitle { get; set; }


        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (ViewModelPage.ViewModel.EtatApresPret.Etat.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"L'état du livre après le prêt n'est pas renseigné.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    args.Cancel = true;
                    return;
                }

                var state = await DbServices.Book.ReturnBookAsync(ViewModelPage.ViewModel.Id, ViewModelPage.ViewModel.Exemplary.Id, ViewModelPage.ViewModel.EtatApresPret.Etat, ViewModelPage.ViewModel.EtatApresPret.Observations);
                if (state.IsSuccess)
                {
                    ViewModelPage.ResultMessageTitle = "Succès";
                    ViewModelPage.ResultMessage = state.Message;
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                    ViewModelPage.IsResultMessageOpen = true;
                }
                else
                {
                    //Erreur
                    ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                    ViewModelPage.ResultMessage = state.Message;
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                    ViewModelPage.IsResultMessageOpen = true;
                    args.Cancel = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

    }

    public class ReturnBookExemplaryCDVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public readonly IEnumerable<string> EtatList = LibraryHelpers.Book.EtatModelList;

        private string _ResultMessage;
        public string ResultMessage
        {
            get => this._ResultMessage;
            set
            {
                if (this._ResultMessage != value)
                {
                    this._ResultMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private InfoBarSeverity _ResultMessageSeverity = InfoBarSeverity.Informational;
        public InfoBarSeverity ResultMessageSeverity
        {
            get => this._ResultMessageSeverity;
            set
            {
                if (this._ResultMessageSeverity != value)
                {
                    this._ResultMessageSeverity = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsResultMessageOpen;
        public bool IsResultMessageOpen
        {
            get => this._IsResultMessageOpen;
            set
            {
                if (this._IsResultMessageOpen != value)
                {
                    this._IsResultMessageOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ResultMessageTitle;
        public string ResultMessageTitle
        {
            get => this._ResultMessageTitle;
            set
            {
                if (this._ResultMessageTitle != value)
                {
                    this._ResultMessageTitle = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private LivrePretVM _ViewModel;
        public LivrePretVM ViewModel
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


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
