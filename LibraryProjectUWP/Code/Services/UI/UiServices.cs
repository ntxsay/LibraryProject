using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.Views.Book;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace LibraryProjectUWP.Code.Services.UI
{
    internal class UiServices
    {
        public SearchBookUC GetSearchBookUCSideBar(Pivot pivot)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                if (pivot.Items.Count > 0)
                {
                    object itemPivot = pivot.Items.FirstOrDefault(f => f is SearchBookUC item);
                    if (itemPivot != null)
                    {
                        return itemPivot as SearchBookUC;
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

        public BookExemplaryListUC GetBookExemplariesSideBarByGuid(Pivot pivot, Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                if (pivot.Items.Count > 0)
                {
                    object itemPivot = pivot.Items.FirstOrDefault(f => f is BookExemplaryListUC item && item.ViewModelPage.ItemGuid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as BookExemplaryListUC;
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

        public BookPretListUC GetBookPretListUCSideBarByGuid(Pivot pivot, Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                if (pivot.Items.Count > 0)
                {
                    object itemPivot = pivot.Items.FirstOrDefault(f => f is BookPretListUC item && item.ViewModelPage.ItemGuid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as BookPretListUC;
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

        public NewEditBookPretUC GetNewEditBookPretUCSideBarByGuid(Pivot pivot, Guid guid)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                if (pivot.Items.Count > 0)
                {
                    object itemPivot = pivot.Items.FirstOrDefault(f => f is NewEditBookPretUC item && item.ViewModelPage.ItemGuid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as NewEditBookPretUC;
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

        public GridViewItem GetSelectedGridViewItem(long idBook, Pivot pivot, string gridViewName = "GridViewItems", bool selectAfterFinded = false)
        {
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                foreach (var pivotItem in pivot.Items)
                {
                    if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == idBook))
                    {
                        if (pivot.SelectedItem != pivotItem)
                        {
                            pivot.SelectedItem = pivotItem;
                        }

                        var _container = pivot.ContainerFromItem(pivotItem);
                        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container, gridViewName);
                        if (gridView != null)
                        {
                            foreach (var gridViewItem in gridView.Items)
                            {
                                if (gridViewItem is LivreVM _viewModel && _viewModel.Id == idBook)
                                {
                                    if (selectAfterFinded)
                                    {
                                        if (gridView.SelectedItem != gridViewItem)
                                        {
                                            gridView.SelectedItem = gridViewItem;
                                        }
                                    }

                                    var _gridContainer = gridView.ContainerFromItem(gridViewItem);
                                    return _gridContainer as GridViewItem;
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

        public LivreVM SearchViewModelInCurrentGridView(long idBook, Pivot pivot, string gridViewName = "GridViewItems", bool selectAfterFinded = false)
        {
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                foreach (var pivotItem in pivot.Items)
                {
                    if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == idBook))
                    {
                        if (pivot.SelectedItem != pivotItem)
                        {
                            pivot.SelectedItem = pivotItem;
                        }

                        var _container = pivot.ContainerFromItem(pivotItem);
                        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container, gridViewName);
                        if (gridView != null)
                        {
                            foreach (var gridViewItem in gridView.Items)
                            {
                                if (gridViewItem is LivreVM _viewModel && _viewModel.Id == idBook)
                                {
                                    if (selectAfterFinded)
                                    {
                                        if (gridView.SelectedItem != gridViewItem)
                                        {
                                            gridView.SelectedItem = gridViewItem;
                                        }
                                    }

                                    return _viewModel;
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

        public LivreVM SearchViewModelInCurrentDataGrid(Pivot pivot, long idBook, string dataGridName = "DataGridItems", bool selectAfterFinded = false)
        {
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                foreach (var pivotItem in pivot.Items)
                {
                    if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == idBook))
                    {
                        if (selectAfterFinded)
                        {
                            if (pivot.SelectedItem != pivotItem)
                            {
                                pivot.SelectedItem = pivotItem;
                            }
                        }
                        
                        var _container = pivot.ContainerFromItem(pivotItem);
                        DataGrid dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(_container, dataGridName);
                        if (dataGrid != null)
                        {
                            foreach (var dataGridItem in dataGrid.ItemsSource)
                            {
                                if (dataGridItem is LivreVM _viewModel && _viewModel.Id == idBook)
                                {
                                    if (selectAfterFinded)
                                    {
                                        if (dataGrid.SelectedItem != dataGridItem)
                                        {
                                            dataGrid.SelectedItem = dataGridItem;
                                        }
                                    }

                                    return _viewModel;
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

        public GridView GetSelectedGridViewFromPivotTemplate(Pivot pivot, string gridViewName = "GridViewItems")
        {
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                if (pivot.SelectedItem == null)
                {
                    return null;
                }

                var _container = pivot.ContainerFromItem(pivot.SelectedItem);
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

        public DataGrid GetSelectedDataGridFromPivotTemplate(Pivot pivot, string dataGridName = "DataGridItems")
        {
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                if (pivot.SelectedItem == null)
                {
                    return null;
                }

                var _container = pivot.ContainerFromItem(pivot.SelectedItem);
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


    }
}
