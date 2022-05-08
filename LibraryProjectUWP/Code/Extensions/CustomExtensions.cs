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
        /// <summary>
        /// Crée une copie profonde d'un objet
        /// </summary>
        /// <remarks>Remarques : Cette extension crée une nouvelle référence de l'objet </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T self) where T : new()
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        /// <summary>
        /// Crée une copie profonde d'un objet
        /// </summary>
        /// <remarks>Remarques : Cette extension permet de garder la même référence</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T self, T source) where T : new()
        {
            Type type = typeof(T);
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object sourceValue = type.GetProperty(pi.Name).GetValue(source, null);
                type.GetProperty(pi.Name).SetValue(self, sourceValue);
            }

            return self;
        }
    }
}
