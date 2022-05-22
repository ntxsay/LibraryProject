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
