﻿using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Services.Db
{
    public partial class DbServices
    {
        public struct Collection
        {
            static string NameEmptyMessage = "Le nom de la collection doit être renseigné avant l'enregistrement.";
            static string NameAlreadyExistMessage = "Cette collection existe déjà.";
            #region All
            /// <summary>
            /// Retourne tous les objets de la base de données
            /// </summary>
            /// <typeparam name="T">Modèle de base de données</typeparam>
            /// <returns></returns>
            public static async Task<IList<Tcollection>> AllAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var collection = await context.Tcollection.ToListAsync();
                        if (collection == null || !collection.Any()) return Enumerable.Empty<Tcollection>().ToList();

                        return collection;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tcollection>().ToList();
                }
            }

            /// <summary>
            /// Retourne tous les modèles de vue depuis la base de données
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <returns></returns>
            public static async Task<IList<CollectionVM>> AllVMAsync()
            {
                try
                {
                    var collection = await AllAsync();
                    if (!collection.Any()) return Enumerable.Empty<CollectionVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(d => d.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<CollectionVM>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.Tcollection.Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<long>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdInBookAsync(long idBook)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TbookCollections.Where(w => w.IdBook == idBook).Select(s => s.IdCollection).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<long>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdInLibraryAsync(long idLibrary)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.Tcollection.Where(w => w.IdLibrary == idLibrary).Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<long>().ToList();
                }
            }
            #endregion

            #region Multiple
            public static async Task<IList<Tcollection>> GetCollectionsInBookAsync(long idBook, CollectionTypeEnum collectionType = CollectionTypeEnum.All)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var preCollection = await context.TbookCollections.Where(w => w.IdBook == idBook).ToListAsync();
                        if (preCollection.Any())
                        {
                            List<Tcollection> collection = new List<Tcollection>();
                            foreach (TbookCollections driver in preCollection)
                            {
                                if (collectionType == CollectionTypeEnum.All)
                                {
                                    Tcollection model = await context.Tcollection.SingleOrDefaultAsync(w => w.Id == driver.IdCollection);
                                    if (model != null)
                                    {
                                        collection.Add(model);
                                    }
                                }
                                else
                                {
                                    Tcollection model = await context.Tcollection.SingleOrDefaultAsync(w => w.Id == driver.IdCollection && w.CollectionType == (long)collectionType);
                                    if (model != null)
                                    {
                                        collection.Add(model);
                                    }
                                }
                            }

                            return collection;
                        }
                    }
                    
                    return Enumerable.Empty<Tcollection>().ToList();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tcollection>().ToList();
                }
            }

            public static async Task<IList<CollectionVM>> GetCollectionsVmInBookAsync(long idBook, CollectionTypeEnum collectionType = CollectionTypeEnum.All)
            {
                try
                {
                    var collection = await GetCollectionsInBookAsync(idBook, collectionType);
                    if (!collection.Any()) return Enumerable.Empty<CollectionVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(d => d.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<CollectionVM>().ToList();
                }
            }

            public static async Task<IList<Tcollection>> MultipleInLibraryAsync(long idLibrary, CollectionTypeEnum collectionType = CollectionTypeEnum.All, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tcollection> collection = null;
                        if (collectionType == CollectionTypeEnum.All)
                        {
                            collection = await context.Tcollection.Where(w => w.IdLibrary == idLibrary).ToListAsync(cancellationToken);
                        }
                        else
                        {
                            collection = await context.Tcollection.Where(w => w.IdLibrary == idLibrary && w.CollectionType == (long)collectionType).ToListAsync(cancellationToken);
                        }

                        return collection ?? Enumerable.Empty<Tcollection>().ToList();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tcollection>().ToList();
                }
            }

            public static async Task<IList<CollectionVM>> MultipleVmInLibraryAsync(long idLibrary, CollectionTypeEnum collectionType = CollectionTypeEnum.All, CancellationToken cancellationToken = default)
            {
                try
                {
                    var collection = await MultipleInLibraryAsync(idLibrary, collectionType, cancellationToken);
                    if (!collection.Any()) return Enumerable.Empty<CollectionVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(d => d.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<CollectionVM>().ToList();
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
            public static async Task<Tcollection> SingleAsync(long id)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var s = await context.Tcollection.SingleOrDefaultAsync(d => d.Id == id);
                        if (s == null) return null;

                        return s;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
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
            public static async Task<CollectionVM> SingleVMAsync(long id)
            {
                return await ViewModelConverterAsync(await SingleAsync(id));
            }
            #endregion

#warning Cette méthode nuit considérablement aux performances lorsqu'il y a beaucoup de collection
            public static async Task<long> CountUnCategorizedBooks(long idLibrary)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var collections = await context.Tbook.Where(d => d.IdLibrary == idLibrary && d.TbookCollections.Count == 0).ToListAsync();
                        return collections.Distinct().ToList().Count;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return 0;
                }
            }

                //public static async Task<IList<long>> GetUnCategorizedBooksId(long idLibrary)
                //{
                //    try
                //    {
                //        using (LibraryDbContext context = new LibraryDbContext())
                //        {
                //            return await context.TbookCollectionConnector.Where(d => d.id == idLibrary && d.IdCategorie == null && d.IdSubCategorie == null).Select(s => s.IdBook).ToListAsync();
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        MethodBase m = MethodBase.GetCurrentMethod();
                //        Logs.Log(ex, m);
                //        return new List<long>();
                //    }
                //}

                public static async Task<IList<long>> GetBooksIdInCollectionAsync(long idCollection)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TbookCollections.Where(d => d.IdCollection == idCollection).Select(s => s.IdBook).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return new List<long>();
                }
            }

            public static async Task<long> CountBooksInCollectionAsync(long idCollection)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TbookCollections.CountAsync(d => d.IdCollection == idCollection);
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return 0;
                }
            }

            public static IEnumerable<CollectionVM> CreateViewModel(long idLibrary, string value, char separator = ',')
            {
                try
                {
                    if (!value.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var splittedValue = StringHelpers.SplitWord(value, new string[] { separator.ToString() });
                        if (splittedValue != null && splittedValue.Length > 0)
                        {
                            List<CollectionVM> viewModelList = new List<CollectionVM>();
                            foreach (var _value in splittedValue)
                            {
                                CollectionVM collectionVm = new CollectionVM()
                                {
                                    IdLibrary = idLibrary,
                                    Name = _value,
                                };
                                viewModelList.Add(collectionVm);
                            }

                            return viewModelList;
                        }

                    }
                    return Enumerable.Empty<CollectionVM>();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<CollectionVM>();
                }
            }


            public static async Task<OperationStateVM> CreateCollectionConnectorAsync(IEnumerable<long> idBooks, CollectionVM viewModel)
            {
                try
                {
                    if (viewModel == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage,
                        };
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<long> validIdList = new List<long>();

                        foreach (var idBook in idBooks)
                        {
                            var isBookExist = await context.Tbook.AnyAsync(a => a.Id == idBook);
                            if (!isBookExist)
                            {
                                continue;
                            }
                            validIdList.Add(idBook);
                        }

                        if (!validIdList.Any())
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
                            };
                        }

                        var isCollectionExist = await context.Tcollection.AnyAsync(a => a.Id == viewModel.Id);
                        if (!isCollectionExist)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = RecordNotExistMessage,
                            };
                        }

                        
                        foreach (var idBook in validIdList)
                        {
                            var bookConnector = await context.TbookCollections.SingleOrDefaultAsync(c => c.IdBook == idBook);
                            if (bookConnector != null)
                            {
                                bookConnector.IdCollection = viewModel.Id;

                                context.TbookCollections.Update(bookConnector);
                                await context.SaveChangesAsync();
                            }
                            else
                            {
                                TbookCollections tbookCollectionConnector = new TbookCollections()
                                {
                                    IdBook = idBook,
                                    IdCollection = viewModel.Id,
                                };

                                await context.TbookCollections.AddAsync(tbookCollectionConnector);
                                await context.SaveChangesAsync();
                            }
                        }

                        return new OperationStateVM()
                        {
                            Message = $"Les livres ont été ajoutés à la collection : {viewModel.Name}.",
                            IsSuccess = true,
                        };
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            public static async Task<OperationStateVM> DecategorizeBooksAsync(IEnumerable<long> idBooks)
            {
                try
                {
                    if (idBooks == null || !idBooks.Any())
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage,
                        };
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<long> validIdList = new List<long>();

                        foreach (var idBook in idBooks)
                        {
                            var isBookExist = await context.Tbook.AnyAsync(a => a.Id == idBook);
                            if (!isBookExist)
                            {
                                continue;
                            }
                            validIdList.Add(idBook);
                        }

                        if (!validIdList.Any())
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
                            };
                        }

                        foreach (var idBook in validIdList)
                        {
                            var bookConnector = await context.TbookCollections.SingleOrDefaultAsync(c => c.IdBook == idBook);
                            if (bookConnector == null)
                            {
                                continue;
                            }

                            context.TbookCollections.Remove(bookConnector);
                            await context.SaveChangesAsync();
                        }


                        return new OperationStateVM()
                        {
                            Message = $"Les livres ont été décatégorisé avec succès.",
                            IsSuccess = true,
                        };
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            public static async Task<OperationStateVM> CreateAsync(CollectionVM viewModel, long idLibrary)
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

                    if (idLibrary <= 0)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = "L'id de la bibliothèque n'est pas valide",
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
                        var isExist = await IsExistAsync(idLibrary,viewModel);
                        if (isExist)
                        {
                            Logs.Log(m, $"La collection \"{viewModel.Name}\" existe déjà.");
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = $"La collection \"{viewModel.Name}\" existe déjà."
                            };
                        }

                        var record = new Tcollection()
                        {
                            IdLibrary = idLibrary,
                            Name = viewModel.Name,
                            Description = viewModel.Description,
                        };

                        await context.Tcollection.AddAsync(record);
                        await context.SaveChangesAsync();

                        if (viewModel.IdBook != -1)
                        {
                            var recordConnector = new TbookCollections()
                            {
                                IdBook = viewModel.IdBook,
                                IdCollection = record.Id,
                            };

                            await context.TbookCollections.AddAsync(recordConnector);
                            await context.SaveChangesAsync();
                        }

                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Id = record.Id,
                            Message = $"La collection \"{viewModel.Name}\" a été créé avec succès."
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

            /// <summary>
            /// Met à jour un élément existant dans la base de données
            /// </summary>
            /// <typeparam name="T">Type d'entrée (Modèle de vue)</typeparam>
            /// <param name="viewModel">Modèle de vue</param>
            /// <returns></returns>
            public static async Task<OperationStateVM> UpdateAsync(CollectionVM viewModel)
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
                        var record = await context.Tcollection.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                        if (record == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
                            };
                        }

                        var isExist = await IsExistAsync(record.IdLibrary, viewModel, true, record.Id);
                        if (isExist)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = NameAlreadyExistMessage,
                                Id = record.Id
                            };
                        }

                        record.Name = viewModel.Name;
                        record.Description = viewModel.Description;

                        context.Tcollection.Update(record);
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
                    Logs.Log(ex, m);

                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            public static async Task<IEnumerable<OperationStateVM>> DeleteAsync(long[] ids)
            {
                try
                {
                    List<OperationStateVM> result = new List<OperationStateVM>();
                    foreach (var id in ids)
                    {
                        result.Add(await DeleteAsync(id));
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);

                    return new List<OperationStateVM>()
                    {
                        new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = $"Exception : {ex.Message}",
                        }
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
                    LibraryDbContext context = new LibraryDbContext();

                    Tcollection record = await context.Tcollection.SingleOrDefaultAsync(a => a.Id == Id);
                    if (record == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage
                        };
                    }

                    List<TbookCollections> driverCollection = await context.TbookCollections.Where(w => w.IdCollection == Id).ToListAsync();
                    if (driverCollection.Any())
                    {
                        context.TbookCollections.RemoveRange(driverCollection);
                        //await context.SaveChangesAsync();
                    }

                    context.Tcollection.Remove(record);
                    await context.SaveChangesAsync();
                    driverCollection = null;

                    var result = new OperationStateVM()
                    {
                        IsSuccess = true,
                        Message = $"La collection \"{record.Name?.ToString()}\" a été supprimée avec succès."
                    };

                    record = null;
                    return result;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);

                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            #region Helpers
            private static async Task<bool> IsExistAsync(long idLibrary, CollectionVM viewModel, bool isEdit = false, long? modelId = null)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        string name = viewModel.Name?.Trim()?.ToLower();

                        if (!isEdit)
                        {
                            return await context.Tcollection.AnyAsync(c => c.IdLibrary == idLibrary && c.Name.ToLower() == name);
                        }
                        else
                        {
                            return await context.Tcollection.AnyAsync(c => c.IdLibrary == idLibrary && c.Id != (long)modelId && c.Name.ToLower() == name);
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


            /// <summary>
            /// Convertit un modèle en modèle de vue
            /// </summary>
            /// <typeparam name="T1">Type d'entrée</typeparam>
            /// <typeparam name="T2">Type sortie</typeparam>
            /// <param name="model">Modèle de base de données</param>
            /// <returns>Un modèle de vue</returns>
            public static async Task<CollectionVM> ViewModelConverterAsync(Tcollection model)
            {
                try
                {
                    if (model == null) return null;

                    var viewModel = new CollectionVM()
                    {
                        Id = model.Id,
                        Description = model.Description,
                        Name = model.Name,
                        BooksId = (await GetBooksIdInCollectionAsync(model.Id)).ToList()
                    };

                    return viewModel;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }

            public static CollectionVM DeepCopy(CollectionVM viewModelToCopy)
            {
                try
                {
                    if (viewModelToCopy == null) return null;

                    CollectionVM newViewModel = new CollectionVM();

                    return DeepCopy(newViewModel, viewModelToCopy);
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }

            public static CollectionVM DeepCopy(CollectionVM viewModel, CollectionVM viewModelToCopy)
            {
                try
                {
                    if (viewModel == null) return null;
                    if (viewModelToCopy == null) return null;

                    viewModel.Id = viewModelToCopy.Id;
                    viewModel.IdBook = viewModelToCopy.IdBook;
                    viewModel.IdLibrary = viewModelToCopy.IdLibrary;
                    viewModel.Description = viewModelToCopy.Description;
                    viewModel.Name = viewModelToCopy.Name;
                    viewModel.BooksId = viewModelToCopy.BooksId;

                    return viewModel;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }
            #endregion
        }
    }

}
