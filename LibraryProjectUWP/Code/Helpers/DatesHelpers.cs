using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Helpers
{
    public enum DateCompare : byte
    {
        /// <summary>
        /// La date comparée est suprérieur à la date Model.
        /// La date1 est plus grande que la date2
        /// </summary>
        DateSuperieur,
        /// <summary>
        /// La date comparée est égale à la date Model.
        /// La date1 est égale à la date2
        /// </summary>
        DateEgal,
        /// <summary>
        /// La date comparée est inférieur à la date Model.
        /// La date1 est plus petite que la date2
        /// </summary>
        DateInferieur,
        /// <summary>
        /// Inconnue
        /// </summary>
        Unknow
    }


    public enum DateRemainsModele
    {
        /// <summary>
        /// Il vous reste ...
        /// </summary>
        Modele1,
        /// <summary>
        /// Tant de jours restant ...
        /// </summary>
        Modele2
    }

    public static class DateTimeHelpersExtensions
    {
        public static DateCompare CompareDate(this DateTime Date1, DateTime Date2)
        {
            return DatesHelpers.Analysis.CompareDate(Date1, Date2);
        }

    }
    internal class DatesHelpers
    {
        public const string NoAnswer = "N/A";
        public static IEnumerable<string> ChooseDays() => new List<string>()
        {
            {NoAnswer },
            {"1" },
            {"2" },
            {"3" },
            {"4" },
            {"5" },
            {"6" },
            {"7" },
            {"8" },
            {"9" },
            {"10" },
            {"11" },
            {"12" },
            {"13" },
            {"14" },
            {"15" },
            {"16" },
            {"17" },
            {"18" },
            {"19" },
            {"20" },
            {"21" },
            {"22" },
            {"23" },
            {"24" },
            {"25" },
            {"26" },
            {"27" },
            {"28" },
            {"29" },
            {"30" },
            {"31" },
        };

        public static IEnumerable<string> ChooseMonth() => new List<string>()
        {
            {NoAnswer },
            {"Janvier" },
            {"Février" },
            {"Mars" },
            {"Avril" },
            {"Mai" },
            {"Juin" },
            {"Juillet" },
            {"Août" },
            {"Septembre" },
            {"Octobre" },
            {"Novembre" },
            {"Décembre" },
        };

        public static IEnumerable<string> ChooseYear() => Enumerable.Range(1900, 130).Select(s => s.ToString());
        

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
            public static string StringDateToStringDate(string value, char separator, out string day, out string month, out string year, bool isMonthString = true)
            {
                try
                {
                    var splitString = StringHelpers.SplitWord(value, new string[] { separator.ToString() });
                    if (splitString != null && splitString.Length > 0)
                    {
                        if (splitString.Length == 1)
                        {
                            day = null;
                            month = null;
                            year = splitString[0];
                            return splitString[0];
                        }
                        else if (splitString.Length == 2)
                        {
                            day = null;
                            month = !isMonthString ? splitString[0] : ChooseMonth().ToList()[Convert.ToInt32(splitString[0])];
                            year = splitString[1];
                            return $"{splitString[0]}/{splitString[1]}";
                        }
                        else if (splitString.Length == 3)
                        {
                            day = splitString[0];
                            month = !isMonthString ? splitString[1] : ChooseMonth().ToList()[Convert.ToInt32(splitString[1])];
                            year = splitString[2];
                            return $"{splitString[0]}/{splitString[1]}/{splitString[2]}";
                        }
                    }

                    day = null;
                    month = null;
                    year = null;
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    day = null;
                    month = null;
                    year = null;
                    return null;
                }
            }
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

            public static TimeSpan? GetNullableTimeSpanFromString(string value)
            {
                try
                {
                    if (value.IsStringNullOrEmptyOrWhiteSpace() == true)
                    {
                        return null;
                    }

                    if (TimeSpan.TryParse(value, out TimeSpan timeSpan))
                    {
                        return timeSpan;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
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

        internal struct Analysis
        {
            /// <summary>
            /// Obtient l'interval entre deux dates
            /// </summary>
            /// <param name="dateTime1">Généralement la plus petite date ou la date de départ</param>
            /// <param name="dateTime2">Généralement la plus grande date ou la date de fin</param>
            /// <returns>Un objet <see cref="TimeSpan"/></returns>
            public static TimeSpan GetInterval(DateTime dateTime1, DateTime dateTime2)
            {
                try
                {
                    TimeSpan interval = dateTime2.Subtract(dateTime1);
                    return interval;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return TimeSpan.Zero;
                }
            }

            /// <summary>
            /// Obtient une valeur qui teste si la date actuelle est supérieure ou égale à la date fourni
            /// </summary>
            /// <param name="currentDate">Date actuelle</param>
            /// <param name="compareDate">date fourni</param>
            /// <returns>un objet <see cref="bool"/></returns>
            public static bool GreaterThanOrEqual(DateTime currentDate, DateTime compareDate) => currentDate >= compareDate;

            /// <summary>
            /// Obtient une valeur qui teste si la date actuelle est supérieure à la date fourni
            /// </summary>
            /// <param name="currentDate">Date actuelle</param>
            /// <param name="compareDate">date fourni</param>
            /// <returns>un objet <see cref="bool"/></returns>
            public static bool GreaterThan(DateTime currentDate, DateTime compareDate) => currentDate > compareDate;

            /// <summary>
            /// Obtient une valeur qui teste si la date actuelle est inférieure ou égale à la date fourni
            /// </summary>
            /// <param name="currentDate">Date actuelle</param>
            /// <param name="compareDate">date fourni</param>
            /// <returns>un objet <see cref="bool"/></returns>
            public static bool LessThanOrEqual(DateTime currentDate, DateTime compareDate) => currentDate <= compareDate;

            /// <summary>
            /// Obtient une valeur qui teste si la date actuelle est inférieure à la date fourni
            /// </summary>
            /// <param name="currentDate">Date actuelle</param>
            /// <param name="compareDate">date fourni</param>
            /// <returns>un objet <see cref="bool"/></returns>
            public static bool LessThan(DateTime currentDate, DateTime compareDate) => currentDate < compareDate;


            /// <summary>
            /// Compare la <paramref name="Date1"/> la par rapport à la <paramref name="Date2"/>
            /// </summary>
            /// <param name="Date1">Date 1</param>
            /// <param name="Date2">Date 2</param>
            /// <returns></returns>
            public static DateCompare CompareDate(DateTime Date1, DateTime Date2)
            {
                try
                {
                    int? result = null;
                    result = DateTime.Compare(Date1, Date2);


                    if (result < 0) //t1 est antérieur à t2
                    {
                        return DateCompare.DateInferieur;
                    }
                    else if (result == 0) //t1 est identique à t2 
                    {
                        return DateCompare.DateEgal;
                    }
                    else if (result > 0) //t1 est ultérieur à t2
                    {
                        return DateCompare.DateSuperieur;
                    }
                    return DateCompare.Unknow;
                }
                catch (Exception)
                {
                    return DateCompare.Unknow;
                }
            }

            public static System.TimeSpan? GetDateAndTimeRemains(DateTime DateDebut, DateTime DateFin)
            {
                try
                {
                    if (DateDebut == null || DateFin == null)
                    {
                        return null;
                    }

                    System.TimeSpan diff = DateFin.Subtract(DateDebut);
                    if (diff != null)
                    {
                        return diff;
                    }

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }



            public static string GetDateAndTimeRemainsPrefixe(string Prefixe, DateTime DateDebut, DateTime DateFin)
            {
                try
                {
                    if (DateDebut == null || DateFin == null)
                    {
                        return null;
                    }

                    if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(Prefixe))
                    {
                        return null;
                    }

                    System.TimeSpan GetDifference = DateFin.Subtract(DateDebut);

                    if (GetDifference == null)
                    {
                        return null;
                    }

                    if (GetDifference.Days > 0)
                    {
                        string text = $"{Prefixe} {GetDifference.Days} {((GetDifference.Days > 1) ? "jours" : "jour")}";
                        if (GetDifference.Hours > 0)
                        {
                            text += $" et {GetDifference.Hours} {((GetDifference.Hours > 1) ? "heures" : "heure")}";
                        }
                        return text;
                    }
                    else if (GetDifference.Days == 0)
                    {
                        string text = null;
                        if (GetDifference.Hours > 0)
                        {
                            text = $"{Prefixe} {GetDifference.Hours} {((GetDifference.Hours > 1) ? "heures" : "heure")}";
                            if (GetDifference.Minutes > 0)
                            {
                                text += $" et {GetDifference.Minutes} {((GetDifference.Minutes > 1) ? "minutes" : "minute")}";
                            }
                        }
                        else if (GetDifference.Hours == 0)
                        {
                            if (GetDifference.Minutes > 0)
                            {
                                text = $"{Prefixe} {GetDifference.Minutes} {((GetDifference.Minutes > 1) ? "minutes" : "minute")}";
                                if (GetDifference.Seconds > 0)
                                {
                                    text += $" et {GetDifference.Seconds} {((GetDifference.Seconds > 1) ? "secondes" : "seconde")}";
                                }
                            }
                            else if (GetDifference.Minutes == 0)
                            {
                                if (GetDifference.Seconds > 0)
                                {
                                    text = $"{Prefixe} {GetDifference.Seconds} {((GetDifference.Seconds > 1) ? "secondes" : "seconde")}";
                                }
                            }
                        }
                        return text;
                    }

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static string GetDateAndTimeRemainsSufixe(string Sufixe, DateTime DateDebut, DateTime DateFin)
            {
                try
                {
                    if (DateDebut == null || DateFin == null)
                    {
                        return null;
                    }

                    if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(Sufixe))
                    {
                        return null;
                    }

                    System.TimeSpan GetDifference = DateFin.Subtract(DateDebut);

                    if (GetDifference == null)
                    {
                        return null;
                    }

                    if (GetDifference.Days > 0)
                    {
                        string text = $"{GetDifference.Days} {((GetDifference.Days > 1) ? "jours" : "jour")} {Sufixe}";
                        if (GetDifference.Hours > 0)
                        {
                            text += $" et {GetDifference.Hours} {((GetDifference.Hours > 1) ? "heures" : "heure")}";
                        }
                        return text;
                    }
                    else if (GetDifference.Days == 0)
                    {
                        string text = null;
                        if (GetDifference.Hours > 0)
                        {
                            text = $"{GetDifference.Hours} {((GetDifference.Hours > 1) ? "heures" : "heure")} {Sufixe}";
                            if (GetDifference.Minutes > 0)
                            {
                                text += $" et {GetDifference.Minutes} {((GetDifference.Minutes > 1) ? "minutes" : "minute")}";
                            }
                        }
                        else if (GetDifference.Hours == 0)
                        {
                            if (GetDifference.Minutes > 0)
                            {
                                text = $"{GetDifference.Minutes} {((GetDifference.Minutes > 1) ? "minutes" : "minute")} {Sufixe}";
                                if (GetDifference.Seconds > 0)
                                {
                                    text += $" et {GetDifference.Seconds} {((GetDifference.Seconds > 1) ? "secondes" : "seconde")}";
                                }
                            }
                            else if (GetDifference.Minutes == 0)
                            {
                                if (GetDifference.Seconds > 0)
                                {
                                    text = $"{GetDifference.Seconds} {((GetDifference.Seconds > 1) ? "secondes" : "seconde")} {Sufixe}";
                                }
                            }
                        }
                        return text;
                    }

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /// <summary>
            /// Obtient le nombre de jour, d'heure, de minute et de seconde entre la date actuelle (<paramref name="ActualDate"/>) et une date de fin (<paramref name="DateFin"/>) [inférieur à <paramref name="ActualDate"/>] avec un préfixe
            /// </summary>
            /// <param name="Prefixe">préfixe</param>
            /// <param name="ActualDate">Date actuelle</param>
            /// <param name="DateFin">Date de fin</param>
            /// <returns></returns>
            public static string GetDateAndTimeElapsedPrefixe(string Prefixe, DateTime ActualDate, DateTime DateFin)
            {
                try
                {
                    if (ActualDate == null || DateFin == null)
                    {
                        return null;
                    }

                    if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(Prefixe))
                    {
                        return null;
                    }

                    System.TimeSpan GetDifference = DateTime.UtcNow.Subtract((DateTime)DateFin);

                    if (GetDifference != null)
                    {
                        if (GetDifference.Days < 0 || GetDifference.Hours < 0 || GetDifference.Minutes < 0 || GetDifference.Seconds < 0 || GetDifference.Milliseconds < 0)
                        {
                            return null;
                        }

                        if (GetDifference == null)
                        {
                            return null;
                        }

                        if (GetDifference.Days > 0)
                        {
                            string text = $"{Prefixe} {GetDifference.Days} {((GetDifference.Days > 1) ? "jours" : "jour")}";
                            if (GetDifference.Hours > 0)
                            {
                                text += $" et {GetDifference.Hours} {((GetDifference.Hours > 1) ? "heures" : "heure")}";
                            }
                            return text;
                        }
                        else if (GetDifference.Days == 0)
                        {
                            string text = null;
                            if (GetDifference.Hours > 0)
                            {
                                text = $"{Prefixe} {GetDifference.Hours} {((GetDifference.Hours > 1) ? "heures" : "heure")}";
                                if (GetDifference.Minutes > 0)
                                {
                                    text += $" et {GetDifference.Minutes} {((GetDifference.Minutes > 1) ? "minutes" : "minute")}";
                                }
                            }
                            else if (GetDifference.Hours == 0)
                            {
                                if (GetDifference.Minutes > 0)
                                {
                                    text = $"{Prefixe} {GetDifference.Minutes} {((GetDifference.Minutes > 1) ? "minutes" : "minute")}";
                                    if (GetDifference.Seconds > 0)
                                    {
                                        text += $" et {GetDifference.Seconds} {((GetDifference.Seconds > 1) ? "secondes" : "seconde")}";
                                    }
                                }
                                else if (GetDifference.Minutes == 0)
                                {
                                    if (GetDifference.Seconds > 0)
                                    {
                                        text = $"{Prefixe} {GetDifference.Seconds} {((GetDifference.Seconds > 1) ? "secondes" : "seconde")}";
                                    }
                                }
                            }
                            return text;
                        }
                    }
                    return null;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static double? DivideTime(TimeSpan dividend, TimeSpan divisor)
            {
                try
                {
                    return (double)dividend.Ticks / (double)divisor.Ticks;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static double? DivideRestTime(TimeSpan dividend, TimeSpan divisor)
            {
                try
                {
                    return (double)dividend.Ticks % (double)divisor.Ticks;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static double? GetDateTimeRemainsPurcent(DateTime DateDebut, DateTime DateFin)
            {
                try
                {
                    if (DateDebut == null || DateFin == null)
                    {
                        return null;
                    }

                    TimeSpan timeDebut = new TimeSpan(DateDebut.Day, DateDebut.Hour, DateDebut.Minute, DateDebut.Second, DateDebut.Millisecond);
                    TimeSpan timeFin = new TimeSpan(DateFin.Day, DateFin.Hour, DateFin.Minute, DateFin.Second, DateFin.Millisecond);


                    if (timeDebut == null || timeFin == null)
                    {
                        return null;
                    }
                    double? result = DivideTime(timeFin, timeDebut);

                    if (result == null)
                    {
                        return null;
                    }

                    return result * 100;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static string GetDateTimeRemainsPurcentString(DateTime DateDebut, DateTime DateFin)
            {
                try
                {
                    if (DateDebut == null || DateFin == null)
                    {
                        return null;
                    }

                    TimeSpan timeDebut = new TimeSpan(DateDebut.Day, DateDebut.Hour, DateDebut.Minute, DateDebut.Second, DateDebut.Millisecond);
                    TimeSpan timeFin = new TimeSpan(DateFin.Day, DateFin.Hour, DateFin.Minute, DateFin.Second, DateFin.Millisecond);


                    if (timeDebut == null || timeFin == null)
                    {
                        return null;
                    }
                    double? result = DivideTime(timeFin, timeDebut);
                    return result?.ToString("P", new CultureInfo("fr-FR", false).NumberFormat);
                }
                catch (Exception)
                {
                    return null;
                }
            }


            public static string GetDateTimeRemainsPurcentResteString(DateTime DateDebut, DateTime DateFin)
            {
                try
                {
                    if (DateDebut == null || DateFin == null)
                    {
                        return null;
                    }

                    TimeSpan timeDebut = new TimeSpan(DateDebut.Day, DateDebut.Hour, DateDebut.Minute, DateDebut.Second, DateDebut.Millisecond);
                    TimeSpan timeFin = new TimeSpan(DateFin.Day, DateFin.Hour, DateFin.Minute, DateFin.Second, DateFin.Millisecond);


                    if (timeDebut == null || timeFin == null)
                    {
                        return null;
                    }
                    double? result = 100.0d - DivideTime(timeFin, timeDebut);
                    return result?.ToString("P", new CultureInfo("fr-FR", false).NumberFormat);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static string PurcentDate(DateTime DateDebut, DateTime DateFin, out string Exceeded)
            {
                try
                {
                    if (DateDebut == null || DateFin == null)
                    {
                        Exceeded = null;
                        return null;
                    }
                    System.TimeSpan GetDifference = DateFin.Subtract(DateDebut);
                    if (GetDifference == null)
                    {
                        Exceeded = null;
                        return null;
                    }
                    Thread.Sleep(500);

                    System.TimeSpan GetDifference2 = DateTime.UtcNow.Subtract(DateDebut);
                    //var percentage = GetDifference2.TotalSeconds * 100/ GetDifference.TotalSeconds;
                    var percentage = GetDifference2.TotalSeconds / GetDifference.TotalSeconds;
                    Exceeded = (percentage > 1.0000d) ? (percentage - 1.0000d).ToString("P", new CultureInfo("fr-FR", false).NumberFormat) : "0";
                    return percentage.ToString("P", new CultureInfo("fr-FR", false).NumberFormat);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

    }
}
