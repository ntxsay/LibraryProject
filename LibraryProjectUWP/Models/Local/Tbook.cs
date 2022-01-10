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
            TbookEtat = new HashSet<TbookEtat>();
            TbookLangue = new HashSet<TbookLangue>();
            TbookOtherTitle = new HashSet<TbookOtherTitle>();
            TbookPret = new HashSet<TbookPret>();
            TbookPrice = new HashSet<TbookPrice>();
            TlibraryBookConnector = new HashSet<TlibraryBookConnector>();
        }

        public long Id { get; set; }
        public string Guid { get; set; }
        public string DateAjout { get; set; }
        public string DateAjoutUser { get; set; }
        public string DateEdition { get; set; }
        public string DateAchat { get; set; }
        public string MainTitle { get; set; }
        public long CountOpening { get; set; }
        public long NbExactExemplaire { get; set; }
        public long? MinAge { get; set; }
        public long? MaxAge { get; set; }
        public long IsJourParutionKnow { get; set; }
        public long IsMoisParutionKnow { get; set; }
        public long IsJourParutionVisible { get; set; }
        public long IsMoisParutionVisible { get; set; }
        public long IsFavori { get; set; }
        public string DateParution { get; set; }
        public string Resume { get; set; }
        public string Notes { get; set; }

        public virtual TbookFormat TbookFormat { get; set; }
        public virtual TbookIdentification TbookIdentification { get; set; }
        public virtual ICollection<TbookAuthorConnector> TbookAuthorConnector { get; set; }
        public virtual ICollection<TbookCollectionConnector> TbookCollectionConnector { get; set; }
        public virtual ICollection<TbookEditeurConnector> TbookEditeurConnector { get; set; }
        public virtual ICollection<TbookEtat> TbookEtat { get; set; }
        public virtual ICollection<TbookLangue> TbookLangue { get; set; }
        public virtual ICollection<TbookOtherTitle> TbookOtherTitle { get; set; }
        public virtual ICollection<TbookPret> TbookPret { get; set; }
        public virtual ICollection<TbookPrice> TbookPrice { get; set; }
        public virtual ICollection<TlibraryBookConnector> TlibraryBookConnector { get; set; }
    }
}
