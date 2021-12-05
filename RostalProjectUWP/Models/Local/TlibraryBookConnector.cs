using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RostalProjectUWP.Models.Local
{
    public partial class TlibraryBookConnector
    {
        public long Id { get; set; }
        public long IdLibrary { get; set; }
        public long? IdCategorie { get; set; }
        public long? IdSubCategorie { get; set; }
        public long IdBook { get; set; }

        public virtual Tbooks IdBookNavigation { get; set; }
        public virtual TlibraryCategorie IdCategorieNavigation { get; set; }
        public virtual Tlibrary IdLibraryNavigation { get; set; }
        public virtual TlibrarySubCategorie IdSubCategorieNavigation { get; set; }
    }
}
