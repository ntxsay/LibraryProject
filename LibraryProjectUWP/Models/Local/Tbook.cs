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
            TbookCollections = new HashSet<TbookCollections>();
            TbookEditeurConnector = new HashSet<TbookEditeurConnector>();
            TbookExemplary = new HashSet<TbookExemplary>();
            TbookIllustratorConnector = new HashSet<TbookIllustratorConnector>();
            TbookOtherTitle = new HashSet<TbookOtherTitle>();
            TbookTranslatorConnector = new HashSet<TbookTranslatorConnector>();
        }

        public long Id { get; set; }
        public long IdLibrary { get; set; }
        public long? IdCategorie { get; set; }
        public long? IdSubCategorie { get; set; }
        public string Guid { get; set; }
        public string DateAjout { get; set; }
        public string DateEdition { get; set; }
        public string MainTitle { get; set; }
        public long CountOpening { get; set; }
        public string DateParution { get; set; }
        public string Resume { get; set; }
        public string Notes { get; set; }
        public string Langue { get; set; }
        public string Pays { get; set; }
        public string PhysicalLocation { get; set; }

        public virtual TlibraryCategorie IdCategorieNavigation { get; set; }
        public virtual Tlibrary IdLibraryNavigation { get; set; }
        public virtual TlibrarySubCategorie IdSubCategorieNavigation { get; set; }
        public virtual TbookClassification TbookClassification { get; set; }
        public virtual TbookFormat TbookFormat { get; set; }
        public virtual TbookIdentification TbookIdentification { get; set; }
        public virtual TbookReading TbookReading { get; set; }
        public virtual ICollection<TbookAuthorConnector> TbookAuthorConnector { get; set; }
        public virtual ICollection<TbookCollections> TbookCollections { get; set; }
        public virtual ICollection<TbookEditeurConnector> TbookEditeurConnector { get; set; }
        public virtual ICollection<TbookExemplary> TbookExemplary { get; set; }
        public virtual ICollection<TbookIllustratorConnector> TbookIllustratorConnector { get; set; }
        public virtual ICollection<TbookOtherTitle> TbookOtherTitle { get; set; }
        public virtual ICollection<TbookTranslatorConnector> TbookTranslatorConnector { get; set; }
    }
}
