using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookCollectionPage : Page
    {
        private void NewCollectionXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            NewEditCollection(null, EditMode.Create);
        }


        //internal async Task EditCollection(CollectionVM viewModel, Guid? guid = null, Type ownerType = null)
        //{
        //    MethodBase m = MethodBase.GetCurrentMethod();
        //    try
        //    {
        //        var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCollectionUC item && item.ViewModelPage.EditMode == Code.EditMode.Edit);
        //        if (checkedItem != null)
        //        {
        //            this.PivotRightSideBar.SelectedItem = checkedItem;
        //        }
        //        else
        //        {
        //            var itemList = await DbServices.Collection.AllVMAsync();
        //            NewEditCollectionUC userControl = new NewEditCollectionUC(new ManageCollectionParametersDriverVM()
        //            {
        //                ParentSideBarItemType = ownerType,
        //                EditMode = Code.EditMode.Edit,
        //                ViewModelList = itemList,
        //                //ParentLibrary = _parameters.ParentLibrary,
        //                CurrentViewModel = viewModel,
        //            });

        //            if (guid != null)
        //            {
        //                userControl.ViewModelPage.ParentGuid = guid;
        //            }

        //            userControl.CancelModificationRequested += NewEditCollectionUC_Create_CancelModificationRequested;
        //            userControl.UpdateItemRequested += NewEditCollectionUC_UpdateItemRequested;

        //            this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
        //            {
        //                Glyph = userControl.ViewModelPage.Glyph,
        //                Title = userControl.ViewModelPage.Header,
        //                IdItem = userControl.ViewModelPage.ItemGuid,
        //            });
        //        }
        //        this.ViewModelPage.IsSplitViewOpen = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.Log(ex, m);
        //        return;
        //    }
        //}


        private async void DisplayCollectionListXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f.GetType() == typeof(CollectionListUC));
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    //IList<CollectionVM> itemList = await DbServices.Collection.MultipleVmInLibraryAsync(_parameters.ParentLibrary.Id, Code.CollectionTypeEnum.Collection);
                    CollectionListUC userControl = new CollectionListUC(new CollectionListParametersDriverVM()
                    {
                        ParentPage = this,
                        ParentLibrary = Parameters.ParentLibrary,
                        //ViewModelList = itemList?.OrderBy(o => o.Name).ToList(), //ViewModelPage.ContactViewModelList,
                    });

                    userControl.CancelModificationRequested += CollectionListUC_CancelModificationRequested;

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
                Logs.Log(ex, m);
                return;
            }
        }

        private void CollectionListUC_CancelModificationRequested(CollectionListUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= CollectionListUC_CancelModificationRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task UpdateLibraryCollectionAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (Parameters.ParentLibrary != null)
                {
                    Parameters.ParentLibrary.CountNotInCollectionBooks = await DbServices.Collection.CountUnCategorizedBooks(Parameters.ParentLibrary.Id);
                    if (Parameters.ParentLibrary.Collections.Any())
                    {
                        Parameters.ParentLibrary.Collections.Clear();
                        var itemList = await DbServices.Collection.MultipleVmInLibraryAsync(Parameters.ParentLibrary.Id);
                        if (itemList != null && itemList.Any())
                        {
                            foreach (var item in itemList)
                            {
                                Parameters.ParentLibrary.Collections.Add(item);
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

    }
}
