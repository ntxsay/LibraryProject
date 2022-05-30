using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace LibraryProjectUWP.ViewModels.General
{
    public class IntermediatePageToTertiaryPageDriverVM<T> where T : class
    {
        public Type Type => typeof(T);
        public IEnumerable<T> Tables { get; set; }
        public T Value { get; set; }
        public Page IntermediatePage { get; set; }
    }
}
