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
using System.Threading;

namespace LibraryProjectUWP.Code.Services.Db
{
    public partial class DbServices
    {
        public struct Contact
        {
            static string NameEmptyMessage = "Les informations minimales obligatoires à renseigner sont : le titre de civilité, le nom de naissance et le prénom si Adherant ou auteur sinon le nom de la société.";
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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var collection = await context.Tcontact.ToListAsync();
                        if (collection == null || !collection.Any()) return Enumerable.Empty<Tcontact>().ToList();
                        collection.Select(async f => f.TcontactRole = await context.TcontactRole.Where(s => s.IdContact == f.Id).ToListAsync());

                        return collection;
                    }
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
            /// <param name="id">Identifiant unique</param>
            /// <returns></returns>
            public static async Task<Tcontact> SingleAsync(long id)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var s = await context.Tcontact.SingleOrDefaultAsync(d => d.Id == id);
                        if (s == null) return null;
                        s.TcontactRole = await context.TcontactRole.Where(f => f.IdContact == f.Id).ToListAsync();


                        return s;
                    }
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

            #region Multiple
            public static async Task<List<Tcontact>> MultipleAsync(ContactType? contactType = null, IEnumerable<ContactRole> contactRoleList = null, CancellationToken cancellationToken = default)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tcontact> collection = new List<Tcontact>();
                        
                        //Si aucun type et aucun role alors affiche tous les contacts
                        if (contactType == null && (contactRoleList == null || contactRoleList != null && !contactRoleList.Any()))
                        {
                            var result = await context.Tcontact.ToListAsync(cancellationToken);
                            if (result != null && result.Any())
                            {
                                collection.AddRange(result);
                            }
                        }
                        //S'il y a un type mais aucun role alors afficher tous les contacts de ce type
                        else if (contactType != null && (contactRoleList == null || contactRoleList != null && !contactRoleList.Any()))
                        {
                            var result = await context.Tcontact.Where(w => w.Type == (byte)contactType).ToListAsync(cancellationToken);
                            if (result != null && result.Any())
                            {
                                collection.AddRange(result);
                            }
                        }
                        //Si aucun type mais un ou plusieurs roles alors affiche tous les contacts avec ces roles
                        else if (contactType == null && contactRoleList != null && contactRoleList.Any())
                        {
                            List<TcontactRole> tRoleList = new List<TcontactRole>();
                            foreach (var role in contactRoleList)
                            {
                                var result = await context.TcontactRole.Where(w => w.Role == (byte)role).ToListAsync(cancellationToken);
                                if (result != null && result.Any())
                                {
                                    tRoleList.AddRange(result);
                                }
                            }

                            if (tRoleList != null && tRoleList.Any())
                            {
                                foreach (var role in tRoleList)
                                {
                                    var result = await context.Tcontact.Where(w => w.Id == role.IdContact).ToListAsync(cancellationToken);
                                    if (result != null && result.Any())
                                    {
                                        collection.AddRange(result);
                                    }
                                }
                            }
                        }
                        //S'il y a un type  avec plusieurs roles alors afficher tous les contacts de ce type et avec ces roles
                        else if (contactType != null && contactRoleList != null && contactRoleList.Any())
                        {
                            List<TcontactRole> tRoleList = new List<TcontactRole>();
                            foreach (var role in contactRoleList)
                            {
                                var result = await context.TcontactRole.Where(w => w.Role == (byte)role).ToListAsync(cancellationToken);
                                if (result != null && result.Any())
                                {
                                    tRoleList.AddRange(result);
                                }
                            }

                            if (tRoleList != null && tRoleList.Any())
                            {
                                foreach (var role in tRoleList)
                                {
                                    var result = await context.Tcontact.Where(w => w.Id == role.IdContact && w.Type == (byte)contactType).ToListAsync(cancellationToken);
                                    if (result != null && result.Any())
                                    {
                                        collection.AddRange(result);
                                    }
                                }
                            }
                        }

                        if (collection != null && collection.Any())
                        {
                            collection.ForEach(async ws => await CompleteModelInfos(ws));
                            return collection.Distinct().ToList();
                        }
                        else
                        {
                            return Enumerable.Empty<Tcontact>().ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<Tcontact>().ToList();
                }
            }

            public static async Task<List<ContactVM>> MultipleVMAsync(ContactType? contactType = null, IEnumerable<ContactRole> contactRoleList = null, CancellationToken cancellationToken = default)
            {
                try
                {
                    var collection = await MultipleAsync(contactType, contactRoleList, cancellationToken);
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

            public static async Task<IList<Tcontact>> GetContactsInBookAsync<T>(long idBook) where T : class
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tcontact> collection = new List<Tcontact>();
                        if (typeof(T) == typeof(TbookAuthorConnector))
                        {
                            var preCollection = await context.TbookAuthorConnector.Where(w => w.IdBook == idBook).ToListAsync();
                            if (preCollection.Any())
                            {
                                foreach (TbookAuthorConnector driver in preCollection)
                                {
                                    Tcontact model = await context.Tcontact.SingleOrDefaultAsync(w => w.Id == driver.IdContact);
                                    if (model != null)
                                    {
                                        collection.Add(model);
                                    }
                                }

                                return collection;
                            }
                        }
                        else if (typeof(T) == typeof(TbookEditeurConnector))
                        {
                            var preCollection = await context.TbookEditeurConnector.Where(w => w.IdBook == idBook).ToListAsync();
                            if (preCollection.Any())
                            {
                                foreach (TbookEditeurConnector driver in preCollection)
                                {
                                    Tcontact model = await context.Tcontact.SingleOrDefaultAsync(w => w.Id == driver.IdContact);
                                    if (model != null)
                                    {
                                        collection.Add(model);
                                    }
                                }

                                return collection;
                            }
                        }
                        else if (typeof(T) == typeof(TbookTranslatorConnector))
                        {
                            var preCollection = await context.TbookTranslatorConnector.Where(w => w.IdBook == idBook).ToListAsync();
                            if (preCollection.Any())
                            {
                                foreach (TbookTranslatorConnector driver in preCollection)
                                {
                                    Tcontact model = await context.Tcontact.SingleOrDefaultAsync(w => w.Id == driver.IdContact);
                                    if (model != null)
                                    {
                                        collection.Add(model);
                                    }
                                }

                                return collection;
                            }
                        }
                        
                        return collection;
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<Tcontact>().ToList();
                }
            }

            public static async Task<IList<ContactVM>> GetContactsVmInBookAsync<T>(long idBook) where T : class
            {
                try
                {
                    var collection = await GetContactsInBookAsync<T>(idBook);
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

            public static async Task<IList<long>> GetContactsIdInBookAsync<T>(long idBook) where T : class
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        if (typeof(T) == typeof(TbookAuthorConnector))
                        {
                            return await context.TbookAuthorConnector.Where(w => w.IdBook == idBook).Select(s => s.Id).ToListAsync();
                        }
                        else if (typeof(T) == typeof(TbookEditeurConnector))
                        {
                            return await context.TbookEditeurConnector.Where(w => w.IdBook == idBook).Select(s => s.Id).ToListAsync();
                        }
                        else if (typeof(T) == typeof(TbookTranslatorConnector))
                        {
                            return await context.TbookTranslatorConnector.Where(w => w.IdBook == idBook).Select(s => s.Id).ToListAsync();
                        }
                        return Enumerable.Empty<long>().ToList();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine(Logs.GetLog(ex, m));
                    return Enumerable.Empty<long>().ToList();
                }
            }

            #endregion


            public static IEnumerable<ContactVM> CreateViewModel(string value, ContactType contactType, ContactRole contactRole, char separator = ',')
            {
                try
                {
                    if (!value.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var splittedValue = StringHelpers.SplitWord(value, new string[] { separator.ToString() });
                        if (splittedValue != null && splittedValue.Length > 0)
                        {
                            List<ContactVM> contactVMs = new List<ContactVM>();
                            foreach (var _value in splittedValue)
                            {
                                ContactVM authorVm = new ContactVM()
                                {
                                    ContactType = contactType,
                                    ContactRoles = new ObservableCollection<ContactRole>() { contactRole },
                                    TitreCivilite = CivilityHelpers.NonSpecifie,
                                };


                                if (contactType != ContactType.Society)
                                {
                                    var split = StringHelpers.SplitWord(_value, new string[] { " " });
                                    if (split.Length == 1)
                                    {
                                        authorVm.Prenom = split[0].Trim();
                                    }
                                    else if (split.Length >= 2)
                                    {
                                        authorVm.Prenom = split[0].Trim();
                                        authorVm.NomNaissance = split[1].Trim();
                                    }
                                }
                                else
                                {
                                    authorVm.SocietyName = value;
                                }

                                contactVMs.Add(authorVm);
                            }

                            return contactVMs;
                        }

                    }
                    return Enumerable.Empty<ContactVM>();
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return Enumerable.Empty<ContactVM>();
                }
            }

            public static async Task AddContactsToBookAsync<T>(IEnumerable<ContactVM> viewModelList, long idBook) where T : class
            {
                try
                {
                    if (viewModelList != null && viewModelList.Any())
                    {
                        using (LibraryDbContext context = new LibraryDbContext())
                        {
                            foreach (ContactVM contactVM in viewModelList)
                            {
                                if (await context.Tcontact.AnyAsync(a => a.Id == contactVM.Id))
                                {
                                    if (typeof(T) == typeof(TbookAuthorConnector))
                                    {
                                        var itemConnector = new TbookAuthorConnector()
                                        {
                                            IdBook = idBook,
                                            IdContact = contactVM.Id,
                                        };
                                        _ = await context.TbookAuthorConnector.AddAsync(itemConnector);
                                        await context.SaveChangesAsync();
                                    }
                                    else if (typeof(T) == typeof(TbookEditeurConnector))
                                    {
                                        var itemConnector = new TbookEditeurConnector()
                                        {
                                            IdBook = idBook,
                                            IdContact = contactVM.Id,
                                        };

                                        _ = await context.TbookEditeurConnector.AddAsync(itemConnector);
                                        await context.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return;
                }
            }


            public static async Task<OperationStateVM> CreateAsync(ContactVM viewModel)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                try
                {
                    if (viewModel == null)
                    {
                        Logs.Log(m, $"Le modèle de vue du contact ne peut être null.");
                        return new OperationStateVM()
                        {
                            IsSuccess = false,
                            Message = DbServices.ViewModelNullOrEmptyMessage,
                        };
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        if (viewModel.ContactType == ContactType.Human)
                        {
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
                        }
                        else if (viewModel.ContactType == ContactType.Society)
                        {
                            if (viewModel.SocietyName.IsStringNullOrEmptyOrWhiteSpace())
                            {
                                return new OperationStateVM()
                                {
                                    IsSuccess = false,
                                    Message = NameEmptyMessage,
                                };
                            }
                        }

                        var isExist = await IsContactExistAsync(viewModel);
                        if (isExist.Item1)
                        {
                            Logs.Log(m, $"Le contact : \"{viewModel.NomNaissance} {viewModel.Prenom}\" ou la société: \"{viewModel.SocietyName}\" existe déjà.");
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordAlreadyExistMessage,
                                Id = isExist.Item2,
                            };
                        }

                        var record = new Tcontact()
                        {
                            Guid = Guid.NewGuid().ToString(),
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
                            DateDeces = viewModel.DateDeces?.ToString(),
                            LieuDeces = viewModel.LieuDeces,
                            DateNaissance = viewModel.DateNaissance?.ToString(),
                            LieuNaissance = viewModel.LieuNaissance,
                            Biographie = viewModel.Biographie,
                            Nationality = viewModel.Nationality,
                            SocietyName = viewModel.SocietyName,
                            Type = (long)viewModel.ContactType,
                        };

                        _ = await context.Tcontact.AddAsync(record);
                        await context.SaveChangesAsync();

                        if (viewModel.ContactRoles != null && viewModel.ContactRoles.Any())
                        {
                            List<TcontactRole> tRoles = new List<TcontactRole>();
                            foreach (var role in viewModel.ContactRoles)
                            {
                                TcontactRole tcontactRole = new TcontactRole()
                                {
                                    IdContact = record.Id,
                                    Role = (long)role,
                                };

                                tRoles.Add(tcontactRole);
                            }

                            if (tRoles != null && tRoles.Any())
                            {
                                await context.TcontactRole.AddRangeAsync(tRoles);
                                await context.SaveChangesAsync();
                            }
                        }

                        await CreateFolderAsync(viewModel.Guid);

                        if (viewModel.ContactType == ContactType.Human)
                        {
                            var result = new OperationStateVM()
                            {
                                IsSuccess = true,
                                Id = record.Id,
                                Message = $"Le contact \"{record.NomNaissance} {record.Prenom}\" a été créé avec succès."
                            };
                            return result;
                        }
                        else if (viewModel.ContactType == ContactType.Society)
                        {
                            var result = new OperationStateVM()
                            {
                                IsSuccess = true,
                                Id = record.Id,
                                Message = $"La société \"{record.SocietyName}\" a été créé avec succès."
                            };
                            return result;
                        }
                        else 
                        {
                            var result = new OperationStateVM()
                            {
                                IsSuccess = true,
                                Id = record.Id,
                                Message = $"Le type de contact inconnue a été créé avec succès."
                            };
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
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

                    if (viewModel.ContactType == ContactType.Human)
                    {
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
                    }
                    else if (viewModel.ContactType == ContactType.Society)
                    {
                        if (viewModel.SocietyName.IsStringNullOrEmptyOrWhiteSpace())
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = NameEmptyMessage,
                            };
                        }
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        var record = await context.Tcontact.SingleOrDefaultAsync(a => a.Id == viewModel.Id);
                        if (record == null)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = false,
                                Message = DbServices.RecordNotExistMessage,
                            };
                        }

                        var isExist = await IsContactExistAsync(viewModel, true, record.Id);
                        if (isExist.Item1)
                        {
                            return new OperationStateVM()
                            {
                                IsSuccess = true,
                                Message = DbServices.RecordAlreadyExistMessage,
                                Id = isExist.Item2,
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
                        record.Biographie = viewModel.Biographie;
                        record.DateDeces = viewModel.DateDeces?.ToString();
                        record.LieuDeces = viewModel.LieuDeces;
                        record.DateNaissance = viewModel.DateNaissance?.ToString();
                        record.LieuNaissance = viewModel.LieuNaissance;
                        record.Type = (long)viewModel.ContactType;
                        record.Nationality = viewModel.Nationality;
                        record.SocietyName = viewModel.SocietyName;

                        var recordRoles = await context.TcontactRole.Where(a => a.IdContact == record.Id).ToListAsync();
                        if (recordRoles.Any())
                        {
                            context.TcontactRole.RemoveRange(recordRoles);
                        }

                        if (viewModel.ContactRoles != null && viewModel.ContactRoles.Any())
                        {
                            List<TcontactRole> tcontactRoles = new List<TcontactRole>();
                            foreach (var role in viewModel.ContactRoles)
                            {
                                var tcontactRole = new TcontactRole()
                                {
                                    IdContact = record.Id,
                                    Role = (long)role,
                                };

                                tcontactRoles.Add(tcontactRole);
                            }

                            if (tcontactRoles != null && tcontactRoles.Any())
                            {
                                await context.TcontactRole.AddRangeAsync(tcontactRoles);
                            }
                        }

                        context.Tcontact.Update(record);
                        await context.SaveChangesAsync();

                        return new OperationStateVM()
                        {
                            IsSuccess = true,
                            Id = record.Id,
                        };
                    }
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
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
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
            public static async Task CompleteModelInfos(Tcontact model)
            {
                try
                {
                    if (model == null)
                    {
                        return;
                    }

                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        model.TcontactRole = await context.TcontactRole.Where(s => s.IdContact == model.Id).ToListAsync();
                    }
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return;
                }
            }

            public static async Task<Tuple<bool, long>> IsContactExistAsync(ContactVM viewModel, bool isEdit = false, long? modelId = null)
            {
                try
                {
                    using (LibraryDbContext context = new LibraryDbContext())
                    {
                        List<Tcontact> existingItemList = null;
                        
                        if (!isEdit || modelId == null)
                        {
                            existingItemList = await context.Tcontact.ToListAsync();
                        }
                        else
                        {
                            existingItemList = await context.Tcontact.Where(c => c.Id != (long)modelId).ToListAsync();
                        }

                        if (existingItemList != null && existingItemList.Any())
                        {
                            string titreCivilite = viewModel.TitreCivilite?.Trim()?.ToLower();
                            string nomNaissance = viewModel.NomNaissance?.Trim()?.ToLower();
                            string prenom = viewModel.Prenom?.Trim()?.ToLower();
                            string autrePrenom = viewModel.AutresPrenoms?.Trim()?.ToLower();
                            string nomUsage = viewModel.NomUsage?.Trim()?.ToLower();
                            string societyName = viewModel.SocietyName?.Trim()?.ToLower();
                            
                            foreach (var item in existingItemList)
                            {
                                //Si personne Physique
                                if (viewModel.ContactType == ContactType.Human)
                                {
                                    if (item.TitreCivilite?.ToLower() == titreCivilite && item.NomNaissance?.ToLower() == nomNaissance &&
                                    item.Prenom?.ToLower() == prenom && item.AutresPrenoms?.ToLower() == autrePrenom &&
                                    item.NomUsage?.ToLower() == nomUsage)
                                    {
                                        return new Tuple<bool, long>(true, item.Id);
                                    }
                                }
                                //Si personne Morale
                                else if (viewModel.ContactType == ContactType.Society)
                                {
                                    if (item.SocietyName?.ToLower() == societyName)
                                    {
                                        return new Tuple<bool, long>(true, item.Id);
                                    }
                                }
                            }
                        }
                    }

                    return new Tuple<bool, long>(false, 0);

                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Logs.Log(ex, m);
                    return new Tuple<bool, long>(false, 0);
                }
            }

            /// <summary>
            /// Convertit un modèle en modèle de vue
            /// </summary>
            /// <typeparam name="T1">Type d'entrée</typeparam>
            /// <typeparam name="T2">Type sortie</typeparam>
            /// <param name="model">Modèle de base de données</param>
            /// <returns>Un modèle de vue</returns>
            public static async Task<ContactVM> ViewModelConverterAsync(Tcontact model)
            {
                try
                {
                    if (model == null) return null;

                    var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                    if (isGuidCorrect == false) return null;

                    var viewModel = new ContactVM()
                    {
                        Id = model.Id,
                        Guid = isGuidCorrect ? guid : Guid.Empty,
                        ContactType = (ContactType)model.Type,
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
                        DateDeces = DatesHelpers.Converter.GetNullableDateFromString(model.DateDeces),
                        LieuDeces = model.LieuDeces,
                        DateNaissance = DatesHelpers.Converter.GetNullableDateFromString(model.DateNaissance),
                        LieuNaissance = model.LieuNaissance,
                        Biographie = model.Biographie,
                        SocietyName = model.SocietyName,
                        Nationality = model.Nationality,
                    };

                    if (model.TcontactRole == null || !model.TcontactRole.Any())
                    {
                        await CompleteModelInfos(model);
                    }

                    if (model.TcontactRole != null && model.TcontactRole.Count > 0)
                    {
                        viewModel.ContactRoles = new ObservableCollection<ContactRole>(model.TcontactRole.Select(s => (ContactRole)s.Role));
                    }

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
