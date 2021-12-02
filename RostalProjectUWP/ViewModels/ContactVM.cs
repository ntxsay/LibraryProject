using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RostalProjectUWP.ViewModels
{
    public class ContactVM
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenoms { get; set; }
        public string Adress { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public string NoFixe { get; set; }
        public string NoPortable { get; set; }
        public string MailAdress { get; set; }
    }
}
