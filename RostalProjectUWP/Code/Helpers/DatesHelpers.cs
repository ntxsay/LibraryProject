using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Helpers
{
    internal class DatesHelpers
    {
        internal struct StringFormat
        {
            public const string FrenchDateStringFormat = "ddd dd MMMM yyyy";
            public const string FrenchDateMonthYearStringFormat = "MMMM yyyy";
            public const string FrenchDateYearStringFormat = "yyyy";
            public const string FrenchDateMinStringFormat = "dd/MM/yy";
            public const string FrenchDateMinStringFormat2 = "dd/MM/yyyy";
            public const string USADateStringFormat = "MM/dd/yyyy";
            public const string USADateStringFormat2 = "MM-dd-yyyy";
            public const string WebDateStringFormat = "yyyy-MM-dd";
            public const string WebDateTimeStringFormat = "yyyy-MM-ddTHH:mm";
            public const string FrenchDateTimeStringFormat = "dd/MM/yyyy à HH:mm";
            public const string FrenchDateTimeMinStringFormat = "dd/MM/yy à HH:mm";
            public const string FrenchFullDateTimeStringFormat2 = "dddd dd MMM yyyy à HH:mm";
            public const string FrenchFullDateTimeStringFormat = "ddd dd MMM yyyy à HH:mm";
            public const string DayFullOnlyDateStringFormat = "dddd";
            public const string MonthFullOnlyDateStringFormat = "MMMM";
            public const string YearFullOnlyDateStringFormat = "yyyy";
            public const string FrenchTimeStringFormat = "HH:mm";
            public const string DateTimeLogFormat = "yyyyMMddHHmmss";
        }
        internal struct Converter
        {
            #region Nullable DateTimeOffset
            /// <summary>
            /// Convertit une chaine de caractères en <see cref="DateTimeOffset"/>
            /// </summary>
            /// <remarks>
            /// Remarques : Si <paramref name="value"/> ne peut pas être converti, null  sera retournée.
            /// </remarks>
            /// <param name="value">Chaîne de caractères à convertir en <see cref="DateTimeOffset"/></param>
            /// <returns></returns>
            public static DateTimeOffset? StringToNullableDateTimeOffset(string value)
            {
                try
                {
                    if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(value))
                    {
                        return null;
                    }

                    if (DateTimeOffset.TryParse(value, out DateTimeOffset nValue))
                    {
                        return nValue;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
            #endregion

            #region String
            /// <summary>
            /// Convertit un objet <see cref="DateTimeOffset" en chaîne de caractères./>
            /// </summary>
            /// <param name="dateTimeOffset">Objet <see cref="DateTimeOffset"/> à convertir en <see cref="string"/></param>
            /// <param name="DateTimeFormat">Format de la date et l'heure</param>
            /// <returns></returns>
            public static string DateTimeOffsetToString(DateTimeOffset? dateTimeOffset, string DateTimeFormat = StringFormat.WebDateStringFormat)
            {
                try
                {
                    if (dateTimeOffset == null)
                    {
                        return null;
                    }

                    if (dateTimeOffset.HasValue)
                    {
                        return dateTimeOffset.Value.ToString(DateTimeFormat);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }


            /// <summary>
            /// Convertit un objet <see cref="DateTime"/> en une chaîne de caractères formatée
            /// </summary>
            /// <param name="value">Objet <see cref="DateTime"/> à convertir en chaîne de caractères.</param>
            /// <param name="DateTimeFormat">Format de la date et l'heure</param>
            /// <returns></returns>
            public static string DateTimeToString(DateTime? value, string DateTimeFormat = StringFormat.WebDateStringFormat)
            {
                try
                {
                    if (value == null)
                    {
                        return null;
                    }

                    return ((DateTime)value).ToString(DateTimeFormat);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }

            public static string EnglishStringDateToStringDate(string value, string UnKnowDateText = "?")
            {
                try
                {
                    if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(value))
                    {
                        return UnKnowDateText;
                    }

                    if (value.Contains("MM") && value.Contains("dd"))
                    {
                        var date = value.Replace("MM", "01");
                        date = date.Replace("dd", "01");
                        if (DateTime.TryParse(date, out DateTime nValue))
                        {
                            return nValue.Year.ToString();
                        }
                    }
                    else if (value.Contains("MM"))
                    {
                        var date = value.Replace("MM", "01");
                        if (DateTime.TryParse(date, out DateTime nValue))
                        {
                            return nValue.Year.ToString();
                        }
                    }
                    else if (value.Contains("dd"))
                    {
                        var date = value.Replace("dd", "01");
                        if (DateTime.TryParse(date, out DateTime nValue))
                        {
                            return nValue.ToString(StringFormat.FrenchDateMonthYearStringFormat);
                        }
                    }
                    else
                    {
                        if (DateTimeOffset.TryParse(value, out DateTimeOffset nValue))
                        {
                            return nValue.Date.ToString(StringFormat.FrenchDateStringFormat);
                        }
                    }


                    return UnKnowDateText;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return UnKnowDateText;
                }
            }

            public static string TimeSpanToStringDuration(TimeSpan value)
            {
                try
                {
                    if (value == null || value == TimeSpan.Zero)
                    {
                        return null;
                    }

                    var hours = (value.Hours > 0 ? value.Hours.ToString() + " h " : string.Empty);
                    var minutes = (value.Minutes > 0 ? value.Minutes.ToString() + " min " : string.Empty);
                    var seconds = (value.Seconds > 0 ? value.Seconds.ToString() + " s" : string.Empty);
                    return $"{hours}{minutes}{seconds}";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }

            public static string TimeSpanToStringDurationDefault(TimeSpan value)
            {
                try
                {
                    if (value == null || value == TimeSpan.Zero)
                    {
                        return null;
                    }

                    var hours = (value.Hours > 0 ? value.Hours.ToString("00") + ":" : string.Empty);
                    var minutes = (value.Minutes > 0 ? value.Minutes.ToString("00") + ":" : "00:");
                    var seconds = value.Seconds.ToString("00");
                    return $"{hours}{minutes}{seconds}";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }

            public static string TimeSpanToString(TimeSpan value, bool joinNumberAndLetter = false, bool useSpacing = true, string hourSeparator = "h", string minuteSeparator = "m", string secondSeparator = "s")
            {
                try
                {
                    if (value == null || value == TimeSpan.Zero)
                    {
                        return null;
                    }

                    var hourLetter = hourSeparator.IsStringNullOrEmptyOrWhiteSpace() ? string.Empty : joinNumberAndLetter ? hourSeparator : " " + hourSeparator;
                    var minuteLetter = minuteSeparator.IsStringNullOrEmptyOrWhiteSpace() ? string.Empty : joinNumberAndLetter ? minuteSeparator : " " + minuteSeparator;
                    var secondLetter = secondSeparator.IsStringNullOrEmptyOrWhiteSpace() ? string.Empty : joinNumberAndLetter ? secondSeparator : " " + secondSeparator;

                    var hours = (value.Hours > 0 ? value.Hours.ToString() + (useSpacing ? $"{hourLetter} " : hourLetter) : string.Empty);
                    var minutes = (value.Minutes > 0 ? value.Minutes.ToString() + (useSpacing ? $"{minuteLetter} " : minuteLetter) : string.Empty);
                    var seconds = (value.Seconds > 0 ? value.Seconds.ToString() + (useSpacing ? $"{secondLetter} " : secondLetter) : string.Empty);
                    return $"{hours}{minutes}{seconds}";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }

            public static string TimeSpanToString(TimeSpan? timeSpan, string timestringFormat = null)
            {
                try
                {
                    if (timeSpan == null)
                    {
                        return null;
                    }

                    if (timeSpan.HasValue)
                    {
                        return timeSpan.Value.ToString(timestringFormat);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }

            public static string StringTimeToStringDuration(string time)
            {
                try
                {
                    if (time.IsStringNullOrEmptyOrWhiteSpace() == true)
                    {
                        return null;
                    }

                    if (TimeSpan.TryParse(time, out TimeSpan value))
                    {
                        return TimeSpanToStringDuration(value);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }

            public static string DoubleToDurationMinutesHours(double value)
            {
                try
                {
                    int Hour;
                    int Minutes;

                    if (value >= 1.0)
                    {
                        var MinutesHourValue = value * (1 / 60);
                        string[] digits = value.ToString().Split(',');
                        Hour = digits[0].Length;

                        if (digits.Length == 2)
                        {
                            Minutes = digits[1].Length;
                        }
                        else
                        {
                            Minutes = 0;
                        }
                    }


                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
            #endregion

            #region Nullable DateTime
            public static DateTime? GetNullableDateFromString(string DateString)
            {
                try
                {
                    if (!StringHelpers.IsStringNullOrEmptyOrWhiteSpace(DateString))
                    {
                        var IsDate = DateTime.TryParse(DateString, out DateTime result);
                        if (IsDate == true)
                        {
                            return result;
                        }
                        else
                        {
                            return null;
                        }
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }

            public static DateTime? GetDateFromNullableDateTimeOffSet(DateTimeOffset? dateTimeOffset)
            {
                try
                {
                    if (dateTimeOffset == null) return null;
                    return dateTimeOffset.Value.Date;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }

            /// <summary>
            /// Convertit une chaîne de caractères en objet <see cref="DateTime"/>
            /// </summary>
            /// <param name="value">Chaîne de caractères à convertir en <see cref="DateTime"/></param>
            /// <returns></returns>
            public static DateTime? StringToDateTime(string value)
            {
                try
                {
                    if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(value))
                    {
                        return null;
                    }

                    if (DateTime.TryParse(value, out DateTime nValue))
                    {
                        return nValue;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
            #endregion

            #region DateTime
            public static DateTime GetDateFromString(string DateString)
            {
                try
                {
                    if (!StringHelpers.IsStringNullOrEmptyOrWhiteSpace(DateString))
                    {
                        var IsDate = DateTime.TryParse(DateString, out DateTime result);
                        if (IsDate == true)
                        {
                            return result;
                        }
                    }

                    return new DateTime(1950, 01, 01);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }

            #endregion
        }
    }
}
