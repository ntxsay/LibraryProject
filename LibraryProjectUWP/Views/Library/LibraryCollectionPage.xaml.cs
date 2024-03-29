﻿using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Categorie;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Categories;
using LibraryProjectUWP.Views.Library.Manage;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Toolkit.Uwp.UI.Controls;
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
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using LibraryProjectUWP.Code.Extensions;
using Windows.UI.Core;
using LibraryProjectUWP.Code;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Library
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class LibraryCollectionPage : Page
    {
        public LibraryCollectionPageVM ViewModelPage { get; set; } = new LibraryCollectionPageVM();
        EsLibrary esLibrary = new EsLibrary();
        readonly EsAppBaseApi esAppBaseApi = new EsAppBaseApi();
        public MainPage MainPage { get; private set; }

        public LibraryCollectionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is MainPage parameters)
            {
                MainPage = parameters;
            }
        }

        #region Loading
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataAsync(true);
        }

        private void ReloadDataXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            LoadDataAsync(false);
        }

        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Image imageCtrl)
                {
                    var bitmapImage = await Files.BitmapImageFromFileAsync(imageCtrl?.Tag?.ToString());
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

        private void LoadDataAsync(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                InitializeLoadingLibrariesWorker();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async Task InitializeDataAsync(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ViewModelList != null && ViewModelPage.ViewModelList.Any())
                {
                    foreach (var item in ViewModelPage.ViewModelList)
                    {
                        string combinedPath = await esAppBaseApi.GetJaquettePathAsync<BibliothequeVM>(item.Guid);
                        item.JaquettePath = !combinedPath.IsStringNullOrEmptyOrWhiteSpace() ? combinedPath : EsGeneral.LibraryDefaultJaquette;
                    }
                }

                this.GridViewMode(firstLoad);
                //this.InitializeCountBookWorker();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewItems_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is GridView gridView)
                {
                    if (this.ViewModelPage.SelectedItems.Any())
                    {
                        foreach (var gridViewItem in gridView.Items)
                        {
                            foreach (var item in this.ViewModelPage.SelectedItems)
                            {
                                if (gridViewItem is BibliothequeVM _viewModel && _viewModel.Id == item.Id && !gridView.SelectedItems.Contains(item))
                                {
                                    gridView.SelectedItems.Add(item);
                                    break;
                                }
                            }
                        }
                    }
                    gridView.SelectionChanged += GridViewItems_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridItems_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is DataGrid dataGrid)
                {
                    if (this.ViewModelPage.SelectedItems.Any())
                    {
                        foreach (var dataGridItem in dataGrid.ItemsSource)
                        {
                            foreach (var item in this.ViewModelPage.SelectedItems)
                            {
                                if (dataGridItem is BibliothequeVM _viewModel && _viewModel.Id == item.Id && !dataGrid.SelectedItems.Contains(item))
                                {
                                    dataGrid.SelectedItems.Add(item);
                                    break;
                                }
                            }
                        }
                    }

                    dataGrid.SelectionChanged += DataGridItems_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridItems_Unloaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is DataGrid dataGrid)
                {
                    dataGrid.SelectionChanged -= DataGridItems_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewItems_Unloaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is GridView gridView)
                {
                    gridView.SelectionChanged -= GridViewItems_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Navigation
        private void ABTBtn_GridViewMode_Click(object sender, RoutedEventArgs e)
        {
            this.GridViewMode(true);
        }

        private void ABTBtn_GridViewMode_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is AppBarToggleButton toggleButton && toggleButton.IsChecked != true &&
                    ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.GridView)
                {
                    toggleButton.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ABTBtn_DataGridViewMode_Click(object sender, RoutedEventArgs e)
        {
            this.DataGridViewMode(true);
        }

        private void ABTBtn_DataGridViewMode_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is AppBarToggleButton toggleButton && toggleButton.IsChecked != true &&
                    ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.DataGridView)
                {
                    toggleButton.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewMode(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.PivotItems.SelectionChanged -= PivotItems_SelectionChanged;

                if (ViewModelPage.GroupedRelatedViewModel.DataViewMode != Code.DataViewModeEnum.GridView)
                {
                    ViewModelPage.GroupedRelatedViewModel.DataViewMode = Code.DataViewModeEnum.GridView;
                }
                
                this.RefreshItemsGrouping();
                this.PivotItems.SelectedIndex = this.ViewModelPage.SelectedPivotIndex;
                this.PivotItems.SelectionChanged += PivotItems_SelectionChanged;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridViewMode(bool firstLoad)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.PivotItems.SelectionChanged -= PivotItems_SelectionChanged;

                if (ViewModelPage.GroupedRelatedViewModel.DataViewMode != Code.DataViewModeEnum.DataGridView)
                {
                    ViewModelPage.GroupedRelatedViewModel.DataViewMode = Code.DataViewModeEnum.DataGridView;
                }

                this.RefreshItemsGrouping();
                this.PivotItems.SelectedIndex = this.ViewModelPage.SelectedPivotIndex;
                this.PivotItems.SelectionChanged += PivotItems_SelectionChanged;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region SideBar
        private void CmbxSideBarItemTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender is ComboBox cmbx && cmbx.SelectedItem is SideBarItemHeaderVM headerVM)
                {
                    //foreach (var item in this.PivotRightSideBar.Items)
                    //{
                    //    if (item is NewEditBookExemplaryUC bookExemplaryUC)
                    //    {
                    //        if (bookExemplaryUC.IdItem == headerVM.IdItem)
                    //        {
                    //            this.PivotRightSideBar.SelectedItem = item;
                    //        }
                    //    }
                    //    else if (item is NewEditBookUC newEditBookUC)
                    //    {
                    //        if (newEditBookUC.IdItem == headerVM.IdItem)
                    //        {
                    //            this.PivotRightSideBar.SelectedItem = item;
                    //        }
                    //    }
                    //    else if (item is NewEditCollectionUC newEditCollectionUC)
                    //    {
                    //        if (newEditCollectionUC.IdItem == headerVM.IdItem)
                    //        {
                    //            this.PivotRightSideBar.SelectedItem = item;
                    //        }
                    //    }
                    //    else if (item is NewEditContactUC newEditContactUC)
                    //    {
                    //        if (newEditContactUC.IdItem == headerVM.IdItem)
                    //        {
                    //            this.PivotRightSideBar.SelectedItem = item;
                    //        }
                    //    }
                    //    else if (item is ContactListUC contactListUC)
                    //    {
                    //        if (contactListUC.IdItem == headerVM.IdItem)
                    //        {
                    //            this.PivotRightSideBar.SelectedItem = item;
                    //        }
                    //    }
                    //    else if (item is CollectionListUC collectionListUC)
                    //    {
                    //        if (collectionListUC.IdItem == headerVM.IdItem)
                    //        {
                    //            this.PivotRightSideBar.SelectedItem = item;
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        #region Selection
        private void PivotItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is Pivot pivot)
                {
                    this.ViewModelPage.SelectedItems = new List<BibliothequeVM>();
                    this.ViewModelPage.SelectedPivotIndex = pivot.SelectedIndex;
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
                    this.ViewModelPage.SelectedItems = gridView.SelectedItems.Cast<BibliothequeVM>().ToList();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is DataGrid dataGrid)
                {
                    this.ViewModelPage.SelectedItems = dataGrid.SelectedItems.Cast<BibliothequeVM>().ToList();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void Lv_SelectedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ListView listView && listView.SelectedItem is BibliothequeVM viewModel)
                {
                    SearchViewModel(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void Btn_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.GridView)
                {
                    var gridViewItems = this.GetSelectedGridView();
                    if (gridViewItems != null)
                    {
                        gridViewItems.SelectAll();
                    }
                }
                else if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.DataGridView)
                {
                    var dataGridItems = this.GetSelectedDataGridItems();
                    if (dataGridItems != null)
                    {
                        foreach (var dataGridItem in dataGridItems.ItemsSource)
                        {
                            if (!dataGridItems.SelectedItems.Contains(dataGridItem))
                            {
                                dataGridItems.SelectedItems.Add(dataGridItem);
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

        private void Btn_UnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.GridView)
                {
                    var gridViewItems = this.GetSelectedGridView();
                    if (gridViewItems != null)
                    {
                        gridViewItems.SelectedItems.Clear();
                    }
                }
                else if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.DataGridView)
                {
                    var dataGridItems = this.GetSelectedDataGridItems();
                    if (dataGridItems != null)
                    {
                        dataGridItems.SelectedItems.Clear();
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

        private void Btn_DeleteAll_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Paginations
        private void GotoPageXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is int page)
                {
                    GotoPage(page);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void GridViewItems_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (e.Key == Windows.System.VirtualKey.Q)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        var selectedPage = this.GetSelectedPage - 1;
                        if (selectedPage >= 1)
                        {
                            this.GotoPage(selectedPage);
                        }
                    });
                }
                else if (e.Key == Windows.System.VirtualKey.D)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        var selectedPage = this.GetSelectedPage;
                        this.GotoPage(selectedPage + 1);
                    });
                }
                else if (e.Key == Windows.System.VirtualKey.Z)
                {
                    if (sender is GridView gridView && gridView.Items.Count > 0)
                    {
                        gridView.SelectedItem = gridView.Items[0];
                    }
                }
                else if (e.Key == Windows.System.VirtualKey.S)
                {
                    if (sender is GridView gridView && gridView.Items.Count > 0)
                    {
                        gridView.SelectedItem = gridView.Items[gridView.Items.Count - 1];
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


        #region Events
        private async void ExportAllLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (ViewModelPage.ViewModelList != null)
                {
                    var suggestedFileName = $"Rostalotheque_Bibliotheques_All_{DateTime.Now:yyyyMMddHHmmss}";

                    var savedFile = await Files.SaveStorageFileAsync(new Dictionary<string, IList<string>>()
                    {
                        {"JavaScript Object Notation", new List<string>() { ".json" } }
                    }, suggestedFileName);

                    if (savedFile == null)
                    {
                        Logs.Log(m, "Le fichier n'a pas pû être créé.");
                        return;
                    }

                    //Voir : https://docs.microsoft.com/fr-fr/windows/uwp/files/quickstart-reading-and-writing-files
                    bool isFileSaved = await Files.Serialization.Json.SerializeAsync(ViewModelPage.ViewModelList, savedFile);// savedFile.Path
                    if (isFileSaved == false)
                    {
                        Logs.Log(m, "Le flux n'a pas été enregistré dans le fichier.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            //MethodBase m = MethodBase.GetCurrentMethod();
            //try
            //{
            //    var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditLibraryUC item && item.ViewModelPage.EditMode == Code.EditMode.Create);
            //    if (checkedItem != null)
            //    {
            //        this.PivotRightSideBar.SelectedItem = checkedItem;
            //    }
            //    else
            //    {
            //        NewEditLibraryUC userControl = new NewEditLibraryUC(new ManageLibraryDialogParametersVM()
            //        {
            //            EditMode = Code.EditMode.Create,
            //            ViewModelList = ViewModelPage.ViewModelList,
            //        });

            //        userControl.CancelModificationRequested += NewEditLibraryUC_CancelModificationRequested;
            //        userControl.CreateItemRequested += NewEditLibraryUC_CreateItemRequested;

            //        this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
            //        {
            //            Glyph = userControl.ViewModelPage.Glyph,
            //            Title = userControl.ViewModelPage.Header,
            //            IdItem = userControl.ViewModelPage.ItemGuid,
            //        });
            //    }
            //    this.ViewModelPage.IsSplitViewOpen = true;
            //}
            //catch (Exception ex)
            //{
            //    Logs.Log(ex, m);
            //    return;
            //}
        }

        private void EditLibraryInfosXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            //MethodBase m = MethodBase.GetCurrentMethod();
            //try
            //{
            //    var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditLibraryUC item && item.ViewModelPage.EditMode == Code.EditMode.Edit);
            //    if (checkedItem != null)
            //    {
            //        this.PivotRightSideBar.SelectedItem = checkedItem;
            //    }
            //    else
            //    {
            //        NewEditLibraryUC userControl = new NewEditLibraryUC(new ManageLibraryDialogParametersVM()
            //        {
            //            CurrentLibrary = args.Parameter as BibliothequeVM,
            //            EditMode = Code.EditMode.Edit,
            //            ViewModelList = ViewModelPage.ViewModelList,
            //        });

            //        userControl.CancelModificationRequested += NewEditLibraryUC_CancelModificationRequested;
            //        userControl.UpdateItemRequested += NewEditLibraryUC_UpdateItemRequested;

            //        this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
            //        {
            //            Glyph = userControl.ViewModelPage.Glyph,
            //            Title = userControl.ViewModelPage.Header,
            //            IdItem = userControl.ViewModelPage.ItemGuid,
            //        });
            //    }
            //    this.ViewModelPage.IsSplitViewOpen = true;
            //}
            //catch (Exception ex)
            //{
            //    Logs.Log(ex, m);
            //    return;
            //}
        }

        private async void NewEditLibraryUC_CreateItemRequested(NewEditLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                //if (sender._parameters != null)
                //{
                //    var newValue = sender.ViewModelPage.Value?.Trim();
                //    var newDescription = sender.ViewModelPage.Description?.Trim();

                //    var newViewModel = new BibliothequeVM()
                //    {
                //        Name = newValue,
                //        Description = newDescription,
                //    };

                //    var creationResult = await DbServices.Library.CreateAsync(newViewModel);
                //    if (creationResult.IsSuccess)
                //    {
                //        newViewModel.Id = creationResult.Id;
                //        ViewModelPage.ViewModelList.Add(newViewModel);
                //        await esLibrary.SaveLibraryViewModelAsync(newViewModel);

                //        sender.ViewModelPage.ResultMessageTitle = "Succès";
                //        sender.ViewModelPage.ResultMessage = creationResult.Message;
                //        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                //        sender.ViewModelPage.IsResultMessageOpen = true;
                //        this.RefreshItemsGrouping();
                //    }
                //    else
                //    {
                //        //Erreur
                //        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                //        sender.ViewModelPage.ResultMessage = creationResult.Message;
                //        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                //        sender.ViewModelPage.IsResultMessageOpen = true;
                //        return;
                //    }
                //}

                //Reset viewModel
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void NewEditLibraryUC_UpdateItemRequested(NewEditLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                //if (sender._parameters != null)
                //{
                //    var newValue = sender.ViewModelPage.Value?.Trim();
                //    var newDescription = sender.ViewModelPage.Description?.Trim();

                //    var updatedViewModel = new BibliothequeVM()
                //    {
                //        Id = sender._parameters.CurrentLibrary.Id,
                //        Name = newValue,
                //        Description = newDescription,
                //        DateEdition = DateTime.UtcNow,
                //    };

                //    var updateResult = await DbServices.Library.UpdateAsync(updatedViewModel);
                //    if (updateResult.IsSuccess)
                //    {
                //        sender._parameters.CurrentLibrary.Name = newValue;
                //        sender._parameters.CurrentLibrary.Description = newDescription;
                //        await esLibrary.SaveLibraryViewModelAsync(sender._parameters.CurrentLibrary);

                //        sender.ViewModelPage.ResultMessageTitle = "Succès";
                //        sender.ViewModelPage.ResultMessage = updateResult.Message;
                //        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                //        sender.ViewModelPage.IsResultMessageOpen = true;
                //        //this.RefreshItemsGrouping();
                //    }
                //    else
                //    {
                //        //Erreur
                //        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                //        sender.ViewModelPage.ResultMessage = updateResult.Message;
                //        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                //        sender.ViewModelPage.IsResultMessageOpen = true;
                //        return;
                //    }
                //}

                //sender.CancelModificationRequested -= NewEditLibraryUC_CancelModificationRequested;
                //sender.UpdateItemRequested -= NewEditLibraryUC_UpdateItemRequested;

                //this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        

        private void ImportLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }
        private async void ChangeJaquetteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    var result = await esAppBaseApi.ReplaceJaquetteAsync<BibliothequeVM>(viewModel.Guid);
                    if (!result.IsSuccess)
                    {
                        return;
                    }

                    viewModel.JaquettePath = result.Result?.ToString() ?? EsLibrary.LibraryDefaultJaquette;
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

        private async void ExportLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    var suggestedFileName = $"Rostalotheque_Bibliotheque_{viewModel.Name}_{DateTime.Now:yyyyMMddHHmmss}";

                    var savedFile = await Files.SaveStorageFileAsync(new Dictionary<string, IList<string>>()
                    {
                        {"JavaScript Object Notation", new List<string>() { ".json" } }
                    }, suggestedFileName);

                    if (savedFile == null)
                    {
                        Logs.Log(m, "Le fichier n'a pas pû être créé.");
                        return;
                    }

                    //Voir : https://docs.microsoft.com/fr-fr/windows/uwp/files/quickstart-reading-and-writing-files
                    bool isFileSaved = await Files.Serialization.Json.SerializeAsync(viewModel, savedFile);// savedFile.Path
                    if (isFileSaved == false)
                    {
                        Logs.Log(m, "Le flux n'a pas été enregistré dans le fichier.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DeleteLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is BibliothequeVM viewModel)
                {
                    //_commonView.DeleteLibrary(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        

        private void CategorieListXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f.GetType() == typeof(CategoriesListUC));
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    if (args.Parameter is BibliothequeVM viewModel)
                    {
                        CategoriesListUC userControl = new CategoriesListUC(new CategorieParameterDriverVM()
                        {
                            LibraryPage = this,
                            ParentLibrary = viewModel,
                        });

                        this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                        {
                            Glyph = userControl.ViewModelPage.Glyph,
                            Title = userControl.ViewModelPage.Header,
                            IdItem = userControl.ViewModelPage.ItemGuid,
                        });
                    }
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Events Categorie
        public void AddNewCategory(BibliothequeVM parentLibrary, Guid? guid = null)
        {
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCategoryUC item && item.ViewModelPage.EditMode == Code.EditMode.Create && item._categorieParameters != null);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditCategoryUC userControl = new NewEditCategoryUC(new ManageCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Create,
                        ParentLibrary = parentLibrary,
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.ParentGuid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditCategoryUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditCategoryUC_CreateItemRequested;


                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;

            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void EditCategory(BibliothequeVM parentLibrary, CategorieLivreVM currentCategorie, Guid? guid = null)
        {
            NewEditCategoryUC userControl = null;
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCategoryUC item && item.ViewModelPage.EditMode == Code.EditMode.Edit && item._categorieParameters != null);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    userControl = new NewEditCategoryUC(new ManageCategorieDialogParametersVM()
                    {
                        CurrentCategorie = currentCategorie,
                        EditMode = Code.EditMode.Edit,
                        ParentLibrary = parentLibrary,
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.ParentGuid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditCategoryUC_CancelModificationRequested;
                    userControl.UpdateItemRequested += NewEditCategoryUC_UpdateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
                    });
                }

                this.ViewModelPage.IsSplitViewOpen = true;
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
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.ParentGuid != null)
                        {
                            var bookManager = GetCategorieListSideBarByGuid((Guid)sender.ViewModelPage.ParentGuid);
                            if (bookManager != null)
                            {
                                bookManager.ViewModelPage.ParentLibrary.Categories.Add(newViewModel);
                                NewEditCategoryUC_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
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
                    var value = sender.ViewModelPage.Value?.Trim();
                    var description = sender.ViewModelPage.Description?.Trim();

                    var updatedViewModel = new CategorieLivreVM()
                    {
                        Id = sender._categorieParameters.CurrentCategorie.Id,
                        IdLibrary = sender._categorieParameters.ParentLibrary.Id,
                        Name = value,
                        Description = description,
                    };

                    var updateResult = await DbServices.Categorie.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.ParentGuid != null)
                        {
                            var bookManager = GetCategorieListSideBarByGuid((Guid)sender.ViewModelPage.ParentGuid);
                            if (bookManager != null)
                            {
                                var item = bookManager.ViewModelPage.ParentLibrary.Categories.SingleOrDefault(s => s.Id == sender._categorieParameters.CurrentCategorie.Id);
                                if (item != null)
                                {
                                    item.Name = updatedViewModel.Name;
                                    item.Description = updatedViewModel.Description;
                                }
                                NewEditCategoryUC_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void NewEditCategoryUC_CancelModificationRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CreateItemRequested -= NewEditCategoryUC_CreateItemRequested;
                sender.CancelModificationRequested -= NewEditCategoryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditCategoryUC_UpdateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Events Sub Categorie
        public void AddNewSubCategory(CategorieLivreVM currentCategorieParent, Guid? guid = null)
        {
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCategoryUC item && item.ViewModelPage.EditMode == Code.EditMode.Create && item._subCategorieParameters != null);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditCategoryUC userControl = new NewEditCategoryUC(new ManageSubCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Create,
                        Categorie = currentCategorieParent,
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.ParentGuid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditSubCategoryUC_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditSubCategoryUC_CreateItemRequested;


                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void EditSubCategory(CategorieLivreVM parentCategorie, SubCategorieLivreVM currentSubCategorie, Guid? guid = null)
        {
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCategoryUC item && item.ViewModelPage.EditMode == Code.EditMode.Edit && item._subCategorieParameters != null);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    NewEditCategoryUC userControl = new NewEditCategoryUC(new ManageSubCategorieDialogParametersVM()
                    {
                        EditMode = Code.EditMode.Edit,
                        Categorie = parentCategorie,
                        CurrentSubCategorie = currentSubCategorie,
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.ParentGuid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditSubCategoryUC_CancelModificationRequested;
                    userControl.UpdateItemRequested += NewEditSubCategoryUC_UpdateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.ViewModelPage.ItemGuid,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
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

                    var creationResult = await DbServices.Categorie.CreateSubCategorieAsync(newViewModel);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        sender._subCategorieParameters.Categorie.SubCategorieLivres.Add(newViewModel);

                        if (sender.ViewModelPage.ParentGuid != null)
                        {
                            var bookManager = GetCategorieListSideBarByGuid((Guid)sender.ViewModelPage.ParentGuid);
                            if (bookManager != null)
                            {
                                //bookManager.ViewModelPage.ParentLibrary.Categories.sub.Add(newViewModel);
                                NewEditSubCategoryUC_CancelModificationRequested(sender, e);
                            }
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
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

                    var updateResult = await DbServices.Categorie.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.ParentGuid != null)
                        {
                            var item = sender._subCategorieParameters.Categorie.SubCategorieLivres.SingleOrDefault(s => s.Id == sender._subCategorieParameters.CurrentSubCategorie.Id);
                            if (item != null)
                            {
                                item.Name = updatedViewModel.Name;
                                item.Description = updatedViewModel.Description;
                            }
                            NewEditSubCategoryUC_CancelModificationRequested(sender, e);
                        }
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                        sender.ViewModelPage.IsResultMessageOpen = true;
                        return;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void NewEditSubCategoryUC_CancelModificationRequested(NewEditCategoryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CreateItemRequested -= NewEditSubCategoryUC_CreateItemRequested;
                sender.CancelModificationRequested -= NewEditSubCategoryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditSubCategoryUC_UpdateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Groups
        public void GroupItemsByNone(int goToPage = 1)
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                IEnumerable<BibliothequeVM> itemsPage = GetPaginatedItems(goToPage);

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos bibliothèques").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.None;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        public void GroupItemsByAlphabetic(int goToPage = 1)
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                IEnumerable<BibliothequeVM> itemsPage = GetPaginatedItems(goToPage);

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Name.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.Letter;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupByCreationYear(int goToPage = 1)
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                IEnumerable<BibliothequeVM> itemsPage = GetPaginatedItems(goToPage);

                var GroupingItems = this.OrderItems(itemsPage, this.ViewModelPage.OrderedBy, this.ViewModelPage.SortedBy).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    this.ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, BibliothequeVM>>(GroupingItems);
                    this.ViewModelPage.GroupedBy = LibraryGroupVM.GroupBy.CreationYear;
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

        #region Sort - Group - Order
        private void GroupByLetterXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            this.GroupItemsByAlphabetic();
        }

        private void GroupByCreationYearXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            this.GroupByCreationYear();
        }

        private void GroupByNoneXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            this.GroupItemsByNone();
        }

        private void OrderByCroissantXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.OrderedBy = LibraryGroupVM.OrderBy.Croissant;
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void OrderByDCroissantXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.OrderedBy = LibraryGroupVM.OrderBy.DCroissant;
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void SortByNameXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.SortedBy = LibraryGroupVM.SortBy.Name;
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void SortByDateCreationXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.SortedBy = LibraryGroupVM.SortBy.DateCreation;
                this.RefreshItemsGrouping();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void RefreshItemsGrouping(int goToPage = 1, bool resetPage = true)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                switch (ViewModelPage.GroupedBy)
                {
                    case LibraryGroupVM.GroupBy.None:
                        this.GroupItemsByNone(goToPage);
                        break;
                    case LibraryGroupVM.GroupBy.Letter:
                        this.GroupItemsByAlphabetic();
                        break;
                    case LibraryGroupVM.GroupBy.CreationYear:
                        this.GroupByCreationYear(goToPage);
                        break;
                    default:
                        this.GroupItemsByNone(goToPage);
                        break;
                }

                if (resetPage)
                {
                    this.InitializePages();
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Search
        private void ASB_SearchItem_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var FilteredItems = new List<BibliothequeVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in ViewModelPage.ViewModelList)
                {
                    if (value.Name.IsStringNullOrEmptyOrWhiteSpace()) continue;

                    var found = splitSearchTerm.All((key) =>
                    {
                        return value.Name.ToLower().Contains(key.ToLower());
                    });

                    if (found)
                    {
                        FilteredItems.Add(value);
                    }
                }
                sender.ItemsSource = FilteredItems;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchItem_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem != null && args.SelectedItem is BibliothequeVM value)
                {
                    sender.Text = value.Name;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchItem_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is BibliothequeVM viewModel)
                {
                    this.SearchViewModel(viewModel);
                }
                else
                {
                    //
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void SearchViewModel(BibliothequeVM viewModel)
        {
            if (viewModel == null) return;
            if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.GridView)
            {
                SearchViewModelGridView(viewModel);
            }
            else if (ViewModelPage.GroupedRelatedViewModel.DataViewMode == Code.DataViewModeEnum.DataGridView)
            {
                SearchViewModelDataGridView(viewModel);
            }
        }

        public void SearchViewModelGridView(BibliothequeVM viewModel)
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
                                    OpenFlyoutSearchedItemGridView(_gridViewItemContainer);
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

        private void OpenFlyoutSearchedItemGridView(DependencyObject _gridViewItemContainer)
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
                            buttonActions.Flyout.ShowAt(buttonActions, new FlyoutShowOptions()
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

        private void SearchViewModelDataGridView(BibliothequeVM viewModel)
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
                        DataGrid dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(_container);
                        while (dataGrid != null && dataGrid.Name != "DataGridItems")
                        {
                            dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(dataGrid);
                            if (dataGrid == null)
                            {
                                return;
                            }
                            else
                            {
                                if (dataGrid.Name == "DataGridItems")
                                {
                                    break;
                                }
                            }
                        }

                        if (dataGrid != null)
                        {
                            foreach (var dataGridItem in dataGrid.ItemsSource)
                            {
                                if (dataGridItem is BibliothequeVM _viewModel && _viewModel == viewModel)
                                {
                                    if (dataGrid.SelectedItem != dataGridItem)
                                    {
                                        dataGrid.SelectedItem = dataGridItem;
                                    }
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

        #endregion

        #region Functions
        private void InitializePages()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ViewModelPage.PagesList.Clear();

                if (ViewModelPage.ViewModelList != null && ViewModelPage.ViewModelList.Any())
                {
                    int nbPageDefault = ViewModelPage.ViewModelList.Count() / ViewModelPage.MaxItemsPerPage;
                    double nbPageExact = ViewModelPage.ViewModelList.Count() / Convert.ToDouble(ViewModelPage.MaxItemsPerPage);
                    int nbPageRounded = nbPageExact > nbPageDefault ? nbPageDefault + 1 : nbPageDefault;
                    ViewModelPage.CountPages = nbPageRounded;

                    for (int i = 0; i < ViewModelPage.CountPages; i++)
                    {
                        ViewModelPage.PagesList.Add(new PageSystemVM()
                        {
                            CurrentPage = i + 1,
                            IsPageSelected = i == 0,
                            BackgroundColor = i == 0 ? Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush : Application.Current.Resources["PageNotSelectedBackground"] as SolidColorBrush,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void GotoPage(int page)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                foreach (var pageVm in ViewModelPage.PagesList)
                {
                    if (pageVm.CurrentPage != page && pageVm.IsPageSelected == true)
                    {
                        pageVm.IsPageSelected = false;
                        pageVm.BackgroundColor = Application.Current.Resources["PageNotSelectedBackground"] as SolidColorBrush;
                    }
                    else if (pageVm.CurrentPage == page && pageVm.IsPageSelected == false)
                    {
                        pageVm.IsPageSelected = true;
                        pageVm.BackgroundColor = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush;
                    }
                }
                //this.RefreshItemsGrouping(page, false);
                var buttonsPage = VisualViewHelpers.FindVisualChilds<Button>(this.itemControlPageList);
                if (buttonsPage != null && buttonsPage.Any())
                {
                    var buttonPage = buttonsPage.FirstOrDefault(f => f.CommandParameter is int commandPage && commandPage == page);
                    if (buttonPage != null)
                    {
                        scrollVPages.ScrollToElement(buttonPage, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private int GetSelectedPage
        {
            get
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                try
                {
                    var selectedPage = ViewModelPage.PagesList.FirstOrDefault(f => f.IsPageSelected == true)?.CurrentPage ?? 1;
                    return selectedPage;
                }
                catch (Exception ex)
                {
                    Logs.Log(ex, m);
                    return 1;
                }
            }
        }

        private IEnumerable<BibliothequeVM> GetPaginatedItems()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var selectedPage = ViewModelPage.PagesList.FirstOrDefault(f => f.IsPageSelected == true)?.CurrentPage ?? 1;
                return GetPaginatedItems(selectedPage);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return Enumerable.Empty<BibliothequeVM>();
            }
        }

        private IEnumerable<BibliothequeVM> GetPaginatedItems(int goToPage = 1)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                IEnumerable<BibliothequeVM> itemsPage = Enumerable.Empty<BibliothequeVM>();

                //Si la séquence contient plus d'items que le nombre max éléments par page
                if (ViewModelPage.ViewModelList.Count > ViewModelPage.MaxItemsPerPage)
                {
                    //Si la première page (ou moins ^^')
                    if (goToPage <= 1)
                    {
                        itemsPage = ViewModelPage.ViewModelList.Take(ViewModelPage.MaxItemsPerPage);
                    }
                    else //Si plus que la première page
                    {
                        var nbItemsToSkip = ViewModelPage.MaxItemsPerPage * (goToPage - 1);
                        if (ViewModelPage.ViewModelList.Count >= nbItemsToSkip)
                        {
                            var getRest = ViewModelPage.ViewModelList.Skip(nbItemsToSkip);
                            //Si reste de la séquence contient plus d'items que le nombre max éléments par page
                            if (getRest.Count() > ViewModelPage.MaxItemsPerPage)
                            {
                                itemsPage = getRest.Take(ViewModelPage.MaxItemsPerPage);
                            }
                            else
                            {
                                itemsPage = getRest;
                            }
                        }
                    }
                }
                else //Si la séquence contient moins ou le même nombre d'items que le nombre max éléments par page
                {
                    itemsPage = ViewModelPage.ViewModelList;
                }

                return itemsPage;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return Enumerable.Empty<BibliothequeVM>();
            }
        }

        private void AddItemToSideBar(PivotItem item, SideBarItemHeaderVM sideBarItem)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.PivotRightSideBar.Items.Add(item);
                this.PivotRightSideBar.SelectedItem = item;
                ViewModelPage.ItemsSideBarHeader.Add(sideBarItem);
                this.CmbxSideBarItemTitle.SelectedItem = sideBarItem;
                if (PivotRightSideBar.Items.Count >= 2)
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Visible;
                }
                else
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void RemoveItemToSideBar(PivotItem item)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }

                if (this.CmbxSideBarItemTitle.Items.Count > 0)
                {

                    foreach (var cmbxItem in this.CmbxSideBarItemTitle.Items)
                    {
                        if (cmbxItem is SideBarItemHeaderVM headerVM)
                        {
                            if (item is NewEditLibraryUC bookExemplaryUC)
                            {
                                if (bookExemplaryUC.ItemGuid == headerVM.IdItem)
                                {
                                    ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                                    break;
                                }
                            }
                        }
                    }
                }

                this.PivotRightSideBar.Items.Remove(item);
                if (PivotRightSideBar.Items.Count < 2)
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private CategoriesListUC GetCategorieListSideBarByGuid(Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    object itemPivot = this.PivotRightSideBar.Items.FirstOrDefault(f => f is CategoriesListUC item && item.ViewModelPage.ItemGuid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as CategoriesListUC;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
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

        public GridView GetSelectedGridView(string gridViewName = "GridViewItems")
        {
            try
            {
                if (this.PivotItems.SelectedItem == null)
                {
                    return null;
                }

                var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
                var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
                while (gridView != null && gridView.Name != gridViewName)
                {
                    gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
                    if (gridView == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (gridView.Name == gridViewName)
                        {
                            break;
                        }
                    }
                }

                return gridView;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        private DataGrid GetSelectedDataGridItems(string dataGridName = "DataGridItems")
        {
            try
            {
                if (this.PivotItems.SelectedItem == null)
                {
                    return null;
                }

                var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
                DataGrid dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(_container);
                while (dataGrid != null && dataGrid.Name != dataGridName)
                {
                    dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(dataGrid);
                    if (dataGrid == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (dataGrid.Name == dataGridName)
                        {
                            break;
                        }
                    }
                }

                return dataGrid;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Ouvre la liste des livre de la bibliothèque
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Viewbox viewbox && viewbox.Tag is BibliothequeVM viewModel)
                {
                    MainPage.BookCollectionNavigationAsync(viewModel, null);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridItems_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                if (sender is DataGrid dataGrid && dataGrid.SelectedItem is BibliothequeVM viewModel)
                {
                    MainPage.BookCollectionNavigationAsync(viewModel, null);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }


        #region Loading BacKGroundWorker
        private BackgroundWorker WorkerLoadLibraries;
        public void InitializeLoadingLibrariesWorker()
        {
            try
            {
                if (WorkerLoadLibraries == null)
                {
                    WorkerLoadLibraries = new BackgroundWorker()
                    {
                        WorkerReportsProgress = true,
                        WorkerSupportsCancellation = true,
                    };

                    WorkerLoadLibraries.ProgressChanged += WorkerLoadLibraries_ProgressChanged;
                    WorkerLoadLibraries.DoWork += WorkerLoadLibraries_DoWork; ;
                    WorkerLoadLibraries.RunWorkerCompleted += WorkerLoadLibraries_RunWorkerCompleted; ;
                }

                if (WorkerLoadLibraries != null)
                {
                    if (!WorkerLoadLibraries.IsBusy)
                    {
                        ViewModelPage.ViewModelList.Clear();
                        this.MainPage.OpenBusyLoader(new BusyLoaderParametersVM()
                        {
                            ProgessText = $"Chargement des bibliothèques en cours.",
                            CancelButtonText = "Annuler le chargement",
                            CancelButtonVisibility = Visibility.Visible,
                            CancelButtonCallback = () =>
                            {
                                if (WorkerLoadLibraries.IsBusy)
                                {
                                    WorkerLoadLibraries.CancelAsync();
                                }
                            },
                            OpenedLoaderCallback = () => WorkerLoadLibraries.RunWorkerAsync(),
                        });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void WorkerLoadLibraries_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker)
                {
                    using (Task<IEnumerable<long>> task = DbServices.Library.GetIdLibrariesAsync())
                    {
                        task.Wait();
                        int ModelCount = task.Result.Count();
                        double progressPercentage;
                        int count = 0;

                        List<BibliothequeVM> bibliothequeVMs = new List<BibliothequeVM>();
                        foreach (var id in task.Result)
                        {
                            using (Task<BibliothequeVM> getLibraryTask = DbServices.Library.SingleVMAsync(id))
                            {
                                getLibraryTask.Wait();
                                var viewModel = getLibraryTask.Result;
                                if (viewModel != null)
                                {
                                    using (Task<string> jaquetteTask = esAppBaseApi.GetJaquettePathAsync<BibliothequeVM>(viewModel.Guid))
                                    {
                                        jaquetteTask.Wait();
                                        viewModel.JaquettePath = !jaquetteTask.Result.IsStringNullOrEmptyOrWhiteSpace() ? jaquetteTask.Result : EsGeneral.LibraryDefaultJaquette;
                                    }

                                    bibliothequeVMs.Add(viewModel);
                                }

                                var NumberModel = count + 1;
                                double Operation = (double)NumberModel / (double)ModelCount;
                                progressPercentage = Operation * 100;
                                int ProgressValue = Convert.ToInt32(progressPercentage);

                                if (worker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(500);
                                    worker.ReportProgress(ProgressValue, viewModel);
                                }
                                count++;
                            }
                        }

                        e.Result = bibliothequeVMs.ToArray();
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

        private void WorkerLoadLibraries_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.UserState != null && e.UserState is BibliothequeVM bibliothequeVM)
                {
                    //Progress bar/text
                    var busyLoader = this.MainPage.GetBusyLoader;
                    if (busyLoader != null)
                    {
                        busyLoader.TbcTitle.Text = $"{e.ProgressPercentage} % des bibliothèques chargées.\nBibliothèque en cours : \"{bibliothequeVM.Name}\"";
                        if (busyLoader.BtnCancel.Visibility != Visibility.Visible)
                            busyLoader.BtnCancel.Visibility = Visibility.Visible;
                        
                        ViewModelPage.ViewModelList.Add(bibliothequeVM);
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

        private void WorkerLoadLibraries_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var busyLoader = this.MainPage.GetBusyLoader;
                if (busyLoader != null)
                {
                    // Si erreur
                    if (e.Error != null)
                    {
                        busyLoader.TbcTitle.Text = $"Le chargement des bibliothèques s'est terminé avec l'erreur :\n\"{e.Error.Message}\"\n\nActualisation du catalogue des livres en cours...";
                    }
                    else if (e.Cancelled)
                    {
                        busyLoader.TbcTitle.Text = $"Le chargement des bibliothèques a été annulé par l'utilisateur\nActualisation du catalogue des livres en cours...";
                    }
                    else
                    {
                        if (e.Result != null && e.Result is BibliothequeVM[] viewModelList)
                        {
                            busyLoader.TbcTitle.Text = $"{viewModelList.Length} {(viewModelList.Length > 1 ? "bibliothèques ont été chargées": "bibliothèque a été chargée")}.";
                        }
                    }

                    if (busyLoader.BtnCancel.Visibility != Visibility.Collapsed)
                        busyLoader.BtnCancel.Visibility = Visibility.Collapsed;
                }

                DispatcherTimer dispatcherTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 1),
                };

                dispatcherTimer.Tick += (t, f) =>
                {
                    this.MainPage.CloseBusyLoader();
                    this.GridViewMode(true);
                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();

                WorkerLoadLibraries.Dispose();
                WorkerLoadLibraries = null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }

        }
        #endregion

    }

    public class LibraryCollectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ToastContentBuilder OperationRunning
        {
            get => new ToastContentBuilder()
                        .AddText($"Une tâche en cours d'exécution")
                        .AddText("Une ou plusieurs tâches d'arrière-plan sont en cours d'exécution, nous vous prions de patienter quelques instants.");
        }

        private int _CountPages;
        public int CountPages
        {
            get => this._CountPages;
            set
            {
                if (_CountPages != value)
                {
                    this._CountPages = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private BibliothequeVM _SearchedViewModel = null;
        public BibliothequeVM SearchedViewModel
        {
            get => this._SearchedViewModel;
            set
            {
                if (_SearchedViewModel != value)
                {
                    this._SearchedViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _MaxItemsPerPage = 20;
        public int MaxItemsPerPage
        {
            get => this._MaxItemsPerPage;
            set
            {
                if (_MaxItemsPerPage != value)
                {
                    this._MaxItemsPerPage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<PageSystemVM> _PagesList = new ObservableCollection<PageSystemVM>();
        public ObservableCollection<PageSystemVM> PagesList
        {
            get => this._PagesList;
            set
            {
                if (_PagesList != value)
                {
                    this._PagesList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<TaskVM> _TaskList = new ObservableCollection<TaskVM>();
        public ObservableCollection<TaskVM> TaskList
        {
            get => this._TaskList;
            set
            {
                if (_TaskList != value)
                {
                    this._TaskList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<SideBarItemHeaderVM> _ItemsSideBarHeader = new ObservableCollection<SideBarItemHeaderVM>();
        public ObservableCollection<SideBarItemHeaderVM> ItemsSideBarHeader
        {
            get => this._ItemsSideBarHeader;
            set
            {
                if (_ItemsSideBarHeader != value)
                {
                    this._ItemsSideBarHeader = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ICollection<BibliothequeVM> _SelectedItems = new List<BibliothequeVM>();
        public ICollection<BibliothequeVM> SelectedItems
        {
            get => this._SelectedItems;
            set
            {
                if (_SelectedItems != value)
                {
                    this._SelectedItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _SelectedPivotIndex;
        public int SelectedPivotIndex
        {
            get => this._SelectedPivotIndex;
            set
            {
                if (_SelectedPivotIndex != value)
                {
                    this._SelectedPivotIndex = value;
                    this.OnPropertyChanged();
                }
            }
        }

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

        private LibraryGroupVM.SortBy _SortedBy = LibraryGroupVM.SortBy.Name;
        public LibraryGroupVM.SortBy SortedBy
        {
            get => this._SortedBy;
            set
            {
                if (this._SortedBy != value)
                {
                    this._SortedBy = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private LibraryGroupVM.OrderBy _OrderedBy = LibraryGroupVM.OrderBy.Croissant;
        public LibraryGroupVM.OrderBy OrderedBy
        {
            get => this._OrderedBy;
            set
            {
                if (this._OrderedBy != value)
                {
                    this._OrderedBy = value;
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

        public const double MinSplitViewWidth = 400;

        private double _SplitViewWidth = MinSplitViewWidth;
        public double SplitViewWidth
        {
            get => this._SplitViewWidth;
            set
            {
                if (_SplitViewWidth != value)
                {
                    this._SplitViewWidth = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private List<BibliothequeVM> _ViewModelList = new List<BibliothequeVM>();
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
