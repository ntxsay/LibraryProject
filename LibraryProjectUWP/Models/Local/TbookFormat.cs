using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LibraryProjectUWP.Models.Local
{
    public partial class TbookFormat
    {
        public long Id { get; set; }
        public string Format { get; set; }
        public long? NbOfPages { get; set; }
        public double? Largeur { get; set; }
        public double? Hauteur { get; set; }
        public double? Epaisseur { get; set; }

        public virtual Tbook IdNavigation { get; set; }
    }
}
