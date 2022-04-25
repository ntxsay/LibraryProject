using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.General
{
    public class PropertiesChangedVM
    {
        public string PropertyName { get; set; }
        public string Message { get; set; }
        public object AValue { get; set; }
        public object BValue { get; set; }
    }
}
