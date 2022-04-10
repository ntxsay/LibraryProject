using LibraryProjectUWP.Code.Extensions;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LibraryProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        public partial struct Book
        {
            public static async Task<int> CountPagesAsync(long idLibrary, int MaxItemsPerPage)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var countBook = await context.Tbook.CountAsync(c => c.IdLibrary == idLibrary);
                        if (countBook > 0)
                        {
                            int nbPageDefault = countBook / MaxItemsPerPage;
                            double nbPageExact = countBook / Convert.ToDouble(MaxItemsPerPage);
                            int nbPageRounded = nbPageExact > nbPageDefault ? nbPageDefault + 1 : nbPageDefault;
                            return nbPageRounded;
                        }

                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Logs.Log(ex, m);
                    return 0;
                }
            }

            public static async Task<IEnumerable<PageSystemVM>> InitializePagesAsync(long idLibrary, int MaxItemsPerPage)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                try
                {
                    var countPages = await CountPagesAsync(idLibrary, MaxItemsPerPage);
                    if (countPages > 0)
                    {
                        List<PageSystemVM> pages = new List<PageSystemVM>();
                        for (int i = 0; i < countPages; i++)
                        {
                            pages.Add(new PageSystemVM()
                            {
                                CurrentPage = i + 1,
                                IsPageSelected = i == 0,
                                BackgroundColor = i == 0 ? Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush : Application.Current.Resources["PageNotSelectedBackground"] as SolidColorBrush,
                            });
                        }

                        return pages;
                    }

                    return Enumerable.Empty<PageSystemVM>().ToList();
                }
                catch (Exception ex)
                {
                    Logs.Log(ex, m);
                    return Enumerable.Empty<PageSystemVM>().ToList();
                }
            }

            public static async Task<IEnumerable<Tbook>> GetPaginatedItemsAsync(long idLibrary, int maxItemsPerPage, int goToPage = 1)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                try
                {
                    IEnumerable<Tbook> itemsPage = Enumerable.Empty<Tbook>();

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var tBooks = await context.Tbook.Where(c => c.IdLibrary == idLibrary).ToListAsync();
                        
                        //Si la séquence contient plus d'items que le nombre max éléments par page
                        if (tBooks.Count > maxItemsPerPage)
                        {
                            //Si la première page (ou moins ^^')
                            if (goToPage <= 1)
                            {
                                itemsPage = tBooks.Take(maxItemsPerPage);
                            }
                            else //Si plus que la première page
                            {
                                var nbItemsToSkip = maxItemsPerPage * (goToPage - 1);
                                if (tBooks.Count >= nbItemsToSkip)
                                {
                                    var getRest = tBooks.Skip(nbItemsToSkip);
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
                            itemsPage = tBooks;
                        }
                        return itemsPage;
                    }
                }
                catch (Exception ex)
                {
                    Logs.Log(ex, m);
                    return Enumerable.Empty<Tbook>();
                }
            }

        }
    }

}
