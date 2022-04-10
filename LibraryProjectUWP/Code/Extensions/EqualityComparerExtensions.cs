using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Extensions
{
    public class TbookIdEqualityComparer : IEqualityComparer<Tbook>
    {
        public bool Equals(Tbook m1, Tbook m2)
        {
            try
            {
                if (m2 == null && m1 == null)
                    return true;
                else if (m1 == null || m2 == null)
                    return false;
                else if (m1.Id == m2.Id)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }

        public int GetHashCode(Tbook model)
        {
            int hCode =  Convert.ToInt32(model.Id);
            return hCode.GetHashCode();
        }
    }
}
