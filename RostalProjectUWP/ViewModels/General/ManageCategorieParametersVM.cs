using RostalProjectUWP.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RostalProjectUWP.ViewModels.General
{
    public class ManageCategorieDialogParametersVM
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string ParentName { get; set; }//Seulement pour les sous-catégories
        public IEnumerable<CategorieLivreVM> ViewModelList { get; set; }
        public EditMode EditMode { get; set; }
        public CategorieType Type { get; set; }
    }

    public class ManageLibraryDialogParametersVM
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public IEnumerable<BibliothequeVM> ViewModelList { get; set; }
        public EditMode EditMode { get; set; }
    }
}
