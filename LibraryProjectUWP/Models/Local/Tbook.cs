using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LibraryProjectUWP.Models.Local
{
    public partial class Tbook
    {
        public Tbook()
        {
            TbookAuthorConnector = new HashSet<TbookAuthorConnector>();
            TbookCollectionConnector = new HashSet<TbookCollectionConnector>();
            TbookEditeurConnector = new HashSet<TbookEditeurConnector>();
            TbookExemplary = new HashSet<TbookExemplary>();
            TbookOtherTitle = new HashSet<TbookOtherTitle>();
            TlibraryBookConnector = new HashSet<TlibraryBookConnector>();
        }

        public long Id { get; set; }
        public string Guid { get; set; }
        public string DateAjout { get; set; }
        public string DateEdition { get; set; }
        public string MainTitle { get; set; }
        public long CountOpening { get; set; }
        public string DateParution { get; set; }
        public string Resume { get; set; }
        public string Notes { get; set; }
        public long TypeClassification { get; set; }
        public long ApartirDe { get; set; }
        public long Jusqua { get; set; }
        public long DeTelAge { get; set; }
        public long AtelAge { get; set; }
        public string Langue { get; set; }
        public string Pays { get; set; }
        public string PhysicalLocation { get; set; }

        public virtual TbookFormat TbookFormat { get; set; }
        public virtual TbookIdentification TbookIdentification { get; set; }
        public virtual ICollection<TbookAuthorConnector> TbookAuthorConnector { get; set; }
        public virtual ICollection<TbookCollectionConnector> TbookCollectionConnector { get; set; }
        public virtual ICollection<TbookEditeurConnector> TbookEditeurConnector { get; set; }
        public virtual ICollection<TbookExemplary> TbookExemplary { get; set; }
        public virtual ICollection<TbookOtherTitle> TbookOtherTitle { get; set; }
        public virtual ICollection<TlibraryBookConnector> TlibraryBookConnector { get; set; }
    }
}
