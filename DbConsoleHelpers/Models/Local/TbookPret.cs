﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class TbookPret
    {
        public long Id { get; set; }
        public long IdBook { get; set; }
        public long IdContact { get; set; }
        public long IdEtatBefore { get; set; }
        public long? IdEtatAfter { get; set; }
        public string DatePret { get; set; }
        public string DateRemise { get; set; }
        public string Observation { get; set; }

        public virtual Tbook IdBookNavigation { get; set; }
        public virtual Tcontact IdContactNavigation { get; set; }
        public virtual TbookEtat IdEtatAfterNavigation { get; set; }
        public virtual TbookEtat IdEtatBeforeNavigation { get; set; }
    }
}
