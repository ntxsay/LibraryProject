using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class Tcollection
    {
        public Tcollection()
        {
            TbookCollectionConnector = new HashSet<TbookCollectionConnector>();
        }

        public long Id { get; set; }
        public long IdLibrary { get; set; }
        public long CollectionType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual Tlibrary IdLibraryNavigation { get; set; }
        public virtual ICollection<TbookCollectionConnector> TbookCollectionConnector { get; set; }
    }
}
