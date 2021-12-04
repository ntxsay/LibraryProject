using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA.UWPHelpers.Helpers
{
    public partial class StringHelpers
    {
        public enum UpperLowerMode
        {
            Majuscule = 0,
            Minuscule = 1
        }

        public struct Converter
        {
            /// <summary>
            /// Convertit de manière sécurisé une chaine de caractères vide, null en majuscule ou minuscule 
            /// </summary>
            /// <param name="ChainedeCaractere"></param>
            /// <param name="letterMode"></param>
            /// <returns></returns>
            public static string SafeNullToLower(string ChainedeCaractere, UpperLowerMode letterMode = UpperLowerMode.Minuscule)
            {
                try
                {
                    var value = ChainedeCaractere;

                    if (IsStringNullOrEmptyOrWhiteSpace(value) == true)
                    {
                        value = string.Empty;
                    }

                    if (letterMode == UpperLowerMode.Minuscule)
                    {
                        value.ToLower();
                    }
                    else if (letterMode == UpperLowerMode.Majuscule)
                    {
                        value.ToUpper();
                    }

                    return value;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// <summary>
            /// Convertit la première lettre d'un mot en majuscule ou minuscule
            /// </summary>
            /// <param name="Value">Lettre ou mot(s)</param>
            /// <returns></returns>
            public static string FirstCharToLowerOrUpperCase(string Value, UpperLowerMode ConvertIn)
            {
                try
                {
                    if (string.IsNullOrEmpty(Value))
                    {
                        return Value;
                    }
                    char[] a = Value.ToCharArray();
                    if (ConvertIn == UpperLowerMode.Majuscule)
                    {
                        a[0] = char.ToUpper(a[0]);
                    }
                    else if (ConvertIn == UpperLowerMode.Minuscule)
                    {
                        a[0] = char.ToLower(a[0]);
                    }
                    return new string(a);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static string ConvertFirtLetterInWords(string Mots, UpperLowerMode ConvertIn)
            {
                try
                {
                    char[] array = Mots.ToCharArray();
                    // Handle the first letter in the string.
                    if (array.Length >= 1)
                    {
                        if (ConvertIn == UpperLowerMode.Majuscule)
                        {
                            if (char.IsLower(array[0]))
                            {
                                array[0] = char.ToUpper(array[0]);
                            }
                        }
                        else if (ConvertIn == UpperLowerMode.Minuscule)
                        {
                            if (char.IsUpper(array[0]))
                            {
                                array[0] = char.ToLower(array[0]);
                            }
                        }
                    }
                    // Scan through the letters, checking for spaces.
                    // ... Uppercase the lowercase letters following spaces.
                    for (int i = 1; i < array.Length; i++)
                    {
                        if (array[i - 1] == ' ')
                        {
                            if (ConvertIn == UpperLowerMode.Majuscule)
                            {
                                if (char.IsLower(array[i]))
                                {
                                    array[i] = char.ToUpper(array[i]);
                                }
                            }
                            else if (ConvertIn == UpperLowerMode.Minuscule)
                            {
                                if (char.IsUpper(array[i]))
                                {
                                    array[i] = char.ToLower(array[i]);
                                }
                            }
                        }
                    }
                    return new string(array);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }

}
