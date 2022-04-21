using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
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
