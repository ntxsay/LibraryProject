using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Helpers
{
    public class PropertyHelpers
    {
        public static string GetAttributeDisplayName(PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(
                typeof(DisplayNameAttribute), true);
            if (atts.Length == 0)
                return null;
            return (atts[0] as DisplayNameAttribute).DisplayName;
        }

        public static List<PropertiesChangedVM> GetChangedProperties<T>(T A, T B)
        {
            try
            {
                if (A != null && B != null)
                {
                    Type type = typeof(T);
                    PropertyInfo[] allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo[] allSimpleProperties = allProperties;//.Where(pi => pi.PropertyType.IsSimpleType());
                    IEnumerable<PropertiesChangedVM> enumerable()
                    {
                        foreach (var pi in allSimpleProperties)
                        {
                            var AValue = type.GetProperty(pi.Name).GetValue(A, null);
                            var BValue = type.GetProperty(pi.Name).GetValue(B, null);
                            if (AValue != BValue && (AValue == null || !AValue.Equals(BValue)))
                            {
                                yield return new PropertiesChangedVM()
                                {
                                    PropertyName = GetAttributeDisplayName(pi),
                                    AValue = AValue,
                                    BValue = BValue,
                                };
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

        public static T CopyProperties<T>(T A) where T : new()
        {
            try
            {
                if (A != null)
                {
                    Type type = typeof(T);
                    T newObject = new T();
                    PropertyInfo[] allProperties = type.GetProperties();
                    foreach (var pi in allProperties)
                    {
                        var AValue = type.GetProperty(pi.Name).GetValue(A, null);
                        type.GetProperty(pi.Name).SetValue(newObject, AValue);
                    }

                    return newObject;
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
