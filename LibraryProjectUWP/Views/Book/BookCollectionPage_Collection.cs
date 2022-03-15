using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Collection;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class BookCollectionPage : Page
    {
        private async void NewCollectionXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await NewCollectionAsync(string.Empty);
        }

        internal async Task NewCollectionAsync(string partName, Guid? guid = null, Type ownerType = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCollectionUC item && item.ViewModelPage.EditMode == Code.EditMode.Create);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var itemList = await DbServices.Collection.AllVMAsync();
                    NewEditCollectionUC userControl = new NewEditCollectionUC(new ManageCollectionParametersDriverVM()
                    {
                        ParentSideBarItemType = ownerType,
                        EditMode = Code.EditMode.Create,
                        ViewModelList = itemList,
                        //ParentLibrary = _parameters.ParentLibrary,
                        CurrentViewModel = new CollectionVM()
                        {
                            IdLibrary = _parameters.ParentLibrary.Id,
                            Name = partName,
                        }
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditCollectionUC_Create_CancelModificationRequested;
                    userControl.CreateItemRequested += NewEditCollectionUC_Create_CreateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
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

        private async void NewEditCollectionUC_Create_CreateItemRequested(NewEditCollectionUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    CollectionVM newViewModel = sender.ViewModelPage.ViewModel;

                    var creationResult = await DbServices.Collection.CreateAsync(newViewModel, _parameters.ParentLibrary.Id);
                    if (creationResult.IsSuccess)
                    {
                        newViewModel.Id = creationResult.Id;
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = creationResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.Guid != null)
                        {
                            if (sender._parameters.ParentSideBarItemType == typeof(NewEditBookUC))
                            {
                                NewEditBookUC bookManager = GetBookSideBarByGuid((Guid)sender.ViewModelPage.Guid);
                                if (bookManager != null)
                                {
                                    bookManager.ViewModelPage.ViewModel.Publication.Collections.Add(newViewModel);
                                }
                            }
                            else if (sender._parameters.ParentSideBarItemType == typeof(CollectionListUC))
                            {
                                CollectionListUC bookManager = GetCollectionListSideBarByGuid((Guid)sender.ViewModelPage.Guid);
                                if (bookManager != null)
                                {
                                    bookManager.ViewModelPage.CollectionViewModelList.Add(newViewModel);
                                }
                            }
                            NewEditCollectionUC_Create_CancelModificationRequested(sender, e);
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

                sender.ViewModelPage.ViewModel = new CollectionVM();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        internal async Task EditCollection(CollectionVM viewModel, Guid? guid = null, Type ownerType = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var checkedItem = this.PivotRightSideBar.Items.FirstOrDefault(f => f is NewEditCollectionUC item && item.ViewModelPage.EditMode == Code.EditMode.Edit);
                if (checkedItem != null)
                {
                    this.PivotRightSideBar.SelectedItem = checkedItem;
                }
                else
                {
                    var itemList = await DbServices.Collection.AllVMAsync();
                    NewEditCollectionUC userControl = new NewEditCollectionUC(new ManageCollectionParametersDriverVM()
                    {
                        ParentSideBarItemType = ownerType,
                        EditMode = Code.EditMode.Edit,
                        ViewModelList = itemList,
                        //ParentLibrary = _parameters.ParentLibrary,
                        CurrentViewModel = viewModel,
                    });

                    if (guid != null)
                    {
                        userControl.ViewModelPage.Guid = guid;
                    }

                    userControl.CancelModificationRequested += NewEditCollectionUC_Create_CancelModificationRequested;
                    userControl.UpdateItemRequested += NewEditCollectionUC_UpdateItemRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
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

        private async void NewEditCollectionUC_UpdateItemRequested(NewEditCollectionUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    var updatedViewModel = sender.ViewModelPage.ViewModel;

                    var updateResult = await DbServices.Collection.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender._parameters.CurrentViewModel.Copy(updatedViewModel);
                        sender.ViewModelPage.ResultMessageTitle = "Succès";
                        sender.ViewModelPage.ResultMessage = updateResult.Message;
                        sender.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                        sender.ViewModelPage.IsResultMessageOpen = true;

                        if (sender.ViewModelPage.Guid != null)
                        {

                        }

                        NewEditCollectionUC_Create_CancelModificationRequested(sender, e);
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

                sender.ViewModelPage.ViewModel = new CollectionVM();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditCollectionUC_Create_CancelModificationRequested(NewEditCollectionUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditCollectionUC_Create_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditCollectionUC_Create_CreateItemRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

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
                    IList<CollectionVM> itemList = await DbServices.Collection.MultipleVmInLibraryAsync(_parameters.ParentLibrary.Id, Code.CollectionTypeEnum.Collection);
                    CollectionListUC userControl = new CollectionListUC(new CollectionListParametersDriverVM()
                    {
                        ParentPage = this,
                        ParentLibrary = _parameters.ParentLibrary,
                        ViewModelList = itemList?.OrderBy(o => o.Name).ToList(), //ViewModelPage.ContactViewModelList,
                    });

                    userControl.CancelModificationRequested += CollectionListUC_CancelModificationRequested;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.ViewModelPage.Glyph,
                        Title = userControl.ViewModelPage.Header,
                        IdItem = userControl.IdItem,
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
    }
}
