using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LibraryProjectUWP.Models.Local
{
    public partial class Tlibrary
    {
        public Tlibrary()
        {
            TlibraryBookConnector = new HashSet<TlibraryBookConnector>();
            TlibraryCategorie = new HashSet<TlibraryCategorie>();
        }

        public long Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DateAjout { get; set; }
        public string DateEdition { get; set; }

        public virtual ICollection<TlibraryBookConnector> TlibraryBookConnector { get; set; }
        public virtual ICollection<TlibraryCategorie> TlibraryCategorie { get; set; }
    }
}
