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
            public static async Task<IList<Tbook>> SearchBooksInMainTitle(ResearchContainerVM<Tbook> parameter, long idLibrary, CancellationToken cancellationToken = default)
            {
                try
                {
                    if (parameter == null || parameter.CurrentSearchParameter == null)
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }

                    if (parameter.CurrentSearchParameter.Term.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }

                    if (parameter.CurrentSearchParameter.IdLibrary == null || parameter.CurrentSearchParameter.IdLibrary < 1)
                    {
                        return Enumerable.Empty<Tbook>().ToList();
                    }

                    var term = parameter.CurrentSearchParameter.Term;//.ToLower();
                    List<Tbook> tbooks = new List<Tbook>();

                    if (parameter.CurrentSearchParameter.IsSearchFromParentResult == false)
                    {
                        using (LibraryDbContext context = new LibraryDbContext())
                        {
                            switch (parameter.CurrentSearchParameter.TermParameter)
                            {
                                case Code.Search.Terms.Equals:
                                    tbooks = await context.Tbook.Where(w => w.IdLibrary == idLibrary && w.MainTitle == term).ToListAsync(cancellationToken);
                                    break;
                                case Code.Search.Terms.Contains:
                                    tbooks = await context.Tbook.Where(w => w.IdLibrary == idLibrary && w.MainTitle.Contains(term)).ToListAsync(cancellationToken);
                                    break;
                                case Code.Search.Terms.StartWith:
                                    tbooks = await context.Tbook.Where(w => w.IdLibrary == idLibrary && w.MainTitle.StartsWith(term)).ToListAsync(cancellationToken);
                                    break;
                                case Code.Search.Terms.EndWith:
                                    tbooks = await context.Tbook.Where(w => w.IdLibrary == idLibrary && w.MainTitle.EndsWith(term)).ToListAsync(cancellationToken);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (parameter.ParentSearchedResult != null && parameter.ParentSearchedResult.Any())
                        {
                            switch (parameter.CurrentSearchParameter.TermParameter)
                            {
                                case Code.Search.Terms.Equals:
                                    tbooks = parameter.ParentSearchedResult.Where(w => w.IdLibrary == idLibrary && w.MainTitle == term).ToList();
                                    break;
                                case Code.Search.Terms.Contains:
                                    tbooks = parameter.ParentSearchedResult.Where(w => w.IdLibrary == idLibrary && w.MainTitle.Contains(term)).ToList();
                                    break;
                                case Code.Search.Terms.StartWith:
                                    tbooks = parameter.ParentSearchedResult.Where(w => w.IdLibrary == idLibrary && w.MainTitle.StartsWith(term)).ToList();
                                    break;
                                case Code.Search.Terms.EndWith:
                                    tbooks = parameter.ParentSearchedResult.Where(w => w.IdLibrary == idLibrary && w.MainTitle.EndsWith(term)).ToList();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    return tbooks;
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
                                string contactDisplayStyle1 = Contact.DisplayName(item, true, true);
                                string contactDisplayStyle2 = Contact.DisplayName(item, false, true);
                                string contactDisplayStyle3 = Contact.DisplayName(item, true, false);
                                string contactDisplayStyle4 = Contact.DisplayName(item, false, false);
                                
                                switch (parameters.TermParameter)
                                {
                                    case Code.Search.Terms.Equals:
                                        if (contactDisplayStyle1?.ToLower() == termToLower || contactDisplayStyle2?.ToLower() == termToLower ||
                                            contactDisplayStyle3?.ToLower() == termToLower || contactDisplayStyle4?.ToLower() == termToLower)
                                        {
                                            tcontacts.Add(item);
                                        }
                                        break;
                                    case Code.Search.Terms.Contains:
                                        if (contactDisplayStyle1?.ToLower().Contains(termToLower) == true || contactDisplayStyle2?.ToLower().Contains(termToLower) == true ||
                                            contactDisplayStyle3?.ToLower().Contains(termToLower) == true || contactDisplayStyle4?.ToLower().Contains(termToLower) == true)
                                        {
                                            tcontacts.Add(item);
                                        }

                                        break;
                                    case Code.Search.Terms.StartWith:
                                        if (contactDisplayStyle1?.ToLower().StartsWith(termToLower) == true || contactDisplayStyle2?.ToLower().StartsWith(termToLower) == true ||
                                            contactDisplayStyle3?.ToLower().StartsWith(termToLower) == true || contactDisplayStyle4?.ToLower().StartsWith(termToLower) == true)
                                        {
                                            tcontacts.Add(item);
                                        }
                                        break;
                                    case Code.Search.Terms.EndWith:
                                        if (contactDisplayStyle1?.ToLower().EndsWith(termToLower) == true || contactDisplayStyle2?.ToLower().EndsWith(termToLower) == true ||
                                            contactDisplayStyle3?.ToLower().EndsWith(termToLower) == true || contactDisplayStyle4?.ToLower().EndsWith(termToLower) == true)
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

            //public static async Task<IList<Tbook>> SearchBooksAsync(ResearchItemVM parameters, CancellationToken cancellationToken = default)
            //{
            //    try
            //    {
            //        if (parameters == null)
            //        {
            //            return Enumerable.Empty<Tbook>().ToList();
            //        }
            //        if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace())
            //        {
            //            return Enumerable.Empty<Tbook>().ToList();
            //        }

            //        using (LibraryDbContext context = new LibraryDbContext())
            //        {
            //            List<Tbook> tbooks = new List<Tbook>();
            //            TbookIdEqualityComparer tbookIdEqualityComparer = new TbookIdEqualityComparer();
            //            var termToLower = parameters.Term.ToLower();

            //            if (parameters.SearchInMainTitle == true)
            //            {
            //                IList<Tbook> _tbooks = await SearchBooksInMainTitle(parameters, cancellationToken);
            //                if (_tbooks != null && _tbooks.Any())
            //                {
            //                    tbooks.AddRange(_tbooks);
            //                }
            //            }

            //            if (parameters.SearchInOtherTitles == true)
            //            {
            //                IList<Tbook> _tbooks = await SearchBooksInOtherTitles(parameters, cancellationToken);
            //                if (_tbooks != null && _tbooks.Any())
            //                {
            //                    tbooks.AddRange(_tbooks);
            //                }
            //            }

            //            if (parameters.SearchInAuthors == true)
            //            {
            //                IList<Tbook> _tbooks = await SearchBooksInContacts(parameters, new ContactRole[] { ContactRole.Author }, cancellationToken);
            //                if (_tbooks != null && _tbooks.Any())
            //                {
            //                    tbooks.AddRange(_tbooks);
            //                }
            //            }

            //            if (parameters.SearchInEditors == true)
            //            {
            //                IList<Tbook> _tbooks = await SearchBooksInContacts(parameters, new ContactRole[] { ContactRole.EditorHouse }, cancellationToken);
            //                if (_tbooks != null && _tbooks.Any())
            //                {
            //                    tbooks.AddRange(_tbooks);
            //                }
            //            }

            //            if (parameters.SearchInCollections == true)
            //            {

            //            }

            //            //if (cancellationToken.IsCancellationRequested)
            //            //{

            //            //}

            //            if (tbooks != null && tbooks.Any())
            //            {
            //                tbooks = tbooks.Distinct(tbookIdEqualityComparer).ToList();
            //                tbooks.ForEach(async (book) => await CompleteModelInfos(context, book));
            //                return tbooks;
            //            }
            //        }

            //        return Enumerable.Empty<Tbook>().ToList();
            //    }
            //    catch (Exception ex)
            //    {
            //        MethodBase m = MethodBase.GetCurrentMethod();
            //        Logs.Log(ex, m);
            //        return Enumerable.Empty<Tbook>().ToList();
            //    }
            //}

            //public static async Task<IList<LivreVM>> SearchBooksVMAsync(ResearchItemVM parameters, CancellationToken cancellationToken = default)
            //{
            //    try
            //    {
            //        if (parameters == null)
            //        {
            //            return Enumerable.Empty<LivreVM>().ToList();
            //        }
            //        if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace())
            //        {
            //            return Enumerable.Empty<LivreVM>().ToList();
            //        }

            //        var collection = await SearchBooksAsync(parameters, cancellationToken);
            //        if (!collection.Any()) return Enumerable.Empty<LivreVM>().ToList();

            //        var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(s => s.Result).ToList();
            //        return values;
            //    }
            //    catch (Exception ex)
            //    {
            //        MethodBase m = MethodBase.GetCurrentMethod();
            //        Logs.Log(ex, m);
            //        return Enumerable.Empty<LivreVM>().ToList();
            //    }
            //}

            #endregion
        }
    }
}
