﻿using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.Collection;
using LibraryProjectUWP.Views.Contact;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibraryProjectUWP.Code.Services.UI
{
    internal class UiServices
    {
        #region SideBar
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

        public CollectionListUC GetCollectionListUCSideBarByGuid(Pivot pivot, Guid guid)
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
                    object itemPivot = pivot.Items.FirstOrDefault(f => f is CollectionListUC item && item.ViewModelPage.ItemGuid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as CollectionListUC;
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

        public ContactListUC GetContactListUCSideBarByGuid(Pivot pivot, Guid guid)
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
                    object itemPivot = pivot.Items.FirstOrDefault(f => f is ContactListUC item && item.ItemGuid == guid);
                    if (itemPivot != null)
                    {
                        return itemPivot as ContactListUC;
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
        #endregion

        public GridViewItem GetSelectedGridViewItem<T>(long id, Pivot pivot, string gridViewName = "GridViewItems", bool selectAfterFinded = false) where T : class
        {
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                foreach (var pivotItem in pivot.Items)
                {
                    if (typeof(T) == typeof(BibliothequeVM))
                    {
                        if (pivotItem is IGrouping<string, BibliothequeVM> group && group.Any(f => f.Id == id))
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
                                    if (gridViewItem is BibliothequeVM _viewModel && _viewModel.Id == id)
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
                    else if (typeof(T) == typeof(LivreVM))
                    {
                        if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == id))
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
                                    if (gridViewItem is LivreVM _viewModel && _viewModel.Id == id)
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

        public object SelectDataGridItem<T>(long id, Pivot pivot, string dataGridName = "DataGridItems", bool selectAfterFinded = true) where T : class
        {
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                foreach (var pivotItem in pivot.Items)
                {
                    if (typeof(T) == typeof(BibliothequeVM))
                    {
                        if (pivotItem is IGrouping<string, BibliothequeVM> group && group.Any(f => f.Id == id))
                        {
                            if (pivot.SelectedItem != pivotItem)
                            {
                                pivot.SelectedItem = pivotItem;
                            }

                            var _container = pivot.ContainerFromItem(pivotItem);
                            DataGrid dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(_container, dataGridName);
                            if (dataGrid != null)
                            {
                                foreach (var dataGridItem in dataGrid.ItemsSource)
                                {
                                    if (dataGridItem is BibliothequeVM _viewModel && _viewModel.Id == id)
                                    {
                                        if (selectAfterFinded)
                                        {
                                            if (dataGrid.SelectedItem != dataGridItem)
                                            {
                                                dataGrid.SelectedItem = dataGridItem;
                                            }
                                        }

                                        return dataGridItem;
                                    }
                                }
                            }
                        }

                    }
                    else if (typeof(T) == typeof(LivreVM))
                    {
                        if (pivotItem is IGrouping<string, LivreVM> group && group.Any(f => f.Id == id))
                        {
                            if (pivot.SelectedItem != pivotItem)
                            {
                                pivot.SelectedItem = pivotItem;
                            }

                            var _container = pivot.ContainerFromItem(pivotItem);
                            DataGrid dataGrid = VisualViewHelpers.FindVisualChild<DataGrid>(_container, dataGridName);
                            if (dataGrid != null)
                            {
                                foreach (var dataGridItem in dataGrid.ItemsSource)
                                {
                                    if (dataGridItem is LivreVM _viewModel && _viewModel.Id == id)
                                    {
                                        if (selectAfterFinded)
                                        {
                                            if (dataGrid.SelectedItem != dataGridItem)
                                            {
                                                dataGrid.SelectedItem = dataGridItem;
                                            }
                                        }

                                        return dataGridItem;
                                    }
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


        public Image GetSelectedThumbnailImage<T>(long id, Pivot pivot, string gridViewName = "GridViewItems") where T : class
        {
            try
            {
                GridViewItem gridviewItem = GetSelectedGridViewItem<T>(id, pivot, gridViewName, false);
                if (gridviewItem == null)
                {
                    return null;
                }

                var grid = VisualViewHelpers.FindVisualChild<Grid>(gridviewItem);
                if (grid != null)
                {
                    if (grid.Children.FirstOrDefault(f => f is Viewbox _viewboxThumbnailContainer && _viewboxThumbnailContainer.Name == "ViewboxSimpleThumnailDatatemplate") is Viewbox viewboxThumbnailContainer)
                    {
                        if (viewboxThumbnailContainer.Child is Border border)
                        {
                            if (border.Child is Grid gridImageContainer)
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

                if (pivot.Items.Count == 0 || pivot.SelectedIndex < 0)
                {
                    return null;
                }

                var _container = pivot.ContainerFromItem(pivot.Items[pivot.SelectedIndex]);
                var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container, gridViewName);
                if (gridView != null)
                {
                    return gridView;
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
                    if (grid.Children.FirstOrDefault(f => f is Viewbox _viewboxThumbnailContainer && _viewboxThumbnailContainer.Name == "ViewboxSimpleThumnailDatatemplate") is Viewbox viewboxThumbnailContainer)
                    {
                        if (viewboxThumbnailContainer.Child is Border border)
                        {
                            if (border.Child is Grid gridImageContainer)
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


    }
}
