using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LibraryProjectUWP.Models.Local
{
    public partial class TlibrarySubCategorie
    {
        public TlibrarySubCategorie()
        {
            TlibraryBookConnector = new HashSet<TlibraryBookConnector>();
        }

        public long Id { get; set; }
        public long IdCategorie { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual TlibraryCategorie IdCategorieNavigation { get; set; }
        public virtual ICollection<TlibraryBookConnector> TlibraryBookConnector { get; set; }
    }
}
