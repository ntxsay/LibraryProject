using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Categorie;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
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
using System.Threading;
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

        public CategoriesListUCVM ViewModelPage { get; set; } = new CategoriesListUCVM();


        public delegate void CancelModificationEventHandler(CategoriesListUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

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
                        _parameters.BookPage.AddNewCategory(ViewModelPage.ParentLibrary, ViewModelPage.ItemGuid);
                    }
                    else if (_parameters.LibraryPage != null)
                    {
                        _parameters.LibraryPage.AddNewCategory(ViewModelPage.ParentLibrary, ViewModelPage.ItemGuid);
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
                CategorieLivreVM _categorie = null;

                if (ViewModelPage.ParentLibrary != null)
                {
                    GetSelectedNodes();

                    if (args.Parameter is CategorieLivreVM categorie)
                    {
                        _categorie = categorie;
                    }
                    else if (ViewModelPage.SelectedItems != null && ViewModelPage.SelectedItems.Count == 1)
                    {
                        if (ViewModelPage.SelectedItems[0].Content is CategorieLivreVM _viewModelCategorie)
                        {
                            _categorie = _viewModelCategorie;
                        }
                    }
                }

                if (_categorie == null)
                {
                    ForAddSubCategory(AbbAddItem);
                }
                else
                {
                    if (_parameters.BookPage != null)
                    {
                        _parameters.BookPage.AddNewSubCategory(_categorie, ViewModelPage.ItemGuid);
                    }
                    else if (_parameters.LibraryPage != null)
                    {
                        _parameters.LibraryPage.AddNewSubCategory(_categorie, ViewModelPage.ItemGuid);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void RenameSCategorieXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                CategorieLivreVM _categorie = null;
                SubCategorieLivreVM _subCategorie = null;
                if (ViewModelPage.ParentLibrary != null)
                {
                    GetSelectedNodes();

                    if (args.Parameter is CategorieLivreVM categorie)
                    {
                        _categorie = categorie;
                    }
                    else if (args.Parameter is SubCategorieLivreVM subCategorie)
                    {
                        _subCategorie = subCategorie;
                    }
                    else if (ViewModelPage.SelectedItems != null && ViewModelPage.SelectedItems.Any())
                    {
                        if (ViewModelPage.SelectedItems.Count == 1)
                        {
                            if (ViewModelPage.SelectedItems[0].Content is CategorieLivreVM _viewModelCategorie)
                            {
                                _categorie = _viewModelCategorie;
                            }
                            else if (ViewModelPage.SelectedItems[0].Content is SubCategorieLivreVM _viewModelSubCategorie)
                            {
                                _subCategorie = _viewModelSubCategorie;
                            }
                        }
                        else
                        {
                            MyTeachingTip.Target = ABBRenameCategorie;
                            MyTeachingTip.Title = "Editer";
                            MyTeachingTip.Subtitle = "Vous ne pouvez éditer qu'une seule catégorie/sous-catégorie à la fois.";
                            MyTeachingTip.IsOpen = true;
                        }
                    }
                    else
                    {
                        MyTeachingTip.Target = ABBRenameCategorie;
                        MyTeachingTip.Title = "Editer";
                        MyTeachingTip.Subtitle = "Pour éditer une catégorie ou une sous-catégorie, cliquez d'abord sur la catégorie ou la sous-catégorie que vous souhaitez renommer dans l'arborescence à ci-dessous puis cliquez de nouveau sur ce bouton.";
                        MyTeachingTip.IsOpen = true;
                    }
                }
                else
                {
                    MyTeachingTip.Target = ABBRenameCategorie;
                    MyTeachingTip.Title = "Editer";
                    MyTeachingTip.Subtitle = "Pour éditer une catégorie ou une sous-catégorie, cliquez d'abord sur la catégorie ou la sous-catégorie que vous souhaitez renommer dans l'arborescence à ci-dessous puis cliquez de nouveau sur ce bouton.";
                    MyTeachingTip.IsOpen = true;
                }


                if (_categorie != null)
                {
                    if (_parameters.BookPage != null)
                    {
                        _parameters.BookPage.EditCategory(ViewModelPage.ParentLibrary, _categorie, ViewModelPage.ItemGuid);
                    }
                    else if (_parameters.LibraryPage != null)
                    {
                        _parameters.LibraryPage.EditCategory(ViewModelPage.ParentLibrary, _categorie, ViewModelPage.ItemGuid);
                    }
                }
                else if (_subCategorie != null)
                {
                    CategorieLivreVM viewModelParentCategorie = await DbServices.Categorie.SingleVMAsync(_subCategorie.IdCategorie);
                    if (viewModelParentCategorie == null)
                    {
                        return;
                    }

                    if (_parameters.BookPage != null)
                    {
                        _parameters.BookPage.EditSubCategory(viewModelParentCategorie, _subCategorie, ViewModelPage.ItemGuid);
                    }
                    else if (_parameters.LibraryPage != null)
                    {
                        _parameters.LibraryPage.EditSubCategory(viewModelParentCategorie, _subCategorie, ViewModelPage.ItemGuid);
                    }
                    else
                    {
                        MyTeachingTip.Target = ABBRenameCategorie;
                        MyTeachingTip.Title = "Editer";
                        MyTeachingTip.Subtitle = "Pour éditer une catégorie ou une sous-catégorie, cliquez d'abord sur la catégorie ou la sous-catégorie que vous souhaitez renommer dans l'arborescence à ci-dessous puis cliquez de nouveau sur ce bouton.";
                        MyTeachingTip.IsOpen = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DeleteContextItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ParentLibrary != null)
                {
                    DependencyObject treeItem = TreeCategorie.ContainerFromNode(ViewModelPage.SelectedItems[0]);
                    if (treeItem is Microsoft.UI.Xaml.Controls.TreeViewItem treeViewItem)
                    {
                        var textblock = new TextBlock()
                        {
                            TextWrapping = TextWrapping.Wrap,
                        };

                        Run run1 = new Run()
                        {
                            Text = $"Êtes-vous sûr de vouloir supprimer ",
                            //FontWeight = FontWeights.Medium,
                        };
                        textblock.Inlines.Add(run1);

                        TtipDeleteSCategorie.Target = treeViewItem;
                        TtipDeleteSCategorie.Content = textblock;

                        if (args.Parameter is CategorieLivreVM categorie)
                        {
                            Run runType = new Run()
                            {
                                Text = $"la catégorie « ",
                                //FontWeight = FontWeights.Medium,
                            };

                            Run runName = new Run()
                            {
                                Text = categorie.Name,
                                Foreground = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush,
                                FontWeight = FontWeights.Medium,
                            };

                            textblock.Inlines.Add(runType);
                            textblock.Inlines.Add(runName);

                            Run run2 = new Run()
                            {
                                Text = $" » ?",
                                //FontWeight = FontWeights.Medium,
                            };
                            textblock.Inlines.Add(run2);

                            TtipDeleteSCategorie.Title = "Supprimer une catégorie";

                            textblock.Inlines.Add(new LineBreak());
                            textblock.Inlines.Add(new LineBreak());
                            
                            Run run5 = new Run()
                            {
                                Text = $"Veuillez noter que cette action entraînera la suppression de cette catégorie, ses sous-catégorie ainsi que la décatégorisations des livres concernés.",
                                Foreground = new SolidColorBrush(Colors.OrangeRed),
                            };
                            textblock.Inlines.Add(run5);
                        }
                        else if (args.Parameter is SubCategorieLivreVM subCategorie)
                        {
                            Run runType = new Run()
                            {
                                Text = $"la sous-catégorie « ",
                                //FontWeight = FontWeights.Medium,
                            };

                            Run runName = new Run()
                            {
                                Text = subCategorie.Name,
                                Foreground = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush,
                                FontWeight = FontWeights.Medium,
                            };
                            textblock.Inlines.Add(runType);
                            textblock.Inlines.Add(runName);

                            Run run2 = new Run()
                            {
                                Text = $" » ?",
                                //FontWeight = FontWeights.Medium,
                            };
                            textblock.Inlines.Add(run2);

                            TtipDeleteSCategorie.Title = "Supprimer une sous-catégorie";

                            textblock.Inlines.Add(new LineBreak());
                            textblock.Inlines.Add(new LineBreak());
                            
                            Run run5 = new Run()
                            {
                                Text = $"Veuillez noter que cette action entraînera la suppression de cette sous-catégorie ainsi que la décatégorisations des livres concernés.",
                                Foreground = new SolidColorBrush(Colors.OrangeRed),
                            };
                            textblock.Inlines.Add(run5);
                        }

                        TtipDeleteSCategorie.IsOpen = true;

                    }

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
                    var textblock = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                    };

                    if (ViewModelPage.SelectedItems.Count == 1)
                    {
                        DependencyObject treeItem = TreeCategorie.ContainerFromNode(ViewModelPage.SelectedItems[0]);
                        if (treeItem is Microsoft.UI.Xaml.Controls.TreeViewItem treeViewItem)
                        {
                            //var content = ViewModelPage.SelectedItems[0].Content;

                            Run run1 = new Run()
                            {
                                Text = $"Êtes-vous sûr de vouloir supprimer ",
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
                            textblock.Inlines.Add(new LineBreak());
                            textblock.Inlines.Add(new LineBreak());
                            textblock.Inlines.Add(run3);
                            TtipDeleteSCategorie.IsOpen = true;


                        }
                    }
                    else if (ViewModelPage.SelectedItems.Count > 1)
                    {
                        TtipDeleteSCategorie.Target = ABBDelete;

                        Run run1 = new Run()
                        {
                            Text = $"Êtes-vous sûr de vouloir supprimer ces « {ViewModelPage.SelectedItems.Count} élément(s) » ?",
                            //FontWeight = FontWeights.Medium,
                        };

                        Run run2 = new Run()
                        {
                            Text = $"Veuillez noter que cette action entraînera la suppression de cette collection dans les livres concernés.",
                            Foreground = new SolidColorBrush(Colors.OrangeRed),
                        };

                        textblock.Inlines.Add(run1);
                        textblock.Inlines.Add(new LineBreak());
                        textblock.Inlines.Add(new LineBreak());
                        textblock.Inlines.Add(run2);
                        TtipDeleteSCategorie.Content = textblock;
                        TtipDeleteSCategorie.IsOpen = true;
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


        private void BtnDeleteCancel_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                TtipDeleteSCategorie.IsOpen = false;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void BtnDeleteConfirm_Click(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.SelectedItems != null && ViewModelPage.SelectedItems.Any())
                {
                    foreach (var item in ViewModelPage.SelectedItems)
                    {
                        OperationStateVM result = null;
                        if (item.Content is CategorieLivreVM categorie)
                        {
                            result = await DbServices.Categorie.DeleteAsync<CategorieLivreVM>(categorie.Id);
                            if (result.IsSuccess)
                            {
                                ViewModelPage.ResultMessageTitle = "Succès";
                                ViewModelPage.ResultMessage = result.Message;
                                ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                                ViewModelPage.IsResultMessageOpen = true;
                                var itemVm = ViewModelPage.ParentLibrary.Categories.SingleOrDefault(s => s.Id == categorie.Id);
                                if (itemVm != null)
                                {
                                    ViewModelPage.ParentLibrary.Categories.Remove(itemVm);
                                }
                                Thread.Sleep(500);
                            }
                            else
                            {
                                //Erreur
                                ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                                ViewModelPage.ResultMessage += "\n" + result.Message;
                                ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                                ViewModelPage.IsResultMessageOpen = true;
                                continue;
                            }
                        }
                        else if (item.Content is SubCategorieLivreVM subCategorie)
                        {
                            result = await DbServices.Categorie.DeleteAsync<SubCategorieLivreVM>(subCategorie.Id);
                            if (result.IsSuccess)
                            {
                                ViewModelPage.ResultMessageTitle = "Succès";
                                ViewModelPage.ResultMessage = result.Message;
                                ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                                ViewModelPage.IsResultMessageOpen = true; 
                                foreach (var cat in ViewModelPage.ParentLibrary.Categories)
                                {
                                    var subItemVm = cat.SubCategorieLivres.SingleOrDefault(s => s.Id == subCategorie.Id);
                                    if (subItemVm != null)
                                    {
                                        cat.SubCategorieLivres.Remove(subItemVm);
                                    }
                                }
                                
                                Thread.Sleep(500);
                            }
                            else
                            {
                                //Erreur
                                ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                                ViewModelPage.ResultMessage += "\n" + result.Message;
                                ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                                ViewModelPage.IsResultMessageOpen = true;
                                continue;
                            }
                        }

                    }

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

        private void MenuFlyout_Opened(object sender, object e)
        {
            try
            {
                if (sender is MenuFlyout menuFlyout)
                {
                    if (_parameters.BookPage.ViewModelPage.SelectedItems != null && _parameters.BookPage.ViewModelPage.SelectedItems.Any())
                    {
                        if (menuFlyout.Items[0] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Ajouter {_parameters.BookPage.ViewModelPage.SelectedItems.Count} livre(s) à « {flyoutItem.Tag} »";
                            flyoutItem.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (menuFlyout.Items[0] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Aucun livre à ajouter à « {flyoutItem.Tag} »";
                            flyoutItem.IsEnabled = false;
                        }
                    }

                    if (menuFlyout.Items[1] is MenuFlyoutItem flyoutItemDecategorize)
                    {
                        if (flyoutItemDecategorize.Tag is CategorieLivreVM categorie)
                        {
                            if (categorie.BooksId != null && categorie.BooksId.Any())
                            {
                                flyoutItemDecategorize.Text = $"Décatégoriser {categorie.BooksId.Count} livre(s) dans « {categorie.Name} »";
                                flyoutItemDecategorize.IsEnabled = true;
                            }
                            else
                            {
                                flyoutItemDecategorize.Text = $"Aucun livre à décatégoriser dans « {categorie.Name} »";
                                flyoutItemDecategorize.IsEnabled = false;
                            }
                        }
                        else if (flyoutItemDecategorize.Tag is SubCategorieLivreVM subCategorie)
                        {
                            if (subCategorie.BooksId != null && subCategorie.BooksId.Any())
                            {
                                flyoutItemDecategorize.Text = $"Décatégoriser {subCategorie.BooksId.Count} livre(s) dans « {subCategorie.Name} »";
                                flyoutItemDecategorize.IsEnabled = true;
                            }
                            else
                            {
                                flyoutItemDecategorize.Text = $"Aucun livre à décatégoriser dans « {subCategorie.Name} »";
                                flyoutItemDecategorize.IsEnabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void DecategorizeBooksFromSCategorieXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                OperationStateVM result = null;
                if (args.Parameter is CategorieLivreVM categorieLivreVM)
                {
                    result = await DbServices.Categorie.DecategorizeBooksAsync(categorieLivreVM.BooksId);
                }
                else if (args.Parameter is SubCategorieLivreVM subCategorieLivreVM)
                {
                    result = await DbServices.Categorie.DecategorizeBooksAsync(subCategorieLivreVM.BooksId);
                }

                if (result != null)
                {
                    if (result.IsSuccess)
                    {
                        ViewModelPage.ResultMessageTitle = "Succès";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                        ViewModelPage.IsResultMessageOpen = true;

                        await _parameters.BookPage.UpdateLibraryCategoriesAsync();
                    }
                    else
                    {
                        //Erreur
                        ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                        ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void AddBooksToSCategorieXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (_parameters.BookPage.ViewModelPage.SelectedItems != null && _parameters.BookPage.ViewModelPage.SelectedItems.Any() && _parameters.BookPage.ViewModelPage.SelectedItems is ICollection<LivreVM> collection)
                {
                    if (args.Parameter is CategorieLivreVM categorieLivreVM)
                    {
                        var creationResult = await DbServices.Categorie.CreateCategorieConnectorAsync(collection.Select(s => s.Id), categorieLivreVM, null);
                        if (creationResult.IsSuccess)
                        {
                            ViewModelPage.ResultMessageTitle = "Succès";
                            ViewModelPage.ResultMessage = creationResult.Message;
                            ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                            ViewModelPage.IsResultMessageOpen = true;

                            await _parameters.BookPage.UpdateLibraryCollectionAsync();
                        }
                        else
                        {
                            //Erreur
                            ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                            ViewModelPage.ResultMessage = creationResult.Message;
                            ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                            ViewModelPage.IsResultMessageOpen = true;
                            return;
                        }
                    }
                    else if (args.Parameter is SubCategorieLivreVM subCategorieLivreVM)
                    {
                        var parentCategorie = await DbServices.Categorie.GetParentCategorieVMAsync(subCategorieLivreVM.Id);
                        if (parentCategorie != null)
                        {
                            var creationResult = await DbServices.Categorie.CreateCategorieConnectorAsync(collection.Select(s => s.Id), parentCategorie, subCategorieLivreVM);
                            if (creationResult.IsSuccess)
                            {
                                ViewModelPage.ResultMessageTitle = "Succès";
                                ViewModelPage.ResultMessage = creationResult.Message;
                                ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                                ViewModelPage.IsResultMessageOpen = true;

                                await _parameters.BookPage.UpdateLibraryCategoriesAsync();
                            }
                            else
                            {
                                //Erreur
                                ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                                ViewModelPage.ResultMessage = creationResult.Message;
                                ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                                ViewModelPage.IsResultMessageOpen = true;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        #region Navigation
        private void MenuFlyout_UnCategorized_Opened(object sender, object e)
        {
            try
            {
                if (sender is MenuFlyout menuFlyout)
                {
                    if (_parameters.BookPage.ViewModelPage.SelectedItems != null && _parameters.BookPage.ViewModelPage.SelectedItems.Any() && _parameters.BookPage.ViewModelPage.SelectedItems is ICollection<LivreVM> collection)
                    {
                        if (menuFlyout.Items[0] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Décatégoriser {_parameters.BookPage.ViewModelPage.SelectedItems.Count} livre(s)";
                            flyoutItem.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (menuFlyout.Items[0] is MenuFlyoutItem flyoutItem)
                        {
                            flyoutItem.Text = $"Aucun livre à décatégoriser";
                            flyoutItem.IsEnabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void MenuFlyoutItem_UnCategorizeItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_parameters.BookPage.ViewModelPage.SelectedItems != null && _parameters.BookPage.ViewModelPage.SelectedItems.Any() && _parameters.BookPage.ViewModelPage.SelectedItems is ICollection<LivreVM> collection)
                {
                    var result = await DbServices.Categorie.DecategorizeBooksAsync(collection.Select(s => s.Id));
                    if (result.IsSuccess)
                    {
                        ViewModelPage.ResultMessageTitle = "Succès";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Success;
                        ViewModelPage.IsResultMessageOpen = true;

                        await _parameters.BookPage.UpdateLibraryCategoriesAsync();
                    }
                    else
                    {
                        //Erreur
                        ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        ViewModelPage.ResultMessage = result.Message;
                        ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Error;
                        ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NavigateInUncategorizedItemXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _parameters.BookPage.ViewModelPage.DisplayUnCategorizedBooks = true;
                await _parameters.BookPage.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void MenuFlyout_Navigate_Opened(object sender, object e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is MenuFlyout menuFlyout)
                {
                    this.GetSelectedNodes();
                    if (ViewModelPage.ParentLibrary != null && ViewModelPage.SelectedItems != null && ViewModelPage.SelectedItems.Any())
                    {
                        if (ViewModelPage.SelectedItems.Count > 1)
                        {
                            ViewModelPage.SelectedViewModelMessage = $"Afficher « {ViewModelPage.SelectedItems.Count} catégories/sous-catégories »";
                            if (TtipDeleteSCategorie.IsOpen)
                            {
                                TtipDeleteSCategorie.IsOpen = false;
                            }
                        }
                        else if (ViewModelPage.SelectedItems.Count == 1)
                        {
                            var content = ViewModelPage.SelectedItems[0].Content;
                            if (content is CategorieLivreVM categorieLivreVM)
                            {
                                ViewModelPage.SelectedViewModelMessage = $"Afficher « {categorieLivreVM.Name} »";
                            }
                            else if (content is SubCategorieLivreVM subCategorieLivreVM)
                            {
                                ViewModelPage.SelectedViewModelMessage = $"Afficher « {subCategorieLivreVM.Name} »";
                            }
                        }
                        else
                        {
                            ViewModelPage.SelectedViewModelMessage = $"Aucune catégorie n'est à afficher";
                        }
                    }
                    else
                    {
                        ViewModelPage.SelectedViewModelMessage = $"Aucune catégorie n'est à afficher";
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NavigateInAllItemXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _parameters.BookPage.ViewModelPage.DisplayUnCategorizedBooks = false;
                _parameters.BookPage.ViewModelPage.SelectedSCategories = null;
                await _parameters.BookPage.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NavigateInThisItemXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.SelectedItems != null && ViewModelPage.SelectedItems.Any())
                {
                    _parameters.BookPage.ViewModelPage.SelectedSCategories = ViewModelPage.SelectedItems.Select(s => s.Content).ToList();
                    await _parameters.BookPage.RefreshItemsGrouping();
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }


        #endregion

        private void ForAddSubCategory(FrameworkElement element)
        {
            try
            {
                MyTeachingTip.Target = element;
                MyTeachingTip.Title = "Ajouter une sous-catégorie";
                MyTeachingTip.Subtitle = "Pour ajouter une sous-catégorie, ajoutez ou cliquez d'abord sur une catégorie dans l'arborescence à ci-dessous puis cliquez de nouveau sur ce bouton.\n\nAttention : il n'est pas possible d'ajouter une sous-catégorie à une autre sous-catégorie.";
                MyTeachingTip.IsOpen = true;
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
        public Guid ItemGuid { get; private set; } = Guid.NewGuid();
        //public Guid? ParentGuid { get; set; }

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

        private string _SelectedViewModelMessage;
        public string SelectedViewModelMessage
        {
            get => this._SelectedViewModelMessage;
            set
            {
                if (this._SelectedViewModelMessage != value)
                {
                    this._SelectedViewModelMessage = value;
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
