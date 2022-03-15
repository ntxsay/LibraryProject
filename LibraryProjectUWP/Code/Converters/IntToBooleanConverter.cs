using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace LibraryProjectUWP.Code.Converters
{
    public class IntToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is int counter)
                {
                    return counter > 0;
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
            throw new NotImplementedException();
        }
    }

    public class IntGreaterThanOrEqualToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter != null)
                {
                    var number = System.Convert.ToInt32(parameter);
                    if (value is long int64)
                    {
                        return int64 >= number;
                    }
                    else if (value is int int32)
                    {
                        return int32 >= number;
                    }
                    else if (value is short int16)
                    {
                        return int16 >= number;
                    }
                    else if (value is byte int8)
                    {
                        return int8 >= number;
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
            throw new NotImplementedException();
        }
    }

    public class IntGreaterThanToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter != null)
                {
                    var number = System.Convert.ToInt32(parameter);
                    if (value is long int64)
                    {
                        return int64 > number;
                    }
                    else if (value is int int32)
                    {
                        return int32 > number;
                    }
                    else if (value is short int16)
                    {
                        return int16 > number;
                    }
                    else if (value is byte int8)
                    {
                        return int8 > number;
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
            throw new NotImplementedException();
        }
    }

    public class IntLessThanOrEqualToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter != null)
                {
                    var number = System.Convert.ToInt32(parameter);
                    if (value is long int64)
                    {
                        return int64 <= number;
                    }
                    else if (value is int int32)
                    {
                        return int32 <= number;
                    }
                    else if (value is short int16)
                    {
                        return int16 <= number;
                    }
                    else if (value is byte int8)
                    {
                        return int8 <= number;
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
            throw new NotImplementedException();
        }
    }

    public class IntLessThanToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter != null)
                {
                    var number = System.Convert.ToInt32(parameter);
                    if (value is long int64)
                    {
                        return int64 < number;
                    }
                    else if (value is int int32)
                    {
                        return int32 < number;
                    }
                    else if (value is short int16)
                    {
                        return int16 < number;
                    }
                    else if (value is byte int8)
                    {
                        return int8 < number;
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
            throw new NotImplementedException();
        }
    }

    public class IntEqualToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter != null)
                {
                    var number = System.Convert.ToInt32(parameter);
                    if (value is long int64)
                    {
                        return int64 == number;
                    }
                    else if (value is int int32)
                    {
                        return int32 == number;
                    }
                    else if (value is short int16)
                    {
                        return int16 == number;
                    }
                    else if (value is byte int8)
                    {
                        return int8 == number;
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
            throw new NotImplementedException();
        }
    }

    public class IntDifferentToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter != null)
                {
                    var number = System.Convert.ToInt32(parameter);
                    if (value is long int64)
                    {
                        return int64 != number;
                    }
                    else if (value is int int32)
                    {
                        return int32 != number;
                    }
                    else if (value is short int16)
                    {
                        return int16 != number;
                    }
                    else if (value is byte int8)
                    {
                        return int8 != number;
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
            throw new NotImplementedException();
        }
    }


    public class IntToInvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is int counter)
                {
                    return !(counter > 0);
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
            throw new NotImplementedException();
        }
    }
}
