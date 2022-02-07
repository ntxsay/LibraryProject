using LibraryProjectUWP.Views;
using LibraryProjectUWP.Views.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Settings
{
    public class ManageParametersDriverVM
    {
        public ManageContainerPage ParentPage { get; set; }
    }

    public class ManageSettingsParentChildVM
    {
        public ManageContainerPage ParentPage { get; set; }
        public ManageParametersDriverVM Parameters { get; set; }
    }
}
