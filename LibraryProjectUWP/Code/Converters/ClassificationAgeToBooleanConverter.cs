using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using LibraryProjectUWP.ViewModels.Book;

namespace LibraryProjectUWP.Code.Converters
{
    public class ClassificationAgeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is ClassificationAgeType classificationAgeType && parameter is string type)
                {
                    switch (classificationAgeType)
                    {
                        case ClassificationAgeType.ToutPublic:
                            if (type == "tout-public")
                            {
                                return true;
                            }
                            break;
                        case ClassificationAgeType.ApartirDe:
                            if (type == "a-partir-de")
                            {
                                return true;
                            }
                            break;
                        case ClassificationAgeType.Jusqua:
                            if (type == "jusqua")
                            {
                                return true;
                            }
                            break;
                        case ClassificationAgeType.DeTantATant:
                            if (type == "de-tant-a-tant")
                            {
                                return true;
                            }
                            break;
                        default:
                            break;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is bool isChecked && isChecked == true && parameter is string type)
                {
                    if (type == "tout-public")
                    {
                        return ClassificationAgeType.ToutPublic;
                    }
                    else if (type == "a-partir-de")
                    {
                        return ClassificationAgeType.ApartirDe;
                    }
                    else if (type == "jusqua")
                    {
                        return ClassificationAgeType.Jusqua;
                    }
                    else if (type == "de-tant-a-tant")
                    {
                        return ClassificationAgeType.DeTantATant;
                    }
                }

                return ClassificationAgeType.ToutPublic;
            }
            catch (Exception)
            {
                return ClassificationAgeType.ToutPublic;
            }
        }
    }

    public class ClassificationAgeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is LivreClassificationAgeVM classificationAge)
                {
                    switch (classificationAge.TypeClassification)
                    {
                        case ClassificationAgeType.ToutPublic:
                            return "Tout public";
                        case ClassificationAgeType.ApartirDe:
                            return $"A partir de {classificationAge.ApartirDe} ans";
                        case ClassificationAgeType.Jusqua:
                            return $"Jusqu'à {classificationAge.Jusqua} ans";
                        case ClassificationAgeType.DeTantATant:
                            if (classificationAge.DeTelAge == classificationAge.ATelAge)
                            {
                                return $"{classificationAge.DeTelAge} ans uniquement";
                            }
                            else
                            {
                                return $"De {classificationAge.DeTelAge} à {classificationAge.ATelAge} ans";
                            }
                        default:
                            return string.Empty;
                    }
                }

                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
