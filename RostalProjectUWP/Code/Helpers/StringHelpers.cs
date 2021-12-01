using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RostalProjectUWP.Code.Helpers
{
    public static class StringHelpersExtension
    {
        public static bool IsStringNullOrEmptyOrWhiteSpace(this string source)
        {
            return StringHelpers.IsStringNullOrEmptyOrWhiteSpace(source);
        }

        public static string FirstCharToLowerOrUpperCase(this string Value, StringHelpers.UpperLowerMode upperLowerMode)
        {
            return StringHelpers.Converter.FirstCharToLowerOrUpperCase(Value, upperLowerMode);
        }

        public static string GetLastChars(this string Value, int Count)
        {
            return StringHelpers.GetLastChars(Value, Count);
        }

        public static string GetFirstChars(this string Value, int Count)
        {
            return StringHelpers.GetFirstChars(Value, Count);
        }
    }

    public partial class StringHelpers
    {
        /// <summary>
        /// Obtient une valeur Booléenne indiquant si la chaîne de caractères saisit est Null ou vide ou ne contenant que des espaces blancs.
        /// </summary>
        /// <param name="ChainedeCaractere"></param>
        /// <returns></returns>
        public static bool IsStringNullOrEmptyOrWhiteSpace(string ChainedeCaractere) => (string.IsNullOrEmpty(ChainedeCaractere) || string.IsNullOrWhiteSpace(ChainedeCaractere));

        public static IEnumerable<string> RemoveNullOrEmptyOrWhiteSpaceValues(IEnumerable<string> values)
        {
            try
            {
                if (values == null || values.Count() == 0)
                {
                    return null;
                }

                List<string> NewValues = new List<string>();
                foreach (var item in values)
                {
                    if (!IsStringNullOrEmptyOrWhiteSpace(item))
                    {
                        NewValues.Add(item);
                        continue;
                    }
                }

                return NewValues;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetFirstChars(string value, int count = 1)
        {
            try
            {
                if (value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                if (count < 1)
                {
                    return null;
                }

                if (count > value.Length)
                {
                    return value;
                }

                return value.Substring(0, count);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string GetLastChars(string value, int count = 1)
        {
            try
            {
                if (value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                if (count < 1)
                {
                    return null;
                }

                if (count > value.Length)
                {
                    return value;
                }

                return value.Substring(value.Length - count);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public static string JoinStringArray(string[] values, string separator, out string MessageState)
        {
            try
            {
                if (values == null || values.Count() == 0)
                {
                    MessageState = "La valeur d'entrée est nulle ou ne contenant aucun élément.";
                    return null;
                }

                string value = string.Join(separator, values);
                MessageState = null;
                return value;
            }
            catch (Exception ex)
            {
                MessageState = ex.Message;
                return null;
            }
        }

        public static string JoinIntArray(int[] values, string separator, out string MessageState)
        {
            try
            {
                if (values == null || values.Count() == 0)
                {
                    MessageState = "La valeur d'entrée est nulle ou ne contenant aucun élément.";
                    return null;
                }

                string value = string.Join(separator, values);
                MessageState = null;
                return value;
            }
            catch (Exception ex)
            {
                MessageState = ex.Message;
                return null;
            }
        }

        #region SplitWord
        public static string SplitWord(string value, int index, char[] separator, out string MessageState)
        {
            try
            {
                if (IsStringNullOrEmptyOrWhiteSpace(value))
                {
                    MessageState = "La valeur d'entrée est nulle, vide ou ne contenant que des espaces blancs.";
                    return null;
                }

                var result = value?.Split(separator)?.ToList();
                if (result.Count > 0)
                {
                    var count = result.Count - 1;
                    if (index < count || index > count)
                    {
                        MessageState = "L'index recherché est inférieur ou supérieur.";
                        return null;
                    }

                    for (int i = 0; i < result.Count; i++)
                    {
                        if (i == index)
                        {
                            MessageState = null;
                            return result[i];
                        }
                    }
                }

                MessageState = "L'index n'a pas pu être trouvé.";
                return null;
            }
            catch (Exception ex)
            {
                MessageState = ex.Message;
                return null;
            }
        }

        public static string[] SplitWord(string value, char[] separator, bool removeEmptyEntries = true)
        {
            try
            {
                if (IsStringNullOrEmptyOrWhiteSpace(value))
                {
                    return null;
                }

                if (removeEmptyEntries == true)
                {
                    return value?.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    return value?.Split(separator);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string[] SplitWord(string value, string[] separator)
        {
            try
            {
                if (value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                return value?.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        public static bool? IsSplitWordContainsValue(string Word, string Value, char[] separator)
        {
            try
            {
                if (IsStringNullOrEmptyOrWhiteSpace(Value) || IsStringNullOrEmptyOrWhiteSpace(Word))
                {
                    return null;
                }

                var list = Word?.Split(separator)?.ToList();
                if (list == null)
                {
                    return null;
                }

                foreach (var item in list)
                {
                    if (item == Value)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string GetInitials(string value)
        {
            try
            {
                if (IsStringNullOrEmptyOrWhiteSpace(value) || IsStringNullOrEmptyOrWhiteSpace(value))
                {
                    return null;
                }

                var list = value?.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)?.ToList();
                if (list == null)
                {
                    return null;
                }

                string initials = null;
                foreach (string item in list)
                {
                    if (item.Length > 0 && char.IsLetter(item[0]))
                    {
                        initials += item[0];
                    }
                }

                return initials;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message} - {(ex.InnerException?.Message == null ? string.Empty : "Inner Exception : " + ex.InnerException?.Message) }");
                return null;
            }
        }

        /// <summary>
        /// Convertit une date (sans heure et particulièrement date au format anglais ex : 1993-02-15) au format français ex : 15/02/1993 
        /// </summary>
        /// <param name="EnglishDate">date à convertir</param>
        /// <returns></returns>
        public static string ConvertEnglishDateToFrenchDate(string EnglishDate)
        {
            try
            {
                if (IsStringNullOrEmptyOrWhiteSpace(EnglishDate))
                {
                    return null;
                }

                var preValue = DateTime.TryParse(EnglishDate, out DateTime date);
                if (preValue == true)
                {
                    return date.ToString("dd/MM/yyyy");
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }


        private static bool CompareNombre(object NumericNombre, object MinValue, object MaxValue)
        {
            try
            {
                double? DoubMinValue = null;
                int? IntMinValue = null;
                double? DoubMaxValue = null;
                int? IntMaxValue = null;
                double? DoubNombre = null;
                int? IntNombre = null;

                //MinValue
                if (MinValue.GetType() == typeof(double))
                {
                    DoubMinValue = (double)MinValue;
                }
                else if (MinValue.GetType() == typeof(int))
                {
                    IntMinValue = (int)MinValue;
                }

                //MaxValue
                if (MaxValue.GetType() == typeof(double))
                {
                    DoubMaxValue = (double)MaxValue;
                }
                else if (MaxValue.GetType() == typeof(int))
                {
                    IntMaxValue = (int)MaxValue;
                }

                //NumericNombre
                if (NumericNombre.GetType() == typeof(double))
                {
                    DoubNombre = (double)NumericNombre;
                }
                else if (NumericNombre.GetType() == typeof(int))
                {
                    IntNombre = (int)NumericNombre;
                }

                //Compare
                if (DoubNombre != null && DoubMinValue != null && DoubMaxValue != null)
                {
                    if (DoubNombre >= DoubMinValue && DoubNombre <= DoubMaxValue)
                    {
                        return true;
                    }
                }
                else if (IntNombre != null && IntMinValue != null && IntMaxValue != null)
                {
                    if (IntNombre >= IntMinValue && IntNombre <= IntMaxValue)
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

    }

}
