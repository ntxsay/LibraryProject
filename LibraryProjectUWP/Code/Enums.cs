using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code
{
    public class Search
    {
        public struct Book
        {
            
        }
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
