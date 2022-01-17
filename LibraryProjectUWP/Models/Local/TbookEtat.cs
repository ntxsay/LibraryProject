using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LibraryProjectUWP.Models.Local
{
    public partial class TbookEtat
    {
        public TbookEtat()
        {
            TbookPretIdEtatAfterNavigation = new HashSet<TbookPret>();
            TbookPretIdEtatBeforeNavigation = new HashSet<TbookPret>();
        }

        public long Id { get; set; }
        public long IdBookExemplary { get; set; }
        public string DateAjout { get; set; }
        public long TypeVerification { get; set; }
        public string Etat { get; set; }
        public string Observations { get; set; }

        public virtual TbookExemplary IdBookExemplaryNavigation { get; set; }
        public virtual ICollection<TbookPret> TbookPretIdEtatAfterNavigation { get; set; }
        public virtual ICollection<TbookPret> TbookPretIdEtatBeforeNavigation { get; set; }
    }
}
