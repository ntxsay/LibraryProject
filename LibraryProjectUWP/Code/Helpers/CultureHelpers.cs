using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Helpers
{
    public partial class CultureHelpers
    {
        public struct Money
        {
            public static IEnumerable<string> MoneyList()
            {
                var List = new List<string>()
                    {
                        "$",
                        "€",
                        "¥",
                        "£",
                        "₩",
                    };
                return List;
            }
        }
        public struct YesNo
        {
            public const string ChooseYesNo = "Choisissez une réponse";
            public const string Undetermined = "undetermined";
            public const string UndeterminedDisplay = "Indéterminé(e)";
            public const string Vrai = "True";
            public const string Faux = "False";
            public const string Oui = "Oui";
            public const string Non = "Non";
            public static IEnumerable<string> YesNoList()
            {
                var List = new List<string>
                {
                    "Oui",
                    "Non"
                };
                return List;
            }

            public static Dictionary<string, string> YesNoDictionary()
            {
                try
                {
                    return new Dictionary<string, string>()
                    {
                        //{ null, ChooseYesNo },
                        { bool.FalseString, Non },
                        { bool.TrueString, Oui },
                    };
                }
                catch (Exception)
                {

                    throw;
                }
            }

            public static Dictionary<string, string> YesNoUndeterminedDictionary()
            {
                try
                {
                    return new Dictionary<string, string>()
                    {
                        //{ null, ChooseYesNo },
                        { bool.FalseString, Non },
                        { bool.TrueString, Oui },
                        { Undetermined, UndeterminedDisplay },
                    };
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public struct DisplayName
        {
            public const string SelectNationality = "Sélectionnez une nationalité";
            public const string IdentityPiece = "Pièce d'identité";
            public const string JustificatifAdress = "Justificatif d'adresse";
        }

        public partial struct Civility
        {
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

            public struct SituationFamiliale
            {
                public const string SelectSituation = "Sélectionnez votre situation familiale";
                public const string Celibataire = "Célibataire";
                public const string Separe = "Séparé(e)";
                //public const string Divorce = "divorcé(e)";
                public const string Veuf = "Veuf(ve)";
                public const string Marie = "Marié(e)";
                public const string Remarie = "Remarié(e)";

                public static IEnumerable<string> SituationFamilyList()
                {
                    var List = new List<string>()
                    {
                        //SelectSituation,
                        Celibataire,
                        Marie,
                        Remarie,
                        Separe,
                        Veuf,
                    };
                    return List;
                }
            }

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
            }
        }

        public struct Adresse
        {
            public struct DisplayName
            {
                public const string Adresse = "Adresse";
                public const string CodePostal = "Code postal";
                public const string Ville = "Ville";
                public const string NoTelephone = "No Fixe";
                public const string NoPortable = "No Portable";
                public const string MailAdress = "Adresse mail";
            }
        }

        public struct SituationAdministrative
        {
            public struct DisplayName
            {
                public const string IsDemandeurEmploi = "Demandeur d'emploi";
                public const string IsBeneficiaireRsa = "Bénéficiaire du RSA";
                public const string IsBeneficiaireAre = "Bénéficiaire de l'ARE";
                public const string IsSalarie = "Salarié(e)";
                public const string IsAutre = "Autre situation administrative";
            }
        }

        public struct ParcoursFormations
        {
            public struct DisplayName
            {
                public const string NiveauEtude = "Niveau d'étude";
                public const string DerniereClassFrequente = "Dernière classe fréquentée";
                public const string DiplomePreparer = "Diplôme préparé";
                public const string DiplomeObtenu = "Diplôme obtenu";
                public const string OtherDiplomeObtenu = "Autre diplôme obtenu";
                public const string OtherDiplomeObtenuMultiple = "Autre(s) diplôme(s) obtenu(s)";
                public const string DernierDiplomeObtenu = "Dernier Diplôme obtenu";
                public const string DiplomesObtenusMultiple = "Diplôme(s) obtenu(s)";
                public const string XprienceServPersonne = "Expériences professionnelles ou stages réalisés dans le cadre des services à la personne (salarié ou bénévole)";
                public const string XprienceServPersonneShort = "Expériences (services à la personne)";
                public const string Xprience = "Autres expériences professionnelles salariés ou bénévoles";
                public const string XprienceShort = "Expériences (autres)";
            }

            public struct NiveauEtude
            {
                public const string SelectNivEtude = "Votre niveau d'étude";
                public const string SelectNivEtudeDescription = "Veuillez sélectionner un niveau d'étude";

                public const string None = "Aucun";
                public const string NoneDescription = "Aucun";

                public const string NiveauBrevetCollege = "Niveau Brevet des collèges";
                public const string NiveauBrevetCollegeDescription = "Niveau Brevet des collèges";

                public const string Niveau3 = "Niveau 3 : CAP, BEP";
                public const string Niveau3Description = "Niveau 3 : CAP, BEP (anciennement V)";

                public const string Niveau4 = "Niveau 4 (Bac)";
                public const string Niveau4Description = "Niveau 4 (Bac) : Baccalauréat (anciennement IV)";

                public const string Niveau5 = "Niveau 5 (Bac+2)";
                public const string Niveau5Description = "Niveau 5 (Bac+2) : DEUG, BTS, DUT, DEUST (anciennement III)";

                public const string Niveau6 = "Niveau 6 (Bac+3)";
                public const string Niveau6Description = "Niveau 6 (Bac+3) : Licence, licence professionnelle (anciennement II)";

                public const string Niveau6b = "Niveau 6 (Bac+4)";
                public const string Niveau6bDescription = "Niveau 6 (Bac+4) : Maîtrise, master 1 (anciennement III)";

                public const string Niveau7 = "Niveau 7 (Bac+5)";
                public const string Niveau7Description = "Niveau 7 (Bac+5) : Master, dip. d'études approfondies, dip. d'études sup. spécialisées, dip. d'ingénieur (anciennement I)";

                public const string Niveau8 = "Niveau 8 (Bac+8)";
                public const string Niveau8Description = "Niveau 8 (Bac+8) : Doctorat, hab. à diriger des recherches (anciennement I)";

                public static IEnumerable<string> NiveauEtudeList()
                {
                    var List = new List<string>()
                    {
                        SelectNivEtude,
                        None,
                        NiveauBrevetCollege,
                        Niveau3,
                        Niveau4,
                        Niveau5,
                        Niveau6,
                        Niveau6b,
                        Niveau7,
                        Niveau8,
                    };
                    return List;
                }

                public static Dictionary<string, string> NiveauEtudeDictionary = new Dictionary<string, string>()
                {
                    //{ SelectNivEtude, SelectNivEtudeDescription },
                    { None, NoneDescription },
                    { NiveauBrevetCollege, NiveauBrevetCollegeDescription},
                    { Niveau3, Niveau3Description },
                    { Niveau4, Niveau4Description},
                    { Niveau5, Niveau5Description},
                    { Niveau6, Niveau6Description},
                    { Niveau6b, Niveau6bDescription},
                    { Niveau7, Niveau7Description},
                    { Niveau8, Niveau8Description},
                };

                public class NiveauEtudePair
                {
                    public string NiveauName { get; set; }
                    public string NiveauDesc { get; set; }
                }
            }

            public struct Validation
            {
                public static bool IsNiveauEtudeCorrect(string value, out string ErrorMessage)
                {
                    try
                    {
                        if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(value))
                        {
                            ErrorMessage = "Vous devez sélectionner un niveau d'étude.";
                            return false;
                        }

                        if (value == NiveauEtude.SelectNivEtude || value == NiveauEtude.SelectNivEtudeDescription)
                        {
                            ErrorMessage = "Vous devez sélectionner un niveau d'étude.";
                            return false;
                        }

                        foreach (var item in NiveauEtude.NiveauEtudeDictionary)
                        {
                            if (value == item.Key)
                            {
                                ErrorMessage = null;
                                return true;
                            }
                        }

                        ErrorMessage = "Nous n'avons pas référencé ce niveau d'étude.";
                        return false;
                    }
                    catch (Exception)
                    {
                        ErrorMessage = "Une erreur inconnue s'est produite.";
                        return false;
                    }
                }
            }
        }



    }

}
