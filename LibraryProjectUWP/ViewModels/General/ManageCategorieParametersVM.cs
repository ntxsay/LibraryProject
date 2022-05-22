using LibraryProjectUWP.Code;
using LibraryProjectUWP.ViewModels.Categorie;
using LibraryProjectUWP.ViewModels.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.General
{
    public class ManageCategorieDialogParametersVM
    {
        public BibliothequeVM ParentLibrary { get; set; }
        public CategorieLivreVM CurrentCategorie { get; set; }
        public EditMode EditMode { get; set; }
        //public CategorieType Type { get; set; }
    }

    public class ManageSubCategorieDialogParametersVM
    {
        public CategorieLivreVM Categorie { get; set; }//Seulement pour les sous-catégories        
        public SubCategorieLivreVM CurrentSubCategorie { get; set; }
        public EditMode EditMode { get; set; }
        //public CategorieType Type { get; set; }
    }
}
