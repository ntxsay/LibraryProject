using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RostalProjectUWP.ViewModels
{
    public  class LivreVM
    {
        public int Id { get; set; }
        public string Cotation { get; set; }

        /// <summary>
        /// L'ISBN est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string ISBN { get; set; }
        public List<string> TitresOeuvre { get; set; }
        public string Titre { get; set; }
        public List<string> Auteurs { get; set; }
        public List<string> Langues { get; set; }
        public string Description { get; set; }
        public short AnneeParution { get; set; }
        public List<BibliothequeVM> Bibliotheques { get; set; }
        public List<CategorieLivreVM> Categories { get; set; }
        public PretLivreVM Pret { get; set; }
        public DateTimeOffset DateAjout { get; set; }
    }

    public class CategorieLivreVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<LivreVM> Livres { get; set; }
    }

    public class PretLivreVM
    {
        public int Id { get; set; }
        public LivreVM Livre { get; set; }
        public ContactVM Emprunteur { get; set; }
        public DateTimeOffset DatePret { get; set; }
        public DateTimeOffset? DateRemise { get; set; }
        public string ObservationAv { get; set; }
        public string ObservationAp { get; set; }
    }
}
