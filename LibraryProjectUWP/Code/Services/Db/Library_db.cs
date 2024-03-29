﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using Windows.Storage;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.ViewModels.Categorie;

namespace LibraryProjectUWP.Code.Services.Db
{
    public partial class DbServices
    {
        public partial struct Library
        {
            static string NameEmptyMessage = "Le nom de la bibliothque doit être renseigné avant l'enregistrement.";
            static string NameAlreadyExistMessage = "Cette bibliothèque existe déjà.";
            #region All
            /// <summary>
            /// Retourne tous les objets de la base de données
            /// </summary>
            /// <typeparam name="T">Modèle de base de données</typeparam>
            /// <returns></returns>
            public static async Task<IList<Tlibrary>> AllAsync()
            {
                using (LibraryDbContext context = new LibraryDbContext())
                {
                    try
                    {
                        var collection = await context.Tlibrary.ToListAsync();
                        if (collection == null || !collection.Any()) return Enumerable.Empty<Tlibrary>().ToList();

                        return collection;
                    }
                    catch (Exception ex)
                    {
                        MethodBase m = MethodBase.GetCurrentMethod();
                        Debug.WriteLine(Logs.GetLog(ex, m));
                        return Enumerable.Empty<Tlibrary>().ToList();
                    }
                }
            }

            /// <summary>
            /// Retourne tous les modèles de vue depuis la base de données
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <returns></returns>
            public static async Task<IList<BibliothequeVM>> AllVMAsync()
            {
                try
                {
                    var collection = await Library.AllAsync();
                    if (!collection.Any()) return Enumerable.Empty<BibliothequeVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(t => t.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<BibliothequeVM>().ToList();
                }
            }
            #endregion

            #region Single
            /// <summary>
            /// Retourne un élément de la base de données avec un identifiant unique
            /// </summary>
            /// <typeparam name="T">Type d'entrée et de sortie (Modèle)</typeparam>
            /// <param name="id">Identifiant unique</param>
            /// <returns></returns>
            public static async Task<Tlibrary> SingleAsync(long id)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var s = await context.Tlibrary.SingleOrDefaultAsync(d => d.Id == id);
                        if (s == null) return null;

                        return s;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return null;
                }
            }

            public static async Task<Tlibrary> SingleAsync(string name)
            {
                try
                {
                    if (name.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return null;
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var s = await context.Tlibrary.SingleOrDefaultAsync(d => d.Name.ToLower() == name.ToLower());
                        if (s == null) return null;

                        return s;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return null;
                }
            }

            /// <summary>
            /// Retourne un modèle de vue avec un identifiant unique
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <param name="id"></param>
            /// <returns></returns>
            public static async Task<BibliothequeVM> SingleVMAsync(long id)
            {
                return await Library.ViewModelConverterAsync(await Library.SingleAsync(id));
            }
            #endregion

            public static async Task<int> CountLibrariesAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.Tlibrary.CountAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return 0;
                }
            }

            public static async Task<IEnumerable<long>> GetIdLibrariesAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.Tlibrary.Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<long>();
                }
            }

            public static async Task<long> CountBookAsync(long idLibrary)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.Tbook.LongCountAsync(c => c.IdLibrary == idLibrary);
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return 0;
                }
            }

            public static async Task<OperationStateVM> CreateAsync(BibliothequeVM viewModel)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                try
                {
                    if (viewModel == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.ViewModelNullOrEmptyMessage,
                        };
                    }

                    if (viewModel.Name.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = NameEmptyMessage,
                        };
                    }

                    var isExist = await IsExistAsync(viewModel);
                    if (isExist)
                    {
                        Logs.Log(m, $"La bibliothèque \"{viewModel.Name}\" existe déjà.");
                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Message = DbServices.RecordAlreadyExistMessage,
                            Id = (await SingleAsync(viewModel.Name))?.Id ?? 0,
                        };
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var record = new Tlibrary()
                        {
                            Guid = viewModel.Guid.ToString(),
                            Name = viewModel.Name,
                            DateAjout = viewModel.DateAjout.ToString(),
                            Description = viewModel.Description,
                            DateEdition = null,
                        };

                        await context.Tlibrary.AddAsync(record);
                        await context.SaveChangesAsync();

                        await CreateFolderAsync(viewModel.Guid);

                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Id = record.Id,
                        };
                    }
                }
                catch (Exception ex)
                {
                    Logs.Log(ex, m);
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            internal static async Task CreateFolderAsync(Guid guid)
            {
                try
                {
                    if (guid == Guid.Empty)
                    {
                        return;
                    }

                    EsLibrary esLibrary = new EsLibrary();

                    var libraryFolder = await esLibrary.GetLibraryItemFolderAsync(guid);
                    if (libraryFolder == null)
                    {
                        return;
                    }

                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return;
                }
            }

            /// <summary>
            /// Met à jour un élément existant dans la base de données
            /// </summary>
            /// <typeparam name="T">Type d'entrée (Modèle de vue)</typeparam>
            /// <param name="viewModel">Modèle de vue</param>
            /// <returns></returns>
            public static async Task<OperationStateVM> UpdateAsync(BibliothequeVM viewModel)
            {
                try
                {
                    
                    if (viewModel == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.ViewModelNullOrEmptyMessage,
                        };
                    }

                    if (viewModel.Name.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = NameEmptyMessage,
                        };
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var record = await context.Tlibrary.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                        if (record == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
                            };
                        }

                        var isExist = await IsExistAsync(viewModel, true, record.Id);
                        if (isExist)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = NameAlreadyExistMessage
                            };
                        }

                        record.Name = viewModel.Name;
                        record.Description = viewModel.Description;
                        record.DateEdition = viewModel.DateEdition == null ? null : viewModel.DateEdition.ToString();

                        context.Tlibrary.Update(record);
                        await context.SaveChangesAsync();

                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Id = record.Id,
                        };
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));

                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            /// <summary>
            /// Supprime un élément de la base de données
            /// </summary>
            /// <typeparam name="T">Type d'entrée et de sortie (Modèle)</typeparam>
            /// <param name="Id"></param>
            /// <returns></returns>
            public static async Task<OperationStateVM> DeleteAsync(long Id)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var record = await context.Tlibrary.SingleOrDefaultAsync(a => a.Id == Id);
                        if (record == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage
                            };
                        }



                        context.Tlibrary.Remove(record);
                        await context.SaveChangesAsync();
                    }
                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                    };
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));

                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            private static async Task<bool> IsExistAsync(BibliothequeVM viewModel, bool isEdit = false, long? modelId = null)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        string name = viewModel.Name?.Trim()?.ToLower();

                        if (!isEdit)
                        {
                            return await context.Tlibrary.AnyAsync(c => c.Name.ToLower() == name);
                        }
                        else
                        {
                            return await context.Tlibrary.AnyAsync(c => c.Id != (long)modelId && c.Name.ToLower() == name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return true;
                }
            }


            #region Helpers

            /// <summary>
            /// Convertit un modèle en modèle de vue
            /// </summary>
            /// <typeparam name="T1">Type d'entrée</typeparam>
            /// <typeparam name="T2">Type sortie</typeparam>
            /// <param name="model">Modèle de base de données</param>
            /// <returns>Un modèle de vue</returns>
            private static async Task<BibliothequeVM> ViewModelConverterAsync(Tlibrary model)
            {
                try
                {
                    if (model == null) return null;

                    var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                    if (isGuidCorrect == false) return null;

                    IList<CategorieLivreVM> categoriesList = await Categorie.MultipleVmAsync(model.Id);
                    //IOrderedEnumerable<CollectionVM> collectionList = (await Collection.MultipleVmInLibraryAsync(model.Id))?.OrderBy(o => o.Name);
                    var countBooks = await Book.CountBooksInLibraryAsync(model.Id);
                    var countUnCategorizedBooks = await Categorie.CountUnCategorizedBooks(model.Id);
                    //var countNotInCollectionBooks = await Collection.CountUnCategorizedBooks(model.Id);

                    var viewModel = new BibliothequeVM()
                    {
                        Id = model.Id,
                        DateAjout = DatesHelpers.Converter.GetDateFromString(model.DateAjout),
                        DateEdition = DatesHelpers.Converter.GetNullableDateFromString(model.DateEdition),
                        Description = model.Description,
                        Name = model.Name,
                        Guid = isGuidCorrect ? guid : Guid.Empty,
                        Categories = categoriesList != null && categoriesList.Any() ? new ObservableCollection<CategorieLivreVM>(categoriesList) : new ObservableCollection<CategorieLivreVM>(),
                        CountUnCategorizedBooks = countUnCategorizedBooks,
                        CountBooks = countBooks,
                    };

                    //Collections = collectionList != null && collectionList.Any() ? new ObservableCollection<CollectionVM>(collectionList) : new ObservableCollection<CollectionVM>(),
                    //CountNotInCollectionBooks = countNotInCollectionBooks,


                    if (viewModel.Categories.Any())
                    {
                        await Categorie.AddSubCategoriesToCategoriesVmAsync(viewModel.Categories);
                    }

                    return viewModel;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return null;
                }
            }

            #endregion
        }
    }
}
