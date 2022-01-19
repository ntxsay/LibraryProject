using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Helpers
{
    public class LibraryHelpers
    {
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
        };

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
