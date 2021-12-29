using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class Tediteur
    {
        public Tediteur()
        {
            TbookEditeurConnector = new HashSet<TbookEditeurConnector>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }

        public virtual ICollection<TbookEditeurConnector> TbookEditeurConnector { get; set; }
    }
}
