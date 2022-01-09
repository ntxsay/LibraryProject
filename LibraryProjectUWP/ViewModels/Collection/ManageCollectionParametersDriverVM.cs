using LibraryProjectUWP.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Collection
{
    public class ManageCollectionParametersDriverVM
    {
        public IEnumerable<CollectionVM> ViewModelList { get; set; }
        public CollectionVM CurrentViewModel { get; set; }
        public EditMode EditMode { get; set; }
    }

    public class CollectionListParametersDriverVM
    {
        public IEnumerable<CollectionVM> ViewModelList { get; set; }
        public CollectionVM CurrentViewModel { get; set; }
        public BibliothequeVM ParentLibrary { get; set; }
    }
}
