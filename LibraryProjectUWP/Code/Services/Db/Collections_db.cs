using LibraryProjectUWP.Code.Helpers;
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
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Services.Db
{
    internal partial class DbServices
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
                    LibraryDbContext context = new LibraryDbContext();

                    var collection = await context.Tcollection.ToListAsync();
                    if (collection == null || !collection.Any()) return Enumerable.Empty<Tcollection>().ToList();

                    return collection;
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

                    var values = collection.Select(s => ViewModelConverterAsync(s)).ToList();
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
                    LibraryDbContext context = new LibraryDbContext();
                    return await context.Tcollection.Select(s => s.Id).ToListAsync();
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
                    LibraryDbContext context = new LibraryDbContext();
                    return await context.TbookCollectionConnector.Where(w => w.IdBook == idBook).Select(s => s.IdCollection).ToListAsync();
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
                    LibraryDbContext context = new LibraryDbContext();
                    return await context.Tcollection.Where(w => w.IdLibrary == idLibrary).Select(s => s.Id).ToListAsync();
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
            public static async Task<IList<Tcollection>> MultipleInBookAsync(long idBook, CollectionTypeEnum collectionType = CollectionTypeEnum.All)
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();

                    var preCollection = await context.TbookCollectionConnector.Where(w => w.IdBook == idBook).ToListAsync();
                    if (preCollection.Any())
                    {
                        List<Tcollection> collection = new List<Tcollection>();
                        foreach (TbookCollectionConnector driver in preCollection)
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

                    return Enumerable.Empty<Tcollection>().ToList();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tcollection>().ToList();
                }
            }

            public static async Task<IList<CollectionVM>> MultipleVmInBookAsync(long idBook, CollectionTypeEnum collectionType = CollectionTypeEnum.All)
            {
                try
                {
                    var collection = await MultipleInBookAsync(idBook, collectionType);
                    if (!collection.Any()) return Enumerable.Empty<CollectionVM>().ToList();

                    var values = collection.Select(s => ViewModelConverterAsync(s)).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<CollectionVM>().ToList();
                }
            }

            public static async Task<IList<Tcollection>> MultipleInLibraryAsync(long idLibrary, CollectionTypeEnum collectionType = CollectionTypeEnum.All)
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();
                    List<Tcollection> collection = null;
                    if (collectionType == CollectionTypeEnum.All)
                    {
                        collection = await context.Tcollection.Where(w => w.IdLibrary == idLibrary).ToListAsync();
                    }
                    else
                    {
                        collection = await context.Tcollection.Where(w => w.IdLibrary == idLibrary && w.CollectionType == (long)collectionType).ToListAsync();
                    }
                    
                    return collection ?? Enumerable.Empty<Tcollection>().ToList();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tcollection>().ToList();
                }
            }

            public static async Task<IList<CollectionVM>> MultipleVmInLibraryAsync(long idLibrary, CollectionTypeEnum collectionType = CollectionTypeEnum.All)
            {
                try
                {
                    var collection = await MultipleInLibraryAsync(idLibrary, collectionType);
                    if (!collection.Any()) return Enumerable.Empty<CollectionVM>().ToList();

                    var values = collection.Select(s => ViewModelConverterAsync(s)).ToList();
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
                    LibraryDbContext context = new LibraryDbContext();

                    var s = await context.Tcollection.SingleOrDefaultAsync(d => d.Id == id);
                    if (s == null) return null;

                    return s;
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
                return ViewModelConverterAsync(await SingleAsync(id));
            }
            #endregion

            public static async Task<OperationStateVM> CreateAsync(CollectionVM viewModel)
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

                    LibraryDbContext context = new LibraryDbContext();

                    var isExist = await context.Tcollection.AnyAsync(c => c.Name.ToLower() == viewModel.Name.Trim().ToLower());
                    if (isExist)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Message = DbServices.RecordAlreadyExistMessage
                        };
                    }

                    var record = new Tcollection()
                    {
                        IdLibrary = viewModel.IdLibrary,
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                    };

                    await context.Tcollection.AddAsync(record);
                    await context.SaveChangesAsync();

                    if (viewModel.IdBook != -1)
                    {
                        var recordConnector = new TbookCollectionConnector()
                        {
                            IdBook = viewModel.IdBook,
                            IdCollection = record.Id,
                        };

                        await context.TbookCollectionConnector.AddAsync(recordConnector);
                        await context.SaveChangesAsync();
                    }

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Id = record.Id,
                        Message = $"La collection \"{viewModel.Name}\" a été créé avec succès."
                    };
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

                    LibraryDbContext context = new LibraryDbContext();

                    var record = await context.Tcollection.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                    if (record == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage,
                        };
                    }

                    var isExist = await context.Tcollection.AnyAsync(c => c.Id != record.Id && c.Name.ToLower() == viewModel.Name.Trim().ToLower());
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

                    context.Tcollection.Update(record);
                    await context.SaveChangesAsync();

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Id = record.Id,
                    };
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

                    List<TbookCollectionConnector> driverCollection = await context.TbookCollectionConnector.Where(w => w.IdCollection == Id).ToListAsync();
                    if (driverCollection.Any())
                    {
                        context.TbookCollectionConnector.RemoveRange(driverCollection);
                        //await context.SaveChangesAsync();
                    }

                    context.Tcollection.Remove(record);
                    await context.SaveChangesAsync();

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                    };
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

            /// <summary>
            /// Convertit un modèle en modèle de vue
            /// </summary>
            /// <typeparam name="T1">Type d'entrée</typeparam>
            /// <typeparam name="T2">Type sortie</typeparam>
            /// <param name="model">Modèle de base de données</param>
            /// <returns>Un modèle de vue</returns>
            private static CollectionVM ViewModelConverterAsync(Tcollection model)
            {
                try
                {
                    if (model == null) return null;

                    var viewModel = new CollectionVM()
                    {
                        Id = model.Id,
                        Description = model.Description,
                        Name = model.Name,
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
            #endregion
        }
    }

}
