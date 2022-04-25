using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Helpers
{
    public class BookHelpers
    {
        public static List<PropertiesChangedVM> GetPropertiesChanged(LivreVM viewModelA, LivreVM viewModelB)
        {
            try
            {
                if (viewModelA == null) return null;
                if (viewModelB == null) return null;
                List<PropertiesChangedVM> list = new List<PropertiesChangedVM>();

                if (viewModelA.MainTitle != viewModelB.MainTitle)
                {
                    list.Add(new PropertiesChangedVM()
                    {
                        PropertyName = "Titre du livre",
                        Message = "Le titre du livre a été changé"
                    });
                }

                if (viewModelA.TitresOeuvre != null && viewModelB.TitresOeuvre != null)
                {
                    if (viewModelA.TitresOeuvre.Count > viewModelB.TitresOeuvre.Count)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Autre(s) titre(s)",
                            Message = "Les titres ont été effacés"
                        });
                    }
                    else if (viewModelA.TitresOeuvre.Count < viewModelB.TitresOeuvre.Count)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Autre(s) titre(s)",
                            Message = "De nouveaux titres ont été ajoutés"
                        });
                    }
                }
                else
                {
                    if (viewModelB.TitresOeuvre == null || !viewModelB.TitresOeuvre.Any())
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Autre(s) titre(s)",
                            Message = "Les titres ont été effacés"
                        });
                    }

                    if (viewModelA.TitresOeuvre == null || !viewModelA.TitresOeuvre.Any())
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Autre(s) titre(s)",
                            Message = "De nouveaux titres ont été ajoutés"
                        });
                    }
                }

                if (viewModelA.Auteurs != null && viewModelB.Auteurs != null)
                {
                    if (viewModelA.Auteurs.Count > viewModelB.Auteurs.Count)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Auteur(s)",
                            Message = "Des auteurs ont été effacés"
                        });
                    }
                    else if (viewModelA.Auteurs.Count < viewModelB.Auteurs.Count)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Auteur(s)",
                            Message = "Des auteurs ont été ajoutés"
                        });
                    }
                }
                else
                {
                    if (viewModelB.Auteurs == null || !viewModelB.Auteurs.Any())
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Auteur(s)",
                            Message = "Des auteurs ont été effacés"
                        });
                    }

                    if (viewModelA.Auteurs == null || !viewModelA.Auteurs.Any())
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Auteur(s)",
                            Message = "Des auteurs ont été ajoutés"
                        });
                    }
                }

                if (viewModelA.Identification != viewModelB.Identification)
                {
                    if (viewModelB.Identification == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Identification",
                            Message = "Des informations d'identification ont été effacées"
                        });
                    }
                    else if (viewModelA.Identification == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Identification",
                            Message = "Des informations d'identification ont été ajoutées"
                        });
                    }
                    else
                    {
                        if (viewModelA.Identification.ISBN != viewModelB.Identification.ISBN)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "ISBN",
                                Message = "L'ISBN du livre a été changé"
                            });
                        }

                        if (viewModelA.Identification.ISBN10 != viewModelB.Identification.ISBN10)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "ISBN-10",
                                Message = "L'ISBN-10 du livre a été changé"
                            });
                        }

                        if (viewModelA.Identification.ISBN13 != viewModelB.Identification.ISBN13)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "ISBN-13",
                                Message = "L'ISBN-13 du livre a été changé"
                            });
                        }

                        if (viewModelA.Identification.ISSN != viewModelB.Identification.ISSN)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "ISSN",
                                Message = "L'ISSN du livre a été changé"
                            });
                        }

                        if (viewModelA.Identification.ASIN != viewModelB.Identification.ASIN)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "ASIN",
                                Message = "L'ASIN du livre a été changé"
                            });
                        }

                        if (viewModelA.Identification.CodeBarre != viewModelB.Identification.CodeBarre)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "CodeBarre",
                                Message = "Le Code barre du livre a été changé"
                            });
                        }

                        if (viewModelA.Identification.Cotation != viewModelB.Identification.Cotation)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Cotation",
                                Message = "La cotation du livre a été changé"
                            });
                        }
                    }
                }

                if (viewModelA.ClassificationAge != viewModelB.ClassificationAge)
                {
                    if (viewModelB.ClassificationAge == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Classification Age",
                            Message = "Des informations concernant la classification de l'âge ont été effacées"
                        });
                    }
                    else if (viewModelA.ClassificationAge == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Classification Age",
                            Message = "Des informations concernant la classification de l'âge ont été ajoutées"
                        });
                    }
                    else
                    {
                        if (viewModelA.ClassificationAge.TypeClassification != viewModelB.ClassificationAge.TypeClassification)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Type de classification",
                                Message = "Le type de classification du livre a été changé"
                            });
                        }

                        if (viewModelA.ClassificationAge.ApartirDe != viewModelB.ClassificationAge.ApartirDe ||
                            viewModelA.ClassificationAge.Jusqua != viewModelB.ClassificationAge.Jusqua ||
                            viewModelA.ClassificationAge.DeTelAge != viewModelB.ClassificationAge.DeTelAge ||
                            viewModelA.ClassificationAge.ATelAge != viewModelB.ClassificationAge.ATelAge)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Classification de l'âge",
                                Message = "L'âge de lecture du livre a été changé"
                            });
                        }
                    }
                }

                if (viewModelA.Publication != viewModelB.Publication)
                {
                    if (viewModelB.Publication == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Publication",
                            Message = "Des informations concernant la publication du livre ont été effacées"
                        });
                    }
                    else if (viewModelA.Publication == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Publication",
                            Message = "Des informations concernant la publication du livre ont été ajoutées"
                        });
                    }
                    else
                    {
                        if (viewModelA.Publication.Pays != viewModelB.Publication.Pays)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Pays",
                                Message = "Le pays de publication du livre a été changé"
                            });
                        }

                        if (viewModelA.Publication.Langue != viewModelB.Publication.Langue)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Langue",
                                Message = "La langue de publication du livre a été changé"
                            });
                        }

                        if (viewModelA.Publication.DayParution != viewModelB.Publication.DayParution)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Date de parution",
                                Message = "Le jour de parution a été changé"
                            });
                        }

                        if (viewModelA.Publication.MonthParution != viewModelB.Publication.MonthParution)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Date de parution",
                                Message = "Le mois de parution a été changé"
                            });
                        }

                        if (viewModelA.Publication.YearParution != viewModelB.Publication.YearParution)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Date de parution",
                                Message = "L'année de parution a été changé"
                            });
                        }

                        if (viewModelA.Publication.Editeurs != null && viewModelB.Publication.Editeurs != null)
                        {
                            if (viewModelA.Publication.Editeurs.Count > viewModelB.Publication.Editeurs.Count)
                            {
                                list.Add(new PropertiesChangedVM()
                                {
                                    PropertyName = "Editeurs",
                                    Message = "Des éditeurs/maisons d'édition ont été effacés"
                                });
                            }
                            else if (viewModelA.Publication.Editeurs.Count < viewModelB.Publication.Editeurs.Count)
                            {
                                list.Add(new PropertiesChangedVM()
                                {
                                    PropertyName = "Editeurs",
                                    Message = "Des éditeurs/maisons d'édition ont été ajoutés"
                                });
                            }
                        }
                        else
                        {
                            if (viewModelB.Publication.Editeurs == null || !viewModelB.Publication.Editeurs.Any())
                            {
                                list.Add(new PropertiesChangedVM()
                                {
                                    PropertyName = "Editeurs",
                                    Message = "Des éditeurs/maisons d'édition ont été effacés"
                                });
                            }

                            if (viewModelA.Publication.Editeurs == null || !viewModelA.Publication.Editeurs.Any())
                            {
                                list.Add(new PropertiesChangedVM()
                                {
                                    PropertyName = "Editeurs",
                                    Message = "Des éditeurs/maisons d'édition ont été ajoutés"
                                });
                            }
                        }

                        if (viewModelA.Publication.Collections != null && viewModelB.Publication.Collections != null)
                        {
                            if (viewModelA.Publication.Collections.Count > viewModelB.Publication.Collections.Count)
                            {
                                list.Add(new PropertiesChangedVM()
                                {
                                    PropertyName = "Collections",
                                    Message = "Des collections ont été effacés"
                                });
                            }
                            else if (viewModelA.Publication.Collections.Count < viewModelB.Publication.Collections.Count)
                            {
                                list.Add(new PropertiesChangedVM()
                                {
                                    PropertyName = "Collections",
                                    Message = "Des collections ont été ajoutés"
                                });
                            }
                        }
                        else
                        {
                            if (viewModelB.Publication.Collections == null || !viewModelB.Publication.Collections.Any())
                            {
                                list.Add(new PropertiesChangedVM()
                                {
                                    PropertyName = "Collections",
                                    Message = "Des collections ont été effacés"
                                });
                            }

                            if (viewModelA.Publication.Collections == null || !viewModelA.Publication.Collections.Any())
                            {
                                list.Add(new PropertiesChangedVM()
                                {
                                    PropertyName = "Collections",
                                    Message = "Des collections ont été ajoutés"
                                });
                            }
                        }
                    }
                }

                if (viewModelA.Format != viewModelB.Format)
                {
                    if (viewModelB.Format == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Format",
                            Message = "Des informations concernant le format du livre ont été effacées"
                        });
                    }
                    else if (viewModelA.Format == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Format",
                            Message = "Des informations concernant le format du livre ont été ajoutées"
                        });
                    }
                    else
                    {
                        if (viewModelA.Format.Format != viewModelB.Format.Format)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Format",
                                Message = "Le format du livre a été changé"
                            });
                        }

                        if (viewModelA.Format.NbOfPages != viewModelB.Format.NbOfPages)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Format",
                                Message = "Le nombre de page du livre a été changé"
                            });
                        }

                        if (viewModelA.Format.Epaisseur != viewModelB.Format.Epaisseur)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Format",
                                Message = "L'épaisseur du livre a été changé"
                            });
                        }

                        if (viewModelA.Format.Poids != viewModelB.Format.Poids)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Format",
                                Message = "Le poids du livre a été changé"
                            });
                        }

                        if (viewModelA.Format.Hauteur != viewModelB.Format.Hauteur)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Format",
                                Message = "La hauteur du livre a été changé"
                            });
                        }

                        if (viewModelA.Format.Largeur != viewModelB.Format.Largeur)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Format",
                                Message = "La largeur du livre a été changé"
                            });
                        }
                    }
                }

                if (viewModelA.Description != viewModelB.Description)
                {
                    if (viewModelB.Description == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Description",
                            Message = "Des informations concernant la description du livre ont été effacées"
                        });
                    }
                    else if (viewModelA.Description == null)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Description",
                            Message = "Des informations concernant la description du livre ont été ajoutées"
                        });
                    }
                    else
                    {
                        if (viewModelA.Description.Resume != viewModelB.Description.Resume)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Description",
                                Message = "Le résumé du livre a été changé"
                            });
                        }

                        if (viewModelA.Description.Notes != viewModelB.Description.Notes)
                        {
                            list.Add(new PropertiesChangedVM()
                            {
                                PropertyName = "Description",
                                Message = "Les notes concernant le livre a été changé"
                            });
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

    }
}
