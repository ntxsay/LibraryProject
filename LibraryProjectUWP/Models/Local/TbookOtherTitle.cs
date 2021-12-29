using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LibraryProjectUWP.Models.Local
{
    public partial class TbookOtherTitle
    {
        public long Id { get; set; }
        public long IdBook { get; set; }
        public string Title { get; set; }

        public virtual Tbook IdBookNavigation { get; set; }
    }
}
