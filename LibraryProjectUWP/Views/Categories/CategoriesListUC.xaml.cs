using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace LibraryProjectUWP.Views.Categories
{
    public sealed partial class CategoriesListUC : PivotItem
    {
        public readonly BookCategorieParametersDriverVM _parameters;
        public readonly Guid IdItem = Guid.NewGuid();

        public CategoriesListUCVM ViewModelPage { get; set; } = new CategoriesListUCVM();


        public delegate void CancelModificationEventHandler(CategoriesListUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(CategoriesListUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(CategoriesListUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;


        public CategoriesListUC()
        {
            this.InitializeComponent();
        }

        public CategoriesListUC(BookCategorieParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.Header = $"Catégories";
            ViewModelPage.ParentLibrary = parameters?.ParentLibrary;
            InitializeActionInfos();
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void InitializeActionInfos()
        {
            try
            {
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        private CategorieLivreVM GetParentCategorie()
        {
            try
            {
                if (TreeCategorie.SelectedItem != null && TreeCategorie.SelectedItem == ViewModelPage.SelectedCategorie)
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

        #region TreeView
        private void AddNewCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ParentLibrary != null)
                {
                    _parameters.ParentPage.AddNewCategory(ViewModelPage.ParentLibrary, ViewModelPage.Guid);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void AddNewSubCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ParentLibrary != null && TreeCategorie.SelectedItem != null && TreeCategorie.SelectedItem is CategorieLivreVM categorieParent)
                {

                    //var dialog = new NewCategorieCD(new ManageSubCategorieDialogParametersVM()
                    //{
                    //    EditMode = Code.EditMode.Create,
                    //    ViewModelList = categorieParent.SubCategorieLivres,
                    //    Categorie = categorieParent,
                    //});

                    //var result = await dialog.ShowAsync();
                    //if (result == ContentDialogResult.Primary)
                    //{
                    //    var value = dialog.Value?.Trim();
                    //    var description = dialog.Description?.Trim();

                    //    var newViewModel = new SubCategorieLivreVM()
                    //    {
                    //        IdCategorie = categorieParent.Id,
                    //        Name = value,
                    //        Description = description,
                    //    };

                    //    var creationResult = await DbServices.SubCategorie.CreateAsync(newViewModel);
                    //    if (creationResult.IsSuccess)
                    //    {
                    //        newViewModel.Id = creationResult.Id;
                    //        categorieParent.SubCategorieLivres.Add(newViewModel);
                    //    }
                    //    else
                    //    {
                    //        //Erreur
                    //    }
                    //}
                    //else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                    //{
                    //    return;
                    //}
                }
                else
                {
                    MyTeachingTip.Target = ABBAddSousCategorie;
                    MyTeachingTip.Title = ABBAddSousCategorie.Label;
                    MyTeachingTip.Subtitle = "Pour ajouter une sous-catégorie, ajoutez ou cliquez d'abord sur une catégorie dans l'arborescence à ci-dessous puis cliquez de nouveau sur ce bouton.\n\nAttention : il n'est pas possible d'ajouter une sous-catégorie à une autre sous-catégorie.";
                    MyTeachingTip.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void RenameSCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ParentLibrary != null && TreeCategorie.SelectedItem != null)
                {
                    if (TreeCategorie.SelectedItem is CategorieLivreVM _viewModelCategorie && _viewModelCategorie == ViewModelPage.SelectedCategorie)
                    {
                        _parameters.ParentPage.EditCategory(ViewModelPage.ParentLibrary, _viewModelCategorie, ViewModelPage.Guid);
                    }
                    else if (TreeCategorie.SelectedItem is SubCategorieLivreVM _viewModelSubCategorie && _viewModelSubCategorie == ViewModelPage.SelectedCategorie)
                    {
                        //CategorieLivreVM viewModelParentCategorie = GetParentCategorie();
                        //if (viewModelParentCategorie == null)
                        //{
                        //    return;
                        //}

                        //var dialog = new NewCategorieCD(new ManageSubCategorieDialogParametersVM()
                        //{
                        //    Value = _viewModelSubCategorie.Name,
                        //    Description = _viewModelSubCategorie.Description,
                        //    EditMode = Code.EditMode.Edit,
                        //    ViewModelList = viewModelParentCategorie?.SubCategorieLivres,
                        //    Categorie = viewModelParentCategorie,
                        //});

                        //var result = await dialog.ShowAsync();
                        //if (result == ContentDialogResult.Primary)
                        //{
                        //    var newValue = dialog.Value?.Trim();
                        //    var newDescription = dialog.Description?.Trim();

                        //    var updatedViewModel = new SubCategorieLivreVM()
                        //    {
                        //        Id = _viewModelSubCategorie.Id,
                        //        IdCategorie = viewModelParentCategorie.Id,
                        //        Name = newValue,
                        //        Description = newDescription,
                        //    };

                        //    var updateResult = await DbServices.SubCategorie.UpdateAsync(updatedViewModel);
                        //    if (updateResult.IsSuccess)
                        //    {
                        //        _viewModelSubCategorie.Name = newValue;
                        //        _viewModelSubCategorie.Description = newDescription;
                        //    }
                        //    else
                        //    {
                        //        //Erreur
                        //    }
                        //}
                        //else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                        //{
                        //    return;
                        //}
                    }
                }
                else
                {
                    MyTeachingTip.Target = ABBRenameCategorie;
                    MyTeachingTip.Title = "Renommer";
                    MyTeachingTip.Subtitle = "Pour renommer une catégorie ou une sous-catégorie, cliquez d'abord sur la catégorie ou la sous-catégorie que vous souhaitez renommer dans l'arborescence à ci-dessous puis cliquez de nouveau sur ce bouton.";
                    MyTeachingTip.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        private async void ExportTreeToJsonXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ParentLibrary == null)
                {
                    return;
                }
                var suggestedFileName = $"Rostalotheque_{ViewModelPage.ParentLibrary.Name}_Categories_{DateTime.Now:yyyyMMddHHmmss}";

                var savedFile = await Files.SaveStorageFileAsync(new Dictionary<string, IList<string>>()
                    {
                        {"JavaScript Object Notation", new List<string>() { ".json" } }
                    }, suggestedFileName);

                if (savedFile == null)
                {
                    Logs.Log(m, $"{m.ReflectedType.Name}.{m.Name} : Le fichier n'a pas pû être créé.");
                    return;
                }

                //Voir : https://docs.microsoft.com/fr-fr/windows/uwp/files/quickstart-reading-and-writing-files
                bool isFileSaved = await Files.Serialization.Json.SerializeAsync(ViewModelPage.ParentLibrary.Categories, savedFile);// savedFile.Path
                if (isFileSaved == false)
                {
                    Logs.Log(m, $"{m.ReflectedType.Name}.{m.Name} : Le flux n'a pas été enregistré dans le fichier.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private void CreateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                CreateItemRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void UpdateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                UpdateItemRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }


        private bool IsModelValided()
        {
            try
            {
                //if (ViewModelPage.Value.IsStringNullOrEmptyOrWhiteSpace())
                //{
                //    ViewModelPage.ResultMessage = $"Le nom de la bibliothèque ne peut pas être vide\nou ne contenir que des espaces blancs.";
                //    return false;
                //}

                //if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(a => a.Name.ToLower() == ViewModelPage.Value.Trim().ToLower()))
                //{
                //    var isError = !(_parameters.EditMode == Code.EditMode.Edit && _parameters.CurrentLibrary?.Name?.Trim().ToLower() == ViewModelPage.Value?.Trim().ToLower());
                //    if (isError)
                //    {
                //        TbxErrorMessage.Text = $"Cette bibliothèque existe déjà.";
                //        return false;
                //    }
                //}

                //ViewModelPage.ResultMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }


       

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CancelModificationRequested != null)
                {
                    CancelModificationRequested = null;
                }

                if (CreateItemRequested != null)
                {
                    CreateItemRequested = null;
                }

                if (UpdateItemRequested != null)
                {
                    UpdateItemRequested = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DeleteItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }

        private void MenuFlyout_Opened(object sender, object e)
        {

        }

        private void ASB_SearchEditor_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void ASB_SearchEditor_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void ASB_SearchEditor_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private void ASB_SearchCollection_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private void ASB_SearchCollection_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void ASB_SearchCollection_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }
    }

    public class CategoriesListUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public Guid Guid { get; private set; } = Guid.NewGuid();

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

        private string _Glyph = "\uE81E";
        public string Glyph
        {
            get => _Glyph;
            set
            {
                if (_Glyph != value)
                {
                    _Glyph = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private string _ArgName;
        public string ArgName
        {
            get => this._ArgName;
            set
            {
                if (this._ArgName != value)
                {
                    this._ArgName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        

        private Brush _ResultMessageForeGround;
        public Brush ResultMessageForeGround
        {
            get => this._ResultMessageForeGround;
            set
            {
                if (this._ResultMessageForeGround != value)
                {
                    this._ResultMessageForeGround = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private BibliothequeVM _ParentLibrary;
        public BibliothequeVM ParentLibrary
        {
            get => this._ParentLibrary;
            set
            {
                if (this._ParentLibrary != value)
                {
                    this._ParentLibrary = value;
                    this.OnPropertyChanged();
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
