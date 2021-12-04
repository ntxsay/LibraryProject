using RostalProjectUWP.Code.Services.ES;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using RostalProjectUWP.Views.Book.Manage;
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

namespace RostalProjectUWP.Views.Library.Manage
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ManageLibraryGeneralPage : Page
    {
        private ManageLibraryPage _parentPage;
        private BibliothequeVM ViewModel { get; set; }
        private ObservableCollection<BibliothequeVM> ViewModelList { get; set; } = new ObservableCollection<BibliothequeVM>();
        public ManageBookCategorieViewModel PageViewModel { get; set; } = new ManageBookCategorieViewModel();
        public ManageLibraryGeneralPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is ManageLibraryParentChildVM parameters)
            {
                ViewModel = parameters.ViewModel;
                _parentPage = parameters.ParentPage;

            }
        }

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

        private async void AddNewLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var dialog = new NewLibraryCD(new ManageLibraryDialogParametersVM()
                {
                    EditMode = Code.EditMode.Create,
                    ViewModelList = ViewModelList,
                });

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var value = dialog.Value?.Trim();
                    var description = dialog.Description?.Trim();
                    ViewModelList.Add(new BibliothequeVM()
                    {
                        Name = value,
                        Description = description,
                    });
                }
                else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        private async void RenameLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (PageViewModel.SelectedLibrary != null && ListviewLibrary.SelectedItem != null && ListviewLibrary.SelectedItem is BibliothequeVM _viewModel &&
                    _viewModel == PageViewModel.SelectedLibrary)
                {
                    var parentViewModel = GetParentCategorie();
                    var dialog = new NewLibraryCD(new ManageLibraryDialogParametersVM()
                    {
                        Value = _viewModel.Name,
                        Description = _viewModel.Description,
                        EditMode = Code.EditMode.Edit,
                        ViewModelList = ViewModelList,
                    });

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        var newValue = dialog.Value?.Trim();
                        var newDescription = dialog.Description?.Trim();

                        //_viewModel.Name = newValue;
                        PageViewModel.SelectedLibrary.Name = newValue;
                        PageViewModel.SelectedLibrary.Description = newDescription;
                    }
                    else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                    {
                        return;
                    }
                }
                else
                {
                    RenameLibraryTeachingTip.Target = ABBRenameLibrary;
                    RenameLibraryTeachingTip.Title = ABBRenameLibrary.Label;
                    RenameLibraryTeachingTip.Subtitle = "Pour renommer une bibliothèque, ajoutez ou cliquez d'abord sur une bibliothèque dans la liste ci-dessous puis cliquez de nouveau sur ce bouton.";
                    RenameLibraryTeachingTip.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        private void DeleteLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (PageViewModel.SelectedLibrary != null && ListviewLibrary.SelectedItem != null && ListviewLibrary.SelectedItem is BibliothequeVM _viewModel &&
                    _viewModel == PageViewModel.SelectedLibrary)
                {
                    ViewModelList.Remove(_viewModel);
                }
                else
                {
                    RenameLibraryTeachingTip.Target = ABBDeleteLibrary;
                    RenameLibraryTeachingTip.Title = ABBDeleteLibrary.Label;
                    RenameLibraryTeachingTip.Subtitle = "Pour supprimer une bibliothèque, cliquez d'abord sur une bibliothèque dans la liste ci-dessous puis cliquez de nouveau sur ce bouton.";
                    RenameLibraryTeachingTip.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        private async void ExportLibraryToJsonXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var suggestedFileName = $"Rostalotheque_Bibliothèques_{DateTime.Now:yyyyMMddHHmmss}";

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
                bool isFileSaved = await Files.Serialization.Json.SerializeAsync(ViewModelList, savedFile);
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

        #region TreeView
        private async void AddNewCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (PageViewModel.SelectedLibrary != null && ListviewLibrary.SelectedItem != null && ListviewLibrary.SelectedItem is BibliothequeVM _viewModel &&
                    _viewModel == PageViewModel.SelectedLibrary)
                {
                    var dialog = new NewCategorieCD(new ManageCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Create,
                        Type = Code.CategorieType.Categorie,
                        ViewModelList = _viewModel.Categories,
                    });

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        var value = dialog.Value?.Trim();
                        var description = dialog.Description?.Trim();
                        _viewModel.Categories.Add(new CategorieLivreVM()
                        {
                            Name = value,
                            Description = description,
                            CategorieType = Code.CategorieType.Categorie,
                            SubCategorieLivres = new ObservableCollection<CategorieLivreVM>()
                        });
                    }
                    else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                    {
                        return;
                    }
                }
                
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
                if (TreeCategorie.SelectedItem != null && TreeCategorie.SelectedItem is CategorieLivreVM viewModel && viewModel.CategorieType == Code.CategorieType.Categorie)
                {
                    var dialog = new NewCategorieCD(new ManageCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Create,
                        Type = Code.CategorieType.SubCategorie,
                        ViewModelList = viewModel.SubCategorieLivres,
                        ParentName = viewModel.Name,
                    });

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        var value = dialog.Value?.Trim();
                        var description = dialog.Description?.Trim();
                        viewModel.SubCategorieLivres.Add(new CategorieLivreVM()
                        {
                            Name = value,
                            Description = description,
                            CategorieType = Code.CategorieType.SubCategorie,
                        });
                    }
                    else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                    {
                        return;
                    }
                }
                else
                {
                    AddSubCategorieTeachingTip.Title = ABBAddSousCategorie.Label;
                    AddSubCategorieTeachingTip.Subtitle = "Pour ajouter une sous-catégorie, ajoutez ou cliquez d'abord sur une catégorie dans l'arborescence à gauche puis cliquez de nouveau sur ce bouton.\n\nAttention : il n'est pas possible d'ajouter une sous-catégorie à une autre sous-catégorie.";
                    AddSubCategorieTeachingTip.IsOpen = true;
                }
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
                if (PageViewModel.SelectedCategorie != null && TreeCategorie.SelectedItem != null && TreeCategorie.SelectedItem is CategorieLivreVM _viewModel &&
                    _viewModel == PageViewModel.SelectedCategorie)
                {
                    var parentViewModel = GetParentCategorie();
                    var dialog = new NewCategorieCD(new ManageCategorieDialogParametersVM()
                    {
                        Value = _viewModel.Name,
                        Description = _viewModel.Description,
                        EditMode = Code.EditMode.Edit,
                        Type = _viewModel.CategorieType,
                        ViewModelList = _viewModel.CategorieType == Code.CategorieType.SubCategorie ? parentViewModel?.SubCategorieLivres : ViewModel.Categories,
                        ParentName = _viewModel.CategorieType == Code.CategorieType.SubCategorie ? parentViewModel?.Name : String.Empty,
                    });

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        var newValue = dialog.Value?.Trim();
                        var newDescription = dialog.Description?.Trim();
                        var _container = this.TreeCategorie.ContainerFromItem(this.TreeCategorie.SelectedItem);
                        if (_container is Microsoft.UI.Xaml.Controls.TreeViewItem treeviewItem)
                        {
                            treeviewItem.Content = newValue;
                        }

                        _viewModel.Name = newValue;
                        _viewModel.Description = newDescription;
                        PageViewModel.SelectedCategorie = null;
                        PageViewModel.SelectedCategorie = _viewModel;
                    }
                    else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                    {
                        return;
                    }
                }
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

        private BibliothequeVM _SelectedLibrary;
        public BibliothequeVM SelectedLibrary
        {
            get => _SelectedLibrary;
            set
            {
                if (_SelectedLibrary != value)
                {
                    _SelectedLibrary = value;
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
