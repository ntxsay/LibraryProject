using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;

namespace LibraryProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        public struct Categorie
        {
            static string NameEmptyMessage = "Le nom de la catégorie doit être renseigné avant l'enregistrement.";
            static string NameAlreadyExistMessage = "Cette catégorie existe déjà.";
            static string SubCategorieNameEmptyMessage = "Le nom de la sous-catégorie doit être renseigné avant l'enregistrement.";
            static string SubCategorieNameAlreadyExistMessage = "Cette sous-catégorie existe déjà.";

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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var collection = await context.TlibraryCategorie.ToListAsync();
                        if (collection == null || !collection.Any()) return Enumerable.Empty<TlibraryCategorie>().ToList();

                        return collection;
                    }
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

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(t => t.Result).ToList();
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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TlibraryCategorie.Select(s => s.Id).ToListAsync();
                    }
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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TlibraryCategorie.Where(w => w.IdLibrary == idLibrary).Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<long>().ToList();
                }
            }

            /// <summary>
            /// Retourne tous les objets de la base de données
            /// </summary>
            /// <typeparam name="T">Modèle de base de données</typeparam>
            /// <returns></returns>
            public static async Task<IList<TlibrarySubCategorie>> AllSubCategorieAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var collection = await context.TlibrarySubCategorie.ToListAsync();
                        if (collection == null || !collection.Any()) return Enumerable.Empty<TlibrarySubCategorie>().ToList();

                        return collection;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<TlibrarySubCategorie>().ToList();
                }
            }

            /// <summary>
            /// Retourne tous les modèles de vue depuis la base de données
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <returns></returns>
            public static async Task<IList<SubCategorieLivreVM>> AllSubCategorieVMAsync()
            {
                try
                {
                    var collection = await AllSubCategorieAsync();
                    if (!collection.Any()) return Enumerable.Empty<SubCategorieLivreVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(s => s.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<SubCategorieLivreVM>().ToList();
                }
            }

            public static async Task<IList<long>> AllSubCategorieIdAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TlibrarySubCategorie.Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<long>().ToList();
                }
            }

            public static async Task<IList<long>> AllSubCategorieIdAsync(long idCategorie)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TlibrarySubCategorie.Where(w => w.IdCategorie == idCategorie).Select(s => s.Id).ToListAsync();
                    }
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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var collection = await context.TlibraryCategorie.Where(w => w.IdLibrary == idLibrary).ToListAsync();
                        if (collection == null || !collection.Any()) return Enumerable.Empty<TlibraryCategorie>().ToList();

                        return collection;
                    }
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
                    var collection = await MultipleAsync(idLibrary);
                    if (!collection.Any()) return Enumerable.Empty<CategorieLivreVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(t => t.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<CategorieLivreVM>().ToList();
                }
            }

            public static async Task<IList<TlibrarySubCategorie>> MultipleSubCategorieAsync(long idCategorie)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var collection = await context.TlibrarySubCategorie.Where(w => w.IdCategorie == idCategorie).ToListAsync();
                        if (collection == null || !collection.Any()) return Enumerable.Empty<TlibrarySubCategorie>().ToList();

                        return collection;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<TlibrarySubCategorie>().ToList();
                }
            }

            public static async Task<IList<SubCategorieLivreVM>> MultipleSubCategorieVmAsync(long idCategorie)
            {
                try
                {
                    var collection = await MultipleSubCategorieAsync(idCategorie);
                    if (!collection.Any()) return Enumerable.Empty<SubCategorieLivreVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(d => d.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<SubCategorieLivreVM>().ToList();
                }
            }

            public static async Task AddSubCategoriesToCategoriesVmAsync(ICollection<CategorieLivreVM> viewModelList)
            {
                try
                {
                    if (viewModelList != null && viewModelList.Any())
                    {
                        foreach (var category in viewModelList)
                        {
                            var subCategoriesList = await MultipleSubCategorieVmAsync(category.Id);
                            if (subCategoriesList != null && subCategoriesList.Any())
                            {
                                category.SubCategorieLivres = subCategoriesList != null && subCategoriesList.Any() ? 
                                    new ObservableCollection<SubCategorieLivreVM>(subCategoriesList) : new ObservableCollection<SubCategorieLivreVM>();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return;
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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var s = await context.TlibraryCategorie.SingleOrDefaultAsync(d => d.Id == id);
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
            public static async Task<CategorieLivreVM> SingleVMAsync(long id)
            {
                return await Categorie.ViewModelConverterAsync(await Categorie.SingleAsync(id));
            }

            public static async Task<TlibraryCategorie> GetParentCategorieAsync(long idSubcategorie)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var s = await context.TlibrarySubCategorie.SingleOrDefaultAsync(d => d.Id == idSubcategorie);
                        if (s == null) return null;
                        s.IdCategorieNavigation = await context.TlibraryCategorie.SingleOrDefaultAsync(q => q.Id == s.IdCategorie);
                        return s.IdCategorieNavigation;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return null;
                }
            }

            public static async Task<CategorieLivreVM> GetParentCategorieVMAsync(long idSubcategorie)
            {
                return await ViewModelConverterAsync(await GetParentCategorieAsync(idSubcategorie));
            }
            #endregion

            public static async Task<long> CountUnCategorizedBooks(long idLibrary)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.Tbook.CountAsync(d => d.IdLibrary == idLibrary && d.IdCategorie == null && d.IdSubCategorie == null);
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return 0;
                }
            }

            public static async Task<IList<long>> GetUnCategorizedBooksId(long idLibrary)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.Tbook.Where(d => d.IdLibrary == idLibrary && d.IdCategorie == null && d.IdSubCategorie == null).Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return new List<long>();
                }
            }

            public static async Task<IList<long>> GetBooksIdInCategorie(long idCategorie)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.Tbook.Where(d => d.IdCategorie == idCategorie && d.IdSubCategorie == null).Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return new List<long>();
                }
            }

            public static async Task<IList<long>> GetBooksIdInSubCategorie(long idSubCategorie)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.Tbook.Where(d => d.IdSubCategorie == idSubCategorie).Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return new List<long>();
                }
            }

            public static async Task<OperationStateVM> CreateCategorieConnectorAsync(IEnumerable<long> idBooks, CategorieLivreVM categorie, SubCategorieLivreVM subCategorie = null)
            {
                try
                {
                    if (categorie == null)
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

                        var isCategorieExist = await context.TlibraryCategorie.AnyAsync(a => a.Id == categorie.Id);
                        if (!isCategorieExist)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
                            };
                        }

                        if (subCategorie != null)
                        {
                            var isSubCategorieExist = await context.TlibrarySubCategorie.AnyAsync(a => a.Id == subCategorie.Id);
                            if (!isSubCategorieExist)
                            {
                                return new OperationStateVM()
                                {
                                    IsSuccess = false,
                                    Message = DbServices.RecordNotExistMessage,
                                };
                            }
                        }

                        foreach (var idBook in validIdList)
                        {
                            var bookConnector = await context.Tbook.SingleOrDefaultAsync(c => c.Id == idBook);
                            if (bookConnector == null)
                            {
                                continue;
                            }

                            bookConnector.IdCategorie = categorie.Id;
                            if (subCategorie != null)
                            {
                                bookConnector.IdSubCategorie = subCategorie.Id;
                            }

                            context.Tbook.Update(bookConnector);
                            await context.SaveChangesAsync();
                        }


                        return new OperationStateVM()
                        {
                            Message = subCategorie == null ? $"Les livres ont été ajoutés à la catégorie : {categorie.Name}." : $"Les livres ont été ajoutés à la sous-catégorie : {subCategorie.Name}",
                            IsSuccess = true,
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
                            var bookConnector = await context.Tbook.SingleOrDefaultAsync(c => c.Id == idBook);
                            if (bookConnector == null)
                            {
                                continue;
                            }

                            bookConnector.IdCategorie = null;
                            bookConnector.IdSubCategorie = null;

                            context.Tbook.Update(bookConnector);
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
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }


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

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
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

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
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

            

            public static async Task<OperationStateVM> CreateSubCategorieAsync(SubCategorieLivreVM viewModel)
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
                        var isExist = await context.TlibrarySubCategorie.AnyAsync(c => c.IdCategorie == viewModel.IdCategorie && c.Name.ToLower() == viewModel.Name.Trim().ToLower());
                        if (isExist)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordAlreadyExistMessage
                            };
                        }

                        var record = new TlibrarySubCategorie()
                        {
                            IdCategorie = viewModel.IdCategorie,
                            Name = viewModel.Name,
                            Description = viewModel.Description,
                        };

                        await context.TlibrarySubCategorie.AddAsync(record);
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
            /// Met à jour un élément existant dans la base de données
            /// </summary>
            /// <typeparam name="T">Type d'entrée (Modèle de vue)</typeparam>
            /// <param name="viewModel">Modèle de vue</param>
            /// <returns></returns>
            public static async Task<OperationStateVM> UpdateAsync(SubCategorieLivreVM viewModel)
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
                        var record = await context.TlibrarySubCategorie.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                        if (record == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
                            };
                        }

                        var isExist = await context.TlibrarySubCategorie.AnyAsync(c => c.Id != record.Id && c.IdCategorie == viewModel.IdCategorie && c.Name.ToLower() == viewModel.Name.Trim().ToLower());
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

                        context.TlibrarySubCategorie.Update(record);
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
            public static async Task<OperationStateVM> DeleteAsync<T>(long Id) where T : class
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        if (typeof(T) == typeof(CategorieLivreVM) || typeof(T) == typeof(TlibraryCategorie))
                        {
                            var record = await context.TlibraryCategorie.SingleOrDefaultAsync(a => a.Id == Id);
                            if (record == null)
                            {
                                return new OperationStateVM()
                                {
                                    IsSuccess = false,
                                    Message = DbServices.RecordNotExistMessage
                                };
                            }

                            var connectors = await context.Tbook.Where(a => a.IdCategorie == record.Id).ToListAsync();
                            if (connectors != null && connectors.Any())
                            {
                                foreach (var connector in connectors)
                                {
                                    connector.IdCategorie = null;
                                    connector.IdSubCategorie = null;
                                }

                                context.Tbook.UpdateRange(connectors);
                                await context.SaveChangesAsync();
                            }

                            context.TlibraryCategorie.Remove(record);
                            await context.SaveChangesAsync();
                            record = null;
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = "La catégorie a été supprimée avec succès."
                            };
                        }
                        else if (typeof(T) == typeof(SubCategorieLivreVM) || typeof(T) == typeof(TlibrarySubCategorie))
                        {
                            var record = await context.TlibrarySubCategorie.SingleOrDefaultAsync(a => a.Id == Id);
                            if (record == null)
                            {
                                return new OperationStateVM()
                                {
                                    IsSuccess = false,
                                    Message = DbServices.RecordNotExistMessage
                                };
                            }

                            var connectors = await context.Tbook.Where(a => a.IdSubCategorie == record.Id).ToListAsync();
                            if (connectors != null && connectors.Any())
                            {
                                foreach (var connector in connectors)
                                {
                                    connector.IdSubCategorie = null;
                                }

                                context.Tbook.UpdateRange(connectors);
                                await context.SaveChangesAsync();
                            }

                            context.TlibrarySubCategorie.Remove(record);
                            await context.SaveChangesAsync();

                            record = null;
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = "La sous-catégorie a été supprimée avec succès."
                            };
                        }

                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = "Cet objet n'est pas reconnu."
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

                    var subCategoriesList = await MultipleSubCategorieVmAsync(model.Id);

                    var viewModel = new CategorieLivreVM()
                    {
                        Id = model.Id,
                        IdLibrary = model.IdLibrary,
                        Description = model.Description,
                        Name = model.Name,
                        SubCategorieLivres = subCategoriesList != null && subCategoriesList.Any() ? new ObservableCollection<SubCategorieLivreVM>(subCategoriesList) : new ObservableCollection<SubCategorieLivreVM>(),
                        BooksId = (await GetBooksIdInCategorie(model.Id))?.ToList(),
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

            /// <summary>
            /// Convertit un modèle en modèle de vue
            /// </summary>
            /// <typeparam name="T1">Type d'entrée</typeparam>
            /// <typeparam name="T2">Type sortie</typeparam>
            /// <param name="model">Modèle de base de données</param>
            /// <returns>Un modèle de vue</returns>
            private static async Task<SubCategorieLivreVM> ViewModelConverterAsync(TlibrarySubCategorie model)
            {
                try
                {
                    if (model == null) return null;

                    var viewModel = new SubCategorieLivreVM()
                    {
                        Id = model.Id,
                        IdCategorie = model.IdCategorie,
                        Description = model.Description,
                        Name = model.Name,
                        BooksId = (await GetBooksIdInSubCategorie(model.Id))?.ToList(),
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
