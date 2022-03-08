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

namespace LibraryProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        public partial struct BookEtat
        {
            

            #region Helpers
            private static async Task CompleteModelInfos(LibraryDbContext _context,TbookEtat model)
            {
                try
                {
                    if (model == null)
                    {
                        return;
                    }

                    using (LibraryDbContext context = _context ?? new LibraryDbContext())
                    {
                        //model.t = await context.TbookPret.Where(s => s.IdBookExemplary == model.Id).ToListAsync();
                        //model.TbookEtat = await context.TbookEtat.Where(s => s.IdBookExemplary == model.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return;
                }
            }
            internal static LivreEtatVM ViewModelConverter(TbookEtat model)
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

            #endregion
        }
    }

}
