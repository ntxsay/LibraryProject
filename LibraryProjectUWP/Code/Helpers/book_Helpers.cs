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

                if (viewModelA.TitresOeuvre != viewModelB.TitresOeuvre)
                {
                    if (viewModelB.TitresOeuvre == null || !viewModelB.TitresOeuvre.Any() || viewModelB.TitresOeuvre.Count < viewModelA.TitresOeuvre.Count)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Autre(s) titre(s)",
                            Message = "Les titres ont été effacés"
                        });
                    }
                    else if (viewModelA.TitresOeuvre == null || !viewModelA.TitresOeuvre.Any() || viewModelA.TitresOeuvre.Count < viewModelB.TitresOeuvre.Count)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Autre(s) titre(s)",
                            Message = "De nouveaux titres ont été ajoutés"
                        });
                    }
                }

                if (viewModelA.Auteurs != viewModelB.Auteurs)
                {
                    if (viewModelB.Auteurs == null || !viewModelB.Auteurs.Any() || viewModelB.Auteurs.Count < viewModelA.Auteurs.Count)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Auteur(s)",
                            Message = "Des auteurs ont été effacés"
                        });
                    }
                    else if (viewModelA.Auteurs == null || !viewModelA.Auteurs.Any() || viewModelA.Auteurs.Count < viewModelB.Auteurs.Count)
                    {
                        list.Add(new PropertiesChangedVM()
                        {
                            PropertyName = "Auteur(s)",
                            Message = "Des auteurs ont été ajoutés"
                        });
                    }
                }


                //if (viewModelB.Identification != null)
                //{
                //    if (viewModelA.Identification == null)
                //    {
                //        viewModelA.Identification = new LivreIdentificationVM();
                //    }

                //    viewModelA.Identification.Id = viewModelB.Identification.Id;
                //    viewModelA.Identification.ISBN = viewModelB.Identification.ISBN;
                //    viewModelA.Identification.ISBN10 = viewModelB.Identification.ISBN10;
                //    viewModelA.Identification.ISBN13 = viewModelB.Identification.ISBN13;
                //    viewModelA.Identification.ISSN = viewModelB.Identification.ISSN;
                //    viewModelA.Identification.ASIN = viewModelB.Identification.ASIN;
                //    viewModelA.Identification.CodeBarre = viewModelB.Identification.CodeBarre;
                //    viewModelA.Identification.Cotation = viewModelB.Identification.Cotation;
                //}

                //if (viewModelB.ClassificationAge != null)
                //{
                //    if (viewModelA.ClassificationAge == null)
                //    {
                //        viewModelA.ClassificationAge = new LivreClassificationAgeVM();
                //    }

                //    viewModelA.ClassificationAge.TypeClassification = viewModelB.ClassificationAge.TypeClassification;
                //    viewModelA.ClassificationAge.ApartirDe = viewModelB.ClassificationAge.ApartirDe;
                //    viewModelA.ClassificationAge.Jusqua = viewModelB.ClassificationAge.Jusqua;
                //    viewModelA.ClassificationAge.DeTelAge = viewModelB.ClassificationAge.DeTelAge;
                //    viewModelA.ClassificationAge.ATelAge = viewModelB.ClassificationAge.ATelAge;
                //}

                //if (viewModelB.Publication != null)
                //{
                //    if (viewModelA.Publication == null)
                //    {
                //        viewModelA.Publication = new LivrePublicationVM();
                //    }

                //    viewModelA.Publication.Pays = viewModelB.Publication.Pays;
                //    viewModelA.Publication.Langue = viewModelB.Publication.Langue;
                //    viewModelA.Publication.DateParution = viewModelB.Publication.DateParution;
                //    viewModelA.Publication.DayParution = viewModelB.Publication.DayParution;
                //    viewModelA.Publication.MonthParution = viewModelB.Publication.MonthParution;
                //    viewModelA.Publication.YearParution = viewModelB.Publication.YearParution;

                //    if (viewModelB.Publication.Editeurs != null && viewModelB.Publication.Editeurs.Any())
                //    {
                //        if (viewModelA.Publication.Editeurs == null)
                //        {
                //            viewModelA.Publication.Editeurs = new ObservableCollection<ContactVM>();
                //        }

                //        viewModelA.Publication.Editeurs.Clear();
                //        foreach (var editeur in viewModelB.Publication.Editeurs)
                //        {
                //            viewModelA.Publication.Editeurs.Add(editeur);
                //        }

                //        viewModelA.Publication.EditeursStringList = StringHelpers.JoinStringArray(viewModelA.Publication.Editeurs?.Select(s => s.SocietyName)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                //    }

                //    if (viewModelB.Publication.Collections != null && viewModelB.Publication.Collections.Any())
                //    {
                //        if (viewModelA.Publication.Collections == null)
                //        {
                //            viewModelA.Publication.Collections = new ObservableCollection<CollectionVM>();
                //        }

                //        viewModelA.Publication.Collections.Clear();
                //        foreach (var collection in viewModelB.Publication.Collections)
                //        {
                //            viewModelA.Publication.Collections.Add(collection);
                //        }

                //        viewModelA.Publication.CollectionsStringList = StringHelpers.JoinStringArray(viewModelA.Publication.Collections?.Select(s => s.Name)?.ToArray() ?? Array.Empty<string>(), ", ", out _);
                //    }
                //}

                //if (viewModelB.Format != null)
                //{
                //    if (viewModelA.Format == null)
                //    {
                //        viewModelA.Format = new LivreFormatVM();
                //    }

                //    viewModelA.Format.Format = viewModelB.Format.Format;
                //    viewModelA.Format.NbOfPages = viewModelB.Format.NbOfPages;
                //    viewModelA.Format.Epaisseur = viewModelB.Format.Epaisseur;
                //    viewModelA.Format.Poids = viewModelB.Format.Poids;
                //    viewModelA.Format.Hauteur = viewModelB.Format.Hauteur;
                //    viewModelA.Format.Largeur = viewModelB.Format.Largeur;
                //    viewModelA.Format.Dimensions = viewModelB.Format.Dimensions;
                //    viewModelA.Format.Id = viewModelB.Format.Id;
                //}

                //if (viewModelB.Description != null)
                //{
                //    if (viewModelA.Description == null)
                //    {
                //        viewModelA.Description = new LivreDescriptionVM();
                //    }

                //    viewModelA.Description.Resume = viewModelB.Description.Resume;
                //    viewModelA.Description.Notes = viewModelB.Description.Notes;
                //}

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
