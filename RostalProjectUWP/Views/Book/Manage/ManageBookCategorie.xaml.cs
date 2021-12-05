using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using RostalProjectUWP.Code.Services.ES;
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

namespace RostalProjectUWP.Views.Book.Manage
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ManageBookCategorie : Page
    {
        private ManageBookPage _parentPage;
        private LivreVM ViewModel { get; set; }
        public ManageBookCategorieViewModel PageViewModel { get; set; } = new ManageBookCategorieViewModel();
        public ManageBookCategorie()
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

        #region TreeView
        private CategorieLivreVM GetParentCategorie()
        {
            try
            {
                if (TreeCategorie.SelectedItem != null && TreeCategorie.SelectedItem == PageViewModel.SelectedCategorie)
                {
                    if (TreeCategorie.SelectedNode.Parent.Content is CategorieLivreVM viewModel)
                    {
                        return viewModel;
                    }
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void AddNewCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
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

        private async void AddNewSubCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
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

        private async void RenameSCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
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

        private async void ExportTreeToJsonXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var suggestedFileName = $"Rostalotheque_Arborescence_Categories_{DateTime.Now:yyyyMMddHHmmss}";

                var savedFile = await Files.SaveStorageFileAsync(new Dictionary<string, IList<string>>()
                {
                    {"JavaScript Object Notation", new List<string>() { ".json" } }
                }, suggestedFileName);

                if (savedFile == null)
                {
                    Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : Le fichier n'a pas pû être créé.");
                    return;
                }

                //Voir : https://docs.microsoft.com/fr-fr/windows/uwp/files/quickstart-reading-and-writing-files
                bool isFileSaved = await Files.Serialization.Json.SerializeAsync(ViewModel.Categories, savedFile);// savedFile.Path
                if (isFileSaved == false)
                {
                    Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : Le flux n'a pas été enregistré dans le fichier.");
                    return;
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

        private void TbxCategorieDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is TextBox tb && PageViewModel.SelectedCategorie != null)
                {
                    PageViewModel.SelectedCategorie.Description = tb.Text;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        
    }

    public class ManageBookCategorieViewModel : INotifyPropertyChanged
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

        private CategorieLivreVM _SelectedCategorie;
        public CategorieLivreVM SelectedCategorie
        {
            get => _SelectedCategorie;
            set
            {
                if (_SelectedCategorie != value)
                {
                    _SelectedCategorie = value;
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
