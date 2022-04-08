using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LibraryProjectUWP.Models.Local
{
    public partial class TbookClassification
    {
        public long Id { get; set; }
        public long TypeClassification { get; set; }
        public long ApartirDe { get; set; }
        public long Jusqua { get; set; }
        public long DeTelAge { get; set; }
        public long AtelAge { get; set; }

        public virtual Tbook IdNavigation { get; set; }
    }
}
