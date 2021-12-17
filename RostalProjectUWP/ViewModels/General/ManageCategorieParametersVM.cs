﻿using RostalProjectUWP.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RostalProjectUWP.ViewModels.General
{
    public class ManageCategorieDialogParametersVM
    {
        [Obsolete]
        public string Value { get; set; }
        
        [Obsolete]
        public string Description { get; set; }
        public BibliothequeVM ParentLibrary { get; set; }
        public CategorieLivreVM CurrentCategorie { get; set; }
        public IEnumerable<CategorieLivreVM> ViewModelList { get; set; }
        public EditMode EditMode { get; set; }
        //public CategorieType Type { get; set; }
    }

    public class ManageSubCategorieDialogParametersVM
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public CategorieLivreVM Categorie { get; set; }//Seulement pour les sous-catégories
        public IEnumerable<SubCategorieLivreVM> ViewModelList { get; set; }
        public EditMode EditMode { get; set; }
        //public CategorieType Type { get; set; }
    }

    public class ManageLibraryDialogParametersVM
    {
        [Obsolete]
        public string Value { get; set; }

        [Obsolete]
        public string Description { get; set; }
        public IEnumerable<BibliothequeVM> ViewModelList { get; set; }
        public BibliothequeVM CurrentLibrary { get; set; }
        public EditMode EditMode { get; set; }
    }
}
