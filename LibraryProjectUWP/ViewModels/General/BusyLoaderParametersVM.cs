using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace LibraryProjectUWP.ViewModels.General
{
    public class BusyLoaderParametersVM
    {        
        public string ProgessText { get; set; }
        public Visibility PrimaryButtonVisibility { get; set; }
        public string PrimaryButtonText { get; set; }
        public Visibility SecondaryButtonVisibility { get; set; }
        public string SecondaryButtonText { get; set; }
        public Visibility CancelButtonVisibility { get; set; }
        public string CancelButtonText { get; set; }
        public object PrimaryButtonParameter { get; set; }
        public Action PrimaryButtonCallback { get; set; }
        public object SecondaryButtonParameter { get; set; }
        public Action SecondaryButtonCallback { get; set; }
        public object CancelButtonParameter { get; set; }
        public Action CancelButtonCallback { get; set; }
        public object OpenedLoaderParameter { get; set; }
        public Action OpenedLoaderCallback { get; set; }

    }

}
