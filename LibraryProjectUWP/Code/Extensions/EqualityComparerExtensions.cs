using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels.General;
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

        public bool Equals(Tlibrary m1, Tlibrary m2)
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

    public class TLibraryIdEqualityComparer : IEqualityComparer<Tlibrary>
    {
        public bool Equals(Tlibrary m1, Tlibrary m2)
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

        public int GetHashCode(Tlibrary model)
        {
            int hCode = Convert.ToInt32(model.Id);
            return hCode.GetHashCode();
        }
    }


    public class EqualityComparerExtentions
    {
        public static bool PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            try
            {
                if (self != null && to != null)
                {
                    Type type = typeof(T);
                    List<string> ignoreList = new List<string>(ignore);
                    foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (!ignoreList.Contains(pi.Name))
                        {
                            object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                            object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                            if (selfValue != toValue && (selfValue == null || selfValue.Equals(toValue) == false))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                return self == to;
            }
            catch (Exception)
            {

                throw;
            }
        }

        
    }
}
