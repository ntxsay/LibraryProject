using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.Code.Services.Db;
using RostalProjectUWP.Code.Services.ES;
using RostalProjectUWP.Code.Services.Logging;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using RostalProjectUWP.ViewModels.Library;
using RostalProjectUWP.Views.Book.Manage;
using RostalProjectUWP.Views.Library.Manage;
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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RostalProjectUWP.Views.Library.Collection
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class LibraryCollectionGridViewPage : Page
    {
        public LibraryCollectionGridViewPageVM ViewModelPage { get; set; } = new LibraryCollectionGridViewPageVM();
        private LibraryCollectionParentChildParamsVM _libraryParameters;
        public LibraryCollectionGridViewPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is LibraryCollectionParentChildParamsVM libraryParameters)
            {
                _libraryParameters = libraryParameters;
                ViewModelPage.ViewModelList = _libraryParameters.ViewModelList?.ToList();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeModelList();
        }

        private void InitializeModelList()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _libraryParameters.ParentPage.RefreshItemsGrouping();
                this.PivotItems.SelectedIndex = _libraryParameters.ParentPage.ViewModelPage.SelectedPivotIndex;
                this.PivotItems.SelectionChanged += PivotItems_SelectionChanged;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void PivotItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is Pivot pivot)
                {
                    _libraryParameters.ParentPage.ViewModelPage.SelectedItems = new List<BibliothequeVM>();
                    _libraryParameters.ParentPage.ViewModelPage.SelectedPivotIndex = pivot.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is GridView gridView)
                {
                    _libraryParameters.ParentPage.ViewModelPage.SelectedItems = gridView.SelectedItems.Cast<BibliothequeVM>().ToList();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Image imageCtrl && imageCtrl.Tag is string path)
                {
                    var bitmapImage = await Files.BitmapImageFromFileAsync(path);
                    imageCtrl.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        #region Groups
        public void GroupItemsByNone()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, _libraryParameters.ParentPage.ViewModelPage.OrderedBy, _libraryParameters.ParentPage.ViewModelPage.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos bibliothèques").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    _libraryParameters.ParentPage.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.None;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        public void GroupItemsByAlphabetic()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, _libraryParameters.ParentPage.ViewModelPage.OrderedBy, _libraryParameters.ParentPage.ViewModelPage.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Name.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    _libraryParameters.ParentPage.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.Letter;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupByCreationYear()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, _libraryParameters.ParentPage.ViewModelPage.OrderedBy, _libraryParameters.ParentPage.ViewModelPage.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    _libraryParameters.ParentPage.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.CreationYear;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Group-Orders
        private IEnumerable<BibliothequeVM> OrderItems(IEnumerable<BibliothequeVM> Collection, LibraryGroupVM.OrderBy OrderBy = LibraryGroupVM.OrderBy.Croissant, LibraryGroupVM.SortBy SortBy = LibraryGroupVM.SortBy.Name)
        {
            try
            {
                if (Collection == null || Collection.Count() == 0)
                {
                    return null;
                }

                if (SortBy == LibraryGroupVM.SortBy.Name)
                {
                    if (OrderBy == LibraryGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.Name);
                    }
                    else if (OrderBy == LibraryGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.Name);
                    }
                }
                else if (SortBy == LibraryGroupVM.SortBy.DateCreation)
                {
                    if (OrderBy == LibraryGroupVM.OrderBy.Croissant)
                    {
                        return Collection.OrderBy(o => o.DateAjout);
                    }
                    else if (OrderBy == LibraryGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.OrderByDescending(o => o.DateAjout);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return Enumerable.Empty<BibliothequeVM>();
            }
        }

        #endregion

        private async void ChangeJaquetteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    EsLibrary esLibrary = new EsLibrary();
                    var result = await esLibrary.ChangeLibraryItemJaquetteAsync(viewModel);
                    if (!result.IsSuccess)
                    {
                        return;
                    }

                    viewModel.JaquettePath = result.Result?.ToString() ?? "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
                    var image = GetSelectedThumbnailImage(viewModel);
                    if (image != null)
                    {
                        var bitmapImage = await Files.BitmapImageFromFileAsync(viewModel.JaquettePath);
                        image.Source = bitmapImage;
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

        private void DeleteLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            DeleteLibraryUC userControl = null;
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    userControl = new DeleteLibraryUC(viewModel);

                    userControl.CancelModificationRequested += DeleteLibraryUC_CancelModificationRequested; ;
                    userControl.DeleteItemRequested += DeleteLibraryUC_DeleteItemRequested;

                    ViewModelPage.SplitViewContent = userControl;
                    ViewModelPage.IsSplitViewOpen = true;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DeleteLibraryUC_DeleteItemRequested(DeleteLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteLibraryUC_CancelModificationRequested(DeleteLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EditLibraryInfosXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            NewEditLibraryUC userControl = null;
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    userControl = new NewEditLibraryUC(new ManageLibraryDialogParametersVM()
                    {
                        CurrentLibrary = viewModel,
                        EditMode = Code.EditMode.Edit,
                        ViewModelList = _libraryParameters.ParentPage.ViewModelPage.ViewModelList,
                    });

                    userControl.CancelModificationRequested += NewEditLibraryUC_CancelModificationRequested;
                    userControl.UpdateItemRequested += NewEditLibraryUC_UpdateItemRequested;

                    ViewModelPage.SplitViewContent = userControl;
                    ViewModelPage.IsSplitViewOpen = true;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
            //try
            //{
            //    if (args.Parameter is BibliothequeVM viewModel)
            //    {
            //        var dialog = new NewLibraryCD(new ManageLibraryDialogParametersVM()
            //        {
            //            Value = viewModel.Name,
            //            Description = viewModel.Description,
            //            EditMode = Code.EditMode.Edit,
            //            ViewModelList = _libraryParameters.ParentPage.ViewModelPage.ViewModelList,
            //        });

            //        var result = await dialog.ShowAsync();
            //        if (result == ContentDialogResult.Primary)
            //        {
            //            var newValue = dialog.Value?.Trim();
            //            var newDescription = dialog.Description?.Trim();

            //            var updatedViewModel = new BibliothequeVM()
            //            {
            //                Id = viewModel.Id,
            //                Name = newValue,
            //                Description = newDescription,
            //                DateEdition = DateTime.UtcNow,
            //            };

            //            var updateResult = await DbServices.Library.UpdateAsync(updatedViewModel);
            //            if (updateResult.IsSuccess)
            //            {
            //                viewModel.Name = newValue;
            //                viewModel.Description = newDescription;
            //            }
            //            else
            //            {
            //                //Erreur
            //            }
            //        }
            //        else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
            //        {
            //            return;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MethodBase m = MethodBase.GetCurrentMethod();
            //    Logs.Log(ex, m);
            //    return;
            //}
        }

        private void NewEditLibraryUC_UpdateItemRequested(NewEditLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void NewEditLibraryUC_CancelModificationRequested(NewEditLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void SearchViewModel(BibliothequeVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                foreach (var pivotItem in PivotItems.Items)
                {
                    if (pivotItem is IGrouping<string, BibliothequeVM> group && group.Any(f => f == viewModel))
                    {
                        if (this.PivotItems.SelectedItem != pivotItem)
                        {
                            this.PivotItems.SelectedItem = pivotItem;
                        }

                        var _container = this.PivotItems.ContainerFromItem(pivotItem);
                        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
                        while (gridView != null && gridView.Name != "GridViewItems")
                        {
                            gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
                            if (gridView == null)
                            {
                                return;
                            }
                            else
                            {
                                if (gridView.Name == "GridViewItems")
                                {
                                    break;
                                }
                            }
                        }

                        if (gridView != null)
                        {
                            foreach (var gridViewItem in gridView.Items)
                            {
                                if (gridViewItem is BibliothequeVM _viewModel && _viewModel == viewModel)
                                {
                                    if (gridView.SelectedItem != gridViewItem)
                                    {
                                        gridView.SelectedItem = gridViewItem;
                                    }

                                    var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                                    OpenFlyoutSearchedItem(_gridViewItemContainer);
                                }
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

        private void OpenFlyoutSearchedItem(DependencyObject _gridViewItemContainer)
        {
            try
            {
                if (_gridViewItemContainer == null)
                {
                    return;
                }

                var grid = VisualViewHelpers.FindVisualChild<Grid>(_gridViewItemContainer);
                if (grid != null)
                {
                    Grid gridActions = grid.Children.FirstOrDefault(f => f is Grid _gridActions && _gridActions.Name == "GridActions") as Grid;
                    if (gridActions != null)
                    {
                        Button buttonActions = gridActions.Children.FirstOrDefault(f => f is Button _buttonActions && _buttonActions.Name == "BtnActions") as Button;
                        if (buttonActions != null)
                        {
                            buttonActions.Flyout.ShowAt(buttonActions,new FlyoutShowOptions() 
                            { 
                                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft, 
                                ShowMode = FlyoutShowMode.Auto
                            });
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

        private Image GetSelectedThumbnailImage(BibliothequeVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return null;
                }

                if (this.PivotItems.SelectedItem != null)
                {
                    if (this.PivotItems.SelectedItem is IGrouping<string, BibliothequeVM> group && group.Any(f => f == viewModel))
                    {
                        
                        var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
                        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
                        while (gridView != null && gridView.Name != "GridViewItems")
                        {
                            gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
                            if (gridView == null)
                            {
                                return null;
                            }
                            else
                            {
                                if (gridView.Name == "GridViewItems")
                                {
                                    break;
                                }
                            }
                        }

                        if (gridView != null)
                        {
                            foreach (var gridViewItem in gridView.Items)
                            {
                                if (gridViewItem is BibliothequeVM _viewModel && _viewModel == viewModel)
                                {
                                    if (gridView.SelectedItem != gridViewItem)
                                    {
                                        gridView.SelectedItem = gridViewItem;
                                    }

                                    var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                                    return SelectImageFromContainer(_gridViewItemContainer);
                                }
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        private Image SelectImageFromContainer(DependencyObject _gridViewItemContainer)
        {
            try
            {
                if (_gridViewItemContainer == null)
                {
                    return null;
                }

                var grid = VisualViewHelpers.FindVisualChild<Grid>(_gridViewItemContainer);
                if (grid != null)
                {
                    Viewbox viewboxThumbnailContainer = grid.Children.FirstOrDefault(f => f is Viewbox _viewboxThumbnailContainer && _viewboxThumbnailContainer.Name == "ViewboxSimpleThumnailDatatemplate") as Viewbox;
                    if (viewboxThumbnailContainer != null)
                    {
                        Border border = viewboxThumbnailContainer.Child as Border;
                        if (border != null)
                        {
                            Grid gridImageContainer = border.Child as Grid;
                            if (gridImageContainer != null)
                            {
                                Image image = gridImageContainer.Children.FirstOrDefault(f => f is Image _image) as Image;
                                return image;
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void MenuFlyoutSubItem_Categorie_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is MenuFlyoutSubItem subItem && subItem.Tag is BibliothequeVM viewModel)
                {
                    PopulateCategoriesMenuItems(subItem, viewModel);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void PopulateCategoriesMenuItems(MenuFlyoutSubItem subItem, BibliothequeVM viewModel)
        {
            try
            {
                subItem.Items.Clear();
                //Add button
                var AddCategoryMenuItem = new MenuFlyoutItem()
                {
                    Text = "Ajouter une catégorie",
                    Icon = new SymbolIcon(Symbol.Add),
                };

                AddCategoryMenuItem.Click +=(BibliothequeVM,e) => AddCategoryMenuItem_Click(viewModel, e);
                subItem.Items.Add(AddCategoryMenuItem);

                if (viewModel.Categories != null && viewModel.Categories.Any())
                {
                    subItem.Items.Add(new MenuFlyoutSeparator());

                    foreach (var category in viewModel.Categories)
                    {
                        //Main Category MenuItem
                        var CategoryMenuItem = new MenuFlyoutSubItem()
                        {
                            Text = category.Name,
                            Icon = new FontIcon() { Glyph = "\uE81E" }
                        };

                        //Add sub categorie button
                        var AddSubCategoryMenuItem = new MenuFlyoutItem()
                        {
                            Text = "Ajouter une sous-catégorie",
                            Icon = new SymbolIcon(Symbol.Add),
                        };
                        AddSubCategoryMenuItem.Click +=(CategorieLivreVM, e) => AddSubCategoryMenuItem_Click(category,e);
                        CategoryMenuItem.Items.Add(AddSubCategoryMenuItem);

                        //Remove categorie button
                        var EditCategoryMenuItem = new MenuFlyoutItem()
                        {
                            Text = $"Editer « {category.Name} »",
                            Icon = new SymbolIcon(Symbol.Edit),
                        };
                        EditCategoryMenuItem.Click += (BibliothequeVM, e) => EditCategoryMenuItem_Click(new Tuple<BibliothequeVM, CategorieLivreVM>(viewModel, category), e);
                        CategoryMenuItem.Items.Add(EditCategoryMenuItem);

                        subItem.Items.Add(CategoryMenuItem);

                        PopulateSubCategoriesMenuItems(CategoryMenuItem, category);
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
        private void PopulateSubCategoriesMenuItems(MenuFlyoutSubItem categorieSubMenuItem, CategorieLivreVM categorieVM)
        {
            try
            {
                if (categorieVM.SubCategorieLivres != null && categorieVM.SubCategorieLivres.Any())
                {
                    categorieSubMenuItem.Items.Add(new MenuFlyoutSeparator());

                    foreach (var subCategory in categorieVM.SubCategorieLivres)
                    {
                        //Main Sub-Category MenuItem
                        var SubCategoryMenuItem = new MenuFlyoutItem()
                        {
                            Text = subCategory.Name,
                            Icon = new FontIcon() { Glyph = "\uE81E" }
                        };

                        SubCategoryMenuItem.Click += (CategorieLivreVM, e) => EditSubCategoryMenuItem_Click(new Tuple<CategorieLivreVM, SubCategorieLivreVM>(categorieVM, subCategory), e);
                        categorieSubMenuItem.Items.Add(SubCategoryMenuItem);
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

        #region Events Categorie
        private void AddCategoryMenuItem_Click(BibliothequeVM sender, RoutedEventArgs e)
        {
            NewEditCategoryUC userControl = null;
            try
            {
                userControl = new NewEditCategoryUC(new ManageCategorieDialogParametersVM()
                {
                    EditMode = Code.EditMode.Create,
                    ParentLibrary = sender,
                });

                userControl.CancelModificationRequested += NewEditCategoryUC_CancelModificationRequested;
                userControl.CreateItemRequested += NewEditCategoryUC_CreateItemRequested;

                ViewModelPage.SplitViewContent = userControl;
                ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void EditCategoryMenuItem_Click(Tuple<BibliothequeVM, CategorieLivreVM> sender, RoutedEventArgs e)
        {
            NewEditCategoryUC userControl = null;
            try
            {
                userControl = new NewEditCategoryUC(new ManageCategorieDialogParametersVM()
                {
                    CurrentCategorie = sender.Item2,
                    EditMode = Code.EditMode.Edit,
                    ParentLibrary = sender.Item1,
                });

                userControl.CancelModificationRequested += NewEditCategoryUC_CancelModificationRequested;
                userControl.UpdateItemRequested += NewEditCategoryUC_UpdateItemRequested;

                ViewModelPage.SplitViewContent = userControl;
                ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }


        private async void NewEditCategoryUC_CreateItemRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                if (sender._categorieParameters != null)
                {
                    var value = sender.ViewModelPage.Value?.Trim();
                    var description = sender.ViewModelPage.Description?.Trim();

                    var newViewModel = new CategorieLivreVM()
                    {
                        IdLibrary = sender._categorieParameters.ParentLibrary.Id,
                        Name = value,
                        Description = description,
                    };

                    var creationResult = await DbServices.Categorie.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender._categorieParameters.ParentLibrary.Categories.Add(newViewModel);
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ErrorMessage = creationResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditCategoryUC_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditCategoryUC_CreateItemRequested;

                ViewModelPage.IsSplitViewOpen = false;
                ViewModelPage.SplitViewContent = null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void NewEditCategoryUC_UpdateItemRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                if (sender._categorieParameters != null)
                {
                    var newValue = sender.ViewModelPage.Value?.Trim();
                    var newDescription = sender.ViewModelPage.Description?.Trim();

                    var updatedViewModel = new CategorieLivreVM()
                    {
                        Id = sender._categorieParameters.CurrentCategorie.Id,
                        IdLibrary = sender._categorieParameters.ParentLibrary.Id,
                        Name = newValue,
                        Description = newDescription,
                    };

                    var updateResult = await DbServices.Categorie.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender._categorieParameters.CurrentCategorie.Name = newValue;
                        sender._categorieParameters.CurrentCategorie.Description = newDescription;
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ErrorMessage = updateResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditCategoryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditCategoryUC_UpdateItemRequested;

                ViewModelPage.IsSplitViewOpen = false;
                ViewModelPage.SplitViewContent = null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void NewEditCategoryUC_CancelModificationRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                sender.CancelModificationRequested -= NewEditCategoryUC_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditCategoryUC_CreateItemRequested;
                sender.UpdateItemRequested -= NewEditCategoryUC_UpdateItemRequested;

                ViewModelPage.IsSplitViewOpen = false;
                ViewModelPage.SplitViewContent = null;
            }
            catch (Exception)
            {

                throw;
            }
        } 
        #endregion

        #region Events Sub Categorie
        private void AddSubCategoryMenuItem_Click(CategorieLivreVM sender, RoutedEventArgs e)
        {
            NewEditCategoryUC userControl = null;
            try
            {
                userControl = new NewEditCategoryUC(new ManageSubCategorieDialogParametersVM()
                {
                    EditMode = Code.EditMode.Create,
                    Categorie = sender,
                });

                userControl.CancelModificationRequested += NewEditSubCategoryUC_CancelModificationRequested;
                userControl.CreateItemRequested += NewEditSubCategoryUC_CreateItemRequested;

                ViewModelPage.SplitViewContent = userControl;
                ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void EditSubCategoryMenuItem_Click(Tuple<CategorieLivreVM, SubCategorieLivreVM> sender, RoutedEventArgs e)
        {
            NewEditCategoryUC userControl = null;
            try
            {
                userControl = new NewEditCategoryUC(new ManageSubCategorieDialogParametersVM()
                {
                    EditMode = Code.EditMode.Edit,
                    Categorie = sender.Item1,
                    CurrentSubCategorie = sender.Item2,
                });

                userControl.CancelModificationRequested += NewEditSubCategoryUC_CancelModificationRequested;
                userControl.UpdateItemRequested += NewEditSubCategoryUC_UpdateItemRequested;

                ViewModelPage.SplitViewContent = userControl;
                ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditSubCategoryUC_CreateItemRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                if (sender._subCategorieParameters != null)
                {
                    var value = sender.ViewModelPage.Value?.Trim();
                    var description = sender.ViewModelPage.Description?.Trim();

                    var newViewModel = new SubCategorieLivreVM()
                    {
                        IdCategorie = sender._subCategorieParameters.Categorie.Id,
                        Name = value,
                        Description = description,
                    };

                    var creationResult = await DbServices.SubCategorie.CreateAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender._subCategorieParameters.Categorie.SubCategorieLivres.Add(newViewModel);
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ErrorMessage = creationResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditSubCategoryUC_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditSubCategoryUC_CreateItemRequested;

                ViewModelPage.IsSplitViewOpen = false;
                ViewModelPage.SplitViewContent = null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void NewEditSubCategoryUC_UpdateItemRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                if (sender._subCategorieParameters != null)
                {
                    var newValue = sender.ViewModelPage.Value?.Trim();
                    var newDescription = sender.ViewModelPage.Description?.Trim();
                    var updatedViewModel = new SubCategorieLivreVM()
                    {
                        Id = sender._subCategorieParameters.CurrentSubCategorie.Id,
                        IdCategorie = sender._subCategorieParameters.Categorie.Id,
                        Name = newValue,
                        Description = newDescription,
                    };

                    var updateResult = await DbServices.SubCategorie.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender._subCategorieParameters.CurrentSubCategorie.Name = newValue;
                        sender._subCategorieParameters.CurrentSubCategorie.Description = newDescription;
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ErrorMessage = updateResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditSubCategoryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditSubCategoryUC_UpdateItemRequested;

                ViewModelPage.IsSplitViewOpen = false;
                ViewModelPage.SplitViewContent = null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void NewEditSubCategoryUC_CancelModificationRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            try
            {
                sender.CancelModificationRequested -= NewEditSubCategoryUC_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditSubCategoryUC_CreateItemRequested;
                sender.UpdateItemRequested -= NewEditSubCategoryUC_UpdateItemRequested;

                ViewModelPage.IsSplitViewOpen = false;
                ViewModelPage.SplitViewContent = null;
            }
            catch (Exception)
            {

                throw;
            }
        } 
        #endregion

    }

    public class LibraryCollectionGridViewPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private LibraryGroupVM _GroupedRelatedViewModel = new LibraryGroupVM();
        public LibraryGroupVM GroupedRelatedViewModel
        {
            get => this._GroupedRelatedViewModel;
            set
            {
                if (this._GroupedRelatedViewModel != value)
                {
                    this._GroupedRelatedViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsSplitViewOpen;
        public bool IsSplitViewOpen
        {
            get => this._IsSplitViewOpen;
            set
            {
                if (_IsSplitViewOpen != value)
                {
                    this._IsSplitViewOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private UserControl _SplitViewContent;
        public UserControl SplitViewContent
        {
            get => this._SplitViewContent;
            set
            {
                if (_SplitViewContent != value)
                {
                    this._SplitViewContent = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private LibraryGroupVM.GroupBy _GroupedBy = LibraryGroupVM.GroupBy.None;
        public LibraryGroupVM.GroupBy GroupedBy
        {
            get => this._GroupedBy;
            set
            {
                if (this._GroupedBy != value)
                {
                    this._GroupedBy = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _SearchingLibraryVisibility = Visibility.Visible;
        public Visibility SearchingLibraryVisibility
        {
            get => this._SearchingLibraryVisibility;
            set
            {
                if (_SearchingLibraryVisibility != value)
                {
                    this._SearchingLibraryVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _countItems;
        public int CountItems
        {
            get => this._countItems;
            set
            {
                if (_countItems != value)
                {
                    this._countItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _countSelectedItems;
        public int CountSelectedItems
        {
            get => this._countSelectedItems;
            set
            {
                if (_countSelectedItems != value)
                {
                    this._countSelectedItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private List<BibliothequeVM> _ViewModelList;
        public List<BibliothequeVM> ViewModelList
        {
            get => this._ViewModelList;
            set
            {
                if (_ViewModelList != value)
                {
                    this._ViewModelList = value;
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
