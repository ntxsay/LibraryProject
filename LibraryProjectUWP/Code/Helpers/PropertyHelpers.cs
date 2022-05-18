using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public static DataTable CreateDataTableOfObject<T>(IEnumerable<T> objectList, string[] excludePropertiesName = null) where T : class
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (objectList == null || !objectList.Any())
                {
                    return null;
                }

                DataTable dataTable = new DataTable();

                Type type = typeof(T);
                List<PropertyInfo> allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                
                if (excludePropertiesName != null && excludePropertiesName.Any())
                {
                    for (int i = 0; i < allProperties.Count; i++)
                    {
                        if (excludePropertiesName.Any(a => a == allProperties[i].Name))
                        {
                            allProperties.RemoveAt(i);
                            i = -1;
                            continue;
                        }
                    }
                }
                
                bool firstRow = true;
                int countRow = 1;
                foreach (var viewModel in objectList)
                {
                    if (firstRow)
                    {
                        dataTable.Columns.Add("#");
                        foreach (var pi in allProperties)
                        {
                            var displayName = GetAttributeDisplayName(pi) ?? pi.Name;
                            dataTable.Columns.Add(displayName);
                        }
                    }

                    //Add rows to DataTable.
                    dataTable.Rows.Add();

                    dataTable.Rows[dataTable.Rows.Count - 1][0] = $"{countRow}";
                    countRow++;

                    int i = 1;
                    foreach (var pi in allProperties)
                    {
                        var value = type.GetProperty(pi.Name).GetValue(viewModel, null);
                        dataTable.Rows[dataTable.Rows.Count - 1][i] = value?.ToString() ?? string.Empty;
                        i++;
                    }

                    firstRow = false;
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
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
