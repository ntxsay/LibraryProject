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
        public partial struct BookExemplary
        {
            static string NameEmptyMessage = "Le nom du livre doit être renseigné avant l'enregistrement.";
            static readonly string NameAlreadyExistMessage = "Ce livre existe déjà.";

            #region All
            /// <summary>
            /// Retourne tous les objets de la base de données
            /// </summary>
            /// <typeparam name="T">Modèle de base de données</typeparam>
            /// <returns></returns>
            public static async Task<IList<TbookExemplary>> AllExemplaryAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<TbookExemplary> collection = await context.TbookExemplary.ToListAsync();
                        foreach (TbookExemplary item in collection)
                        {
                            await CompleteModelInfos(context, item);
                        }

                        if (collection == null || !collection.Any()) return Enumerable.Empty<TbookExemplary>().ToList();

                        return collection;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<TbookExemplary>().ToList();
                }
            }

            /// <summary>
            /// Retourne tous les modèles de vue depuis la base de données
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <returns></returns>
            public static async Task<IList<LivreExemplaryVM>> AllExemplaryVMAsync()
            {
                try
                {
                    var collection = await AllExemplaryAsync();
                    if (!collection.Any()) return Enumerable.Empty<LivreExemplaryVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(s => s.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<LivreExemplaryVM>().ToList();
                }
            }

            public static async Task<IList<long>> AllIdAsync()
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext()) 
                    { 
                        return await context.TbookExemplary.Select(s => s.Id).ToListAsync(); 
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
                        return await context.TbookExemplary.Where(w => w.IdBook == idBook).Select(s => s.Id).ToListAsync();
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
            public static async Task<long> CountExemplaryInBookAsync(long idBook, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        return await context.TbookExemplary.LongCountAsync(w => w.IdBook == idBook && w.IdBook > -1, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return 0;
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
                    var collection = await GetBookExemplaryAsync(idBook, cancellationToken);
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

            public static async Task<IList<LivreExemplaryVM>> GetAvailableBookExemplaryVMAsync(long idBook, CancellationToken cancellationToken = default)
            {
                try
                {
                    var collection = await GetBookExemplaryAsync(idBook, cancellationToken);
                    if (!collection.Any()) return Enumerable.Empty<LivreExemplaryVM>().ToList();

                    var values = collection.Where(w => w.TbookPret == null || !w.TbookPret.Any()).Select(async s => await ViewModelConverterAsync(s)).Select(s => s.Result).Where(w => w != null).ToList();
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
            public static async Task<long?> GetParentBookIdAsync(long idExemplary, LibraryDbContext _context = null)
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
            public static async Task<TbookExemplary> SingleAsync(long id)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var s = await context.TbookExemplary.SingleOrDefaultAsync(d => d.Id == id);
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
            public static async Task<LivreExemplaryVM> SingleVMAsync(long id)
            {
                return await ViewModelConverterAsync(await SingleAsync(id));
            }
            #endregion


            public static async Task<OperationStateVM<TbookExemplary>> CreateAsync(long idBook, LivreExemplaryVM viewModel)
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
                                NoExemplary = i,
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
            public static async Task<OperationStateVM> UpdateAsync(LivreExemplaryVM viewModel)
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
                        var record = await context.TbookExemplary.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                        if (record == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
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

                        record.DateEdition = DateTime.UtcNow.ToString();
                        record.IdContactSource = viewModel.IdContactSource;
                        record.DateAcquisition = viewModel.DateAcquisition;
                        record.DateRemise = viewModel.DateRemiseLivre?.ToString();
                        record.TypeAcquisition = viewModel.Source;
                        record.Observations = viewModel.Observations;
                        record.Price = nullablePrice;
                        record.DeviceName = viewModel.DeviceName;

                        context.TbookExemplary.Update(record);
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
            private static async Task CompleteModelInfos(LibraryDbContext _context,TbookExemplary model)
            {
                try
                {
                    if (model == null)
                    {
                        return;
                    }

                    using (LibraryDbContext context = _context ?? new LibraryDbContext())
                    {
                        model.TbookPret = await context.TbookPret.Where(s => s.IdBookExemplary == model.Id).ToListAsync();
                        model.TbookEtat = await context.TbookEtat.Where(s => s.IdBookExemplary == model.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return;
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

            public static async Task<LivreExemplaryVM> ViewModelConverterAsync(TbookExemplary model)
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
                        Source = model.TypeAcquisition,
                        NoExemplaire = (int)(model.NoExemplary < int.MinValue || model.NoExemplary > int.MaxValue ? 0 : model.NoExemplary),
                        NoGroup = model.NoGroup,
                        Observations = model.Observations,
                        Price = model.Price ?? 0,
                        IsPriceUnavailable = model.Price == null,
                        DeviceName = model.DeviceName,
                        ContactSource = contactSource,
                    };

                    string dateAcquisition = DatesHelpers.Converter.StringDateToStringDate(model.DateAcquisition, '/', out string dayAcquisition, out string monthAquisition, out string yearAcquisition);
                    viewModel.DateAcquisition = dateAcquisition;
                    viewModel.DayAcquisition = dayAcquisition;
                    viewModel.MonthAcquisition = monthAquisition;
                    viewModel.YearAcquisition = yearAcquisition;

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
