using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class TlibrarySubCategorie
    {
        public TlibrarySubCategorie()
        {
            Tbook = new HashSet<Tbook>();
        }

        public long Id { get; set; }
        public long IdCategorie { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual TlibraryCategorie IdCategorieNavigation { get; set; }
        public virtual ICollection<Tbook> Tbook { get; set; }
    }
}
