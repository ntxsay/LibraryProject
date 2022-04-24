using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
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
        public partial struct Book
        {
            #region All
            /// <summary>
            /// Retourne tous les objets de la base de données
            /// </summary>
            /// <typeparam name="T">Modèle de base de données</typeparam>
            /// <returns></returns>
            public static async Task<IList<TbookPret>> AllPretsAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<TbookPret> collection = await context.TbookPret.ToListAsync();
                        foreach (TbookPret item in collection)
                        {
                            await CompleteModelInfos(item);
                        }

                        if (collection == null || !collection.Any()) return Enumerable.Empty<TbookPret>().ToList();

                        return collection;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<TbookPret>().ToList();
                }
            }

            /// <summary>
            /// Retourne tous les modèles de vue depuis la base de données
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <returns></returns>
            public static async Task<IList<LivrePretVM>> AllPretsVMAsync()
            {
                try
                {
                    var collection = await AllPretsAsync();
                    if (!collection.Any()) return Enumerable.Empty<LivrePretVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(s => s.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<LivrePretVM>().ToList();
                }
            }

            public static async Task<IList<long>> AllPretsIdAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TbookPret.Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<long>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdInContactAsync(long idContact)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TbookPret.Where(w => w.IdContact == idContact).Select(s => s.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<long>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdInBookExemplaryAsync(long idBookExemplary)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TbookPret.Where(w => w.IdBookExemplary == idBookExemplary).Select(s => s.Id).ToListAsync();
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
            public static async Task<long> CountInContactAsync(long idContact, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TbookPret.LongCountAsync(w => w.IdContact == idContact, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return 0;
                }
            }

            public static async Task<long> CountInBookExemplaryAsync(long idBookExemplary, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TbookPret.LongCountAsync(w => w.IdBookExemplary == idBookExemplary, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return 0;
                }
            }



            #endregion

            #region Single
            public static async Task<long?> GetParentIdAsync(long idExemplary, LibraryDbContext _context = null)
            {
                try
                {
                    using (LibraryDbContext context = _context ?? new LibraryDbContext())
                    {
                        var id = (await context.TbookExemplary.SingleOrDefaultAsync(w => w.Id == idExemplary))?.IdBook;
                        return id;
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
            public static async Task<TbookPret> SinglePretAsync(long id)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var s = await context.TbookPret.SingleOrDefaultAsync(d => d.Id == id);
                        await CompleteModelInfos(s);
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
            public static async Task<LivrePretVM> SinglePretVMAsync(long id)
            {
                return await ViewModelConverterAsync(await SinglePretAsync(id));
            }
            #endregion
            public static async Task<IList<TbookPret>> GetBookPretAsync(long idItem, BookPretFrom bookFrom = BookPretFrom.BookExemplaire, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<TbookPret> preCollection = new List<TbookPret>();
                        switch (bookFrom)
                        {
                            case BookPretFrom.Emprunteur:
                                preCollection = await context.TbookPret.Where(w => w.IdContact == idItem).ToListAsync(cancellationToken);
                                break;
                            case BookPretFrom.BookExemplaire:
                                preCollection = await context.TbookPret.Where(w => w.IdBookExemplary == idItem).ToListAsync(cancellationToken);
                                break;

                            case BookPretFrom.Book:
                                var exemplaries = await BookExemplary.AllIdInBookAsync(idItem);
                                if (exemplaries != null && exemplaries.Any())
                                {
                                    foreach (var idExemplary in exemplaries)
                                    {
                                        var items = await context.TbookPret.Where(w => w.IdBookExemplary == idExemplary).ToListAsync(cancellationToken);
                                        if (items != null && items.Any())
                                        {
                                            preCollection.AddRange(items);
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        if (preCollection != null && preCollection.Any())
                        {
                            foreach (TbookPret item in preCollection)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    return preCollection;
                                }
                                await CompleteModelInfos(item);
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

            public static async Task<IList<LivrePretVM>> GetBookPretVMAsync(long idItem, BookPretFrom bookFrom = BookPretFrom.BookExemplaire, CancellationToken cancellationToken = default)
            {
                try
                {
                    var collection = await GetBookPretAsync(idItem, bookFrom, cancellationToken);
                    if (!collection.Any()) return Enumerable.Empty<LivrePretVM>().ToList();

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

            public static async Task<int> CountPretInBookAsync(long idBook, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var exemplariesListId = await GetBookExemplaryListOfIdAsync(idBook, cancellationToken);
                        List<TbookPret> tbookPrets = new List<TbookPret>();
                        foreach (var idExemplary in exemplariesListId)
                        {
                            List<TbookPret> _tbookPrets = await context.TbookPret.Where(w => w.IdBookExemplary == idExemplary && !w.DateRemiseUser.IsStringNullOrEmptyOrWhiteSpace()).ToListAsync(cancellationToken);
                            if (_tbookPrets != null && _tbookPrets.Any())
                            {
                                for (int i = 0; i < _tbookPrets.Count; i++)
                                {
                                    DateTime? dateRemise = DatesHelpers.Converter.GetNullableDateFromString(_tbookPrets[i].DateRemise);
                                    TimeSpan? timeRemise = DatesHelpers.Converter.GetNullableTimeSpanFromString(_tbookPrets[i].TimeRemise);
                                    
                                    if (dateRemise.HasValue)
                                    {
                                        var compare = dateRemise.Value.CompareDate(DateTime.UtcNow);
                                        if (compare != DateCompare.DateSuperieur && compare != DateCompare.DateEgal)
                                        {
                                            _tbookPrets.RemoveAt(i);
                                            i = 0;
                                        }
                                    }
                                    
                                }

                                if (_tbookPrets != null && _tbookPrets.Any())
                                {
                                    tbookPrets.AddRange(_tbookPrets);
                                }
                            }
                        }

                        return tbookPrets.Count;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return 0;
                }
            }

            public static async Task<OperationStateVM> CreatePretAsync(long idExemplary, LivrePretVM viewModel)
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

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        TbookExemplary bookExemplaryRecord = await context.TbookExemplary.SingleOrDefaultAsync(c => c.Id == idExemplary);
                        if (bookExemplaryRecord == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = RecordNotExistMessage
                            };
                        }

                        var recordEtatAvantPret = new TbookEtat()
                        {
                            IdBookExemplary = bookExemplaryRecord.Id,
                            DateAjout = DateTime.UtcNow.ToString(),
                            Etat = viewModel.EtatAvantPret.Etat,
                            Observations = viewModel.EtatAvantPret.Observations,
                            TypeVerification = (byte)viewModel.EtatAvantPret.TypeVerification,
                        };

                        await context.TbookEtat.AddAsync(recordEtatAvantPret);
                        await context.SaveChangesAsync();

                        var record = new TbookPret()
                        {
                            IdBookExemplary = bookExemplaryRecord.Id,
                            DatePret = viewModel.DatePret.ToUniversalTime().ToString(),
                            TimePret = !viewModel.TimePret.HasValue ? null : viewModel.TimePret.Value.ToString(),
                            IdContact = viewModel.Emprunteur.Id,
                            IdEtatBefore = recordEtatAvantPret.Id,
                            DateRemise = viewModel.DateRemise.HasValue ? viewModel.DateRemise.Value.ToUniversalTime().ToString() : null,
                            TimeRemise = !viewModel.TimeRemise.HasValue ? null : viewModel.TimeRemise.Value.ToString(),
                        };

                        await context.TbookPret.AddAsync(record);
                        await context.SaveChangesAsync();

                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            //Id = record.Id,
                            Message = $"Le prêt a été accordé avec succès."
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

            public static async Task<OperationStateVM> EditPretAsync(long idExemplary, LivrePretVM viewModel)
            {
                try
                {
                    if (viewModel == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = ViewModelNullOrEmptyMessage,
                        };
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        TbookExemplary bookExemplaryRecord = await context.TbookExemplary.SingleOrDefaultAsync(c => c.Id == idExemplary);
                        if (bookExemplaryRecord == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = RecordNotExistMessage
                            };
                        }

                        TbookPret bookPretRecord = await context.TbookPret.SingleOrDefaultAsync(c => c.Id == viewModel.Id);
                        if (bookPretRecord == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = RecordNotExistMessage
                            };
                        }

                        bookPretRecord.IdBookExemplary = bookExemplaryRecord.Id;
                        bookPretRecord.DatePret = viewModel.DatePret.ToUniversalTime().ToString();
                        bookPretRecord.TimePret = !viewModel.TimePret.HasValue ? null : viewModel.TimePret.Value.ToString();
                        bookPretRecord.IdContact = viewModel.Emprunteur.Id;
                        bookPretRecord.DateRemise = viewModel.DateRemise.HasValue ? viewModel.DateRemise.Value.ToUniversalTime().ToString() : null;
                        bookPretRecord.TimeRemise = !viewModel.TimeRemise.HasValue ? null : viewModel.TimeRemise.Value.ToString();
                        context.TbookPret.Update(bookPretRecord);

                        TbookEtat recordEtatAvantPret = await context.TbookEtat.SingleOrDefaultAsync(c => c.Id == bookPretRecord.IdEtatBefore);
                        if (recordEtatAvantPret == null)
                        {
                            recordEtatAvantPret = new TbookEtat()
                            {
                                IdBookExemplary = bookExemplaryRecord.Id,
                                DateAjout = viewModel.EtatAvantPret.DateAjout.ToUniversalTime().ToString(),
                                Etat = viewModel.EtatAvantPret.Etat,
                                Observations = viewModel.EtatAvantPret.Observations,
                                TypeVerification = (byte)viewModel.EtatAvantPret.TypeVerification,
                            };

                            await context.TbookEtat.AddAsync(recordEtatAvantPret);
                            await context.SaveChangesAsync();

                            bookPretRecord.IdEtatBefore = recordEtatAvantPret.Id;
                        }
                        else
                        {
                            recordEtatAvantPret.IdBookExemplary = bookExemplaryRecord.Id;
                            //recordEtatAvantPret.DateAjout = viewModel.EtatAvantPret.DateAjout.ToUniversalTime().ToString();
                            recordEtatAvantPret.Etat = viewModel.EtatAvantPret.Etat;
                            recordEtatAvantPret.Observations = viewModel.EtatAvantPret.Observations;
                            recordEtatAvantPret.TypeVerification = (byte)viewModel.EtatAvantPret.TypeVerification;
                            context.TbookEtat.Update(recordEtatAvantPret);
                        }

                        await context.SaveChangesAsync();

                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            //Id = record.Id,
                            Message = $"Le prêt a été mis à jour avec succès."
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

            public static async Task<OperationStateVM> ReturnBookAsync(long idPret, long idExemplary, string etat, string observations)
            {
                try
                {
                    if (etat.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = "L'état du livre au retour doit être renseigné.",
                        };
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        bool isBookExemplary = await context.TbookExemplary.AnyAsync(c => c.Id == idExemplary);
                        if (!isBookExemplary)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordNotExistMessage
                            };
                        }

                        TbookPret bookPretRecord = await context.TbookPret.SingleOrDefaultAsync(c => c.Id == idPret);
                        if (bookPretRecord == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordNotExistMessage
                            };
                        }

                        TbookEtat recordEtatApresPret = await context.TbookEtat.SingleOrDefaultAsync(c => c.Id == bookPretRecord.IdEtatAfter);
                        if (recordEtatApresPret == null)
                        {
                            recordEtatApresPret = new TbookEtat()
                            {
                                IdBookExemplary = idExemplary,
                                DateAjout = DateTime.UtcNow.ToString(),
                                Etat = etat,
                                Observations = observations,
                                TypeVerification = (byte)BookTypeVerification.ApresPret,
                            };

                            await context.TbookEtat.AddAsync(recordEtatApresPret);
                            await context.SaveChangesAsync();

                            bookPretRecord.IdEtatAfter = recordEtatApresPret.Id;
                        }
                        else
                        {
                            recordEtatApresPret.IdBookExemplary = idExemplary;
                            recordEtatApresPret.DateAjout = DateTime.UtcNow.ToString();
                            recordEtatApresPret.Etat = etat;
                            recordEtatApresPret.Observations = observations;
                            recordEtatApresPret.TypeVerification = (byte)BookTypeVerification.ApresPret;
                            context.TbookEtat.Update(recordEtatApresPret);
                        }

                        bookPretRecord.DateRemiseUser = DateTime.UtcNow.ToString();

                        context.TbookPret.Update(bookPretRecord);
                        await context.SaveChangesAsync();

                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            //Id = record.Id,
                            Message = $"L'exemplaire a été restitué avec succès."
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


            #region Helpers
            private static async Task CompleteModelInfos(TbookPret model)
            {
                try
                {
                    if (model == null)
                    {
                        return;
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        model.IdBookExemplaryNavigation = await context.TbookExemplary.SingleOrDefaultAsync(s => s.Id == model.IdBookExemplary);
                        model.IdContactNavigation = await context.Tcontact.SingleOrDefaultAsync(s => s.Id == model.IdContact);
                        model.IdEtatBeforeNavigation = await context.TbookEtat.SingleOrDefaultAsync(s => s.Id == model.IdEtatBefore);
                        if (model.IdEtatAfter != null)
                        {
                            model.IdEtatAfterNavigation = await context.TbookEtat.SingleOrDefaultAsync(s => s.Id == (long)model.IdEtatAfter);
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

            private static async Task<LivrePretVM> ViewModelConverterAsync(TbookPret model)
            {
                try
                {
                    if (model == null) return null;

                    ContactVM emprunteur = null;
                    if (model.IdContactNavigation != null)
                    {
                        emprunteur = await Contact.ViewModelConverterAsync(model.IdContactNavigation);
                    }

                    LivreExemplaryVM exemplary = null;
                    if (model.IdBookExemplaryNavigation != null)
                    {
                        exemplary = await BookExemplary.ViewModelConverterAsync(model.IdBookExemplaryNavigation);
                    }

                    var viewModel = new LivrePretVM()
                    {
                        Id = model.Id,
                        IdBookExemplary = model.IdBookExemplary,
                        Exemplary = exemplary,
                        //IdBook = model.IdBook,
                        IdEmprunteur = model.IdContact,
                        Emprunteur = emprunteur,
                        DatePret = DatesHelpers.Converter.GetDateFromString(model.DatePret).ToLocalTime(),
                        TimePret = DatesHelpers.Converter.GetNullableTimeSpanFromString(model.TimePret),
                        DateRemise = DatesHelpers.Converter.GetNullableDateFromString(model.DateRemise)?.ToLocalTime(),
                        TimeRemise = DatesHelpers.Converter.GetNullableTimeSpanFromString(model.TimeRemise),
                        DateRealRemise = DatesHelpers.Converter.GetNullableDateFromString(model.DateRemiseUser)?.ToLocalTime(),
                    };

                    if (model.IdEtatBeforeNavigation != null)
                    {
                        viewModel.EtatAvantPret = BookEtat.ViewModelConverter(model.IdEtatBeforeNavigation);
                    }

                    if (model.IdEtatAfterNavigation != null)
                    {
                        viewModel.EtatApresPret = BookEtat.ViewModelConverter(model.IdEtatAfterNavigation);
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

            public static LivreExemplaryVM DeepCopy(LivreExemplaryVM viewModelToCopy)
            {
                try
                {
                    if (viewModelToCopy == null) return null;

                    LivreExemplaryVM newViewModel = new LivreExemplaryVM();

                    return DeepCopy(newViewModel, viewModelToCopy);
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return null;
                }
            }

            public static LivreExemplaryVM DeepCopy(LivreExemplaryVM viewModel, LivreExemplaryVM viewModelToCopy)
            {
                try
                {
                    if (viewModel == null) return null;
                    if (viewModelToCopy == null) return null;

                    viewModel.Id = viewModelToCopy.Id;
                    viewModel.IdBook = viewModelToCopy.IdBook;
                    viewModel.IdContactSource = viewModelToCopy.IdContactSource;
                    viewModel.DateAjout = viewModelToCopy.DateAjout;
                    viewModel.DateEdition = viewModelToCopy.DateEdition;
                    viewModel.DateAcquisition = viewModelToCopy.DateAcquisition;
                    viewModel.DateRemiseLivre = viewModelToCopy.DateRemiseLivre;
                    viewModel.DayAcquisition = viewModelToCopy.DayAcquisition;
                    viewModel.MonthAcquisition = viewModelToCopy.MonthAcquisition;
                    viewModel.YearAcquisition = viewModelToCopy.YearAcquisition;
                    viewModel.DayRemise = viewModelToCopy.DayRemise;
                    viewModel.MonthRemise = viewModelToCopy.MonthRemise;
                    viewModel.YearRemise = viewModelToCopy.YearRemise;
                    viewModel.IsPriceUnavailable = viewModelToCopy.IsPriceUnavailable;
                    viewModel.IsVisible = viewModelToCopy.IsVisible;
                    viewModel.NoGroup = viewModelToCopy.NoGroup;
                    viewModel.NbExemplaire = viewModelToCopy.NbExemplaire;
                    viewModel.NoExemplaire = viewModelToCopy.NoExemplaire;
                    viewModel.Observations = viewModelToCopy.Observations;
                    viewModel.ContactSource = viewModelToCopy.ContactSource;
                    viewModel.Source = viewModelToCopy.Source;
                    viewModel.Parent = viewModelToCopy.Parent;
                    viewModel.Price = viewModelToCopy.Price;
                    viewModel.DeviceName = viewModelToCopy.DeviceName;
                    viewModel.Etat = viewModelToCopy.Etat;

                    if (viewModelToCopy.Etats != null && viewModelToCopy.Etats.Any())
                    {
                        if (viewModel.Etats == null)
                        {
                            viewModel.Etats = new ObservableCollection<LivreEtatVM>();
                        }

                        viewModel.Etats.Clear();
                        foreach (var item in viewModelToCopy.Etats)
                        {
                            viewModel.Etats.Add(item);
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
            #endregion
        }
    }
}
