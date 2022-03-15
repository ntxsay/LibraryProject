using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Helpers
{
    public static class BookHelpersExtension
    {
        public static void Copy(this LivreVM source, LivreVM viewModelToCopy)
        {
            DbServices.Book.DeepCopy(source, viewModelToCopy);
        }

        public static void Copy(this CollectionVM source, CollectionVM viewModelToCopy)
        {
            DbServices.Collection.DeepCopy(source, viewModelToCopy);
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

    public class LibraryHelpers
    {
        public struct Book
        {
            public static string GetDimensionsInCm(double longueur, double largeur, double epaisseur)
            {
                try
                {
                    if (longueur <= 0 || largeur <= 0 || epaisseur <= 0)
                    {
                        return string.Empty;
                    }
                    return $"{longueur} cm × {largeur} cm × {epaisseur} cm";
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            public struct Entry
            {
                public const string Achat = "Achat";
                public const string Pret = "Prêt";
                public const string Don = "Don";
                public const string Autre = "Autre";

                public static IEnumerable<string> EntrySourceList => new List<string>()
                {
                    Achat,
                    Pret,
                    Don,
                    Autre,
                };

                public static Dictionary<byte, string> TypeAcquisitionDictionary = new Dictionary<byte, string>()
                {
                    {(byte)BookTypeAcquisition.Achat, Achat },
                    {(byte)BookTypeAcquisition.Pret, Pret },
                    {(byte)BookTypeAcquisition.Don, Don },
                    {(byte)BookTypeAcquisition.Autre, Autre }
                };
            }

            public static IEnumerable<string> EtatModelList => new List<string>()
            {
                "Neuf",
                "Comme neuf",
                "Très bon",
                "Bon",
                "Assez bon",
                "Satisfaisant",
                "Moyen",
                "Mauvais",
                "Je ne sais pas",
            };
        }
        

        

        public struct Contact
        {
            public const string Adherant = "Adhérant";
            public const string Author = "Auteur";
            public const string HouseEditor = "Maison d'édition";
            public const string Enterprise = "Société";

            public static IEnumerable<string> ContactList => new List<string>()
            {
                Adherant,
                Author,
                HouseEditor,
                Enterprise,
            };

            public static Dictionary<byte, string> ContactTypeDictionary = new Dictionary<byte, string>()
            {
                {(byte)ContactType.Adherant, Adherant },
                {(byte)ContactType.Author, Author },
                {(byte)ContactType.EditorHouse, HouseEditor },
                {(byte)ContactType.Enterprise, Enterprise }
            };
        }
    }
}
