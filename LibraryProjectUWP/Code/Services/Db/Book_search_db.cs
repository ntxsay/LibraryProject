using LibraryProjectUWP.Code.Extensions;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Categorie;
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
    public partial class DbServices
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

            #region Search
            public static async Task<IList<Tbook>> SearchBooksInMainTitle(ResearchItemVM parameters, CancellationToken cancellationToken = default)
            {
                try
                {
                    if (parameters == null)
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }
                    if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tbook> tbooks = new List<Tbook>();
                        var termToLower = parameters.Term.ToLower();

                        switch (parameters.TermParameter)
                        {
                            case Code.Search.Terms.Equals:
                                tbooks = await context.Tbook.Where(w => w.IdLibrary == parameters.IdLibrary && w.MainTitle == parameters.Term).ToListAsync(cancellationToken);
                                break;
                            case Code.Search.Terms.Contains:
                                tbooks = await context.Tbook.Where(w => w.IdLibrary == parameters.IdLibrary && w.MainTitle.Contains(parameters.Term)).ToListAsync(cancellationToken);
                                break;
                            case Code.Search.Terms.StartWith:
                                tbooks = await context.Tbook.Where(w => w.IdLibrary == parameters.IdLibrary && w.MainTitle.StartsWith(parameters.Term)).ToListAsync(cancellationToken);
                                break;
                            case Code.Search.Terms.EndWith:
                                tbooks = await context.Tbook.Where(w => w.IdLibrary == parameters.IdLibrary && w.MainTitle.EndsWith(parameters.Term)).ToListAsync(cancellationToken);
                                break;
                            default:
                                break;
                        }

                        return tbooks;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>().ToList();
                }
            }

            public static async Task<IList<Tbook>> SearchBooksInOtherTitles(ResearchItemVM parameters, CancellationToken cancellationToken = default)
            {
                try
                {
                    if (parameters == null)
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }
                    if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tbook> tbooks = new List<Tbook>();
                        TbookIdEqualityComparer tbookIdEqualityComparer = new TbookIdEqualityComparer();
                        var termToLower = parameters.Term.ToLower();

                        List<TbookOtherTitle> booksTitles = null;
                        switch (parameters.TermParameter)
                        {
                            case Code.Search.Terms.Equals:
                                booksTitles = await context.TbookOtherTitle.Where(w => w.Title == parameters.Term).ToListAsync(cancellationToken);
                                break;
                            case Code.Search.Terms.Contains:
                                booksTitles = await context.TbookOtherTitle.Where(w => w.Title.Contains(parameters.Term)).ToListAsync(cancellationToken);
                                break;
                            case Code.Search.Terms.StartWith:
                                booksTitles = await context.TbookOtherTitle.Where(w => w.Title.StartsWith(parameters.Term)).ToListAsync(cancellationToken);
                                break;
                            case Code.Search.Terms.EndWith:
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

                        return tbooks;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>().ToList();
                }
            }

            public static async Task<IList<Tbook>> SearchBooksInContacts(ResearchItemVM parameters, IEnumerable<ContactRole> contactRoleList, CancellationToken cancellationToken = default)
            {
                try
                {
                    if (parameters == null)
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }
                    if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tbook> tbooks = new List<Tbook>();
                        var termToLower = parameters.Term.ToLower();
                        TbookIdEqualityComparer tbookIdEqualityComparer = new TbookIdEqualityComparer();

                        List<Tcontact> existingItemList = (await Contact.MultipleAsync(null, contactRoleList))?.ToList();
                        List<Tcontact> tcontacts = new List<Tcontact>();

                        if (existingItemList != null && existingItemList.Any())
                        {
                            foreach (var item in existingItemList)
                            {
                                switch (parameters.TermParameter)
                                {
                                    case Code.Search.Terms.Equals:
                                        if (item.TitreCivilite?.ToLower() == termToLower || item.NomNaissance?.ToLower() == termToLower ||
                                            item.Prenom?.ToLower() == termToLower || item.AutresPrenoms?.ToLower() == termToLower ||
                                            item.NomUsage?.ToLower() == termToLower || item.SocietyName?.ToLower() == termToLower)
                                        {
                                            tcontacts.Add(item);
                                        }
                                        break;
                                    case Code.Search.Terms.Contains:
                                        if (item.TitreCivilite?.ToLower().Contains(termToLower) == true || item.NomNaissance?.ToLower().Contains(termToLower) == true ||
                                            item.Prenom?.ToLower().Contains(termToLower) == true || item.AutresPrenoms?.ToLower().Contains(termToLower) == true ||
                                            item.NomUsage?.ToLower().Contains(termToLower) == true || item.SocietyName?.ToLower().Contains(termToLower) == true)
                                        {
                                            tcontacts.Add(item);
                                        }

                                        break;
                                    case Code.Search.Terms.StartWith:
                                        if (item.TitreCivilite?.ToLower().StartsWith(termToLower) == true || item.NomNaissance?.ToLower().StartsWith(termToLower) == true ||
                                            item.Prenom?.ToLower().StartsWith(termToLower) == true || item.AutresPrenoms?.ToLower().StartsWith(termToLower) == true ||
                                            item.NomUsage?.ToLower().StartsWith(termToLower) == true || item.SocietyName?.ToLower().StartsWith(termToLower) == true)
                                        {
                                            tcontacts.Add(item);
                                        }
                                        break;
                                    case Code.Search.Terms.EndWith:
                                        if (item.TitreCivilite?.ToLower().EndsWith(termToLower) == true || item.NomNaissance?.ToLower().EndsWith(termToLower) == true ||
                                            item.Prenom?.ToLower().EndsWith(termToLower) == true || item.AutresPrenoms?.ToLower().EndsWith(termToLower) == true ||
                                            item.NomUsage?.ToLower().EndsWith(termToLower) == true || item.SocietyName?.ToLower().EndsWith(termToLower) == true)
                                        {
                                            tcontacts.Add(item);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        if (tcontacts != null && tcontacts.Any())
                        {
                            var selectedBooks = await GetListOfIdBooksFromContactListAsync(tcontacts.Select(s => s.Id), contactRoleList);
                            List<Tbook> _tbooks = selectedBooks.Select(async s => await SingleAsync(s, parameters.IdLibrary)).Select(t => t.Result).Distinct(tbookIdEqualityComparer).ToList();
                            if (_tbooks != null && _tbooks.Any())
                            {
                                tbooks.AddRange(_tbooks);
                            }
                        }

                        return tbooks;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>().ToList();
                }
            }

            public static async Task<IList<Tbook>> SearchBooksAsync(ResearchItemVM parameters, CancellationToken cancellationToken = default)
            {
                try
                {
                    if (parameters == null)
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }
                    if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tbook> tbooks = new List<Tbook>();
                        TbookIdEqualityComparer tbookIdEqualityComparer = new TbookIdEqualityComparer();
                        var termToLower = parameters.Term.ToLower();

                        if (parameters.SearchInMainTitle == true)
                        {
                            IList<Tbook> _tbooks = await SearchBooksInMainTitle(parameters, cancellationToken);
                            if (_tbooks != null && _tbooks.Any())
                            {
                                tbooks.AddRange(_tbooks);
                            }
                        }

                        if (parameters.SearchInOtherTitles == true)
                        {
                            IList<Tbook> _tbooks = await SearchBooksInOtherTitles(parameters, cancellationToken);
                            if (_tbooks != null && _tbooks.Any())
                            {
                                tbooks.AddRange(_tbooks);
                            }
                        }

                        if (parameters.SearchInAuthors == true)
                        {
                            IList<Tbook> _tbooks = await SearchBooksInContacts(parameters, new ContactRole[] { ContactRole.Author }, cancellationToken);
                            if (_tbooks != null && _tbooks.Any())
                            {
                                tbooks.AddRange(_tbooks);
                            }
                        }

                        if (parameters.SearchInEditors == true)
                        {
                            IList<Tbook> _tbooks = await SearchBooksInContacts(parameters, new ContactRole[] { ContactRole.EditorHouse }, cancellationToken);
                            if (_tbooks != null && _tbooks.Any())
                            {
                                tbooks.AddRange(_tbooks);
                            }
                        }

                        if (parameters.SearchInCollections == true)
                        {

                        }

                        //if (cancellationToken.IsCancellationRequested)
                        //{

                        //}

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

            public static async Task<IList<LivreVM>> SearchBooksVMAsync(ResearchItemVM parameters, CancellationToken cancellationToken = default)
            {
                try
                {
                    if (parameters == null)
                    {
                        return Enumerable.Empty<LivreVM>().ToList();
                    }
                    if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace())
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

            #endregion
        }
    }
}
