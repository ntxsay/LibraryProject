using RostalProjectUWP.Code.Services.Db;
using RostalProjectUWP.Code.Services.ES;
using RostalProjectUWP.Models.Local;
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
    [Obsolete]
    public sealed partial class ManageLibraryGeneralPage : Page
    {
        private ManageLibraryPage _parentPage;
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
                _parentPage = parameters.ParentPage;
                PageViewModel.ViewModelList = new ObservableCollection<BibliothequeVM>(parameters.ViewModelList);
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
                    ViewModelList = PageViewModel.ViewModelList,
                });

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var value = dialog.Value?.Trim();
                    var description = dialog.Description?.Trim();

                    var newViewModel = new BibliothequeVM()
                    {
                        Name = value,
                        Description = description,
                    };

                    var creationResult = await DbServices.Library.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        PageViewModel.ViewModelList.Add(newViewModel);
                    }
                    else
                    {
                        //Erreur
                    }
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
                        ViewModelList = PageViewModel.ViewModelList,
                    });

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        var newValue = dialog.Value?.Trim();
                        var newDescription = dialog.Description?.Trim();

                        var updatedViewModel = new BibliothequeVM()
                        {
                            Id = _viewModel.Id,
                            Name = newValue,
                            Description = newDescription,
                            DateEdition = DateTime.UtcNow,
                        };

                        var updateResult = await DbServices.Library.UpdateAsync(updatedViewModel);
                        if (updateResult.IsSuccess)
                        {
                            //_viewModel.Name = newValue;
                            PageViewModel.SelectedLibrary.Name = newValue;
                            PageViewModel.SelectedLibrary.Description = newDescription;
                        }
                        else
                        {
                            //Erreur
                        }
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
                    PageViewModel.ViewModelList.Remove(_viewModel);
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
                bool isFileSaved = await Files.Serialization.Json.SerializeAsync(PageViewModel.ViewModelList, savedFile);
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
                if (ListviewLibrary.SelectedItem != null && ListviewLibrary.SelectedItem is BibliothequeVM _viewModel && _viewModel == PageViewModel.SelectedLibrary)
                {
                    var dialog = new NewCategorieCD(new ManageCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ViewModelList = _viewModel.Categories,
                    });

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        var value = dialog.Value?.Trim();
                        var description = dialog.Description?.Trim();

                        var newViewModel = new CategorieLivreVM()
                        {
                            IdLibrary = _viewModel.Id,
                            Name = value,
                            Description = description,
                        };

                        var creationResult = await DbServices.Categorie.CreateAsync(newViewModel);
                        if (creationResult.IsSuccess)
                        {
                            newViewModel.Id = creationResult.Id;
                            _viewModel.Categories.Add(newViewModel);
                        }
                        else
                        {
                            //Erreur
                        }
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
                if (ListviewLibrary.SelectedItem != null && ListviewLibrary.SelectedItem is BibliothequeVM _viewModel && _viewModel == PageViewModel.SelectedLibrary &&
                    TreeCategorie.SelectedItem != null && TreeCategorie.SelectedItem is CategorieLivreVM categorieParent)
                {
                    var dialog = new NewCategorieCD(new ManageSubCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ViewModelList = categorieParent.SubCategorieLivres,
                        Categorie = categorieParent,
                    });

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        var value = dialog.Value?.Trim();
                        var description = dialog.Description?.Trim();
                        
                        var newViewModel = new SubCategorieLivreVM()
                        {
                            IdCategorie = categorieParent.Id,
                            Name = value,
                            Description = description,
                        };

                        var creationResult = await DbServices.SubCategorie.CreateAsync(newViewModel);
                        if (creationResult.IsSuccess)
                        {
                            newViewModel.Id = creationResult.Id;
                            categorieParent.SubCategorieLivres.Add(newViewModel);
                        }
                        else
                        {
                            //Erreur
                        }
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
                if (ListviewLibrary.SelectedItem != null && ListviewLibrary.SelectedItem is BibliothequeVM _viewModelLibrary && _viewModelLibrary == PageViewModel.SelectedLibrary && TreeCategorie.SelectedItem != null)
                {
                    if (TreeCategorie.SelectedItem is CategorieLivreVM _viewModelCategorie && _viewModelCategorie == PageViewModel.SelectedCategorie)
                    {
                        var parentViewModel = GetParentCategorie();
                        var dialog = new NewCategorieCD(new ManageCategorieDialogParametersVM()
                        {
                            Value = _viewModelCategorie.Name,
                            Description = _viewModelCategorie.Description,
                            EditMode = Code.EditMode.Edit,
                            ViewModelList = _viewModelLibrary.Categories,
                        });

                        var result = await dialog.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            var newValue = dialog.Value?.Trim();
                            var newDescription = dialog.Description?.Trim();

                            var updatedViewModel = new CategorieLivreVM()
                            {
                                Id = _viewModelCategorie.Id,
                                IdLibrary = _viewModelLibrary.Id,
                                Name = newValue,
                                Description = newDescription,
                            };

                            var updateResult = await DbServices.Categorie.UpdateAsync(updatedViewModel);
                            if (updateResult.IsSuccess)
                            {
                                _viewModelCategorie.Name = newValue;
                                _viewModelCategorie.Description = newDescription;
                            }
                            else
                            {
                                //Erreur
                            }
                        }
                        else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                        {
                            return;
                        }
                    }
                    else if (TreeCategorie.SelectedItem is SubCategorieLivreVM _viewModelSubCategorie && _viewModelSubCategorie == PageViewModel.SelectedCategorie)
                    {
                        CategorieLivreVM viewModelParentCategorie = GetParentCategorie();
                        if (viewModelParentCategorie == null)
                        {
                            return;
                        }

                        var dialog = new NewCategorieCD(new ManageSubCategorieDialogParametersVM()
                        {
                            Value = _viewModelSubCategorie.Name,
                            Description = _viewModelSubCategorie.Description,
                            EditMode = Code.EditMode.Edit,
                            ViewModelList = viewModelParentCategorie?.SubCategorieLivres,
                            Categorie = viewModelParentCategorie,
                        });

                        var result = await dialog.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            var newValue = dialog.Value?.Trim();
                            var newDescription = dialog.Description?.Trim();

                            var updatedViewModel = new SubCategorieLivreVM()
                            {
                                Id = _viewModelSubCategorie.Id,
                                IdCategorie = viewModelParentCategorie.Id,
                                Name = newValue,
                                Description = newDescription,
                            };

                            var updateResult = await DbServices.SubCategorie.UpdateAsync(updatedViewModel);
                            if (updateResult.IsSuccess)
                            {
                                _viewModelSubCategorie.Name = newValue;
                                _viewModelSubCategorie.Description = newDescription;
                            }
                            else
                            {
                                //Erreur
                            }
                        }
                        else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                        {
                            return;
                        }
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
                if (ListviewLibrary.SelectedItem != null && ListviewLibrary.SelectedItem is BibliothequeVM _viewModel && _viewModel == PageViewModel.SelectedLibrary)
                {
                    var suggestedFileName = $"Rostalotheque_{_viewModel.Name}_Categories_{DateTime.Now:yyyyMMddHHmmss}";

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
                    bool isFileSaved = await Files.Serialization.Json.SerializeAsync(_viewModel.Categories, savedFile);// savedFile.Path
                    if (isFileSaved == false)
                    {
                        Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : Le flux n'a pas été enregistré dans le fichier.");
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

        private ObservableCollection<BibliothequeVM> _ViewModelList = new ObservableCollection<BibliothequeVM>();
        public ObservableCollection<BibliothequeVM> ViewModelList
        {
            get => _ViewModelList;
            set
            {
                if (_ViewModelList != value)
                {
                    _ViewModelList = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SelectedSCategorieName;
        public string SelectedSCategorieName
        {
            get => _SelectedSCategorieName;
            set
            {
                if (_SelectedSCategorieName != value)
                {
                    _SelectedSCategorieName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SelectedCategorieName;
        public string SelectedCategorieName
        {
            get => _SelectedCategorieName;
            set
            {
                if (_SelectedCategorieName != value)
                {
                    _SelectedCategorieName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SelectedSubCategorieName;
        public string SelectedSubCategorieName
        {
            get => _SelectedSubCategorieName;
            set
            {
                if (_SelectedSubCategorieName != value)
                {
                    _SelectedSubCategorieName = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _SelectedCategorie;
        public object SelectedCategorie
        {
            get => _SelectedCategorie;
            set
            {
                if (_SelectedCategorie != value)
                {
                    _SelectedCategorie = value;
                    OnPropertyChanged();
                    if (value != null)
                    {
                        if (value is CategorieLivreVM categorie)
                        {
                            SelectedSCategorieName = categorie.Name;
                            SelectedCategorieName = categorie.Name;
                            SelectedSubCategorieName = String.Empty;
                        }
                        else if (value is SubCategorieLivreVM subCategorie)
                        {
                            SelectedSCategorieName = subCategorie.Name;
                            SelectedCategorieName = String.Empty;
                            SelectedSubCategorieName = subCategorie.Name;
                        }
                    }
                    else
                    {
                        SelectedSCategorieName = String.Empty;
                        SelectedCategorieName = String.Empty;
                        SelectedSubCategorieName = String.Empty;
                    }
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
