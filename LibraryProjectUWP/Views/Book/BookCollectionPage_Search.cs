using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Extensions;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookCollectionPage : Page
    {
        public void SearchViewModel(LivreVM viewModel)
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

        public void SearchViewModelGridView(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }
                var isMatchInCurrent = SearchViewModelInCurrentGridView(viewModel);
                if (!isMatchInCurrent)
                {
                    foreach (var pageVm in ViewModelPage.PagesList)
                    {
                        var search = GetPaginatedItems(_parameters.ParentLibrary.Books, pageVm.CurrentPage);
                        if (search != null && search.Any(f => f.Id == viewModel.Id))
                        {
                            ViewModelPage.SearchedViewModel = viewModel;
                            GotoPage(pageVm.CurrentPage);
                            return;
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

        public bool SearchViewModelInCurrentGridView(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return false;
                }

                foreach (var pivotItem in PivotItems.Items)
                {
                    if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == viewModel.Id))
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
                                return false;
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
                                if (gridViewItem is LivreVM _viewModel && _viewModel.Id == viewModel.Id)
                                {
                                    if (gridView.SelectedItem != gridViewItem)
                                    {
                                        gridView.SelectedItem = gridViewItem;
                                    }

                                    var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                                    var selectedGridViewitem = _gridViewItemContainer as GridViewItem;

                                    var _selectedPivotItem = PivotItems.ContainerFromItem(pivotItem);
                                    var selectedPivotItem = _selectedPivotItem as PivotItem;

                                    var scrollViewer = VisualViewHelpers.FindVisualChild<ScrollViewer>(selectedPivotItem, "scrollItems") ;
                                    if (scrollViewer != null)
                                    {
                                        var result = scrollViewer.ScrollToElement(selectedGridViewitem, true, false);
                                        if (result == true)
                                        {
                                            OpenFlyoutSearchedItemGridView(_gridViewItemContainer);
                                        }
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
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

        private void SearchViewModelDataGridView(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                var isMatch = SearchViewModelInCurrentDataGrid(viewModel);
                if (!isMatch)
                {
                    foreach (var pageVm in ViewModelPage.PagesList)
                    {
                        var search = GetPaginatedItems(_parameters.ParentLibrary.Books, pageVm.CurrentPage);
                        if (search != null && search.Any(f => f.Id == viewModel.Id))
                        {
                            ViewModelPage.SearchedViewModel = viewModel;
                            GotoPage(pageVm.CurrentPage);
                            return;
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

        public bool SearchViewModelInCurrentDataGrid(LivreVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return false;
                }

                foreach (var pivotItem in PivotItems.Items)
                {
                    if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == viewModel.Id))
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
                                return false;
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
                                if (dataGridItem is LivreVM _viewModel && _viewModel == viewModel)
                                {
                                    if (dataGrid.SelectedItem != dataGridItem)
                                    {
                                        dataGrid.SelectedItem = dataGridItem;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }

        private ScrollViewer GetScrollViewerFromGridView(DependencyObject dependencyObject)
        {
            try
            {
                return VisualViewHelpers.FindVisualChild<ScrollViewer>(dependencyObject);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
