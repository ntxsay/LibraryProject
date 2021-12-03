using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RostalProjectUWP.Code;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.Views;
using RostalProjectUWP.Views.Book;

namespace RostalProjectUWP.ViewModels.General
{

    public class ManageBookParametersVM
    {
        public LivreVM ViewModel { get; set; }
        public EditMode EditMode { get; set; }
        public string ImageBackgroundPath { get; set; }
        public ManageContainerPage ParentPage { get; set; }
    }

    //public class ManageBookContainerParentVM
    //{
    //    public LivreVM ViewModel { get; set; }
    //    public ManageContainerPage ParentPage { get; set; }
    //    public ManageBookParametersVM Parameters { get; set; }
    //}

    public class ManageBookParentChildVM
    {
        public LivreVM ViewModel { get; set; }
        public ManageBookPage ParentPage { get; set; }
        public ManageBookParametersVM Parameters { get; set; }
    }

}
