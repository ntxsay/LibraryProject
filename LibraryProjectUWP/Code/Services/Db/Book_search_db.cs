using LibraryProjectUWP.Code.Extensions;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LibraryProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        public partial struct Book
        {
            public static async Task<IEnumerable<Tbook>> OrderBooksAsync(long idLibrary, BookGroupVM.OrderBy OrderBy = BookGroupVM.OrderBy.Croissant, BookGroupVM.SortBy SortBy = BookGroupVM.SortBy.Name)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var tBooks = await context.Tbook.Where(c => c.IdLibrary == idLibrary).ToListAsync();
                        if (tBooks == null || !tBooks.Any())
                        {
                            return Enumerable.Empty<Tbook>();
                        }

                        if (SortBy == BookGroupVM.SortBy.Name)
                        {
                            if (OrderBy == BookGroupVM.OrderBy.Croissant)
                            {
                                return tBooks.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.MainTitle);
                            }
                            else if (OrderBy == BookGroupVM.OrderBy.DCroissant)
                            {
                                return tBooks.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.MainTitle);
                            }
                        }
                        else if (SortBy == BookGroupVM.SortBy.DateCreation)
                        {
                            if (OrderBy == BookGroupVM.OrderBy.Croissant)
                            {
                                return tBooks.OrderBy(o => o.DateAjout);
                            }
                            else if (OrderBy == BookGroupVM.OrderBy.DCroissant)
                            {
                                return tBooks.OrderByDescending(o => o.DateAjout);
                            }
                        }

                        return Enumerable.Empty<Tbook>();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>();
                }
            }

            public static IEnumerable<Tbook> OrderBooks(IEnumerable<Tbook> modelList, BookGroupVM.OrderBy OrderBy = BookGroupVM.OrderBy.Croissant, BookGroupVM.SortBy SortBy = BookGroupVM.SortBy.Name)
            {
                try
                {
                    if (modelList == null || !modelList.Any())
                    {
                        return Enumerable.Empty<Tbook>();
                    }

                    if (SortBy == BookGroupVM.SortBy.Name)
                    {
                        if (OrderBy == BookGroupVM.OrderBy.Croissant)
                        {
                            return modelList.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.MainTitle);
                        }
                        else if (OrderBy == BookGroupVM.OrderBy.DCroissant)
                        {
                            return modelList.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.MainTitle);
                        }
                    }
                    else if (SortBy == BookGroupVM.SortBy.DateCreation)
                    {
                        if (OrderBy == BookGroupVM.OrderBy.Croissant)
                        {
                            return modelList.OrderBy(o => o.DateAjout);
                        }
                        else if (OrderBy == BookGroupVM.OrderBy.DCroissant)
                        {
                            return modelList.OrderByDescending(o => o.DateAjout);
                        }
                    }

                    return Enumerable.Empty<Tbook>();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>();
                }
            }

            public static async Task<IEnumerable<Tbook>> FilterBooksAsync(long idLibrary, IEnumerable<long> collectionsId, bool displayUnCategorizedBooks, IEnumerable<object> selectedSCategories
                , BookGroupVM.OrderBy OrderBy = BookGroupVM.OrderBy.Croissant, BookGroupVM.SortBy SortBy = BookGroupVM.SortBy.Name)
            {
                try
                {
                    List<Tbook> PreSelectedModelList = new List<Tbook>();
                    var tBooks = await OrderBooksAsync(idLibrary, OrderBy, SortBy);
                    if (collectionsId != null && collectionsId.Any() || displayUnCategorizedBooks == true ||
                        selectedSCategories != null && selectedSCategories.Any())
                    {
                        using (LibraryDbContext context = new LibraryDbContext())
                        {
                            List<Tbook> vms = new List<Tbook>();
                            foreach (Tbook model in tBooks)
                            {
                                if (collectionsId != null && collectionsId.Any())
                                {
                                    foreach (long id in collectionsId)
                                    {
                                        if (await context.TbookCollections.AnyAsync(a => a.IdBook == model.Id && a.IdCollection == id))
                                        {
                                            if (!vms.Contains(model))
                                            {
                                                vms.Add(model);
                                            }
                                            break;
                                        }
                                    }
                                }

                                if (displayUnCategorizedBooks == false && selectedSCategories != null && selectedSCategories.Any())
                                {
                                    foreach (var item in selectedSCategories)
                                    {
                                        if (item is CategorieLivreVM categorie)
                                        {
                                            if (categorie.BooksId.Any(f => f == model.Id))
                                            {
                                                if (!vms.Contains(model))
                                                {
                                                    vms.Add(model);
                                                }
                                                break;
                                            }
                                        }
                                        else if (item is SubCategorieLivreVM subCategorie)
                                        {
                                            var result = subCategorie.BooksId.Any(a => a == model.Id);
                                            if (result == true)
                                            {
                                                if (!vms.Contains(model))
                                                {
                                                    vms.Add(model);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (displayUnCategorizedBooks == true)
                                {
                                    var uncategorizedBooksId = await Categorie.GetUnCategorizedBooksId(idLibrary);
                                    if (uncategorizedBooksId.Any(f => f == model.Id))
                                    {
                                        if (!vms.Contains(model))
                                        {
                                            vms.Add(model);
                                        }
                                    }
                                }
                            }

                            if (vms.Count > 0)
                            {
                                PreSelectedModelList.AddRange(vms);
                            }

                            vms.Clear();
                            vms = null;
                        }
                    }

                    return PreSelectedModelList == null || PreSelectedModelList.Count == 0 ? tBooks : PreSelectedModelList.Distinct();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>();
                }
            }

            public static async Task<IEnumerable<LivreVM>> FilterBooksVmAsync(long idLibrary, IEnumerable<long> collectionsId, bool displayUnCategorizedBooks, IEnumerable<object> selectedSCategories
                , BookGroupVM.OrderBy OrderBy = BookGroupVM.OrderBy.Croissant, BookGroupVM.SortBy SortBy = BookGroupVM.SortBy.Name)
            {
                try
                {
                    var collection = await FilterBooksAsync(idLibrary, collectionsId, displayUnCategorizedBooks, selectedSCategories, OrderBy, SortBy);
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


            public static async Task<IEnumerable<Tbook>> FilterBooksAsync(long idLibrary, IEnumerable<long> collectionsId, bool displayUnCategorizedBooks, IEnumerable<object> selectedSCategories)
            {
                try
                {
                    List<Tbook> PreSelectedModelList = new List<Tbook>();
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var tBooks = await context.Tbook.Where(c => c.IdLibrary == idLibrary).ToListAsync();
                        if (collectionsId != null && collectionsId.Any() || displayUnCategorizedBooks == true ||
                            selectedSCategories != null && selectedSCategories.Any())
                        {
                            List<Tbook> vms = new List<Tbook>();
                            foreach (Tbook model in tBooks)
                            {
                                if (collectionsId != null && collectionsId.Any())
                                {
                                    foreach (long id in collectionsId)
                                    {
                                        if (await context.TbookCollections.AnyAsync(a => a.IdBook == model.Id && a.IdCollection == id))
                                        {
                                            if (!vms.Contains(model))
                                            {
                                                vms.Add(model);
                                            }
                                            break;
                                        }
                                    }
                                }

                                if (displayUnCategorizedBooks == false && selectedSCategories != null && selectedSCategories.Any())
                                {
                                    foreach (var item in selectedSCategories)
                                    {
                                        if (item is CategorieLivreVM categorie)
                                        {
                                            if (categorie.BooksId.Any(f => f == model.Id))
                                            {
                                                if (!vms.Contains(model))
                                                {
                                                    vms.Add(model);
                                                }
                                                break;
                                            }
                                        }
                                        else if (item is SubCategorieLivreVM subCategorie)
                                        {
                                            var result = subCategorie.BooksId.Any(a => a == model.Id);
                                            if (result == true)
                                            {
                                                if (!vms.Contains(model))
                                                {
                                                    vms.Add(model);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (displayUnCategorizedBooks == true)
                                {
                                    var uncategorizedBooksId = await Categorie.GetUnCategorizedBooksId(idLibrary);
                                    if (uncategorizedBooksId.Any(f => f == model.Id))
                                    {
                                        if (!vms.Contains(model))
                                        {
                                            vms.Add(model);
                                        }
                                    }
                                }
                            }

                            if (vms.Count > 0)
                            {
                                PreSelectedModelList.AddRange(vms);
                            }

                            vms.Clear();
                            vms = null;
                        }

                        return PreSelectedModelList == null || PreSelectedModelList.Count == 0 ? tBooks : PreSelectedModelList.Distinct();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>();
                }
            }

            public static async Task<IEnumerable<LivreVM>> FilterBooksVmAsync(long idLibrary, IEnumerable<long> collectionsId, bool displayUnCategorizedBooks, IEnumerable<object> selectedSCategories)
            {
                try
                {
                    var collection = await FilterBooksAsync(idLibrary, collectionsId, displayUnCategorizedBooks, selectedSCategories);
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

            public static async Task<IEnumerable<Tbook>> FilterBooksAsync(IEnumerable<Tbook> modelList, IEnumerable<long> collectionsId, bool displayUnCategorizedBooks, IEnumerable<object> selectedSCategories)
            {
                try
                {
                    if (modelList == null || !modelList.Any())
                    {
                        return Enumerable.Empty<Tbook>();
                    }

                    List<Tbook> PreSelectedModelList = new List<Tbook>();
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        if (collectionsId != null && collectionsId.Any() || displayUnCategorizedBooks == true ||
                            selectedSCategories != null && selectedSCategories.Any())
                        {
                            List<Tbook> vms = new List<Tbook>();
                            foreach (Tbook model in modelList)
                            {
                                if (collectionsId != null && collectionsId.Any())
                                {
                                    foreach (long id in collectionsId)
                                    {
                                        if (await context.TbookCollections.AnyAsync(a => a.IdBook == model.Id && a.IdCollection == id))
                                        {
                                            if (!vms.Contains(model))
                                            {
                                                vms.Add(model);
                                            }
                                            break;
                                        }
                                    }
                                }

                                if (displayUnCategorizedBooks == false && selectedSCategories != null && selectedSCategories.Any())
                                {
                                    foreach (var item in selectedSCategories)
                                    {
                                        if (item is CategorieLivreVM categorie)
                                        {
                                            if (categorie.BooksId.Any(f => f == model.Id))
                                            {
                                                if (!vms.Contains(model))
                                                {
                                                    vms.Add(model);
                                                }
                                                break;
                                            }
                                        }
                                        else if (item is SubCategorieLivreVM subCategorie)
                                        {
                                            var result = subCategorie.BooksId.Any(a => a == model.Id);
                                            if (result == true)
                                            {
                                                if (!vms.Contains(model))
                                                {
                                                    vms.Add(model);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (displayUnCategorizedBooks == true)
                                {
                                    var uncategorizedBooksId = await Categorie.GetUnCategorizedBooksId(model.IdLibrary);
                                    if (uncategorizedBooksId.Any(f => f == model.Id))
                                    {
                                        if (!vms.Contains(model))
                                        {
                                            vms.Add(model);
                                        }
                                    }
                                }
                            }

                            if (vms.Count > 0)
                            {
                                PreSelectedModelList.AddRange(vms);
                            }

                            vms.Clear();
                            vms = null;
                        }

                        return PreSelectedModelList == null || PreSelectedModelList.Count == 0 ? modelList : PreSelectedModelList.Distinct();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>();
                }
            }

            public static async Task<IEnumerable<LivreVM>> FilterBooksVmAsync(IEnumerable<Tbook> modelList, IEnumerable<long> collectionsId, bool displayUnCategorizedBooks, IEnumerable<object> selectedSCategories)
            {
                try
                {
                    if (modelList == null || !modelList.Any())
                    {
                        return Enumerable.Empty<LivreVM>();
                    }

                    var collection = await FilterBooksAsync(modelList, collectionsId, displayUnCategorizedBooks, selectedSCategories);
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

            public static async Task<IList<Tbook>> SearchBooksAsync(ResearchBookVM parameters, CancellationToken cancellationToken = default)
            {
                try
                {
                    if (parameters == null)
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }
                    if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace() || parameters.SearchIn == null || !parameters.SearchIn.Any())
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tbook> tbooks = new List<Tbook>();
                        TbookIdEqualityComparer tbookIdEqualityComparer = new TbookIdEqualityComparer();

                        foreach (Search.Book.In _searchin in parameters.SearchIn)
                        {
                            switch (_searchin)
                            {
                                case Search.Book.In.MainTitle:
                                    switch (parameters.TermParameter)
                                    {
                                        case Search.Book.Terms.Equals:
                                            tbooks = await context.Tbook.Where(w => w.IdLibrary == parameters.IdLibrary && w.MainTitle == parameters.Term).ToListAsync(cancellationToken);
                                            break;
                                        case Search.Book.Terms.Contains:
                                            tbooks = await context.Tbook.Where(w => w.IdLibrary == parameters.IdLibrary && w.MainTitle.Contains(parameters.Term)).ToListAsync(cancellationToken);
                                            break;
                                        case Search.Book.Terms.StartWith:
                                            tbooks = await context.Tbook.Where(w => w.IdLibrary == parameters.IdLibrary && w.MainTitle.StartsWith(parameters.Term)).ToListAsync(cancellationToken);
                                            break;
                                        case Search.Book.Terms.EndWith:
                                            tbooks = await context.Tbook.Where(w => w.IdLibrary == parameters.IdLibrary && w.MainTitle.EndsWith(parameters.Term)).ToListAsync(cancellationToken);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case Search.Book.In.OtherTitle:
                                    List<TbookOtherTitle> booksTitles = null;
                                    switch (parameters.TermParameter)
                                    {
                                        case Search.Book.Terms.Equals:
                                            booksTitles = await context.TbookOtherTitle.Where(w => w.Title == parameters.Term).ToListAsync(cancellationToken);
                                            break;
                                        case Search.Book.Terms.Contains:
                                            booksTitles = await context.TbookOtherTitle.Where(w => w.Title.Contains(parameters.Term)).ToListAsync(cancellationToken);
                                            break;
                                        case Search.Book.Terms.StartWith:
                                            booksTitles = await context.TbookOtherTitle.Where(w => w.Title.StartsWith(parameters.Term)).ToListAsync(cancellationToken);
                                            break;
                                        case Search.Book.Terms.EndWith:
                                            booksTitles = await context.TbookOtherTitle.Where(w => w.Title.EndsWith(parameters.Term)).ToListAsync(cancellationToken);
                                            break;
                                        default:
                                            break;
                                    }

                                    if (booksTitles != null && booksTitles.Any())
                                    {
                                        List<Tbook> _tbooks = booksTitles.Select(async s => await SingleAsync(s.Id, parameters.IdLibrary)).Select(t => t.Result).Distinct(tbookIdEqualityComparer).ToList();
                                        if (_tbooks != null && _tbooks.Any())
                                        {
                                            tbooks.AddRange(_tbooks);
                                        }
                                    }
                                    break;
                                case Search.Book.In.Author:
                                    if (await context.Tcontact.AnyAsync())
                                    {
                                        List<Tcontact> tcontactsAuthor = null;
                                        switch (parameters.TermParameter)
                                        {
                                            case Search.Book.Terms.Equals:
                                                tcontactsAuthor = await context.Tcontact.Where(w => (!w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && w.NomNaissance.ToUpperInvariant() == parameters.Term.ToUpperInvariant()) ||
                                                (!w.Prenom.IsStringNullOrEmptyOrWhiteSpace() && w.Prenom == parameters.Term) || 
                                                (!w.AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace() && w.AutresPrenoms.ToUpperInvariant() == parameters.Term.ToUpperInvariant()) || 
                                                (!w.SocietyName.IsStringNullOrEmptyOrWhiteSpace() && w.SocietyName.ToUpperInvariant() == parameters.Term.ToUpperInvariant())).ToListAsync(cancellationToken);
                                                break;
                                            case Search.Book.Terms.Contains:
                                                var nomNaissance = await context.Tcontact.Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace()).Select(s => s.NomNaissance.Contains(parameters.Term)).ToListAsync();

                                                //tcontactsAuthor = await context.Tcontact.Where(w => (!w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && w.NomNaissance.ToUpperInvariant().Contains(parameters.Term.ToUpperInvariant())) || 
                                                //(!w.Prenom.IsStringNullOrEmptyOrWhiteSpace() && w.Prenom.ToUpperInvariant().Contains(parameters.Term.ToUpperInvariant())) || 
                                                //(!w.AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace() && w.AutresPrenoms.ToUpperInvariant().Contains(parameters.Term.ToUpperInvariant())) || 
                                                //(!w.SocietyName.IsStringNullOrEmptyOrWhiteSpace() && w.SocietyName.ToUpperInvariant().Contains(parameters.Term.ToUpperInvariant()))).ToListAsync(cancellationToken);
                                                break;
                                            case Search.Book.Terms.StartWith:
                                                tcontactsAuthor = await context.Tcontact.Where(w => (!w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && w.NomNaissance.ToUpperInvariant().StartsWith(parameters.Term.ToUpperInvariant())) || 
                                                (!w.Prenom.IsStringNullOrEmptyOrWhiteSpace() && w.Prenom.ToUpperInvariant().StartsWith(parameters.Term.ToUpperInvariant())) || 
                                                (!w.AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace() && w.AutresPrenoms.ToUpperInvariant().StartsWith(parameters.Term.ToUpperInvariant())) || 
                                                (!w.SocietyName.IsStringNullOrEmptyOrWhiteSpace() && w.SocietyName.ToUpperInvariant().StartsWith(parameters.Term.ToUpperInvariant()))).ToListAsync(cancellationToken);
                                                break;
                                            case Search.Book.Terms.EndWith:
                                                tcontactsAuthor = await context.Tcontact.Where(w => (!w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && w.NomNaissance.ToUpperInvariant().EndsWith(parameters.Term.ToUpperInvariant())) || 
                                                (!w.Prenom.IsStringNullOrEmptyOrWhiteSpace() && w.Prenom.ToUpperInvariant().EndsWith(parameters.Term.ToUpperInvariant())) || 
                                                (!w.AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace() && w.AutresPrenoms.ToUpperInvariant().EndsWith(parameters.Term.ToUpperInvariant())) || 
                                                (!w.SocietyName.IsStringNullOrEmptyOrWhiteSpace() && w.SocietyName.ToUpperInvariant().EndsWith(parameters.Term.ToUpperInvariant()))).ToListAsync(cancellationToken);
                                                break;
                                            default:
                                                break;
                                        }

                                        if (tcontactsAuthor != null && tcontactsAuthor.Any())
                                        {
                                            var selectedBooks = await GetListOfIdBooksFromContactListAsync(tcontactsAuthor.Select(s => s.Id), ContactType.Author);
                                            List<Tbook> _tbooks = selectedBooks.Select(async s => await SingleAsync(s, parameters.IdLibrary)).Select(t => t.Result).Distinct(tbookIdEqualityComparer).ToList();
                                            if (_tbooks != null && _tbooks.Any())
                                            {
                                                tbooks.AddRange(_tbooks);
                                            }
                                        }
                                    }

                                    break;
                                case Search.Book.In.Collection:
                                    break;
                                case Search.Book.In.Editor:
                                    break;
                                default:
                                    break;
                            }

                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }
                        }

                        if (tbooks != null && tbooks.Any())
                        {
                            tbooks = tbooks.Distinct(tbookIdEqualityComparer).ToList();
                            tbooks.ForEach(async (book) => await CompleteModelInfos(context, book));
                            return tbooks;
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

            public static async Task<IList<LivreVM>> SearchBooksVMAsync(ResearchBookVM parameters, CancellationToken cancellationToken = default)
            {
                try
                {
                    if (parameters == null)
                    {
                        return Enumerable.Empty<LivreVM>().ToList();
                    }
                    if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace() || parameters.SearchIn == null || !parameters.SearchIn.Any())
                    {
                        return Enumerable.Empty<LivreVM>().ToList();
                    }

                    var collection = await SearchBooksAsync(parameters, cancellationToken);
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

        }
    }
}
