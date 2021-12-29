using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class TbookLangue
    {
        public long Id { get; set; }
        public long IdBook { get; set; }
        public string Langue { get; set; }

        public virtual Tbook IdBookNavigation { get; set; }
    }
}
