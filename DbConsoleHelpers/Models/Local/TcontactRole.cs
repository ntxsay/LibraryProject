﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class TcontactRole
    {
        public long Id { get; set; }
        public long IdContact { get; set; }
        public long Role { get; set; }

        public virtual Tcontact IdContactNavigation { get; set; }
    }
}
