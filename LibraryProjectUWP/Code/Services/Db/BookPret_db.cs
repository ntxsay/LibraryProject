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
        public partial struct BookPret
        {
            static string NameEmptyMessage = "Le nom du livre doit être renseigné avant l'enregistrement.";
            static readonly string NameAlreadyExistMessage = "Ce livre existe déjà.";

            #region All
            /// <summary>
            /// Retourne tous les objets de la base de données
            /// </summary>
            /// <typeparam name="T">Modèle de base de données</typeparam>
            /// <returns></returns>
            public static async Task<IList<TbookPret>> AllAsync()
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
            public static async Task<IList<LivrePretVM>> AllVMAsync()
            {
                try
                {
                    var collection = await AllAsync();
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

            public static async Task<IList<long>> AllIdAsync()
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
                                    foreach(var idExemplary in exemplaries)
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
            public static async Task<TbookPret> SingleAsync(long id)
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
            public static async Task<LivrePretVM> SingleVMAsync(long id)
            {
                return await ViewModelConverterAsync(await SingleAsync(id));
            }
            #endregion

            public static async Task<OperationStateVM> CreateAsync(long idExemplary, LivrePretVM viewModel)
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
                        var bookRecord = await context.TbookExemplary.SingleOrDefaultAsync(c => c.Id == idExemplary);
                        if (bookRecord == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordNotExistMessage
                            };
                        }

                        var recordEtatAvantPret = new TbookEtat()
                        {
                            IdBookExemplary = viewModel.Emprunteur.Id,
                            DateAjout = viewModel.EtatAvantPret.DateAjout.ToUniversalTime().ToString(),
                            Etat = viewModel.EtatAvantPret.Etat,
                            Observations = viewModel.EtatAvantPret.Observations,
                            TypeVerification = (byte)viewModel.EtatAvantPret.TypeVerification,
                        };

                        await context.TbookEtat.AddAsync(recordEtatAvantPret);
                        await context.SaveChangesAsync();

                        var record = new TbookPret()
                        {
                            IdBookExemplary = viewModel.IdBookExemplary,
                            DatePret = viewModel.DatePret.ToUniversalTime().ToString(),
                            IdContact = viewModel.Emprunteur.Id,
                            IdEtatBefore = recordEtatAvantPret.Id,
                            DateRemise = viewModel.DateRemise.HasValue ? viewModel.DateRemise.Value.ToUniversalTime().ToString() : null,
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

            public static async Task<OperationStateVM> ReturnBookAsync(long idPret, long idExemplary, string etat, string observations)
            {
                try
                {
                    if (etat.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.ViewModelNullOrEmptyMessage,
                        };
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var isBookExemplary = await context.TbookExemplary.AnyAsync(c => c.Id == idExemplary);
                        if (!isBookExemplary)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordNotExistMessage
                            };
                        }

                        var bookRecord = await context.TbookPret.SingleOrDefaultAsync(c => c.Id == idPret);
                        if (bookRecord == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordNotExistMessage
                            };
                        }

                        var recordEtatApresPret = new TbookEtat()
                        {
                            IdBookExemplary = idExemplary,
                            DateAjout = DateTime.UtcNow.ToString(),
                            Etat = etat,
                            Observations = observations,
                            TypeVerification = (byte)BookTypeVerification.ApresPret,
                        };

                        await context.TbookEtat.AddAsync(recordEtatApresPret);
                        await context.SaveChangesAsync();

                        bookRecord.IdEtatAfter = recordEtatApresPret.Id;
                        
                        context.TbookPret.Update(bookRecord);
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
                        TbookExemplary record = await context.TbookExemplary.SingleOrDefaultAsync(a => a.Id == Id);
                        if (record == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage
                            };
                        }

                        List<TbookEtat> etats = await context.TbookEtat.Where(w => w.IdBookExemplary == record.Id).ToListAsync();
                        if (etats.Any())
                        {
                            context.TbookEtat.RemoveRange(etats);
                        }

                        List<TbookPret> prets = await context.TbookPret.Where(w => w.IdBookExemplary == record.Id).ToListAsync();
                        if (prets.Any())
                        {
                            context.TbookPret.RemoveRange(prets);
                        }

                        context.TbookExemplary.Remove(record);
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
                        DatePret = DatesHelpers.Converter.GetDateFromString(model.DatePret),
                        DateRemise = DatesHelpers.Converter.GetNullableDateFromString(model.DateRemise),
                    };

                    if (model.IdEtatBeforeNavigation != null)
                    {
                        viewModel.EtatAvantPret = BookEtat.ViewModelConverter(model.IdEtatBeforeNavigation);
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
