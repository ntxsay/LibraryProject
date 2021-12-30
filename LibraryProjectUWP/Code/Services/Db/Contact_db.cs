using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using Windows.Storage;
using LibraryProjectUWP.ViewModels.Contact;

namespace LibraryProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        public struct Contact
        {
            static string NameEmptyMessage = "Les informations minimales obligatoires à renseigner sont : le titre de civilité, le nom de naissance et le prénom.";
            static string NameAlreadyExistMessage = "Ce contact existe déjà.";
            #region All
            /// <summary>
            /// Retourne tous les objets de la base de données
            /// </summary>
            /// <typeparam name="T">Modèle de base de données</typeparam>
            /// <returns></returns>
            public static async Task<IList<Tcontact>> AllAsync()
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();

                    var collection = await context.Tcontact.ToListAsync();
                    if (collection == null || !collection.Any()) return Enumerable.Empty<Tcontact>().ToList();

                    return collection;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<Tcontact>().ToList();
                }
            }

            /// <summary>
            /// Retourne tous les modèles de vue depuis la base de données
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <returns></returns>
            public static async Task<IList<ContactVM>> AllVMAsync()
            {
                try
                {
                    var collection = await AllAsync();
                    if (!collection.Any()) return Enumerable.Empty<ContactVM>().ToList();

                    var values = collection.Select(async s => await ViewModelConverterAsync(s)).Select(t => t.Result).ToList();
                    return values;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<ContactVM>().ToList();
                }
            }
            #endregion

            #region Single
            /// <summary>
            /// Retourne un élément de la base de données avec un identifiant unique
            /// </summary>
            /// <typeparam name="T">Type d'entrée et de sortie (Modèle)</typeparam>
            /// <param name="id">Identifiant unique</param>
            /// <returns></returns>
            public static async Task<Tcontact> SingleAsync(long id)
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();

                    var s = await context.Tcontact.SingleOrDefaultAsync(d => d.Id == id);
                    if (s == null) return null;

                    return s;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return null;
                }
            }

            /// <summary>
            /// Retourne un modèle de vue avec un identifiant unique
            /// </summary>
            /// <typeparam name="T1">Type d'entrée (Modèle)</typeparam>
            /// <typeparam name="T2">Type sortie (Modèle de vue)</typeparam>
            /// <param name="id"></param>
            /// <returns></returns>
            public static async Task<ContactVM> SingleVMAsync(long id)
            {
                return await ViewModelConverterAsync(await SingleAsync(id));
            }
            #endregion

            public static async Task<OperationStateVM> CreateAsync(ContactVM viewModel)
            {
                try
                {
                    if (viewModel == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.ViewModelNullOrEmptyMessage,
                        };
                    }

                    if (viewModel.TitreCivilite.IsStringNullOrEmptyOrWhiteSpace() || 
                        viewModel.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() || 
                        viewModel.Prenom.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = NameEmptyMessage,
                        };
                    }

                    LibraryDbContext context = new LibraryDbContext();
                    var isExist = await context.Tcontact.AnyAsync(c => c.TitreCivilite.ToLower() == viewModel.TitreCivilite.Trim().ToLower() && c.NomNaissance.ToLower() == viewModel.NomNaissance.Trim().ToLower() && c.Prenom.ToLower() == viewModel.Prenom.Trim().ToLower() &&
                                                                  c.NomUsage.ToLower() == viewModel.NomUsage.Trim().ToLower() && c.AutresPrenoms.ToLower() == viewModel.AutresPrenoms.Trim().ToLower());
                    if (isExist)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Message = DbServices.RecordAlreadyExistMessage
                        };
                    }

                    var record = new Tcontact()
                    {
                        Guid = viewModel.Guid.ToString(),
                        DateAjout = viewModel.DateAjout.ToString(),
                        DateEdition = null,
                        Observation = viewModel.Observation,
                        TitreCivilite = viewModel.TitreCivilite,
                        NomNaissance = viewModel.NomNaissance,
                        NomUsage = viewModel.NomUsage,
                        Prenom = viewModel.Prenom,
                        AutresPrenoms = viewModel.AutresPrenoms,
                        AdressPostal = viewModel.AdressePostal,
                        CodePostal = viewModel.CodePostal,
                        Ville = viewModel.Ville,
                        NoMobile = viewModel.NoMobile,
                        NoTelephone = viewModel.NoTelephone,
                        MailAdress = viewModel.AdresseMail,
                    };

                    await context.Tcontact.AddAsync(record);
                    await context.SaveChangesAsync();

                    await CreateFolderAsync(viewModel.Guid);

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Id = record.Id,
                    };
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            internal static async Task CreateFolderAsync(Guid guid)
            {
                try
                {
                    if (guid == Guid.Empty)
                    {
                        return;
                    }

                    EsGeneral esGeneral = new EsGeneral();

                    var itemFolder = await esGeneral.GetChildItemFolderAsync(guid, EsGeneral.MainPathEnum.Contacts);
                    if (itemFolder == null)
                    {
                        return;
                    }

                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return;
                }
            }

            /// <summary>
            /// Met à jour un élément existant dans la base de données
            /// </summary>
            /// <typeparam name="T">Type d'entrée (Modèle de vue)</typeparam>
            /// <param name="viewModel">Modèle de vue</param>
            /// <returns></returns>
            public static async Task<OperationStateVM> UpdateAsync(ContactVM viewModel)
            {
                try
                {
                    
                    if (viewModel == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.ViewModelNullOrEmptyMessage,
                        };
                    }

                    if (viewModel.TitreCivilite.IsStringNullOrEmptyOrWhiteSpace() ||
                        viewModel.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() ||
                        viewModel.Prenom.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = NameEmptyMessage,
                        };
                    }

                    LibraryDbContext context = new LibraryDbContext();

                    var record = await context.Tcontact.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                    if (record == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage,
                        };
                    }

                    var isExist = await context.Tcontact.AnyAsync(c => c.Id != record.Id && c.TitreCivilite.ToLower() == viewModel.TitreCivilite.Trim().ToLower() && c.NomNaissance.ToLower() == viewModel.NomNaissance.Trim().ToLower() && c.Prenom.ToLower() == viewModel.Prenom.Trim().ToLower() &&
                                                                  c.NomUsage.ToLower() == viewModel.NomUsage.Trim().ToLower() && c.AutresPrenoms.ToLower() == viewModel.AutresPrenoms.Trim().ToLower());
                    if (isExist)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Message = NameAlreadyExistMessage
                        };
                    }

                    record.DateEdition = viewModel.DateEdition?.ToString();
                    record.Observation = viewModel.Observation;
                    record.TitreCivilite = viewModel.TitreCivilite;
                    record.NomNaissance = viewModel.NomNaissance;
                    record.NomUsage = viewModel.NomUsage;
                    record.Prenom = viewModel.Prenom;
                    record.AutresPrenoms = viewModel.AutresPrenoms;
                    record.AdressPostal = viewModel.AdressePostal;
                    record.CodePostal = viewModel.CodePostal;
                    record.Ville = viewModel.Ville;
                    record.NoTelephone = viewModel.NoTelephone;
                    record.NoMobile = viewModel.NoMobile;
                    record.MailAdress = viewModel.AdresseMail;

                    context.Tcontact.Update(record);
                    await context.SaveChangesAsync();

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Id = record.Id,
                    };
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));

                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            /// <summary>
            /// Supprime un élément de la base de données
            /// </summary>
            /// <typeparam name="T">Type d'entrée et de sortie (Modèle)</typeparam>
            /// <param name="Id"></param>
            /// <returns></returns>
            public static async Task<OperationStateVM> DeleteAsync(long Id)
            {
                try
                {
                    LibraryDbContext context = new LibraryDbContext();

                    var record = await context.Tcontact.SingleOrDefaultAsync(a => a.Id == Id);
                    if (record == null)
                    {
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.RecordNotExistMessage
                        };
                    }

                    context.Tcontact.Remove(record);
                    await context.SaveChangesAsync();

                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                    };
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));

                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Exception : {ex.Message}",
                    };
                }
            }

            #region Helpers

            /// <summary>
            /// Convertit un modèle en modèle de vue
            /// </summary>
            /// <typeparam name="T1">Type d'entrée</typeparam>
            /// <typeparam name="T2">Type sortie</typeparam>
            /// <param name="model">Modèle de base de données</param>
            /// <returns>Un modèle de vue</returns>
            private static async Task<ContactVM> ViewModelConverterAsync(Tcontact model)
            {
                try
                {
                    if (model == null) return null;

                    var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                    if (isGuidCorrect == false) return null;

                    var viewModel = new ContactVM()
                    {
                        Id = model.Id,
                        DateAjout = DatesHelpers.Converter.GetDateFromString(model.DateAjout),
                        DateEdition = DatesHelpers.Converter.GetNullableDateFromString(model.DateEdition),
                        Observation = model.Observation,
                        TitreCivilite = model.TitreCivilite,
                        NomNaissance = model.NomNaissance,
                        NomUsage = model.NomUsage,
                        Prenom = model.Prenom,
                        AutresPrenoms = model.AutresPrenoms,
                        AdressePostal = model.AdressPostal,
                        CodePostal = model.CodePostal,
                        Ville = model.Ville,
                        NoMobile = model.NoMobile,
                        NoTelephone = model.NoTelephone,
                        AdresseMail = model.MailAdress,
                    };
                    return viewModel;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return null;
                }
            }
            #endregion
        }
    }
}
