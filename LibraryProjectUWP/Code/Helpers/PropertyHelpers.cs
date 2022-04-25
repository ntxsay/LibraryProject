using LibraryProjectUWP.ViewModels.General;
using Newtonsoft.Json;
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
        

        public static object CloneObject(object objSource)
        {
            //step : 1 Get the type of source object and create a new instance of that type
            Type typeSource = objSource.GetType();
            object objTarget = Activator.CreateInstance(typeSource);
            
            //Step2 : Get all the properties of source object type
            PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            //Step : 3 Assign all source property to taget object 's properties
            foreach (PropertyInfo property in propertyInfo)
            {
                //Check whether property can be written to
                if (property.CanWrite)
                {
                    //Step : 4 check whether property type is value type, enum or string type
                    if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    else
                    {
                        object objPropertyValue = property.GetValue(objSource, null);
                        if (objPropertyValue == null)
                        {
                            property.SetValue(objTarget, null, null);
                        }
                        else
                        {
                            property.SetValue(objTarget, CloneObject(objPropertyValue), null);
                        }
                    }
                }
            }
            return objTarget;
        }
    }
}
