﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class TbookAuthorConnector
    {
        public long Id { get; set; }
        public long IdBook { get; set; }
        public long IdAuthor { get; set; }

        public virtual Tcontact IdAuthorNavigation { get; set; }
        public virtual Tbook IdBookNavigation { get; set; }
    }
}
