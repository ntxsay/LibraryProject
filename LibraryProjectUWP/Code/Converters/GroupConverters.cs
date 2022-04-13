using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace LibraryProjectUWP.Code.Converters
{
    public class GroupToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is LibraryGroupVM.GroupBy libraryGroupBy && parameter is string groupby)
                {
                    if (groupby == "letter" && libraryGroupBy == LibraryGroupVM.GroupBy.Letter)
                    {
                        return true;
                    }
                    else if (groupby == "none" && libraryGroupBy == LibraryGroupVM.GroupBy.None)
                    {
                        return true;
                    }
                    else if (groupby == "year" && libraryGroupBy == LibraryGroupVM.GroupBy.CreationYear)
                    {
                        return true;
                    }
                }
                else if (value is BookGroupVM.GroupBy bookGroupBy && parameter is string bookGroupby)
                {
                    if (bookGroupby == "letter" && bookGroupBy == BookGroupVM.GroupBy.Letter)
                    {
                        return true;
                    }
                    else if (bookGroupby == "none" && bookGroupBy == BookGroupVM.GroupBy.None)
                    {
                        return true;
                    }
                    else if (bookGroupby == "creation-year" && bookGroupBy == BookGroupVM.GroupBy.CreationYear)
                    {
                        return true;
                    }
                    else if (bookGroupby == "parution-year" && bookGroupBy == BookGroupVM.GroupBy.ParutionYear)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SortToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is LibraryGroupVM.SortBy librarySortBy && parameter is string sortby)
                {
                    if (sortby == "name" && librarySortBy == LibraryGroupVM.SortBy.Name)
                    {
                        return true;
                    }
                    else if (sortby == "dateCreation" && librarySortBy == LibraryGroupVM.SortBy.DateCreation)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class OrderByToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is LibraryGroupVM.OrderBy libraryOrderBy && parameter is string sortby)
                {
                    if (sortby == "croissant" && libraryOrderBy == LibraryGroupVM.OrderBy.Croissant)
                    {
                        return true;
                    }
                    else if (sortby == "dCroissant" && libraryOrderBy == LibraryGroupVM.OrderBy.DCroissant)
                    {
                        return true;
                    }
                }
                else if (value is BookGroupVM.OrderBy bookOrderBy && parameter is string sortby2)
                {
                    if (sortby2 == "croissant" && bookOrderBy == BookGroupVM.OrderBy.Croissant)
                    {
                        return true;
                    }
                    else if (sortby2 == "dCroissant" && bookOrderBy == BookGroupVM.OrderBy.DCroissant)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
