using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code
{
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
        Adherant,
        Author,
        EditorHouse,
        Enterprise,
    }
}
