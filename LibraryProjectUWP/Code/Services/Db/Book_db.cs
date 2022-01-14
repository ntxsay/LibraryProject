using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Publishers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(s => s.Result).ToList();
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

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(s => s.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<LivreVM>().ToList();
                }
            }

            public static async Task<IList<string>> GetOtherTitlesInBookAsync(long idBook)
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();

                    List<string> collection = await context.TbookOtherTitle.Where(w => w.IdBook == idBook).Select(s => s.Title).ToListAsync();
                    return collection;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<string>().ToList();
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
                return await ViewModelConverterAsync(await SingleAsync(id));
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
                        DateParution = viewModel.Publication.DateParution?.ToString(),
                        IsJourParutionKnow = viewModel.Publication.IsJourParutionKnow ? 1 : 0,
                        IsMoisParutionKnow = viewModel.Publication.IsMoisParutionKnow ? 1 : 0,
                        MainTitle = viewModel.MainTitle,
                        CountOpening = viewModel.CountOpening,
                        NbExactExemplaire = viewModel.NbExactExemplaire,
                        Resume = viewModel.Description?.Resume,
                        Notes = viewModel.Description?.Notes,
                        MinAge = viewModel.ClassificationAge?.MinAge,
                        MaxAge = viewModel.ClassificationAge?.MaxAge
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
                            Cotation = viewModel.Identification.Cotation,
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
                            await context.SaveChangesAsync();
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
                            await context.SaveChangesAsync();
                        }
                    }

                    if (viewModel.Publication.Collections != null && viewModel.Publication.Collections.Any())
                    {
                        foreach (CollectionVM collection in viewModel.Publication.Collections)
                        {
                            var itemConnector = new TbookCollectionConnector()
                            {
                                IdBook = record.Id,
                                IdCollection = collection.Id,
                            };

                            _ = await context.TbookCollectionConnector.AddAsync(itemConnector);
                            await context.SaveChangesAsync();
                        }
                    }

                    if (viewModel.Publication.Editeurs != null && viewModel.Publication.Editeurs.Any())
                    {
                        foreach (PublisherVM editeur in viewModel.Publication.Editeurs)
                        {
                            var itemConnector = new TbookEditeurConnector()
                            {
                                IdBook = record.Id,
                                IdEditeur = editeur.Id,
                            };

                            _ = await context.TbookEditeurConnector.AddAsync(itemConnector);
                            await context.SaveChangesAsync();
                        }
                    }

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Id = record.Id,
                        Message = $"Le livre {viewModel.MainTitle} a été créé avec succès."
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
                    record.DateParution = viewModel.Publication.DateParution?.ToString();
                    record.IsJourParutionKnow = viewModel.Publication.IsJourParutionKnow ? 1 : 0;
                    record.IsMoisParutionKnow = viewModel.Publication.IsMoisParutionKnow ? 1 : 0;
                    record.MainTitle = viewModel.MainTitle;
                    record.CountOpening = viewModel.CountOpening;
                    record.NbExactExemplaire = viewModel.NbExactExemplaire;
                    record.Resume = viewModel.Description.Resume;
                    record.Notes = viewModel.Description.Notes;
                    record.MinAge = viewModel.ClassificationAge?.MinAge;
                    record.MaxAge = viewModel.ClassificationAge?.MaxAge;


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

                    if (viewModel.Publication.Collections != null)
                    {
                        var recorditemList = await context.TbookCollectionConnector.Where(a => a.IdBook == record.Id).ToListAsync();
                        if (recorditemList.Any())
                        {
                            context.TbookCollectionConnector.RemoveRange(recorditemList);
                        }
                        
                        if (viewModel.Publication.Collections.Any())
                        {
                            foreach (CollectionVM collection in viewModel.Publication.Collections)
                            {
                                var itemConnector = new TbookCollectionConnector()
                                {
                                    IdBook = record.Id,
                                    IdCollection = collection.Id,
                                };

                                _ = await context.TbookCollectionConnector.AddAsync(itemConnector);
                            }
                        }
                    }

                    if (viewModel.Publication.Editeurs != null)
                    {
                        var recorditemList = await context.TbookEditeurConnector.Where(a => a.IdBook == record.Id).ToListAsync();
                        if (recorditemList.Any())
                        {
                            context.TbookEditeurConnector.RemoveRange(recorditemList);
                        }

                        if (viewModel.Publication.Editeurs.Any())
                        {
                            foreach (PublisherVM editeur in viewModel.Publication.Editeurs)
                            {
                                var itemConnector = new TbookEditeurConnector()
                                {
                                    IdBook = record.Id,
                                    IdEditeur = editeur.Id,
                                };

                                _ = await context.TbookEditeurConnector.AddAsync(itemConnector);
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
                        recordIdentification.Cotation = viewModel.Identification.Cotation;
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

                    var recordEditors = await context.TbookEditeurConnector.Where(a => a.IdBook == record.Id).ToListAsync();
                    if (recordEditors.Any())
                    {
                        context.TbookEditeurConnector.RemoveRange(recordEditors);
                    }

                    var recordCollection = await context.TbookCollectionConnector.Where(a => a.IdBook == record.Id).ToListAsync();
                    if (recordCollection.Any())
                    {
                        context.TbookCollectionConnector.RemoveRange(recordCollection);
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
            private static async Task<LivreVM> ViewModelConverterAsync(Tbook model)
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
                        MainTitle = model.MainTitle,
                        CountOpening = model.CountOpening,
                        NbExactExemplaire = Convert.ToInt16(model.NbExactExemplaire),
                        Description = new LivreDescriptionVM()
                        {
                            Resume = model.Resume,
                            Notes = model.Notes,
                        },
                        Publication = new LivrePublicationVM()
                        {
                            DateParution = DatesHelpers.Converter.GetNullableDateFromString(model.DateParution),
                        },
                        ClassificationAge = new LivreClassificationAgeVM()
                        {
                            MinAge = (byte)(model.MinAge < byte.MinValue || model.MinAge > byte.MaxValue ? 0 : model.MinAge),
                            MaxAge = (byte)(model.MaxAge < byte.MinValue || model.MaxAge > byte.MaxValue ? 0 : model.MaxAge),
                        },
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
                            Cotation = model.TbookIdentification.Cotation,
                        };
                    }

                    return await ViewModelConverterConnectorAsync(model, viewModel);
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }

            private static async Task<LivreVM> ViewModelConverterConnectorAsync(Tbook model, LivreVM viewModel)
            {
                try
                {
                    if (model == null) return null;
                    if (viewModel == null) return null;

                    IList<string> titres = await GetOtherTitlesInBookAsync(model.Id);
                    IList<AuthorVM> authors = await Author.MultipleVmInBookAsync(model.Id);
                    IList<PublisherVM> editors = await Editors.MultipleVmInBookAsync(model.Id);
                    IList<CollectionVM> collections = await Collection.MultipleVmInBookAsync(model.Id, CollectionTypeEnum.Collection);

                    viewModel.TitresOeuvre = titres != null && titres.Any() ? new ObservableCollection<string>(titres) : new ObservableCollection<string>();
                    viewModel.Auteurs = authors != null && authors.Any() ? new ObservableCollection<AuthorVM>(authors) : new ObservableCollection<AuthorVM>();
                    viewModel.Publication = new LivrePublicationVM()
                    {
                        Collections = collections != null && collections.Any() ? new ObservableCollection<CollectionVM>(collections) : new ObservableCollection<CollectionVM>(),
                        Editeurs = editors != null && editors.Any() ? new ObservableCollection<PublisherVM>(editors) : new ObservableCollection<PublisherVM>(),
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

            public static LivreVM DeepCopy(LivreVM viewModelToCopy)
            {
                try
                {
                    if (viewModelToCopy == null) return null;

                    LivreVM newViewModel = new LivreVM()
                    {
                        Id = viewModelToCopy.Id,
                        Guid = viewModelToCopy.Guid,
                        DateAjout = viewModelToCopy.DateAjout,
                        DateAjoutUser = viewModelToCopy.DateAjoutUser,
                        DateEdition = viewModelToCopy.DateEdition,
                        MainTitle = viewModelToCopy.MainTitle,
                        CountOpening = viewModelToCopy.CountOpening,
                        NbExactExemplaire = viewModelToCopy.NbExactExemplaire,
                        TitresOeuvre = viewModelToCopy.TitresOeuvre,
                        Auteurs = viewModelToCopy.Auteurs,
                        JaquettePath = viewModelToCopy.JaquettePath,
                    };

                    if (viewModelToCopy.Identification != null)
                    {
                        newViewModel.Identification = new LivreIdentificationVM()
                        {
                            Id = viewModelToCopy.Identification.Id,
                            ISBN = viewModelToCopy.Identification.ISBN,
                            ISBN10 = viewModelToCopy.Identification.ISBN10,
                            ISBN13 = viewModelToCopy.Identification.ISBN13,
                            ISSN = viewModelToCopy.Identification.ISSN,
                            ASIN = viewModelToCopy.Identification.ASIN,
                            CodeBarre = viewModelToCopy.Identification.CodeBarre,
                            Cotation = viewModelToCopy.Identification.Cotation,
                        };
                    }

                    if (viewModelToCopy.ClassificationAge != null)
                    {
                        newViewModel.ClassificationAge = new LivreClassificationAgeVM()
                        {
                            MinAge = viewModelToCopy.ClassificationAge.MinAge,
                            MaxAge = viewModelToCopy.ClassificationAge.MaxAge,
                        };
                    }

                    if (viewModelToCopy.Publication != null)
                    {
                        newViewModel.Publication = new LivrePublicationVM()
                        {
                            DateParution = viewModelToCopy.Publication.DateParution,
                            Collections = viewModelToCopy.Publication.Collections,
                            Editeurs = viewModelToCopy.Publication.Editeurs,
                        };
                    }

                    if (viewModelToCopy.Description != null)
                    {
                        newViewModel.Description = new LivreDescriptionVM()
                        {
                            Resume = viewModelToCopy.Description.Resume,
                            Notes = viewModelToCopy.Description.Notes,
                        };
                    }

                    return newViewModel;
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
