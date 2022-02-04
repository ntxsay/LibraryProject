using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views.Categories;
using LibraryProjectUWP.Views.Library;
using LibraryProjectUWP.Views.Library.Collection;
using LibraryProjectUWP.Views.Library.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LibraryProjectUWP.ViewModels.UI
{
    internal class LibraryCollectionCommonViewVM
    {
        readonly LibraryCollectionPage _parentPage;
        readonly LibraryCollectionGridViewPage _libraryCollectionGridViewPage;
        readonly LibraryCollectionDataGridViewPage _libraryCollectionDataGridViewPage;

        public LibraryCollectionCommonViewVM(LibraryCollectionGridViewPage libraryCollectionGridViewPage, LibraryCollectionPage parentPage)
        {
            _libraryCollectionGridViewPage = libraryCollectionGridViewPage;
            _parentPage = parentPage;
        }

        public LibraryCollectionCommonViewVM(LibraryCollectionDataGridViewPage libraryCollectionDataGridViewPage, LibraryCollectionPage parentPage)
        {
            _libraryCollectionDataGridViewPage = libraryCollectionDataGridViewPage;
            _parentPage = parentPage;
        }

        internal async Task ExportLibraryAsync(BibliothequeVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
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
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        internal void EditLibrary(BibliothequeVM viewModel)
        {
            //try
            //{
            //    NewEditLibraryUC userControl = new NewEditLibraryUC(new ManageLibraryDialogParametersVM()
            //    {
            //        CurrentLibrary = viewModel,
            //        EditMode = Code.EditMode.Edit,
            //        ViewModelList = _parentPage.ViewModelPage.ViewModelList,
            //    });

            //    userControl.CancelModificationRequested += NewEditLibraryUC_CancelModificationRequested;
            //    userControl.UpdateItemRequested += NewEditLibraryUC_UpdateItemRequested;

            //    if (_libraryCollectionDataGridViewPage != null)
            //    {
            //        _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = userControl;
            //        _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = true;
            //    }
            //    else if (_libraryCollectionGridViewPage != null)
            //    {
            //        _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = userControl;
            //        _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MethodBase m = MethodBase.GetCurrentMethod();
            //    Logs.Log(ex, m);
            //    return;
            //}
        }

        private async void NewEditLibraryUC_UpdateItemRequested(NewEditLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender._parameters != null)
                {
                    var newValue = sender.ViewModelPage.Value?.Trim();
                    var newDescription = sender.ViewModelPage.Description?.Trim();

                    var updatedViewModel = new BibliothequeVM()
                    {
                        Id = sender._parameters.CurrentLibrary.Id,
                        Name = newValue,
                        Description = newDescription,
                        DateEdition = DateTime.UtcNow,
                    };

                    var updateResult = await DbServices.Library.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender._parameters.CurrentLibrary.Name = newValue;
                        sender._parameters.CurrentLibrary.Description = newDescription;
                    }
                    else
                    {
                        //Erreur
                        sender.ViewModelPage.ErrorMessage = updateResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditLibraryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditLibraryUC_UpdateItemRequested;
                
                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = null;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = null;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewEditLibraryUC_CancelModificationRequested(NewEditLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= NewEditLibraryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditLibraryUC_UpdateItemRequested;

                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = null;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = null;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #region Delete Library
        internal void DeleteLibrary(BibliothequeVM viewModel)
        {
            try
            {
                DeleteLibraryUC userControl = new DeleteLibraryUC(viewModel);

                userControl.CancelModificationRequested += DeleteLibraryUC_CancelModificationRequested; ;
                userControl.DeleteLibraryWithOutSaveRequested += DeleteLibraryUC_DeleteLibraryWithOutSaveRequested;
                userControl.DeleteLibraryWithSaveRequested += DeleteLibraryUC_DeleteLibraryWithSaveRequested;

                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = userControl;
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = userControl;
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void DeleteLibraryUC_DeleteLibraryWithSaveRequested(DeleteLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (sender.ViewModelPage.ViewModel != null)
                {
                    await ExportLibraryAsync(sender.ViewModelPage.ViewModel);
                }

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DeleteLibraryUC_DeleteLibraryWithOutSaveRequested(DeleteLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void DeleteLibraryUC_CancelModificationRequested(DeleteLibraryUC sender, ExecuteRequestedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                sender.CancelModificationRequested -= DeleteLibraryUC_CancelModificationRequested;
                sender.DeleteLibraryWithOutSaveRequested -= DeleteLibraryUC_DeleteLibraryWithOutSaveRequested;
                sender.DeleteLibraryWithSaveRequested -= DeleteLibraryUC_DeleteLibraryWithSaveRequested;

                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = null;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = null;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        } 
        #endregion

        public void PopulateCategoriesMenuItems(MenuFlyoutSubItem subItem, BibliothequeVM viewModel)
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

                AddCategoryMenuItem.Click += (BibliothequeVM, e) => AddCategoryMenuItem_Click(viewModel, e);
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
                        AddSubCategoryMenuItem.Click += (CategorieLivreVM, e) => AddSubCategoryMenuItem_Click(category, e);
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

                //if (_libraryCollectionDataGridViewPage != null)
                //{
                //    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = userControl;
                //    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                //}
                //else if (_libraryCollectionGridViewPage != null)
                //{
                //    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = userControl;
                //    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                //}
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

                //if (_libraryCollectionDataGridViewPage != null)
                //{
                //    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = userControl;
                //    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                //}
                //else if (_libraryCollectionGridViewPage != null)
                //{
                //    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = userControl;
                //    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                //}
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
                        //sender.ViewModelPage.ErrorMessage = creationResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditCategoryUC_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditCategoryUC_CreateItemRequested;

                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = null;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = null;
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
                        //sender.ViewModelPage.ErrorMessage = updateResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditCategoryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditCategoryUC_UpdateItemRequested;

                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = null;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = null;
                }
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

                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = null;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = null;
                }
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

                //if (_libraryCollectionDataGridViewPage != null)
                //{
                //    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = userControl;
                //    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                //}
                //else if (_libraryCollectionGridViewPage != null)
                //{
                //    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = userControl;
                //    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                //}
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

                //if (_libraryCollectionDataGridViewPage != null)
                //{
                //    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = userControl;
                //    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                //}
                //else if (_libraryCollectionGridViewPage != null)
                //{
                //    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = userControl;
                //    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = true;
                //}
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
                        //sender.ViewModelPage.ErrorMessage = creationResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditSubCategoryUC_CancelModificationRequested;
                sender.CreateItemRequested -= NewEditSubCategoryUC_CreateItemRequested;

                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = null;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = null;
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

                    var updateResult = await DbServices.SubCategorie.UpdateAsync(updatedViewModel);
                    if (updateResult.IsSuccess)
                    {
                        sender._subCategorieParameters.CurrentSubCategorie.Name = newValue;
                        sender._subCategorieParameters.CurrentSubCategorie.Description = newDescription;
                    }
                    else
                    {
                        //Erreur
                        //sender.ViewModelPage.ErrorMessage = updateResult.Message;
                        return;
                    }
                }

                sender.CancelModificationRequested -= NewEditSubCategoryUC_CancelModificationRequested;
                sender.UpdateItemRequested -= NewEditSubCategoryUC_UpdateItemRequested;

                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = null;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = null;
                }
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

                if (_libraryCollectionDataGridViewPage != null)
                {
                    _libraryCollectionDataGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionDataGridViewPage.ViewModelPage.SplitViewContent = null;
                }
                else if (_libraryCollectionGridViewPage != null)
                {
                    _libraryCollectionGridViewPage.ViewModelPage.IsSplitViewOpen = false;
                    _libraryCollectionGridViewPage.ViewModelPage.SplitViewContent = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
