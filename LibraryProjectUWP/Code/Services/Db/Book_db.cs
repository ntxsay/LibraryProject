using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        public struct Book
        {
            static string NameEmptyMessage = "Le nom du livre doit être renseigné avant l'enregistrement.";
            static readonly string NameAlreadyExistMessage = "Ce livre existe déjà.";
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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tbook> collection = await context.Tbook.ToListAsync();
                        foreach (Tbook book in collection)
                        {
                            await CompleteModelInfos(context, book);
                        }
                        if (collection == null || !collection.Any()) return Enumerable.Empty<Tbook>().ToList();

                        return collection;
                    }
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
                    using (LibraryDbContext context = new LibraryDbContext()) 
                    { 
                        return await context.Tbook.Select(s => s.Id).ToListAsync(); 
                    }
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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TlibraryBookConnector.Where(w => w.IdLibrary == idLibrary).Select(s => s.IdBook).ToListAsync();
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
            public static async Task<long> CountBooksInLibraryAsync(long idLibrary, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TlibraryBookConnector.LongCountAsync(w => w.IdLibrary == idLibrary && w.IdBook > -1, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return 0;
                }
            }

            public static async Task<IList<long>> GetIdBooksAsync(long idLibrary)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TlibraryBookConnector.Where(w => w.IdLibrary == idLibrary).Select(s => s.IdBook).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<long>().ToList();
                }
            }

            public static async Task<IList<Tbook>> MultipleWithIdLibraryAsync(long idLibrary, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var preCollection = await context.TlibraryBookConnector.Where(w => w.IdLibrary == idLibrary).ToListAsync(cancellationToken);
                        if (preCollection.Any())
                        {
                            List<Tbook> collection = new List<Tbook>();
                            foreach (TlibraryBookConnector driver in preCollection)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    return collection;
                                }

                                Tbook model = await context.Tbook.SingleOrDefaultAsync(w => w.Id == driver.IdBook, cancellationToken);
                                if (model != null)
                                {
                                    await CompleteModelInfos(context, model);
                                    collection.Add(model);
                                }
                            }

                            return collection;
                        }

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

            public static async Task<IList<LivreVM>> MultipleVmWithIdLibraryAsync(long idLibrary, CancellationToken cancellationToken = default)
            {
                try
                {
                    var collection = await MultipleWithIdLibraryAsync(idLibrary, cancellationToken);
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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<string> collection = await context.TbookOtherTitle.Where(w => w.IdBook == idBook).Select(s => s.Title).ToListAsync();
                        return collection;
                    }

                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<string>().ToList();
                }
            }

            public static async Task<IList<TbookExemplary>> GetBookExemplaryAsync(long idBook, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var preCollection = await context.TbookExemplary.Where(w => w.IdBook == idBook).ToListAsync(cancellationToken);
                        if (preCollection.Any())
                        {
                            foreach (TbookExemplary item in preCollection)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    return preCollection;
                                }

                                if (item.IdContactSource != null && item.IdContactSourceNavigation == null)
                                {
                                    item.IdContactSourceNavigation = await context.Tcontact.SingleOrDefaultAsync(s => s.Id == item.IdContactSource);
                                }

                                if (item.TbookEtat == null || !item.TbookEtat.Any())
                                {
                                    item.TbookEtat = await context.TbookEtat.Where(s => s.IdBookExemplary == item.Id).ToListAsync(cancellationToken);
                                }

                                if (item.TbookPret == null)
                                {
                                    item.TbookPret = await context.TbookPret.Where(s => s.IdBookExemplary == item.Id).ToListAsync(cancellationToken);
                                }
                            }
                        }
                        return preCollection;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }

            public static async Task<IList<LivreExemplaryVM>> GetBookExemplaryVMAsync(long idBook, CancellationToken cancellationToken = default)
            {
                try
                {
                    var collection = await GetBookExemplaryAsync(idBook);
                    if (!collection.Any()) return Enumerable.Empty<LivreExemplaryVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(s => s.Result).Where(w => w != null).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }

            public static async Task<TbookEtat> GetEtatBookExemplaryAsync(long idBookExemplary)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var etat = await context.TbookEtat.FirstOrDefaultAsync(w => w.IdBookExemplary == idBookExemplary);
                        return etat;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }
            public static async Task<LivreEtatVM> GetEtatBookExemplaryVMAsync(long idBookExemplary)
            {
                try
                {
                    var model = await GetEtatBookExemplaryAsync(idBookExemplary);
                    if (model == null) return null;

                    var value = ViewModelConverter(model);
                    return value;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }


            #endregion

            #region Single
            public static async Task<long?> GetLibraryIdAsync(long idBook, LibraryDbContext _context = null)
            {
                try
                {
                    using (LibraryDbContext context = _context ?? new LibraryDbContext())
                    {
                        var tlibraryBookConnector = await context.TlibraryBookConnector.SingleOrDefaultAsync(w => w.IdBook == idBook);
                        if (tlibraryBookConnector == null) return null;
                        return tlibraryBookConnector.IdLibrary;
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
            /// Retourne un élément de la base de données avec un identifiant unique
            /// </summary>
            /// <typeparam name="T">Type d'entrée et de sortie (Modèle)</typeparam>
            /// <param name="id">Identifiant unique</param>
            /// <returns></returns>
            public static async Task<Tbook> SingleAsync(long id)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var s = await context.Tbook.SingleOrDefaultAsync(d => d.Id == id);
                        await CompleteModelInfos(context, s);
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
            public static async Task<LivreVM> SingleVMAsync(long id)
            {
                return await ViewModelConverterAsync(await SingleAsync(id));
            }
            #endregion

            public static async Task<OperationStateVM> CreateAsync(LivreVM viewModel, long idLibrary)
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

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var isExist = await IsBookExistAsync(viewModel);
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
                            DateEdition = viewModel.DateEdition?.ToString(),
                            DateParution = viewModel.Publication.DateParution?.ToString(),
                            IsJourParutionKnow = viewModel.Publication.IsJourParutionKnow ? 1 : 0,
                            IsMoisParutionKnow = viewModel.Publication.IsMoisParutionKnow ? 1 : 0,
                            MainTitle = viewModel.MainTitle,
                            CountOpening = viewModel.CountOpening,
                            Resume = viewModel.Description?.Resume,
                            Notes = viewModel.Description?.Notes,
                            MinAge = viewModel.ClassificationAge?.MinAge,
                            MaxAge = viewModel.ClassificationAge?.MaxAge,
                            Pays = viewModel.Publication?.Pays,
                            Langue = viewModel.Publication?.Langue,
                            Price = viewModel.Publication?.Price ?? 0,
                            DeviceName = viewModel.Publication?.DeviceName ?? "€"
                        };

                        await context.Tbook.AddAsync(record);
                        await context.SaveChangesAsync();

                        var libraryBookConnector = new TlibraryBookConnector()
                        {
                            IdLibrary = idLibrary,
                            IdBook = record.Id,
                        };

                        await context.TlibraryBookConnector.AddAsync(libraryBookConnector);
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

                        if (viewModel.Format != null)
                        {
                            var recordConnector = new TbookFormat()
                            {
                                Id = record.Id,
                                Format = viewModel.Format.Format,
                                NbOfPages = viewModel.Format.NbOfPages,
                                Epaisseur = viewModel.Format.Epaisseur,
                                Hauteur = viewModel.Format.Hauteur,
                                Largeur = viewModel.Format.Largeur,
                            };

                            await context.TbookFormat.AddAsync(recordConnector);
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
                            foreach (ContactVM author in viewModel.Auteurs)
                            {
                                if (await context.Tcontact.AnyAsync(a => a.Id == author.Id))
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
                        }

                        if (viewModel.Publication.Collections != null && viewModel.Publication.Collections.Any())
                        {
                            foreach (CollectionVM collection in viewModel.Publication.Collections)
                            {
                                if (await context.Tcollection.AnyAsync(a => a.Id == collection.Id))
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
                        }

                        if (viewModel.Publication.Editeurs != null && viewModel.Publication.Editeurs.Any())
                        {
                            foreach (ContactVM editeur in viewModel.Publication.Editeurs)
                            {
                                if (await context.Tcollection.AnyAsync(a => a.Id == editeur.Id))
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
                        }

                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Id = record.Id,
                            Message = $"Le livre {viewModel.MainTitle} a été créé avec succès."
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

            public static async Task<OperationStateVM<TbookExemplary>> CreateExemplaryAsync(long idBook, LivreExemplaryVM viewModel)
            {
                try
                {
                    if (viewModel == null)
                    {
                        return new OperationStateVM<TbookExemplary>()
                        {
                            IsSuccess = false,
                            Message = DbServices.ViewModelNullOrEmptyMessage,
                        };
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var bookRecord = await context.Tbook.SingleOrDefaultAsync(c => c.Id == idBook);
                        if (bookRecord == null)
                        {
                            return new OperationStateVM<TbookExemplary>()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordNotExistMessage
                            };
                        }

                        double? nullablePrice;
                        if (viewModel.IsPriceUnavailable == false)
                        {
                            nullablePrice = viewModel.Price;
                        }
                        else
                        {
                            nullablePrice = null;
                        }

                        string group = $"{DateTime.UtcNow.Day}-{IdHelpers.GenerateMalaxedGUID(5)}-{DateTime.UtcNow.Year:00}";
                        while (await context.TbookExemplary.AnyAsync(a => a.NoGroup == group) == true)
                        {
                            group = $"{DateTime.UtcNow.Day}-{IdHelpers.GenerateMalaxedGUID(5)}-{DateTime.UtcNow.Year:00}";
                            if (await context.TbookExemplary.AnyAsync(a => a.NoGroup == group) == false)
                            {
                                break;
                            }
                        }
#warning Changer le type NoExemplary en integer
                        var minNoExemplary = await context.TbookExemplary.Where(w => w.IdBook == bookRecord.Id)?.Select(s => Convert.ToInt32(s.NoExemplary))?.ToListAsync() ?? null;
                        var MinNoExemplary = minNoExemplary == null || minNoExemplary.Count == 0 ? 1 : minNoExemplary.Max();
                        var MaxNoExemplary = MinNoExemplary + viewModel.NbExemplaire;
                        
                        List<TbookExemplary> recordCollection = new List<TbookExemplary>();
                        for (int i = MinNoExemplary; i <= MaxNoExemplary; i++)
                        {
                            var record = new TbookExemplary()
                            {
                                IdBook = idBook,
                                IdContactSource = viewModel.IdContactSource,
                                DateAjout = viewModel.DateAjout.ToString(),
                                DateEdition = viewModel.DateEdition?.ToString(),
                                DateAcquisition = viewModel.DateAcquisition,
                                DateRemise = viewModel.DateRemiseLivre?.ToString(),
                                TypeAcquisition = viewModel.Source,
                                Observations = viewModel.Observations,
                                NoExemplary = i.ToString("00"),
                                Quantity = 1,
                                NoGroup = group,
                                Price = nullablePrice,
                                DeviceName = viewModel.DeviceName
                            };


                            recordCollection.Add(record);
                        }

                        await context.TbookExemplary.AddRangeAsync(recordCollection);
                        await context.SaveChangesAsync();

                        List<TbookEtat> etatCollection = new List<TbookEtat>();
                        foreach (var record in recordCollection)
                        {
                            var recordEtat = new TbookEtat()
                            {
                                IdBookExemplary = record.Id,
                                DateAjout = viewModel.DateAjout.ToString(),
                                Etat = viewModel.Etat.Etat,
                                Observations = viewModel.Observations,
                                TypeVerification = (byte)viewModel.Etat.TypeVerification,
                            };

                            etatCollection.Add(recordEtat);
                        }

                        await context.TbookEtat.AddRangeAsync(etatCollection);
                        await context.SaveChangesAsync();

                        return new OperationStateVM<TbookExemplary>(recordCollection)
                        {
                            IsSuccess = true,
                            //Id = record.Id,
                            Message = $"{viewModel.NbExemplaire} exemplaire(s) ont été enregistrés distinctement avec succès."
                        };
                    }
                        
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return new OperationStateVM<TbookExemplary>()
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

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var record = await context.Tbook.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                        if (record == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
                            };
                        }

                        var isExist = await IsBookExistAsync(viewModel, true, record.Id);
                        if (isExist)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordAlreadyExistMessage
                            };
                        }

                        record.DateEdition = DateTime.UtcNow.ToString();
                        record.DateParution = viewModel.Publication.DateParution?.ToString();
                        record.IsJourParutionKnow = viewModel.Publication.IsJourParutionKnow ? 1 : 0;
                        record.IsMoisParutionKnow = viewModel.Publication.IsMoisParutionKnow ? 1 : 0;
                        record.MainTitle = viewModel.MainTitle;
                        record.CountOpening = viewModel.CountOpening;
                        record.Resume = viewModel.Description.Resume;
                        record.Notes = viewModel.Description.Notes;
                        record.MinAge = viewModel.ClassificationAge?.MinAge;
                        record.MaxAge = viewModel.ClassificationAge?.MaxAge;
                        record.Langue = viewModel.Publication?.Langue;
                        record.Pays = viewModel.Publication?.Pays;
                        record.Price = viewModel.Publication?.Price ?? 0;
                        record.DeviceName = viewModel.Publication?.DeviceName;

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
                                foreach (ContactVM author in viewModel.Auteurs)
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
                                foreach (ContactVM editeur in viewModel.Publication.Editeurs)
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

                        if (viewModel.Format != null)
                        {
                            var recordFormat = await context.TbookFormat.SingleOrDefaultAsync(a => a.Id == record.Id);
                            if (recordFormat == null)
                            {
                                return new OperationStateVM()
                                {
                                    IsSuccess = false,
                                    Message = DbServices.RecordNotExistMessage,
                                };
                            }

                            recordFormat.Format = viewModel.Format.Format;
                            recordFormat.NbOfPages = viewModel.Format.NbOfPages;
                            recordFormat.Largeur = viewModel.Format.Largeur;
                            recordFormat.Epaisseur = viewModel.Format.Epaisseur;
                            recordFormat.Hauteur = viewModel.Format.Hauteur;
                            _ = context.TbookFormat.Update(recordFormat);
                        }

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

            public static async Task<bool> IsExistAsync(string mainTitle, string format, string langue)
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();
                    return await context.Tbook.AnyAsync(a => a.MainTitle == mainTitle && a.TbookFormat.Format == format && a.Langue == langue);                    
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return false;
                }
            }

            #region Helpers
            private static async Task CompleteModelInfos(LibraryDbContext context,Tbook model)
            {
                try
                {
                    if (context == null || model == null)
                    {
                        return;
                    }

                    model.TbookFormat = await context.TbookFormat.SingleOrDefaultAsync(s => s.Id == model.Id);
                    model.TbookIdentification = await context.TbookIdentification.SingleOrDefaultAsync(s => s.Id == model.Id);
                    
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return;
                }
            }
            private static async Task<bool> IsBookExistAsync(LivreVM viewModel, bool isEdit = false, long? modelId = null)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tbook> existingItemList = null;

                        if (!isEdit)
                        {
                            existingItemList = await context.Tbook.Where(c => c.MainTitle.ToLower() == viewModel.MainTitle.Trim().ToLower()).ToListAsync();
                        }
                        else
                        {
                            existingItemList = await context.Tbook.Where(c => c.Id != (long)modelId && c.MainTitle.ToLower() == viewModel.MainTitle.Trim().ToLower()).ToListAsync();
                        }

                        if (existingItemList != null && existingItemList.Any())
                        {
                            string lang = viewModel.Publication?.Langue?.ToLower() ?? null;
                            foreach (var item in existingItemList)
                            {
                                var isFormatExist = await context.TbookFormat.AnyAsync(c => c.Id == item.Id && c.Format.ToLower() == viewModel.Format.Format.Trim().ToLower());
                                if (isFormatExist && item.Langue?.ToLower() == lang)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                        
                    return false;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return true;
                }
            }
            private static LivreEtatVM ViewModelConverter(TbookEtat model)
            {
                try
                {
                    if (model == null) return null;

                    var viewModel = new LivreEtatVM()
                    {
                        Id = model.Id,
                        IdBookExemplary = model.IdBookExemplary,
                        DateVerification = DatesHelpers.Converter.GetDateFromString(model.DateAjout),
                        DateAjout = DatesHelpers.Converter.GetDateFromString(model.DateAjout),
                        Observations = model.Observations,
                        Etat = model.Etat,
                        TypeVerification = (BookTypeVerification)model.TypeVerification,
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

            private static async Task<LivreExemplaryVM> ViewModelConverterAsync(TbookExemplary model)
            {
                try
                {
                    if (model == null) return null;

                    ContactVM contactSource = null;
                    if (model.IdContactSourceNavigation != null)
                    {
                        contactSource = await Contact.ViewModelConverterAsync(model.IdContactSourceNavigation);
                    }

                    var viewModel = new LivreExemplaryVM()
                    {
                        Id = model.Id,
                        IdBook = model.IdBook,
                        IdContactSource = model.IdContactSource,
                        DateAjout = DatesHelpers.Converter.GetDateFromString(model.DateAjout),
                        DateEdition = DatesHelpers.Converter.GetNullableDateFromString(model.DateEdition),
                        DateRemiseLivre = DatesHelpers.Converter.GetNullableDateFromString(model.DateRemise),
                        NbExemplaire = (int)(model.Quantity < int.MinValue || model.Quantity > int.MaxValue ? 0 : model.Quantity),
                        Source = model.TypeAcquisition,
                        NoExemplaire = model.NoExemplary,
                        NoGroup = model.NoGroup,
                        IsExemplarySeparated = !model.NoGroup.IsStringNullOrEmptyOrWhiteSpace(),
                        Observations = model.Observations,
                        Price = model.Price ?? 0,
                        IsPriceUnavailable = model.Price == null,
                        DeviceName = model.DeviceName,
                        ContactSource = contactSource,
                    };

                    var splitDateAcquisition = StringHelpers.SplitWord(model.DateAcquisition, new string[] { "/" });
                    if (splitDateAcquisition != null && splitDateAcquisition.Length > 0)
                    {
                        if (splitDateAcquisition.Length == 1)
                        {
                            viewModel.DateAcquisition = splitDateAcquisition[0];
                            viewModel.YearAcquisition = splitDateAcquisition[0];
                        }
                        else if (splitDateAcquisition.Length == 2)
                        {
                            viewModel.DateAcquisition = $"{splitDateAcquisition[0]}/{splitDateAcquisition[1]}";
                            viewModel.MonthAcquisition = splitDateAcquisition[0];
                            viewModel.YearAcquisition = splitDateAcquisition[1];
                        }
                        else if (splitDateAcquisition.Length == 3)
                        {
                            viewModel.DateAcquisition = $"{splitDateAcquisition[0]}/{splitDateAcquisition[1]}/{splitDateAcquisition[2]}";
                            viewModel.DayAcquisition = splitDateAcquisition[0];
                            viewModel.MonthAcquisition = splitDateAcquisition[1];
                            viewModel.YearAcquisition = splitDateAcquisition[2];
                        }
                    }

                    if (model.TbookEtat != null && model.TbookEtat.Count > 0)
                    {
                        var collection = model.TbookEtat.Select(s => ViewModelConverter(s)).Where(w => w != null).ToList();
                        viewModel.Etats = new ObservableCollection<LivreEtatVM>(collection);
                        var etat = model.TbookEtat.FirstOrDefault(s => s.IdBookExemplary == viewModel.Id);
                        if (etat != null)
                        {
                            viewModel.Etat = ViewModelConverter(etat);
                        }
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
                        IdLibrary = await GetLibraryIdAsync(model.Id),
                        Guid = isGuidCorrect ? guid : Guid.Empty,
                        DateAjout = DatesHelpers.Converter.GetDateFromString(model.DateAjout),
                        DateEdition = DatesHelpers.Converter.GetNullableDateFromString(model.DateEdition),
                        MainTitle = model.MainTitle,
                        CountOpening = model.CountOpening,
                        Description = new LivreDescriptionVM()
                        {
                            Resume = model.Resume,
                            Notes = model.Notes,
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

                    if (model.TbookFormat != null)
                    {
                        viewModel.Format = new LivreFormatVM()
                        {
                            Id = model.TbookFormat.Id,
                            Format = model.TbookFormat.Format,
                            NbOfPages = (short)model.TbookFormat.NbOfPages,
                            Hauteur = model.TbookFormat.Hauteur ?? 0,
                            Epaisseur = model.TbookFormat?.Epaisseur ?? 0,
                            Largeur = model.TbookFormat?.Largeur ?? 0,
                            //Poids = Ajouter dans la base de donnée ?
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
                    IList<ContactVM> authors = await Contact.MultipleVMAsync(model.Id, ContactType.Author);
                    IList<ContactVM> editors = await Contact.MultipleVMAsync(model.Id, ContactType.EditorHouse);
                    IList<CollectionVM> collections = await Collection.MultipleVmInBookAsync(model.Id, CollectionTypeEnum.Collection);
                    
                    viewModel.TitresOeuvre = titres != null && titres.Any() ? new ObservableCollection<string>(titres) : new ObservableCollection<string>();
                    if (viewModel.TitresOeuvre != null && viewModel.TitresOeuvre.Any())
                    {
                        viewModel.TitresOeuvreStringList = StringHelpers.JoinStringArray(viewModel.TitresOeuvre?.Select(s => s)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                    }

                    viewModel.Auteurs = authors != null && authors.Any() ? new ObservableCollection<ContactVM>(authors) : new ObservableCollection<ContactVM>();
                    if (viewModel.Auteurs != null && viewModel.Auteurs.Any())
                    {
                        viewModel.AuteursStringList = StringHelpers.JoinStringArray(viewModel.Auteurs?.Select(s => $"{s.NomNaissance} {s.Prenom}")?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                    }

                    viewModel.Publication = new LivrePublicationVM()
                    {
                        Pays = model.Pays,
                        Langue = model.Langue,
                        Price = model.Price,
                        DeviceName = model.DeviceName,
                        DateParution = DatesHelpers.Converter.GetNullableDateFromString(model.DateParution),
                        IsJourParutionKnow = model.IsJourParutionKnow >= 1,
                        IsMoisParutionKnow = model.IsMoisParutionKnow >= 1,
                        IsJourParutionVisible = model.IsJourParutionKnow >= 1,
                        IsMoisParutionVisible = model.IsMoisParutionKnow >= 1,
                        Collections = collections != null && collections.Any() ? new ObservableCollection<CollectionVM>(collections) : new ObservableCollection<CollectionVM>(),
                        Editeurs = editors != null && editors.Any() ? new ObservableCollection<ContactVM>(editors) : new ObservableCollection<ContactVM>(),
                    };

                    if (viewModel.Publication != null)
                    {
                        if (viewModel.Publication.Collections != null && viewModel.Publication.Collections.Any())
                        {
                            viewModel.Publication.CollectionsStringList = StringHelpers.JoinStringArray(viewModel.Publication.Collections?.Select(s => s.Name)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                        }

                        if (viewModel.Publication.Editeurs != null && viewModel.Publication.Editeurs.Any())
                        {
                            viewModel.Publication.EditeursStringList = StringHelpers.JoinStringArray(viewModel.Publication.Editeurs?.Select(s => s.SocietyName)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                        }
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

            public static LivreVM DeepCopy(LivreVM viewModelToCopy)
            {
                try
                {
                    if (viewModelToCopy == null) return null;

                    LivreVM newViewModel = new LivreVM();

                    return DeepCopy(newViewModel, viewModelToCopy);
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }

            public static LivreVM DeepCopy(LivreVM viewModel, LivreVM viewModelToCopy)
            {
                try
                {
                    if (viewModel == null) return null;
                    if (viewModelToCopy == null) return null;

                    viewModel.Id = viewModelToCopy.Id;
                    viewModel.IdLibrary = viewModelToCopy.IdLibrary;
                    viewModel.Guid = viewModelToCopy.Guid;
                    viewModel.DateAjout = viewModelToCopy.DateAjout;
                    viewModel.DateEdition = viewModelToCopy.DateEdition;
                    viewModel.MainTitle = viewModelToCopy.MainTitle;
                    viewModel.CountOpening = viewModelToCopy.CountOpening;
                    viewModel.JaquettePath = viewModelToCopy.JaquettePath;

                    if (viewModelToCopy.TitresOeuvre != null && viewModelToCopy.TitresOeuvre.Any())
                    {
                        if (viewModel.TitresOeuvre == null)
                        {
                            viewModel.TitresOeuvre = new ObservableCollection<string>();
                        }

                        viewModel.TitresOeuvre.Clear();
                        foreach (var titre in viewModelToCopy.TitresOeuvre)
                        {
                            viewModel.TitresOeuvre.Add(titre);
                        }

                        viewModel.TitresOeuvreStringList = StringHelpers.JoinStringArray(viewModel.TitresOeuvre?.Select(s => s)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                    }

                    if (viewModelToCopy.Auteurs != null && viewModelToCopy.Auteurs.Any())
                    {
                        if (viewModel.Auteurs == null)
                        {
                            viewModel.Auteurs = new ObservableCollection<ContactVM>();
                        }

                        viewModel.Auteurs.Clear();
                        foreach (var contact in viewModelToCopy.Auteurs)
                        {
                            viewModel.Auteurs.Add(contact);
                        }

                        viewModel.AuteursStringList = StringHelpers.JoinStringArray(viewModel.Auteurs?.Select(s => $"{s.NomNaissance} {s.Prenom}")?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                    }

                    if (viewModelToCopy.Identification != null)
                    {
                        if (viewModel.Identification == null)
                        {
                            viewModel.Identification = new LivreIdentificationVM();
                        }

                        viewModel.Identification.Id = viewModelToCopy.Identification.Id;
                        viewModel.Identification.ISBN = viewModelToCopy.Identification.ISBN;
                        viewModel.Identification.ISBN10 = viewModelToCopy.Identification.ISBN10;
                        viewModel.Identification.ISBN13 = viewModelToCopy.Identification.ISBN13;
                        viewModel.Identification.ISSN = viewModelToCopy.Identification.ISSN;
                        viewModel.Identification.ASIN = viewModelToCopy.Identification.ASIN;
                        viewModel.Identification.CodeBarre = viewModelToCopy.Identification.CodeBarre;
                        viewModel.Identification.Cotation = viewModelToCopy.Identification.Cotation;
                    }

                    if (viewModelToCopy.ClassificationAge != null)
                    {
                        if (viewModel.ClassificationAge == null)
                        {
                            viewModel.ClassificationAge = new LivreClassificationAgeVM();
                        }

                        viewModel.ClassificationAge.MinAge = viewModelToCopy.ClassificationAge.MinAge;
                        viewModel.ClassificationAge.MaxAge = viewModelToCopy.ClassificationAge.MaxAge;
                    }

                    if (viewModelToCopy.Publication != null)
                    {
                        if (viewModel.Publication == null)
                        {
                            viewModel.Publication = new LivrePublicationVM();
                        }

                        viewModel.Publication.Pays = viewModelToCopy.Publication.Pays;
                        viewModel.Publication.Langue = viewModelToCopy.Publication.Langue;
                        viewModel.Publication.Price = viewModelToCopy.Publication.Price;
                        viewModel.Publication.DeviceName = viewModelToCopy.Publication.DeviceName;
                        viewModel.Publication.DateParution = viewModelToCopy.Publication.DateParution;
                        viewModel.Publication.IsJourParutionKnow = viewModelToCopy.Publication.IsJourParutionKnow;
                        viewModel.Publication.IsMoisParutionKnow = viewModelToCopy.Publication.IsMoisParutionKnow;
                        viewModel.Publication.IsJourParutionVisible = viewModelToCopy.Publication.IsJourParutionVisible;
                        viewModel.Publication.IsMoisParutionVisible = viewModelToCopy.Publication.IsMoisParutionVisible;

                        if (viewModelToCopy.Publication.Editeurs != null && viewModelToCopy.Publication.Editeurs.Any())
                        {
                            if (viewModel.Publication.Editeurs == null)
                            {
                                viewModel.Publication.Editeurs = new ObservableCollection<ContactVM>();
                            }

                            viewModel.Publication.Editeurs.Clear();
                            foreach (var editeur in viewModelToCopy.Publication.Editeurs)
                            {
                                viewModel.Publication.Editeurs.Add(editeur);
                            }

                            viewModel.Publication.EditeursStringList = StringHelpers.JoinStringArray(viewModel.Publication.Editeurs?.Select(s => s.SocietyName)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                        }

                        if (viewModelToCopy.Publication.Collections != null && viewModelToCopy.Publication.Collections.Any())
                        {
                            if (viewModel.Publication.Collections == null)
                            {
                                viewModel.Publication.Collections = new ObservableCollection<CollectionVM>();
                            }

                            viewModel.Publication.Collections.Clear();
                            foreach (var collection in viewModelToCopy.Publication.Collections)
                            {
                                viewModel.Publication.Collections.Add(collection);
                            }

                            viewModel.Publication.CollectionsStringList = StringHelpers.JoinStringArray(viewModel.Publication.Collections?.Select(s => s.Name)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                        }
                    }

                    if (viewModelToCopy.Format != null)
                    {
                        if (viewModel.Format == null)
                        {
                            viewModel.Format = new LivreFormatVM();
                        }

                        viewModel.Format.Format = viewModelToCopy.Format.Format;
                        viewModel.Format.NbOfPages = viewModelToCopy.Format.NbOfPages;
                        viewModel.Format.Epaisseur = viewModelToCopy.Format.Epaisseur;
                        viewModel.Format.Hauteur = viewModelToCopy.Format.Hauteur;
                        viewModel.Format.Largeur = viewModelToCopy.Format.Largeur;
                        viewModel.Format.Id = viewModelToCopy.Format.Id;
                    }

                    if (viewModelToCopy.Description != null)
                    {
                        if (viewModel.Description == null)
                        {
                            viewModel.Description = new LivreDescriptionVM();
                        }

                        viewModel.Description.Resume = viewModelToCopy.Description.Resume;
                        viewModel.Description.Notes = viewModelToCopy.Description.Notes;
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
