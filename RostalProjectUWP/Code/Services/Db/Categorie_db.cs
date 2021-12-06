using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.Code.Services.Logging;
using RostalProjectUWP.Models.Local;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;

namespace RostalProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        public struct Categorie
        {
            static string NameEmptyMessage = "Le nom de la catégorie doit être renseigné avant l'enregistrement.";
            static string NameAlreadyExistMessage = "Cette catégorie existe déjà.";
            #region All
            /// <summary>
            /// Retourne tous les objets de la base de données
            /// </summary>
            /// <typeparam name="T">Modèle de base de données</typeparam>
            /// <returns></returns>
            public static async Task<IList<TlibraryCategorie>> AllAsync()
            {
                try
                {
                    RostalDbContext context = new RostalDbContext();

                    var collection = await context.TlibraryCategorie.ToListAsync();
                    if (collection == null || !collection.Any()) return Enumerable.Empty<TlibraryCategorie>().ToList();

                    return collection;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<TlibraryCategorie>().ToList();
                }
            }

            /// <summary>
            /// Retourne tous les modèles de vue depuis la base de données
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <returns></returns>
            public static async Task<IList<CategorieLivreVM>> AllVMAsync()
            {
                try
                {
                    var collection = await Categorie.AllAsync();
                    if (!collection.Any()) return Enumerable.Empty<CategorieLivreVM>().ToList();

                    var values = collection.Select(async s => await Categorie.ViewModelConverterAsync(s)).Select(t => t.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<CategorieLivreVM>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdAsync()
            {
                try
                {
                    RostalDbContext context = new RostalDbContext();
                    return await context.TlibraryCategorie.Select(s => s.Id).ToListAsync();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<long>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdAsync(long idLibrary)
            {
                try
                {
                    RostalDbContext context = new RostalDbContext();
                    return await context.TlibraryCategorie.Where(w => w.IdLibrary == idLibrary).Select(s => s.Id).ToListAsync();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<long>().ToList();
                }
            }
            #endregion

            #region Multiple
            public static async Task<IList<TlibraryCategorie>> MultipleAsync(long idLibrary)
            {
                try
                {
                    RostalDbContext context = new RostalDbContext();

                    var collection = await context.TlibraryCategorie.Where(w => w.IdLibrary == idLibrary).ToListAsync();
                    if (collection == null || !collection.Any()) return Enumerable.Empty<TlibraryCategorie>().ToList();

                    return collection;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<TlibraryCategorie>().ToList();
                }
            }

            public static async Task<IList<CategorieLivreVM>> MultipleVmAsync(long idLibrary)
            {
                try
                {
                    var collection = await Categorie.MultipleAsync(idLibrary);
                    if (!collection.Any()) return Enumerable.Empty<CategorieLivreVM>().ToList();

                    var values = collection.Select(async s => await Categorie.ViewModelConverterAsync(s)).Select(t => t.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<CategorieLivreVM>().ToList();
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
            public static async Task<TlibraryCategorie> SingleAsync(long id)
            {
                try
                {
                    RostalDbContext context = new RostalDbContext();

                    var s = await context.TlibraryCategorie.SingleOrDefaultAsync(d => d.Id == id);
                    if (s == null) return null;

                    return s;
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
            public static async Task<CategorieLivreVM> SingleVMAsync(long id)
            {
                return await Categorie.ViewModelConverterAsync(await Categorie.SingleAsync(id));
            }
            #endregion

            public static async Task<OperationStateVM> CreateAsync(CategorieLivreVM viewModel)
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

                    RostalDbContext context = new RostalDbContext();
                    
                    var isExist = await context.TlibraryCategorie.AnyAsync(c => c.IdLibrary == viewModel.IdLibrary && c.Name.ToLower() == viewModel.Name.Trim().ToLower());
                    if (isExist)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Message = DbServices.RecordAlreadyExistMessage
                        };
                    }

                    var record = new TlibraryCategorie()
                    {
                        IdLibrary = viewModel.IdLibrary,
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                    };

                    await context.TlibraryCategorie.AddAsync(record);
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
                    Debug.WriteLine(Logs.GetLog(ex, m));
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
            public static async Task<OperationStateVM> UpdateAsync(CategorieLivreVM viewModel)
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

                    RostalDbContext context = new RostalDbContext();

                    var record = await context.TlibraryCategorie.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                    if (record == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage,
                        };
                    }

                    var isExist = await context.TlibraryCategorie.AnyAsync(c => c.Id != record.Id && c.IdLibrary == viewModel.IdLibrary && c.Name.ToLower() == viewModel.Name.Trim().ToLower());
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

                    context.TlibraryCategorie.Update(record);
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
                    RostalDbContext context = new RostalDbContext();

                    var record = await context.TlibraryCategorie.SingleOrDefaultAsync(a => a.Id == Id);
                    if (record == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage
                        };
                    }

                    context.TlibraryCategorie.Remove(record);
                    await context.SaveChangesAsync();

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

            #region Helpers

            /// <summary>
            /// Convertit un modèle en modèle de vue
            /// </summary>
            /// <typeparam name="T1">Type d'entrée</typeparam>
            /// <typeparam name="T2">Type sortie</typeparam>
            /// <param name="model">Modèle de base de données</param>
            /// <returns>Un modèle de vue</returns>
            private static async Task<CategorieLivreVM> ViewModelConverterAsync(TlibraryCategorie model)
            {
                try
                {
                    if (model == null) return null;

                    var subCategoriesList = await SubCategorie.MultipleVmAsync(model.Id);

                    var viewModel = new CategorieLivreVM()
                    {
                        Id = model.Id,
                        IdLibrary = model.IdLibrary,
                        Description = model.Description,
                        Name = model.Name,
                        SubCategorieLivres = subCategoriesList != null && subCategoriesList.Any() ? new ObservableCollection<SubCategorieLivreVM>(subCategoriesList) : new ObservableCollection<SubCategorieLivreVM>(),
                    };

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
