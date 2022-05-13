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
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using Windows.Storage;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Library;
using System.Threading;
using LibraryProjectUWP.Code.Extensions;

namespace LibraryProjectUWP.Code.Services.Db
{
    public partial class DbServices
    {
        public partial struct Library
        {
            public static async Task<IList<Tlibrary>> SearchLibrariesInName(ResearchItemVM parameters, CancellationToken cancellationToken = default)
            {
                try
                {
                    if (parameters == null)
                    {
                        return Enumerable.Empty<Tlibrary>().ToList();
                    }
                    if (parameters.Term.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return Enumerable.Empty<Tlibrary>().ToList();
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tlibrary> tlibraries = new List<Tlibrary>();
                        var termToLower = parameters.Term.ToLower();

                        switch (parameters.TermParameter)
                        {
                            case Code.Search.Terms.Equals:
                                tlibraries = await context.Tlibrary.Where(w => w.Name.ToLower() == parameters.Term).ToListAsync(cancellationToken);
                                break;
                            case Code.Search.Terms.Contains:
                                tlibraries = await context.Tlibrary.Where(w => w.Name.ToLower().Contains(parameters.Term)).ToListAsync(cancellationToken);
                                break;
                            case Code.Search.Terms.StartWith:
                                tlibraries = await context.Tlibrary.Where(w => w.Name.ToLower().StartsWith(parameters.Term)).ToListAsync(cancellationToken);
                                break;
                            case Code.Search.Terms.EndWith:
                                tlibraries = await context.Tlibrary.Where(w => w.Name.ToLower().EndsWith(parameters.Term)).ToListAsync(cancellationToken);
                                break;
                            default:
                                break;
                        }

                        return tlibraries;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tlibrary>().ToList();
                }
            }


            public static async Task<IList<T>> SearchAsync<T>(ResearchItemVM parameters, CancellationToken cancellationToken = default) where T : class
            {
                try
                {
                    if (parameters == null)
                    {
                        return Enumerable.Empty<T>().ToList();
                    }
                    if (parameters.IdLibrary < 1 || parameters.Term.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return Enumerable.Empty<T>().ToList();
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var termToLower = parameters.Term.ToLower();

                        if (typeof(T).IsAssignableFrom(typeof(Tlibrary)))
                        {
                            List<Tlibrary> tlibraries = new List<Tlibrary>();
                            if (parameters.SearchInMainTitle == true)
                            {
                                var _tlibraries = await SearchLibrariesInName(parameters, cancellationToken);
                                if (_tlibraries != null && _tlibraries.Any())
                                {
                                    tlibraries.AddRange(_tlibraries);
                                }
                            }

                            if (tlibraries != null && tlibraries.Any())
                            {
                                TLibraryIdEqualityComparer idEqualityComparer = new TLibraryIdEqualityComparer();
                                tlibraries = tlibraries.Distinct(idEqualityComparer).ToList();
                                //tlibraries.ForEach(async (book) => await CompleteModelInfos(context, book));
                                return tlibraries.Select(s => (T)(object)s).ToList();
                            }
                        }
                        else if (typeof(T).IsAssignableFrom(typeof(Tbook)))
                        {
                            List<Tbook> tbooks = new List<Tbook>();
                            
                            if (parameters.SearchInMainTitle == true)
                            {
                                IList<Tbook> _tbooks = await Book.SearchBooksInMainTitle(parameters, cancellationToken);
                                if (_tbooks != null && _tbooks.Any())
                                {
                                    tbooks.AddRange(_tbooks);
                                }
                            }

                            if (parameters.SearchInOtherTitles == true)
                            {
                                IList<Tbook> _tbooks = await Book.SearchBooksInOtherTitles(parameters, cancellationToken);
                                if (_tbooks != null && _tbooks.Any())
                                {
                                    tbooks.AddRange(_tbooks);
                                }
                            }

                            if (parameters.SearchInAuthors == true)
                            {
                                IList<Tbook> _tbooks = await Book.SearchBooksInContacts(parameters, new ContactRole[] { ContactRole.Author }, cancellationToken);
                                if (_tbooks != null && _tbooks.Any())
                                {
                                    tbooks.AddRange(_tbooks);
                                }
                            }

                            if (parameters.SearchInEditors == true)
                            {
                                IList<Tbook> _tbooks = await Book.SearchBooksInContacts(parameters, new ContactRole[] { ContactRole.EditorHouse }, cancellationToken);
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
                                TbookIdEqualityComparer idEqualityComparer = new TbookIdEqualityComparer();
                                tbooks = tbooks.Distinct(idEqualityComparer).ToList();
                                tbooks.ForEach(async (book) => await Book.CompleteModelInfos(context, book));
                                return tbooks.Select(s => (T)(object)s).ToList();
                            }
                        }
                            
                    }

                    return Enumerable.Empty<T>().ToList();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<T>().ToList();
                }
            }

            public static IEnumerable<T> OrderLibraries<T>(IEnumerable<T> modelList, byte OrderBy = 0, byte SortBy = 0) where T : class
            {
                try
                {
                    if (modelList == null || !modelList.Any())
                    {
                        return Enumerable.Empty<T>();
                    }

                    if (typeof(T).IsAssignableFrom(typeof(Tlibrary)) && modelList is IEnumerable<Tlibrary> librariesModel)
                    {
                        if (SortBy == (byte)LibraryGroupVM.SortBy.Name)
                        {
                            if (OrderBy == (byte)LibraryGroupVM.OrderBy.Croissant)
                            {
                                return librariesModel.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.Name).Select(s => (T)(object)s);
                            }
                            else if (OrderBy == (byte)LibraryGroupVM.OrderBy.DCroissant)
                            {
                                return librariesModel.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.Name).Select(s => (T)(object)s);
                            }
                        }
                        else if (SortBy == (byte)LibraryGroupVM.SortBy.DateCreation)
                        {
                            if (OrderBy == (byte)LibraryGroupVM.OrderBy.Croissant)
                            {
                                return librariesModel.OrderBy(o => o.DateAjout).Select(s => (T)(object)s);
                            }
                            else if (OrderBy == (byte)LibraryGroupVM.OrderBy.DCroissant)
                            {
                                return librariesModel.OrderByDescending(o => o.DateAjout).Select(s => (T)(object)s);
                            }
                        }
                    }
                    else if (typeof(T).IsAssignableFrom(typeof(Tbook)) && modelList is IEnumerable<Tbook> booksModel)
                    {
                        if (SortBy == (byte)BookGroupVM.SortBy.Name)
                        {
                            if (OrderBy == (byte)BookGroupVM.OrderBy.Croissant)
                            {
                                return booksModel.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.MainTitle).Select(s => (T)(object)s);
                            }
                            else if (OrderBy == (byte)BookGroupVM.OrderBy.DCroissant)
                            {
                                return booksModel.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.MainTitle).Select(s => (T)(object)s);
                            }
                        }
                        else if (SortBy == (byte)BookGroupVM.SortBy.DateCreation)
                        {
                            if (OrderBy == (byte)BookGroupVM.OrderBy.Croissant)
                            {
                                return booksModel.OrderBy(o => o.DateAjout).Select(s => (T)(object)s);
                            }
                            else if (OrderBy == (byte)BookGroupVM.OrderBy.DCroissant)
                            {
                                return booksModel.OrderByDescending(o => o.DateAjout).Select(s => (T)(object)s);
                            }
                        }
                    }


                    return Enumerable.Empty<T>();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<T>();
                }
            }

            public static async Task<IEnumerable<T>> OrderLibrariesAsync<T>(byte OrderBy = 0, byte SortBy = 0, long? idLibrary = null) where T : class
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        if (typeof(T).IsAssignableFrom(typeof(Tbook)) && idLibrary != null)
                        {
                            var modelList = await context.Tbook.Where(c => c.IdLibrary == idLibrary).ToListAsync();
                            if (modelList == null || !modelList.Any())
                            {
                                return Enumerable.Empty<T>();
                            }

                            return OrderLibraries(modelList.Select(s => (T)(object)s), OrderBy, SortBy);
                        }
                        else if (typeof(T).IsAssignableFrom(typeof(Tlibrary)))
                        {
                            var modelList = await context.Tlibrary.ToListAsync();
                            if (modelList == null || !modelList.Any())
                            {
                                return Enumerable.Empty<T>();
                            }

                            return OrderLibraries(modelList.Select(s => (T)(object)s), OrderBy, SortBy);
                        }


                        return Enumerable.Empty<T>();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<T>();
                }
            }


            public static IEnumerable<Tlibrary> GetPaginatedItems(IEnumerable<Tlibrary> modelList, int maxItemsPerPage, int goToPage = 1)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                try
                {
                    IEnumerable<Tlibrary> itemsPage = Enumerable.Empty<Tlibrary>();

                    //Si la séquence contient plus d'items que le nombre max éléments par page
                    if (modelList.Count() > maxItemsPerPage)
                    {
                        //Si la première page (ou moins ^^')
                        if (goToPage <= 1)
                        {
                            itemsPage = modelList.Take(maxItemsPerPage);
                        }
                        else //Si plus que la première page
                        {
                            var nbItemsToSkip = maxItemsPerPage * (goToPage - 1);
                            if (modelList.Count() >= nbItemsToSkip)
                            {
                                var getRest = modelList.Skip(nbItemsToSkip);
                                //Si reste de la séquence contient plus d'items que le nombre max éléments par page
                                if (getRest.Count() > maxItemsPerPage)
                                {
                                    itemsPage = getRest.Take(maxItemsPerPage);
                                }
                                else
                                {
                                    itemsPage = getRest;
                                }
                            }
                        }
                    }
                    else //Si la séquence contient moins ou le même nombre d'items que le nombre max éléments par page
                    {
                        itemsPage = modelList;
                    }

                    return itemsPage;
                }
                catch (Exception ex)
                {
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tlibrary>();
                }
            }

            public static IEnumerable<BibliothequeVM> GetPaginatedItemsVm(IEnumerable<Tlibrary> modelList, int maxItemsPerPage, int goToPage = 1)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                try
                {
                    var selectedItems = GetPaginatedItems(modelList, maxItemsPerPage, goToPage);
                    List<BibliothequeVM> viewModelList = selectedItems.Select(async s => await SingleVMAsync(s.Id)).Select(t => t.Result).ToList();
                    return viewModelList;
                }
                catch (Exception ex)
                {
                    Logs.Log(ex, m);
                    return Enumerable.Empty<BibliothequeVM>();
                }
            }

        }
    }
}
