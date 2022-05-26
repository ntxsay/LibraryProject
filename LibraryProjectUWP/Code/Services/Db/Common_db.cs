using LibraryProjectUWP.Code.Extensions;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Services.Db
{
    public partial class DbServices
    {
        public partial struct Common
        {
            public static async Task<IList<T>> SearchAsync<T>(ResearchItemVM[] parameters, long? idLibrary = null, CancellationToken cancellationToken = default) where T : class, new ()
            {
                try
                {
                    if (parameters == null || !parameters.Any())
                    {
                        return Enumerable.Empty<T>().ToList();
                    }

                    if (typeof(T) == typeof(Tbook) && (idLibrary == null || idLibrary < 1))
                    {
                        return Enumerable.Empty<T>().ToList();
                    }

                    List<IEnumerable<T>> searchArrayList = new List<IEnumerable<T>>();
                    for (int i = 0; i < parameters.Count(); i++)
                    {
                        var item  = parameters[i];  
                        if (i == 0)
                        {
                            if (item.IsSearchFromParentResult)
                            {
                                item.IsSearchFromParentResult = false;
                            }
                        }

                        IEnumerable<T> valueList = await SearchAsync(new ResearchContainerVM<T>()
                        {
                            CurrentSearchParameter = item,
                            ParentSearchedResult = item.IsSearchFromParentResult ? searchArrayList.LastOrDefault() : null,
                        }, idLibrary, cancellationToken);
                        
                        if (valueList != null && valueList.Any())
                        {
                            if (item.IsSearchFromParentResult)
                            {
                                var lastItem = searchArrayList.LastOrDefault();
                                if (lastItem != null)
                                {
                                    int index = searchArrayList.IndexOf(lastItem);
                                    searchArrayList.Insert(index, valueList);
                                    searchArrayList.Remove(lastItem);
                                }
                                else
                                {
                                    searchArrayList.Add(valueList);
                                }
                            }
                            else
                            {
                                searchArrayList.Add(valueList);
                            }
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }

                    List<T> values = new List<T>();
                    if (searchArrayList != null && searchArrayList.Any())
                    {
                        values = searchArrayList.SelectMany(s => s.ToList()).Select(s => s).ToList();
                    }

                    if (values != null && values.Any())
                    {
                        if (typeof(T).IsAssignableFrom(typeof(Tbook)))
                        {
                            TbookIdEqualityComparer idEqualityComparer = new TbookIdEqualityComparer();
                            values = values.Select(s => (Tbook)(object)s).Distinct(idEqualityComparer).Select(s => (T)(object)s).ToList();
                        }
                    }

                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<T>().ToList();
                }
            }


            public static async Task<IList<T>> SearchAsync<T>(ResearchContainerVM<T> parameter, long? idLibrary = null, CancellationToken cancellationToken = default) where T : class
            {
                try
                {
                    if (parameter == null || parameter.CurrentSearchParameter == null)
                    {
                        return Enumerable.Empty<T>().ToList();
                    }

                    if (parameter.CurrentSearchParameter.Term.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return Enumerable.Empty<T>().ToList();
                    }

                    if (typeof(T) == typeof(Tbook) && (idLibrary == null || idLibrary < 1))
                    {
                        return Enumerable.Empty<T>().ToList();
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var termToLower = parameter.CurrentSearchParameter.Term.ToLower();

                        if (typeof(T).IsAssignableFrom(typeof(Tlibrary)))
                        {
                            List<Tlibrary> tlibraries = new List<Tlibrary>();
                            if (parameter.CurrentSearchParameter.SearchInMainTitle == true)
                            {
                                var _tlibraries = await Library.SearchLibrariesInName(parameter.CurrentSearchParameter, cancellationToken);
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
                        else if (parameter is ResearchContainerVM<Tbook> bookParameter)
                        {
                            List<Tbook> tbooks = new List<Tbook>();

                            if (parameter.CurrentSearchParameter.SearchInMainTitle == true)
                            {
                                IList<Tbook> _tbooks = await Book.SearchBooksInMainTitle(bookParameter, (long)idLibrary, cancellationToken);
                                if (_tbooks != null && _tbooks.Any())
                                {
                                    tbooks.AddRange(_tbooks);
                                }
                            }

                            if (parameter.CurrentSearchParameter.SearchInOtherTitles == true)
                            {
                                IList<Tbook> _tbooks = await Book.SearchBooksInOtherTitles(parameter.CurrentSearchParameter, cancellationToken);
                                if (_tbooks != null && _tbooks.Any())
                                {
                                    tbooks.AddRange(_tbooks);
                                }
                            }

                            if (parameter.CurrentSearchParameter.SearchInAuthors == true)
                            {
                                IList<Tbook> _tbooks = await Book.SearchBooksInContacts(parameter.CurrentSearchParameter, new ContactRole[] { ContactRole.Author }, cancellationToken);
                                if (_tbooks != null && _tbooks.Any())
                                {
                                    tbooks.AddRange(_tbooks);
                                }
                            }

                            if (parameter.CurrentSearchParameter.SearchInEditors == true)
                            {
                                IList<Tbook> _tbooks = await Book.SearchBooksInContacts(parameter.CurrentSearchParameter, new ContactRole[] { ContactRole.EditorHouse }, cancellationToken);
                                if (_tbooks != null && _tbooks.Any())
                                {
                                    tbooks.AddRange(_tbooks);
                                }
                            }

                            if (parameter.CurrentSearchParameter.SearchInCollections == true)
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

            public static IEnumerable<T> Order<T>(IEnumerable<T> modelList, byte OrderBy = 0, byte SortBy = 0) where T : class
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

            public static async Task<IEnumerable<T>> OrderAsync<T>(byte OrderBy = 0, byte SortBy = 0, long? idLibrary = null) where T : class
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

                            return Order(modelList.Select(s => (T)(object)s), OrderBy, SortBy);
                        }
                        else if (typeof(T).IsAssignableFrom(typeof(Tlibrary)))
                        {
                            var modelList = await context.Tlibrary.ToListAsync();
                            if (modelList == null || !modelList.Any())
                            {
                                return Enumerable.Empty<T>();
                            }

                            return Order(modelList.Select(s => (T)(object)s), OrderBy, SortBy);
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

        }
    }
}
