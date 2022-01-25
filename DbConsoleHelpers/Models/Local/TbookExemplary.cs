using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class TbookExemplary
    {
        public TbookExemplary()
        {
            TbookEtat = new HashSet<TbookEtat>();
            TbookPret = new HashSet<TbookPret>();
        }

        public long Id { get; set; }
        public long IdBook { get; set; }
        public long? IdContactSource { get; set; }
        public string NoGroup { get; set; }
        public string NoExemplary { get; set; }
        public long Quantity { get; set; }
        public string DateAjout { get; set; }
        public string DateEdition { get; set; }
        public string TypeAcquisition { get; set; }
        public double? Price { get; set; }
        public string DeviceName { get; set; }
        public string DateAcquisition { get; set; }
        public string DateRemise { get; set; }
        public long IsJourAcquisitionKnow { get; set; }
        public long IsMoisAcquisitionKnow { get; set; }
        public long IsVisible { get; set; }
        public string Observations { get; set; }

        public virtual Tbook IdBookNavigation { get; set; }
        public virtual Tcontact IdContactSourceNavigation { get; set; }
        public virtual ICollection<TbookEtat> TbookEtat { get; set; }
        public virtual ICollection<TbookPret> TbookPret { get; set; }
    }
}
