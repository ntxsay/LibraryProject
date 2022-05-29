using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Categorie;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Categories;
using LibraryProjectUWP.Views.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LibraryProjectUWP.Views.PrincipalPages
{
    public sealed partial class BookCollectionPage : Page
    {
        #region Events Categorie
        private void DisplayCategoriesList()
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
                    CategoriesListUC userControl = new CategoriesListUC(new CategorieParameterDriverVM()
                    {
                        BookPage = this,
                        ParentLibrary = Parameters.ParentLibrary,
                    });

                    userControl.CancelModificationRequested += CategoriesListUC_CancelModificationRequested;

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

        private void CategoriesListUC_CancelModificationRequested(CategoriesListUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= CategoriesListUC_CancelModificationRequested;

                this.RemoveItemToSideBar(sender);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        [Obsolete]
        private async Task AddBookToCategorie(CategoriesListUC categoriesListUC, IEnumerable<long> idBooks, CategorieLivreVM selectedCategorie, SubCategorieLivreVM selectedSubCategorie = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var creationResult = await DbServices.Categorie.CreateCategorieConnectorAsync(idBooks, selectedCategorie, selectedSubCategorie);
                if (creationResult.IsSuccess)
                {
                    categoriesListUC.ViewModelPage.ResultMessageTitle = "Succès";
                    categoriesListUC.ViewModelPage.ResultMessage = creationResult.Message;
                    categoriesListUC.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
                    categoriesListUC.ViewModelPage.IsResultMessageOpen = true;

                    await UpdateLibraryCategoriesAsync();
                    //if (selectedSubCategorie != null)
                    //{
                    //    selectedSubCategorie.BooksId = (await DbServices.Categorie.GetBooksIdInSubCategorie(selectedSubCategorie.Id)).ToList();
                    //}
                    //else
                    //{
                    //    selectedCategorie.BooksId = (await DbServices.Categorie.GetBooksIdInCategorie(selectedCategorie.Id)).ToList();
                    //}

                }
                else
                {
                    //Erreur
                    categoriesListUC.ViewModelPage.ResultMessageTitle = "Une erreur s'est produite";
                    categoriesListUC.ViewModelPage.ResultMessage = creationResult.Message;
                    categoriesListUC.ViewModelPage.ResultMessageSeverity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                    categoriesListUC.ViewModelPage.IsResultMessageOpen = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task UpdateLibraryCategoriesAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (Parameters.ParentLibrary != null)
                {
                    Parameters.ParentLibrary.CountUnCategorizedBooks = await DbServices.Categorie.CountUnCategorizedBooks(Parameters.ParentLibrary.Id);
                    if (Parameters.ParentLibrary.Categories.Any())
                    {
                        Parameters.ParentLibrary.Categories.Clear();
                        var categorieList = await DbServices.Categorie.MultipleVmAsync(Parameters.ParentLibrary.Id);
                        if (categorieList != null && categorieList.Any())
                        {
                            foreach (var category in categorieList)
                            {
                                Parameters.ParentLibrary.Categories.Add(category);
                            }

                            await DbServices.Categorie.AddSubCategoriesToCategoriesVmAsync(Parameters.ParentLibrary.Categories);
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
    }
}
