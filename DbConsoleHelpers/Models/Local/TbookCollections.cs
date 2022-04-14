using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class TbookCollections
    {
        public long Id { get; set; }
        public long IdBook { get; set; }
        public long IdCollection { get; set; }

        public virtual Tbook IdBookNavigation { get; set; }
        public virtual Tcollection IdCollectionNavigation { get; set; }
    }
}
