using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RostalProjectUWP.Code;
using RostalProjectUWP.ViewModels;

namespace RostalProjectUWP.ViewModels.General
{

    public class ManageBookParametersVM
    {
        public LivreVM ViewModel { get; set; }
        public EditMode EditMode { get; set; }
        public string ImageBackgroundPath { get; set; }
    }

}
