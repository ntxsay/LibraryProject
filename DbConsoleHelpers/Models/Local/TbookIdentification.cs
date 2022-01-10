using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class TbookIdentification
    {
        public long Id { get; set; }
        public string Isbn { get; set; }
        public string Isbn10 { get; set; }
        public string Isbn13 { get; set; }
        public string Issn { get; set; }
        public string Asin { get; set; }
        public string Cotation { get; set; }
        public string CodeBarre { get; set; }

        public virtual Tbook IdNavigation { get; set; }
    }
}
