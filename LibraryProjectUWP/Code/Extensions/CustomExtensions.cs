using LibraryProjectUWP.ViewModels.Book;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Extensions
{
    public static class CustomExtensions
    {
        public static T DeepCopy<T>(this T self) where T : new()
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static T DeepCopy<T>(this T self, T source) where T : new()
        {
            Type type = typeof(T);
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object sourceValue = type.GetProperty(pi.Name).GetValue(source, null);
                type.GetProperty(pi.Name).SetValue(source, sourceValue);
            }

            return self;
        }

        //public static void DeepCopy(this LivreVM source, LivreVM viewModelToCopy)
        //{
        //    var serialized = JsonConvert.SerializeObject(viewModelToCopy);
        //    source = JsonConvert.DeserializeObject<LivreVM>(serialized);
        //}
    }
}
