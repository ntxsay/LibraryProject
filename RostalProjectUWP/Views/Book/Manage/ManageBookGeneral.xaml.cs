using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Book.Manage
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ManageBookGeneral : Page
    {
        private ManageBookPage _parentPage;
        private LivreVM ViewModel { get; set; }
        public ManageBookGeneralViewModel PageViewModel { get; set; } = new ManageBookGeneralViewModel();

        public ManageBookGeneral()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is ManageBookParentChildVM parameters)
            {
                ViewModel = parameters.ViewModel;
                _parentPage = parameters.ParentPage;
            }
        }

        #region Auteurs
        private void AddTitleToBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (!this.TBX_TitlesOeuvre.Text.IsStringNullOrEmptyOrWhiteSpace())
                {
                    string value = this.TBX_TitlesOeuvre.Text.Trim();
                    if (ViewModel.TitresOeuvre.Any())
                    {
                        bool IsAlreadyExist = ViewModel.TitresOeuvre.Any(c => c == value);
                        if (!IsAlreadyExist)
                        {
                            ViewModel.TitresOeuvre.Add(value);
                            this.TBX_TitlesOeuvre.Text = String.Empty;
                        }
                    }
                    else
                    {
                        ViewModel.TitresOeuvre.Add(value);
                        this.TBX_TitlesOeuvre.Text = String.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        private void RemoveTitleToBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.LBX_TitresOeuvre.SelectedIndex > -1)
                {
                    ViewModel.TitresOeuvre.RemoveAt(this.LBX_TitresOeuvre.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }
        #endregion

        #region Auteurs
        private void RemoveAuthorToBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.LBX_Auteurs.SelectedIndex > -1)
                {
                    ViewModel.Auteurs.RemoveAt(this.LBX_Auteurs.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        private void AddAuthorToBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (!this.TBX_Author.Text.IsStringNullOrEmptyOrWhiteSpace())
                {
                    string value = this.TBX_Author.Text.Trim();
                    if (ViewModel.Auteurs.Any())
                    {
                        bool IsAlreadyExist = ViewModel.Auteurs.Any(c => c == value);
                        if (!IsAlreadyExist)
                        {
                            ViewModel.Auteurs.Add(value);
                            this.TBX_Author.Text = String.Empty;
                        }
                    }
                    else
                    {
                        ViewModel.Auteurs.Add(value);
                        this.TBX_Author.Text = String.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        } 
        #endregion

        private void ErrorClickXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }
    }

    public class ManageBookGeneralViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        private int _CountError;
        public int CountError
        {
            get => _CountError;
            set
            {
                if (_CountError != value)
                {
                    _CountError = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<OperationStateVM> _ErrorList = new ObservableCollection<OperationStateVM>();
        public ObservableCollection<OperationStateVM> ErrorList
        {
            get => _ErrorList;
            set
            {
                if (_ErrorList != value)
                {
                    _ErrorList = value;
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
