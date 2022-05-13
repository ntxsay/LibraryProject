using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code
{
    public struct Search
    {
        public enum Terms
        {
            Equals,
            Contains,
            StartWith,
            EndWith,
        }

        public enum In
        {
            MainTitle,
            OtherTitle,
            Author,
            Collection,
            Editor,
        }

        public const string TermContains = "Contient";
        public const string TermEquals = "Correspond à";
        public const string TermStartWith = "Commence par";
        public const string TermEndWith = "Se termine par";
        public static IEnumerable<string> SearchOnList => new List<string>()
                {
                    TermContains,
                    TermEquals,
                    TermStartWith,
                    TermEndWith,
                };

        public static Dictionary<byte, string> SearchOnListDictionary => new Dictionary<byte, string>()
                {
                    {(byte)Terms.Contains, TermContains },
                    {(byte)Terms.Equals, TermEquals },
                    {(byte)Terms.StartWith, TermStartWith },
                    {(byte)Terms.EndWith, TermEndWith },
                };
    }

    public enum EditMode
    {
        Create,
        Edit,
    }

    public enum FolderType
    {
        Posters,
        ScreenCaptures,
        Trailers,
        Logo,
    }

    public enum CategorieType
    {
        Categorie,
        SubCategorie,
    }

    public enum ManagePage
    {
        Book,
        Library,
        Contacts,
        Settings,
        None,
    }

    public enum CollectionTypeEnum : short
    {
        All = -1,
        Collection = 0,
        Playlist = 1,
    }

    public enum DataViewModeEnum
    {
        DataGridView,
        GridView,
    }

    public enum ContactType : byte
    {
        Human = 0,
        Society = 1,
    }

    public enum ContactRole : byte
    {
        Adherant = 0,
        Author = 1,
        EditorHouse = 2,
        Translator = 3,
        Illustrator = 4
    }

    public enum BookTypeVerification : byte
    {
        Entree,
        AvantPret,
        ApresPret,
        Sortie,
    }

    public enum BookTypeAcquisition : byte
    {
        Achat,
        Pret,
        Don,
        Autre,
    }

    public enum ClassificationAgeType : byte
    {
        ToutPublic,
        ApartirDe,
        Jusqua,
        DeTantATant,
    }

    public enum BookPretFrom : byte
    {
        Emprunteur,
        Book,
        BookExemplaire,
    }

    public enum EnumTaskId : byte
    {
        SearchBooks,
        ResearchBooks,
        CountBooks,
        SearchBookExemplary,
        SearchBookPret,
        CompleteInfoBook,
    }
}
