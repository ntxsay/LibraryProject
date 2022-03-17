﻿using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Categorie;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
        public readonly CategorieParameterDriverVM _parameters;
        public readonly Guid IdItem = Guid.NewGuid();

        public CategoriesListUCVM ViewModelPage { get; set; } = new CategoriesListUCVM();


        public delegate void CancelModificationEventHandler(CategoriesListUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(CategoriesListUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(CategoriesListUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;

        private object _SelectedItem;
        public object SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                }

                GetSelectedNodes();
            }
        }

        public CategoriesListUC()
        {
            this.InitializeComponent();
        }

        public CategoriesListUC(CategorieParameterDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.Header = $"Catégories";
            ViewModelPage.ParentLibrary = parameters?.ParentLibrary;
            ViewModelPage.ParentLibrary.Categories.CollectionChanged += Categories_CollectionChanged;
            InitializeActionInfos();
        }

        private void Categories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in ViewModelPage.ParentLibrary.Categories)
            {
                item.PropertyChanged += Categorie_PropertyChanged;
                if (item.SubCategorieLivres != null && item.SubCategorieLivres.Any())
                {
                    foreach (var subItem in item.SubCategorieLivres)
                    {
                        subItem.PropertyChanged += SubCategorie_PropertyChanged; ;
                    }
                }
            }
        }

        private void SubCategorie_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //GetSelectedNodes();
        }

        private void Categorie_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //GetSelectedNodes();
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

        public CategorieLivreVM GetParentCategorie()
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

        public CategorieLivreVM GetParentCategorie(Microsoft.UI.Xaml.Controls.TreeViewNode treeViewNode)
        {
            try
            {
                if (treeViewNode != null && treeViewNode.Parent.Content is CategorieLivreVM viewModel)
                {
                    return viewModel;
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
                    if (_parameters.BookPage != null)
                    {
                        _parameters.BookPage.AddNewCategory(ViewModelPage.ParentLibrary, ViewModelPage.Guid);
                    }
                    else if (_parameters.LibraryPage != null)
                    {
                        _parameters.LibraryPage.AddNewCategory(ViewModelPage.ParentLibrary, ViewModelPage.Guid);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void AddNewSubCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ParentLibrary != null && TreeCategorie.SelectedItem != null && TreeCategorie.SelectedItem is CategorieLivreVM categorieParent)
                {
                    if (_parameters.BookPage != null)
                    {
                        _parameters.BookPage.AddNewSubCategory(categorieParent, ViewModelPage.Guid);
                    }
                    else if (_parameters.LibraryPage != null)
                    {
                        _parameters.LibraryPage.AddNewSubCategory(categorieParent, ViewModelPage.Guid);
                    }
                }
                else
                {
                    MyTeachingTip.Target = AbbAddItem;
                    MyTeachingTip.Title = "Ajouter une sous-catégorie";
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
                        if (_parameters.BookPage != null)
                        {
                            _parameters.BookPage.EditCategory(ViewModelPage.ParentLibrary, _viewModelCategorie, ViewModelPage.Guid);
                        }
                        else if (_parameters.LibraryPage != null)
                        {
                            _parameters.LibraryPage.EditCategory(ViewModelPage.ParentLibrary, _viewModelCategorie, ViewModelPage.Guid);
                        }
                    }
                    else if (TreeCategorie.SelectedItem is SubCategorieLivreVM _viewModelSubCategorie && _viewModelSubCategorie == ViewModelPage.SelectedCategorie)
                    {
                        CategorieLivreVM viewModelParentCategorie = GetParentCategorie();
                        if (viewModelParentCategorie == null)
                        {
                            return;
                        }

                        if (_parameters.BookPage != null)
                        {
                            _parameters.BookPage.EditSubCategory(viewModelParentCategorie, _viewModelSubCategorie, ViewModelPage.Guid);
                        }
                        else if (_parameters.LibraryPage != null)
                        {
                            _parameters.LibraryPage.EditSubCategory(viewModelParentCategorie, _viewModelSubCategorie, ViewModelPage.Guid);
                        }
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

        private void DeleteItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                GetSelectedNodes();
                //if (ViewModelPage.ParentLibrary != null && TreeCategorie.SelectedItem != null)
                if (ViewModelPage.ParentLibrary != null && ViewModelPage.SelectedItems != null && ViewModelPage.SelectedItems.Any())
                {
                    if (ViewModelPage.SelectedItems.Count == 1)
                    {
                        DependencyObject treeItem = TreeCategorie.ContainerFromNode(ViewModelPage.SelectedItems[0]);
                        if (treeItem is Microsoft.UI.Xaml.Controls.TreeViewItem treeViewItem)
                        {
                            var content = ViewModelPage.SelectedItems[0].Content;
                            var textblock = new TextBlock()
                            {
                                TextWrapping = TextWrapping.Wrap,
                            };

                            Run run1 = new Run()
                            {
                                Text = $"Êtes-vous sûr de vouloir supprimer « ",
                                //FontWeight = FontWeights.Medium,
                            };

                            textblock.Inlines.Add(run1);

                            TtipDeleteSCategorie.Target = treeViewItem;
                            TtipDeleteSCategorie.Content = textblock;

                            if (ViewModelPage.SelectedItems[0].Content is CategorieLivreVM _viewModelCategorie)
                            {
                                Run runType = new Run()
                                {
                                    Text = $"la catégorie « ",
                                    //FontWeight = FontWeights.Medium,
                                };

                                Run runName = new Run()
                                {
                                    Text = _viewModelCategorie.Name,
                                    Foreground = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush,
                                    FontWeight = FontWeights.Medium,
                                };

                                textblock.Inlines.Add(runType);
                                textblock.Inlines.Add(runName);

                                TtipDeleteSCategorie.Title = "Supprimer une catégorie";
                            }
                            else if (ViewModelPage.SelectedItems[0].Content is SubCategorieLivreVM _viewModelSubCategorie)
                            {
                                Run runType = new Run()
                                {
                                    Text = $"la sous-catégorie « ",
                                    //FontWeight = FontWeights.Medium,
                                };

                                Run runName = new Run()
                                {
                                    Text = _viewModelSubCategorie.Name,
                                    Foreground = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush,
                                    FontWeight = FontWeights.Medium,
                                };
                                textblock.Inlines.Add(runType);
                                textblock.Inlines.Add(runName);

                                TtipDeleteSCategorie.Title = "Supprimer une sous-catégorie";
                            }
                            textblock.Inlines.Add(new LineBreak());
                            textblock.Inlines.Add(new LineBreak());

                            Run run2 = new Run()
                            {
                                Text = $" » ?",
                                //FontWeight = FontWeights.Medium,
                            };

                            Run run3 = new Run()
                            {
                                Text = $"Veuillez noter que cette action entraînera la suppression de cette collection dans les livres concernés.",
                                Foreground = new SolidColorBrush(Colors.OrangeRed),
                            };
                            textblock.Inlines.Add(run2);
                            textblock.Inlines.Add(run3);

                        }
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
                foreach (var item in ViewModelPage.ParentLibrary.Categories)
                {
                    item.PropertyChanged -= Categorie_PropertyChanged;
                    if (item.SubCategorieLivres != null && item.SubCategorieLivres.Any())
                    {
                        foreach (var subItem in item.SubCategorieLivres)
                        {
                            subItem.PropertyChanged -= SubCategorie_PropertyChanged; ;

                        }
                    }
                }

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

        
        private void ABTBtnDisplayCheckMark_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is AppBarToggleButton toggleButton)
                {
                    if (toggleButton.IsChecked == true)
                    {
                        if (TreeCategorie.SelectionMode != Microsoft.UI.Xaml.Controls.TreeViewSelectionMode.Multiple)
                        {
                            TreeCategorie.SelectionMode = Microsoft.UI.Xaml.Controls.TreeViewSelectionMode.Multiple;
                        }
                    }
                    else
                    {
                        if (TreeCategorie.SelectionMode != Microsoft.UI.Xaml.Controls.TreeViewSelectionMode.Single)
                        {
                            TreeCategorie.SelectionMode = Microsoft.UI.Xaml.Controls.TreeViewSelectionMode.Single;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GetSelectedNodes()
        {
            try
            {
                List<Microsoft.UI.Xaml.Controls.TreeViewNode> itemsList = new List<Microsoft.UI.Xaml.Controls.TreeViewNode>(TreeCategorie.SelectedNodes);
                ViewModelPage.SelectedItems = itemsList?.ToList();

                if (itemsList != null && itemsList.Count > 0)
                {
                    List<CategorieLivreVM> categorieViewModelList = new List<CategorieLivreVM>();
                    List<Microsoft.UI.Xaml.Controls.TreeViewNode> categories = itemsList.Where(w => w.Content is CategorieLivreVM).ToList();//.Select(s => s.Content as CategorieLivreVM).Where(q => q != null).ToList();
                    List<Microsoft.UI.Xaml.Controls.TreeViewNode> subcategories = itemsList.Where(w => w.Content is SubCategorieLivreVM).ToList();//.Select(s => s.Content as SubCategorieLivreVM).Where(q => q != null).ToList();
                    if (categories != null && categories.Count > 0)
                    {
                        foreach (var categorie in categories.Select(s => s.Content as CategorieLivreVM).Where(q => q != null))
                        {
                            if (subcategories.Any())
                            {
                                categorie.SubCategorieLivres = new ObservableCollection<SubCategorieLivreVM>(subcategories.Select(s => s.Content as SubCategorieLivreVM).Where(q => q != null && q.IdCategorie == categorie.Id));
                            }
                            categorieViewModelList.Add(categorie);
                        }
                    }
                    else if (subcategories != null && subcategories.Count > 0)
                    {
                        foreach (var treeViewNode in subcategories)
                        {
                            if (!(treeViewNode.Content is SubCategorieLivreVM subViewModel))
                            {
                                continue;
                            }

                            var categorie = this.GetParentCategorie(treeViewNode);
                            if (categorie != null)
                            {
                                var viewModel = categorieViewModelList.SingleOrDefault(a => a.Id == categorie.Id);
                                if (viewModel == null)
                                {
                                    if (!categorie.SubCategorieLivres.Any(a => a.Id == subViewModel.Id))
                                    {
                                        categorie.SubCategorieLivres.Add(subViewModel);
                                    }
                                    categorieViewModelList.Add(categorie);
                                }
                                else
                                {
                                    if (!viewModel.SubCategorieLivres.Any(a => a.Id == subViewModel.Id))
                                    {
                                        viewModel.SubCategorieLivres.Add(subViewModel);
                                    }
                                }
                            }
                        }
                    }
                    ViewModelPage.SelectedCategories = categorieViewModelList;
                }
                else
                {
                    ViewModelPage.SelectedCategories = null;
                }

            }
            catch (Exception)
            {

                throw;
            }
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

        private List<Microsoft.UI.Xaml.Controls.TreeViewNode> _SelectedItems;
        public List<Microsoft.UI.Xaml.Controls.TreeViewNode> SelectedItems
        {
            get => _SelectedItems;
            set
            {
                if (_SelectedItems != value)
                {
                    _SelectedItems = value;
                    OnPropertyChanged();

                }
            }
        }

        private List<CategorieLivreVM> _SelectedCategories;
        public List<CategorieLivreVM> SelectedCategories
        {
            get => _SelectedCategories;
            set
            {
                if (_SelectedCategories != value)
                {
                    _SelectedCategories = value;
                    OnPropertyChanged();
                }
            }
        }

        private CategorieLivreVM _SelectedSCategorie;
        public CategorieLivreVM SelectedSCategorie
        {
            get => _SelectedSCategorie;
            set
            {
                if (_SelectedSCategorie != value)
                {
                    _SelectedSCategorie = value;
                    OnPropertyChanged();
                }
            }
        }

        private SubCategorieLivreVM _SelectedSSubCategorie;
        public SubCategorieLivreVM SelectedSSubCategorie
        {
            get => _SelectedSSubCategorie;
            set
            {
                if (_SelectedSSubCategorie != value)
                {
                    _SelectedSSubCategorie = value;
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
                            SelectedSCategorie = categorie;
                            SelectedSSubCategorie = null;
                            SelectedSCategorieName = categorie.Name;
                            SelectedCategorieName = categorie.Name;
                            SelectedSubCategorieName = String.Empty;
                        }
                        else if (value is SubCategorieLivreVM subCategorie)
                        {
                            SelectedSSubCategorie = subCategorie;
                            SelectedSCategorie = null;
                            SelectedSCategorieName = subCategorie.Name;
                            SelectedCategorieName = String.Empty;
                            SelectedSubCategorieName = subCategorie.Name;
                        }
                    }
                    else
                    {
                        SelectedSCategorie = null;
                        SelectedSSubCategorie = null;
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
