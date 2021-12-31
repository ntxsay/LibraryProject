using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Helpers
{
    public class CivilityHelpers
    {
        public enum Genre
        {
            UnSpecified,
            Female,
            Male,
        }

        public const string SelectCivility = "Sélectionnez un titre de civilité";
        public const string NonSpecifie = "Non spécifié";
        public const string Madame = "Madame";
        public const string Mademoiselle = "Mademoiselle";
        public const string Monsieur = "Monsieur";
        public const string Mme = "Mme";
        public const string Mlle = "Mlle";
        public const string MPoint = "M.";
        public const string MadameEtDiminutif = "Madame (Mme)";
        public const string MademoiselleEtDiminutif = "Mademoiselle (Mlle)";
        public const string MonsieurEtDiminutif = "Monsieur (M.)";

        public static IEnumerable<string> CiviliteListAll()
        {
            var List = new List<string>
                {
                    NonSpecifie,
                    Madame,
                    Mademoiselle,
                    Monsieur,
                    Mme,
                    Mlle,
                    MPoint,
                    MadameEtDiminutif,
                    MademoiselleEtDiminutif,
                    MonsieurEtDiminutif
                };
            return List;
        }

        public static IEnumerable<string> CiviliteList()
        {
            var List = new List<string>
                {
                    //SelectCivility,
                    Madame,
                    Mademoiselle,
                    Monsieur
                };
            return List;
        }

        public static IEnumerable<string> CiviliteListShorted()
        {
            var List = new List<string>
                {
                    NonSpecifie,
                    Mme,
                    Mlle,
                    MPoint
                };
            return List;
        }

        public static IEnumerable<string> CiviliteListWithDim()
        {
            var List = new List<string>()
                {
                    NonSpecifie,
                    MadameEtDiminutif,
                    MademoiselleEtDiminutif,
                    MonsieurEtDiminutif
                };
            return List;
        }

        public static IEnumerable<string> CiviliteManList()
        {
            var List = new List<string>
                {
                    Monsieur,
                    MPoint,
                    MonsieurEtDiminutif
                };
            return List;
        }

        public static IEnumerable<string> CiviliteWomanList()
        {
            var List = new List<string>
                {
                    Madame,
                    Mademoiselle,
                    Mme,
                    Mlle,
                    MadameEtDiminutif,
                    MademoiselleEtDiminutif,
                };
            return List;
        }

        public struct SituationFamiliale
        {
            public const string NbreEnfantsCharge = "Nombre d'enfants à charge";
            public const string SelectSituation = "Sélectionnez votre situation familiale";
            public const string Celibataire = "Célibataire";
            public const string Separe = "Séparé(e)";
            //public const string Divorce = "divorcé(e)";
            public const string Veuf = "Veuf(ve)";
            public const string Marie = "Marié(e)";
            public const string Remarie = "Remarié(e)";

            public static IEnumerable<string> SituationFamilyList()
            {
                var list = new List<string>()
                    {
                        SelectSituation,
                        Celibataire,
                        Marie,
                        Remarie,
                        Separe,
                        Veuf,
                    };
                return list;
            }
        }

        [Obsolete]
        public struct DisplayName
        {
            public const string TitreCivilite = "Titre de civilité";
            public const string NomNaissance = "Nom de naissance";
            public const string NomUsage = "Nom d'usage";
            public const string Prenom = "Prénom";
            public const string AutresPrenoms = "Autres prénoms";
            public const string SituationFamiliale = "Situation familiale";
            public const string NbreEnfantsCharge = "Nombre d'enfants à charge";
            public const string DateNaissance = "Date de naissance";
            public const string LieuNaissance = "Lieu de naissance";
            public const string Nationalite = "Nationalité";

            public struct Adress
            {
                public const string Adresse = "Adresse";
                public const string CodePostal = "Code postal";
                public const string Ville = "Ville";
                public const string NoTelephone = "No Fixe";
                public const string NoPortable = "No Portable";
                public const string MailAdress = "Adresse mail";
            }
        }

        public static string GetCivility(string TitreCivilite, string NomNaissance, string Prenom, string AutresPrenoms, string NomUsage, bool UseTitleCivility = false, bool UseOthersPrenoms = false, bool UseNomUsage = false)
        {
            try
            {
                if (NomNaissance.IsStringNullOrEmptyOrWhiteSpace() || Prenom.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return string.Empty;
                }
                return $"{(UseTitleCivility == true && !TitreCivilite.IsStringNullOrEmptyOrWhiteSpace() ? (TitreCivilite + " ") : string.Empty)}{NomNaissance} {Prenom}{(UseOthersPrenoms == true && !AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace() ? (" " + AutresPrenoms + " ") : string.Empty)}{(UseNomUsage == true && !NomUsage.IsStringNullOrEmptyOrWhiteSpace() ? (" épouse " + NomUsage) : string.Empty)}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }

        }
    }
}
