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
using RostalProjectUWP.Models.Local;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;

namespace RostalProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        #region All
        /// <summary>
        /// Retourne tous les objets de la base de données
        /// </summary>
        /// <typeparam name="T">Modèle de base de données</typeparam>
        /// <returns></returns>
        public static async Task<IList<T>> AllAsync<T>() where T : class
        {
            try
            {
                RostalDbContext context = new RostalDbContext();

                if (typeof(T) == typeof(Tlibrary))
                {
                    var collection = await context.Tlibrary.ToListAsync();
                    if (collection == null || collection.Count == 0) return Enumerable.Empty<T>().ToList();

                    var values = collection.Select(s => (T)(object)s).ToList();
                    return values;
                }
                else if (typeof(T) == typeof(TlibraryCategorie))
                {
                    var collection = await context.TlibraryCategorie.ToListAsync();
                    if (collection == null || collection.Count == 0) return Enumerable.Empty<T>().ToList();

                    var values = collection.Select(s => (T)(object)s).ToList();
                    return values;
                }
                else if (typeof(T) == typeof(TlibraryBookConnector))
                {
                    var collection = await context.TlibraryBookConnector.ToListAsync();
                    if (collection == null || collection.Count == 0) return Enumerable.Empty<T>().ToList();

                    var values = collection.Select(s => (T)(object)s).ToList();
                    return values;
                }
                else if (typeof(T) == typeof(Tbooks))
                {
                    var collection = await context.Tbooks.ToListAsync();
                    if (collection == null || collection.Count == 0) return Enumerable.Empty<T>().ToList();

                    var values = collection.Select(s => (T)(object)s).ToList();
                    return values;
                }
                

                return Enumerable.Empty<T>().ToList();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message} - {(ex.InnerException?.Message == null ? string.Empty : "Inner Exception : " + ex.InnerException?.Message) }");
                return Enumerable.Empty<T>().ToList();
            }
        }

        /// <summary>
        /// Retourne tous les modèles de vue depuis la base de données
        /// </summary>
        /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
        /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
        /// <returns></returns>
        public static async Task<IList<T2>> AllVMAsync<T1, T2>()
            where T1 : class
            where T2 : class
        {
            try
            {
                var collection = await AllAsync<T1>();
                if (!collection.Any()) return Enumerable.Empty<T2>().ToList();

                var values = collection.Select(async s => await ViewModelConverterAsync<T1, T2>(s)).Select(t => t.Result).ToList();
                return values;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<T2>().ToList();
            }
        }
        #endregion

        #region Multiple
        public static async Task<IList<long>> AllIdAsync<T>(long idParent) where T : class
        {
            try
            {
                RostalDbContext context = new RostalDbContext();

                if (typeof(T) == typeof(TlibraryCategorie))
                {
                    return await context.TlibraryCategorie.Where(w => w.IdLibrary == idParent).Select(s => s.Id).ToListAsync();
                }


                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<IList<T>> MultipleAsync<T>(long idParent) 
            where T : class
        {
            try
            {
                RostalDbContext context = new RostalDbContext();

                if (typeof(T) == typeof(TlibraryCategorie))
                {
                    var collection = await context.TlibraryCategorie.Where(w => w.IdLibrary == idParent).ToListAsync();
                    if (collection == null || !collection.Any()) return Enumerable.Empty<T>().ToList();

                    var values = collection.Select(s => (T)(object)s).ToList();
                    return values;
                }
                else if (typeof(T) == typeof(TlibrarySubCategorie))
                {
                    var collection = await context.TlibrarySubCategorie.Where(w => w.IdCategorie == idParent).ToListAsync();
                    if (collection == null || !collection.Any()) return Enumerable.Empty<T>().ToList();

                    var values = collection.Select(s => (T)(object)s).ToList();
                    return values;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<IList<T2>> MultipleVmAsync<T1, T2>(long idParent)
            where T1 : class
            where T2 : class
        {
            try
            {
                var collection = await MultipleAsync<T1>(idParent);
                if (!collection.Any()) return Enumerable.Empty<T2>().ToList();

                var values = collection.Select(async s => await ViewModelConverterAsync<T1, T2>(s)).Select(t => t.Result).ToList();
                return values;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<T2>().ToList();
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
        public static async Task<T> SingleAsync<T>(long id) where T : class
        {
            try
            {
                RostalDbContext context = new RostalDbContext();

                if (typeof(T) == typeof(Tlibrary))
                {
                    var s = await context.Tlibrary.SingleOrDefaultAsync(d => d.Id == id);
                    if (s == null) return null;

                    return (T)(object)s;
                }


                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        public static async Task<T2> SingleVMAsync<T1, T2>(long id)
            where T1 : class
            where T2 : class
        {
            try
            {
                return await ViewModelConverterAsync<T1, T2>(await SingleAsync<T1>(id));
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        public static async Task<OperationStateVM> CreateAsync<T1, T2>(T2 viewModel)
            where T1 : class
            where T2 : class
        {
            try
            {
                if (!IsViewModelTypeSupported<T2>())
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = DbServices.UnsupportedTParameter
                    };
                }

                if (viewModel == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = DbServices.ViewModelNullOrEmptyMessage,
                    };
                }

                var isExist = await IsExistAsync(viewModel);
                if (isExist)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Message = DbServices.RecordAlreadyExistMessage
                    };
                }

                RostalDbContext context = new RostalDbContext();

                if (typeof(T1) == typeof(Tlibrary))
                {
                    if (!(viewModel is BibliothequeVM convertedVm)) return DbServices.ViewModelEmpty;

                    var record = new Tlibrary()
                    {
                        Guid = convertedVm.Guid.ToString(),
                        Name = convertedVm.Name,
                        DateAjout = convertedVm.DateAjout.ToString(),
                        Description = convertedVm.Description,
                        DateEdition = null,
                    };

                    await context.Tlibrary.AddAsync(record);
                    await context.SaveChangesAsync();

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Id = record.Id,
                    };
                }
                else if (typeof(T1) == typeof(TlibraryCategorie))
                {
                    if (!(viewModel is CategorieLivreVM convertedVm)) return DbServices.ViewModelEmpty;
                    //if (IdLibrary == null) return DbServices.ParentIdEmpty;

                    var record = new TlibraryCategorie()
                    {
                        IdLibrary = convertedVm.IdLibrary,
                        Name = convertedVm.Name,
                        Description = convertedVm.Description,
                    };

                    await context.TlibraryCategorie.AddAsync(record);
                    await context.SaveChangesAsync();

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Id = record.Id,
                    };
                }
                else if (typeof(T1) == typeof(TlibrarySubCategorie))
                {
                    if (!(viewModel is SubCategorieLivreVM convertedVm)) return DbServices.ViewModelEmpty;
                    //if (IdCategorie == null) return DbServices.ParentIdEmpty;

                    var record = new TlibrarySubCategorie()
                    {
                        IdCategorie = convertedVm.IdCategorie,
                        Name = convertedVm.Name,
                        Description = convertedVm.Description,
                    };

                    await context.TlibrarySubCategorie.AddAsync(record);
                    await context.SaveChangesAsync();

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Id = record.Id,
                    };
                }


                return new OperationStateVM()
                {
                    IsSuccess = false,
                    Message = DbServices.UnsupportedTParameter
                };
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");
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
        public static async Task<OperationStateVM> UpdateAsync<T>(long id, T viewModel) where T : class
        {
            try
            {
                if (!IsViewModelTypeSupported<T>())
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = DbServices.UnsupportedTParameter
                    };
                }

                if (viewModel == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = DbServices.ViewModelNullOrEmptyMessage,
                    };
                }

                RostalDbContext context = new RostalDbContext();

                if (typeof(T) == typeof(BibliothequeVM))
                {
                    BibliothequeVM vm = (BibliothequeVM)(object)viewModel;
                    var record = await context.Tlibrary.SingleOrDefaultAsync(a => a.Id == id);
                    if (record == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage,
                        };
                    }

                    record.Name = vm.Name;
                    record.Description = vm.Description;

                    context.Tlibrary.Update(record);
                    await context.SaveChangesAsync();

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Id = record.Id,
                    };
                }
                

                return new OperationStateVM()
                {
                    IsSuccess = false,
                    Message = DbServices.UnsupportedTParameter
                };
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");

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
        public static async Task<OperationStateVM> DeleteAsync<T>(long Id) where T : class
        {
            try
            {
                if (!IsModelTypeSupported<T>())
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = DbServices.UnsupportedTParameter
                    };
                }

                RostalDbContext context = new RostalDbContext();

                if (typeof(T) == typeof(Tlibrary))
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
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");

                return new OperationStateVM()
                {
                    IsSuccess = false,
                    Message = $"Exception : {ex.Message}",
                };
            }
        }

        #region Helpers
    //    public static T2 ViewModelConverter<T1, T2>(T1 model)
    //where T1 : class
    //where T2 : class
    //    {
    //        try
    //        {
    //            if (model == null) return null;

    //            if (typeof(T1) == typeof(Tlibrary))
    //            {
    //                Tlibrary modelC = (Tlibrary)(object)model;

    //                var isGuidCorrect = Guid.TryParse(modelC.Guid, out Guid guid);
    //                if (isGuidCorrect == false) return null;

    //                var viewModel = new BibliothequeVM()
    //                {
    //                    Id = modelC.Id,
    //                    DateAjout = DatesHelpers.Converter.GetDateFromString(modelC.DateAjout),
    //                    DateEdition = DatesHelpers.Converter.GetNullableDateFromString(modelC.DateEdition),
    //                    Description = modelC.Description,
    //                    Name = modelC.Name,
    //                    Guid = isGuidCorrect ? guid : Guid.Empty,

    //                };

    //                return (T2)(object)viewModel;
    //            }
                
    //            return null;
    //        }
    //        catch (Exception ex)
    //        {
    //            MethodBase m = MethodBase.GetCurrentMethod();
    //            Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");
    //            return null;
    //        }
    //    }

        /// <summary>
        /// Convertit un modèle en modèle de vue
        /// </summary>
        /// <typeparam name="T1">Type d'entrée</typeparam>
        /// <typeparam name="T2">Type sortie</typeparam>
        /// <param name="model">Modèle de base de données</param>
        /// <returns>Un modèle de vue</returns>
        private static async Task<T2> ViewModelConverterAsync<T1, T2>(T1 model)
            where T1 : class
            where T2 : class
        {
            try
            {
                if (model == null) return null;

                if (typeof(T1) == typeof(Tlibrary))
                {
                    Tlibrary modelC = model as Tlibrary;
                    if (modelC == null) return null;

                    var isGuidCorrect = Guid.TryParse(modelC.Guid, out Guid guid);
                    if (isGuidCorrect == false) return null;

                    var categoriesList = await MultipleVmAsync<TlibraryCategorie, CategorieLivreVM>(modelC.Id);
                    var viewModel = new BibliothequeVM()
                    {
                        Id = modelC.Id,
                        DateAjout = DatesHelpers.Converter.GetDateFromString(modelC.DateAjout),
                        DateEdition = DatesHelpers.Converter.GetNullableDateFromString(modelC.DateEdition),
                        Description = modelC.Description,
                        Name = modelC.Name,
                        Guid = isGuidCorrect ? guid : Guid.Empty,
                        Categories = categoriesList != null && categoriesList.Any() ? new ObservableCollection<CategorieLivreVM>(categoriesList) : new ObservableCollection<CategorieLivreVM>(),
                    };

                    if (viewModel.Categories.Any())
                    {
                        foreach (var category in viewModel.Categories)
                        {
                            var subCategoriesList = await MultipleVmAsync<TlibrarySubCategorie, SubCategorieLivreVM>(category.Id);
                            if (subCategoriesList != null && subCategoriesList.Any())
                            {
                                category.SubCategorieLivres = subCategoriesList != null && subCategoriesList.Any() ? new ObservableCollection<SubCategorieLivreVM>(subCategoriesList) : new ObservableCollection<SubCategorieLivreVM>();
                            }
                        }
                    }

                    return (T2)(object)viewModel;
                }
                else if (typeof(T1) == typeof(TlibraryCategorie))
                {
                    TlibraryCategorie modelC = (TlibraryCategorie)(object)model;

                    var subCategoriesList = await MultipleVmAsync<TlibrarySubCategorie, SubCategorieLivreVM>(modelC.Id);

                    var viewModel = new CategorieLivreVM()
                    {
                        Id = modelC.Id,
                        IdLibrary = modelC.IdLibrary,
                        Description = modelC.Description,
                        Name = modelC.Name,
                        SubCategorieLivres = subCategoriesList != null && subCategoriesList.Any() ? new ObservableCollection<SubCategorieLivreVM>(subCategoriesList) : new ObservableCollection<SubCategorieLivreVM>(),
                    };

                    return (T2)(object)viewModel;
                }
                else if (typeof(T1) == typeof(TlibrarySubCategorie))
                {
                    TlibrarySubCategorie modelC = (TlibrarySubCategorie)(object)model;

                    var viewModel = new SubCategorieLivreVM()
                    {
                        Id = modelC.Id,
                        IdCategorie = modelC.IdCategorie,
                        Description = modelC.Description,
                        Name = modelC.Name,
                    };

                    return (T2)(object)viewModel;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static bool IsViewModelTypeSupported<T>() where T : class
        {
            try
            {
                return typeof(T) == typeof(BibliothequeVM) || typeof(T) == typeof(CategorieLivreVM) || typeof(T) == typeof(SubCategorieLivreVM) || typeof(T) == typeof(LivreVM);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return false;
            }
        }

        public static bool IsModelTypeSupported<T>() where T : class
        {
            try
            {
                return typeof(T) == typeof(Tlibrary) || typeof(T) == typeof(TlibraryCategorie) || typeof(T) == typeof(TlibrarySubCategorie) || typeof(T) == typeof(Tbooks);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return false;
            }
        }

        /// <summary>
        /// Obtient une valeur booléenne indiquant si un enregistrement existe
        /// </summary>
        /// <typeparam name="T">Type d'entrée (Modèle de vue)</typeparam>
        /// <param name="IdAnime"></param>
        /// <param name="viewModel">Modèle de vue</param>
        /// <returns></returns>
        public static async Task<bool> IsExistAsync<T>(T viewModel) where T : class
        {
            try
            {
                if (!IsViewModelTypeSupported<T>()) return false;

                RostalDbContext context = new RostalDbContext();

                if (typeof(T) == typeof(BibliothequeVM))
                {
                    BibliothequeVM vm = (BibliothequeVM)(object)viewModel;
                    var IsExist = await context.Tlibrary.AnyAsync(c => c.Name.ToLower() == vm.Name.Trim().ToLower());
                    return IsExist;
                }
                else if (typeof(T) == typeof(CategorieLivreVM))
                {
                    CategorieLivreVM vm = (CategorieLivreVM)(object)viewModel;
                    var IsExist = await context.TlibraryCategorie.AnyAsync(c => c.IdLibrary == vm.IdLibrary && c.Name.ToLower() == vm.Name.Trim().ToLower());
                    return IsExist;
                }
                else if (typeof(T) == typeof(SubCategorieLivreVM))
                {
                    SubCategorieLivreVM vm = (SubCategorieLivreVM)(object)viewModel;
                    var IsExist = await context.TlibrarySubCategorie.AnyAsync(c => c.IdCategorie == vm.IdCategorie && c.Name.ToLower() == vm.Name.Trim().ToLower());
                    return IsExist;
                }

                return false;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");

                return false;

            }
        } 
        #endregion
    }
}
