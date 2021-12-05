using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RostalConsoleHelpers.Models.Local
{
    public partial class TlibraryCategorie
    {
        public TlibraryCategorie()
        {
            TlibraryBookConnector = new HashSet<TlibraryBookConnector>();
            TlibrarySubCategorie = new HashSet<TlibrarySubCategorie>();
        }

        public long Id { get; set; }
        public long IdLibrary { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual Tlibrary IdLibraryNavigation { get; set; }
        public virtual ICollection<TlibraryBookConnector> TlibraryBookConnector { get; set; }
        public virtual ICollection<TlibrarySubCategorie> TlibrarySubCategorie { get; set; }
    }
}
