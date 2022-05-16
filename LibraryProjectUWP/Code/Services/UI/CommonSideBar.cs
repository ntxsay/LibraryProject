using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.Book.SubViews;
using LibraryProjectUWP.Views.Collection;
using LibraryProjectUWP.Views.Contact;
using LibraryProjectUWP.Views.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Services.UI
{
    public class CommonSideBar
    {
        readonly BookCollectionPage ParentPage;
        readonly ContactListUC contactListUC;
        readonly CollectionListUC collectionListUC;
        readonly EsBook esBook = new EsBook();
        
        public CommonSideBar(BookCollectionPage parentPage, ContactListUC _contactListUC)
        {
            ParentPage = parentPage;
            contactListUC = _contactListUC;
        }

        public CommonSideBar(BookCollectionPage parentPage, CollectionListUC _collectionListUC)
        {
            ParentPage = parentPage;
            collectionListUC = _collectionListUC ;
        }

        private IEnumerable<ContactVM> OrderItems(IEnumerable<ContactVM> Collection, ContactGroupVM.OrderBy OrderBy = ContactGroupVM.OrderBy.Croissant, ContactGroupVM.SortBy SortBy = ContactGroupVM.SortBy.Prenom)
        {
            try
            {
                if (Collection == null || Collection.Count() == 0)
                {
                    return null;
                }

                if (SortBy == ContactGroupVM.SortBy.Prenom)
                {
                    if (OrderBy == ContactGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.Prenom);
                    }
                    else if (OrderBy == ContactGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.Prenom);
                    }
                }
                else if (SortBy == ContactGroupVM.SortBy.NomNaissance)
                {
                    if (OrderBy == ContactGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.NomNaissance);
                    }
                    else if (OrderBy == ContactGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.NomNaissance);
                    }
                }
                else if (SortBy == ContactGroupVM.SortBy.DateCreation)
                {
                    if (OrderBy == ContactGroupVM.OrderBy.Croissant)
                    {
                        return Collection.OrderBy(o => o.DateAjout);
                    }
                    else if (OrderBy == ContactGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.OrderByDescending(o => o.DateAjout);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return Enumerable.Empty<ContactVM>();
            }
        }


    }
}
