using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace LibraryProjectUWP.Models.Local
{
    public partial class Tauthor
    {
        public Tauthor()
        {
            TbookAuthorConnector = new HashSet<TbookAuthorConnector>();
        }

        public long Id { get; set; }
        public string Guid { get; set; }
        public string DateAjout { get; set; }
        public string DateEdition { get; set; }
        public string TitreCivilite { get; set; }
        public string NomNaissance { get; set; }
        public string NomUsage { get; set; }
        public string Prenom { get; set; }
        public string DateNaissance { get; set; }
        public string DateDeces { get; set; }
        public string LieuNaissance { get; set; }
        public string LieuDeces { get; set; }
        public string AutresPrenoms { get; set; }
        public string AdressPostal { get; set; }
        public string MailAdress { get; set; }
        public string NoTelephone { get; set; }
        public string Notes { get; set; }
        public string Biographie { get; set; }

        public virtual ICollection<TbookAuthorConnector> TbookAuthorConnector { get; set; }
    }
}
