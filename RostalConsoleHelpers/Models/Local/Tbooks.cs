using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RostalConsoleHelpers.Models.Local
{
    public partial class Tbooks
    {
        public Tbooks()
        {
            TbookAuthor = new HashSet<TbookAuthor>();
            TbookTitle = new HashSet<TbookTitle>();
            TlibraryBookConnector = new HashSet<TlibraryBookConnector>();
        }

        public long Id { get; set; }
        public string Guid { get; set; }
        public string Isbn { get; set; }
        public string Cotation { get; set; }
        public long? AnneeParution { get; set; }
        public string Description { get; set; }
        public string DateAjout { get; set; }
        public string DateEdition { get; set; }

        public virtual ICollection<TbookAuthor> TbookAuthor { get; set; }
        public virtual ICollection<TbookTitle> TbookTitle { get; set; }
        public virtual ICollection<TlibraryBookConnector> TlibraryBookConnector { get; set; }
    }
}
