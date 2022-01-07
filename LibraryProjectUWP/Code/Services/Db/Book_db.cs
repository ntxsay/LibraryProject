using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Publishers;
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
        public struct Book
        {
            static string NameEmptyMessage = "Le nom du livre doit être renseigné avant l'enregistrement.";
            static string NameAlreadyExistMessage = "Ce livre existe déjà.";
            #region All
            /// <summary>
            /// Retourne tous les objets de la base de données
            /// </summary>
            /// <typeparam name="T">Modèle de base de données</typeparam>
            /// <returns></returns>
            public static async Task<IList<Tbook>> AllAsync()
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();

                    var collection = await context.Tbook.ToListAsync();
                    if (collection == null || !collection.Any()) return Enumerable.Empty<Tbook>().ToList();

                    return collection;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>().ToList();
                }
            }

            /// <summary>
            /// Retourne tous les modèles de vue depuis la base de données
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <returns></returns>
            public static async Task<IList<LivreVM>> AllVMAsync()
            {
                try
                {
                    var collection = await AllAsync();
                    if (!collection.Any()) return Enumerable.Empty<LivreVM>().ToList();

                    var values = collection.Select(s => ViewModelConverterAsync(s)).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<LivreVM>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdAsync()
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();
                    return await context.Tbook.Select(s => s.Id).ToListAsync();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<long>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdLibraryAsync(long idLibrary)
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();
                    return await context.TlibraryBookConnector.Where(w => w.IdLibrary == idLibrary).Select(s => s.IdBook).ToListAsync();
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
            public static async Task<IList<Tbook>> MultipleWithIdLibraryAsync(long idLibrary)
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();

                    var preCollection = await context.TlibraryBookConnector.Where(w => w.IdLibrary == idLibrary).ToListAsync();
                    if (preCollection.Any())
                    {
                        List<Tbook> collection = new List<Tbook>();
                        foreach (TlibraryBookConnector driver in preCollection)
                        {
                            Tbook model = await context.Tbook.SingleOrDefaultAsync(w => w.Id == driver.IdBook);
                            if (model != null)
                            {
                                collection.Add(model);
                            }
                        }

                        return collection;
                    }

                    return Enumerable.Empty<Tbook>().ToList();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>().ToList();
                }
            }

            public static async Task<IList<LivreVM>> MultipleVmWithIdLibraryAsync(long idLibrary)
            {
                try
                {
                    var collection = await MultipleWithIdLibraryAsync(idLibrary);
                    if (!collection.Any()) return Enumerable.Empty<LivreVM>().ToList();

                    var values = collection.Select(s => ViewModelConverterAsync(s)).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<LivreVM>().ToList();
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
            public static async Task<Tbook> SingleAsync(long id)
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();

                    var s = await context.Tbook.SingleOrDefaultAsync(d => d.Id == id);
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
            public static async Task<LivreVM> SingleVMAsync(long id)
            {
                return ViewModelConverterAsync(await SingleAsync(id));
            }
            #endregion

            public static async Task<OperationStateVM> CreateAsync(LivreVM viewModel)
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

                    if (viewModel.MainTitle.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = NameEmptyMessage,
                        };
                    }

                    LibraryDbContext context = new LibraryDbContext();

                    var isExist = await context.Tbook.AnyAsync(c => c.MainTitle.ToLower() == viewModel.MainTitle.Trim().ToLower());
                    if (isExist)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Message = DbServices.RecordAlreadyExistMessage
                        };
                    }

                    var record = new Tbook()
                    {
                        Guid = viewModel.Guid.ToString(),
                        DateAjout = viewModel.DateAjout.ToString(),
                        DateAjoutUser = viewModel.DateAjoutUser.ToString(),
                        DateEdition = viewModel.DateEdition?.ToString(),
                        DateParution = viewModel.DateParution?.ToString(),
                        MainTitle = viewModel.MainTitle,
                        CountOpening = viewModel.CountOpening,
                        NbExactExemplaire = viewModel.NbExactExemplaire,
                        Resume = viewModel.Resume,
                        Notes = viewModel.Notes,
                        Cotation = viewModel.Identification?.Cotation, //A deplacer dans identification
                    };

                    await context.Tbook.AddAsync(record);
                    await context.SaveChangesAsync();

                    if (viewModel.Identification != null)
                    {
                        var recordConnector = new TbookIdentification()
                        {
                            Id = record.Id,
                            Isbn = viewModel.Identification.ISBN,
                            Isbn10 = viewModel.Identification.ISBN10,
                            Isbn13 = viewModel.Identification.ISBN13,
                            Issn = viewModel.Identification.ISSN,
                            Asin = viewModel.Identification.ASIN,
                            CodeBarre = viewModel.Identification.CodeBarre,
                        };

                        await context.TbookIdentification.AddAsync(recordConnector);
                        await context.SaveChangesAsync();
                    }

                    if (viewModel.TitresOeuvre != null && viewModel.TitresOeuvre.Any())
                    {
                        foreach (string title in viewModel.TitresOeuvre)
                        {
                            var titleConnector = new TbookOtherTitle()
                            {
                                IdBook = record.Id,
                                Title = title,
                            };

                            _ = await context.TbookOtherTitle.AddAsync(titleConnector);
                        }
                    }

                    if (viewModel.Auteurs != null && viewModel.Auteurs.Any())
                    {
                        foreach (AuthorVM author in viewModel.Auteurs)
                        {
                            var authorConnector = new TbookAuthorConnector()
                            {
                                IdBook = record.Id,
                                IdAuthor = author.Id,
                            };

                            _ = await context.TbookAuthorConnector.AddAsync(authorConnector);
                        }
                    }

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
            /// Met à jour un élément existant dans la base de données
            /// </summary>
            /// <typeparam name="T">Type d'entrée (Modèle de vue)</typeparam>
            /// <param name="viewModel">Modèle de vue</param>
            /// <returns></returns>
            public static async Task<OperationStateVM> UpdateAsync(LivreVM viewModel)
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

                    if (viewModel.MainTitle.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = NameEmptyMessage,
                        };
                    }

                    LibraryDbContext context = new LibraryDbContext();

                    var record = await context.Tbook.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                    if (record == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage,
                        };
                    }

                    var isExist = await context.Tbook.AnyAsync(c => c.Id != record.Id && c.MainTitle.ToLower() == viewModel.MainTitle.Trim().ToLower());
                    if (isExist)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Message = NameAlreadyExistMessage
                        };
                    }

                    record.DateAjoutUser = viewModel.DateAjoutUser.ToString();
                    record.DateEdition = DateTime.UtcNow.ToString();
                    record.DateParution = viewModel.DateParution?.ToString();
                    record.MainTitle = viewModel.MainTitle;
                    record.CountOpening = viewModel.CountOpening;
                    record.NbExactExemplaire = viewModel.NbExactExemplaire;
                    record.Resume = viewModel.Resume;
                    record.Notes = viewModel.Notes;
                    record.Cotation = viewModel.Identification?.Cotation; //A deplacer dans identification

                    context.Tbook.Update(record);

                    if (viewModel.TitresOeuvre != null)
                    {
                        var recordTitles = await context.TbookOtherTitle.Where(a => a.IdBook == record.Id).ToListAsync();
                        if (recordTitles.Any())
                        {
                            context.TbookOtherTitle.RemoveRange(recordTitles);
                        }

                        if (viewModel.TitresOeuvre.Any())
                        {
                            foreach (string title in viewModel.TitresOeuvre)
                            {
                                var titleConnector = new TbookOtherTitle()
                                {
                                    IdBook = record.Id,
                                    Title = title,
                                };

                                _ = await context.TbookOtherTitle.AddAsync(titleConnector);
                            }
                        }
                    }

                    if (viewModel.Auteurs != null)
                    {
                        var recordAuthors = await context.TbookAuthorConnector.Where(a => a.IdBook == record.Id).ToListAsync();
                        if (recordAuthors.Any())
                        {
                            context.TbookAuthorConnector.RemoveRange(recordAuthors);
                        }

                        if (viewModel.Auteurs.Any())
                        {
                            foreach (AuthorVM author in viewModel.Auteurs)
                            {
                                var authorConnector = new TbookAuthorConnector()
                                {
                                    IdBook = record.Id,
                                    IdAuthor = author.Id,
                                };

                                _ = await context.TbookAuthorConnector.AddAsync(authorConnector);
                            }
                        }
                    }

                    if (viewModel.Identification != null)
                    {
                        var recordIdentification = await context.TbookIdentification.SingleOrDefaultAsync(a => a.Id == record.Id);
                        if (recordIdentification == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
                            };
                        }

                        recordIdentification.Isbn = viewModel.Identification.ISBN;
                        recordIdentification.Isbn10 = viewModel.Identification.ISBN10;
                        recordIdentification.Isbn13 = viewModel.Identification.ISBN13;
                        recordIdentification.Issn = viewModel.Identification.ISSN;
                        recordIdentification.Asin = viewModel.Identification.ASIN;
                        recordIdentification.CodeBarre = viewModel.Identification.CodeBarre;
                        _ = context.TbookIdentification.Update(recordIdentification);
                    }

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

                    Tbook record = await context.Tbook.SingleOrDefaultAsync(a => a.Id == Id);
                    if (record == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage
                        };
                    }

                    List<TlibraryBookConnector> libraryDriverCollection = await context.TlibraryBookConnector.Where(w => w.IdBook == record.Id).ToListAsync();
                    if (libraryDriverCollection.Any())
                    {
                        context.TlibraryBookConnector.RemoveRange(libraryDriverCollection);
                        //await context.SaveChangesAsync();
                    }

                    var recordTitles = await context.TbookOtherTitle.Where(a => a.IdBook == record.Id).ToListAsync();
                    if (recordTitles.Any())
                    {
                        context.TbookOtherTitle.RemoveRange(recordTitles);
                    }

                    var recordAuthors = await context.TbookAuthorConnector.Where(a => a.IdBook == record.Id).ToListAsync();
                    if (recordAuthors.Any())
                    {
                        context.TbookAuthorConnector.RemoveRange(recordAuthors);
                    }

                    TbookIdentification recordIdentification = await context.TbookIdentification.SingleOrDefaultAsync(a => a.Id == record.Id);
                    if (recordIdentification != null)
                    {
                        context.TbookIdentification.Remove(recordIdentification);
                    }

                    context.Tbook.Remove(record);
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
            private static LivreVM ViewModelConverterAsync(Tbook model)
            {
                try
                {
                    if (model == null) return null;
                    
                    var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                    if (isGuidCorrect == false) return null;

                    var viewModel = new LivreVM()
                    {
                        Id = model.Id,
                        Guid = isGuidCorrect ? guid : Guid.Empty,
                        DateAjout = DatesHelpers.Converter.GetDateFromString(model.DateAjout),
                        DateAjoutUser = DatesHelpers.Converter.GetDateFromString(model.DateAjoutUser),
                        DateEdition = DatesHelpers.Converter.GetNullableDateFromString(model.DateEdition),
                        DateParution = DatesHelpers.Converter.GetNullableDateFromString(model.DateParution),
                        MainTitle = model.MainTitle,
                        CountOpening = model.CountOpening,
                        NbExactExemplaire = Convert.ToInt16(model.NbExactExemplaire),
                        Resume = model.Resume,
                        Notes = model.Notes,
                    };

                    if (model.TbookIdentification != null)
                    {
                        viewModel.Identification = new LivreIdentificationVM()
                        {
                            Id = model.TbookIdentification.Id,
                            ISBN = model.TbookIdentification.Isbn,
                            ISBN10 = model.TbookIdentification.Isbn10,
                            ISBN13 = model.TbookIdentification.Isbn13,
                            ISSN = model.TbookIdentification.Issn,
                            ASIN = model.TbookIdentification.Asin,
                            CodeBarre = model.TbookIdentification.CodeBarre,
                            Cotation = model.Cotation,
                        };
                    }

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
