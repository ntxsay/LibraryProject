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

        public static List<string> GetChangedProperties<T>(T A, T B)
        {
            try
            {
                if (A != null && B != null)
                {
                    Type type = typeof(T);
                    PropertyInfo[] allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo[] allSimpleProperties = allProperties;//.Where(pi => pi.PropertyType.IsSimpleType());
                    IEnumerable<string> enumerable()
                    {
                        foreach (var pi in allSimpleProperties)
                        {
                            var AValue = type.GetProperty(pi.Name).GetValue(A, null);
                            var BValue = type.GetProperty(pi.Name).GetValue(B, null);
                            if (AValue != BValue && (AValue == null || !AValue.Equals(BValue)))
                            {
                                yield return pi.Name;
                            }
                        }
                    }

                    var unequalProperties = enumerable();
                    return unequalProperties.ToList();
                }
                else
                {
                    throw new ArgumentNullException("Les objets A et B ne doivent pas être null.");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static T CopyProperties<T>(T A, T B)
        {
            try
            {
                if (A != null && B != null)
                {
                    Type type = typeof(T);
                    PropertyInfo[] allProperties = type.GetProperties();
                    foreach (var pi in allProperties)
                    {
                        var AValue = type.GetProperty(pi.Name).GetValue(A, null);
                        type.GetProperty(pi.Name).SetValue(B, AValue);
                    }

                    return B;
                }
                else
                {
                    throw new ArgumentNullException("Les objets A et B ne doivent pas être null.");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
