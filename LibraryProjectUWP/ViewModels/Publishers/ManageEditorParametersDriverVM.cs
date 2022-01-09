using LibraryProjectUWP.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Publishers
{
    public class ManageEditorParametersDriverVM
    {
        public IEnumerable<PublisherVM> ViewModelList { get; set; }
        public PublisherVM CurrentViewModel { get; set; }
        public EditMode EditMode { get; set; }
    }

    public class EditorListParametersDriverVM
    {
        public IEnumerable<PublisherVM> ViewModelList { get; set; }
        public PublisherVM CurrentViewModel { get; set; }
    }
}
