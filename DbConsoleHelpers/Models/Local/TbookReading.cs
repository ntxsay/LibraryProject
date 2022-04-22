using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class TbookReading
    {
        public long Id { get; set; }
        public long? Status { get; set; }
        public long? LastPageReaded { get; set; }
        public string LastDateReaded { get; set; }
        public double? Note10 { get; set; }

        public virtual Tbook IdNavigation { get; set; }
    }
}
