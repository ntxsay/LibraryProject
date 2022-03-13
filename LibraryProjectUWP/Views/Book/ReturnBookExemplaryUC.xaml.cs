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

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class ReturnBookExemplaryUC : UserControl
    {
        private ReturnBookExemplaryUCVM ViewModelPage = new ReturnBookExemplaryUCVM();
        public ReturnBookExemplaryUC()
        {
            this.InitializeComponent();
        }

        public string ResultMessage { get; set; }

        public InfoBarSeverity ResultMessageSeverity { get; set; }

        public bool IsResultMessageOpen { get; set; }

        public string ResultMessageTitle { get; set; }

        public LivrePretVM ViewModel
        {
            get { return (LivrePretVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(LivrePretVM),
                                                                typeof(ReturnBookExemplaryUC), new PropertyMetadata(null, new PropertyChangedCallback(OnViewModelChanged)));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ReturnBookExemplaryUC parent && e.NewValue is LivrePretVM _viewModel)
            {
                parent.ViewModelPage.ViewModel = _viewModel;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.ViewModel.EtatApresPret.Etat.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"L'état du livre après le prêt n'est pas renseigné.";
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return;
                }

                var state = await DbServices.BookPret.ReturnBookAsync(ViewModelPage.ViewModel.Id, ViewModelPage.ViewModel.Exemplary.Id, ViewModelPage.ViewModel.EtatApresPret.Etat, ViewModelPage.ViewModel.EtatApresPret.Observations);
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
    }

    public class ReturnBookExemplaryUCVM : INotifyPropertyChanged
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