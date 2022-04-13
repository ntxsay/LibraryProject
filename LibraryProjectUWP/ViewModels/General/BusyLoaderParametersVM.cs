using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.General
{
    public class BusyLoaderParametersVM
    {
        public object Parameter { get; set; }
        public Action Callback { get; set; }
        public string ProgessText { get; set; }
    }

    public class BusyLoaderParametersVM<T, U> where T : U
    {
        //public object Parameter { get; set; }
        public Action<T> Callback { get; set; }
        public Action Callback2 { get; set; }
        public T Parameter { get; set; }

    }
}
