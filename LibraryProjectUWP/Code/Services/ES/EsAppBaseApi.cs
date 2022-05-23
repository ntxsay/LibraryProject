using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace LibraryProjectUWP.Code.Services.ES
{
    internal partial class EsAppBaseApi
    {
        /// <summary>
        /// Désérialize un fichier au format json
        /// </summary>
        /// <typeparam name="T">Type de l'objet représentant</typeparam>
        /// <param name="storageFile">Fichier</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> OpenItemFromFileAsync<T>(StorageFile storageFile) where T : class
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (storageFile == null)
                {
                    Logs.Log(m, "Le fichier n'a pas pû être ouvert.");
                    return Enumerable.Empty<T>();
                }

                var jsonType = await Files.Serialization.Json.GetJsonType(storageFile);
                if (jsonType == null)
                {
                    return Enumerable.Empty<T>();
                }
                else if (jsonType is JObject)
                {
                    var result = await Files.Serialization.Json.DeSerializeSingleAsync<T>(storageFile);
                    if (result == null)
                    {
                        Logs.Log(m, "Le flux n'a pas été ouvert correctement.");
                        return Enumerable.Empty<T>();
                    }
                    return new T[] { result };
                }
                else if (jsonType is JArray)
                {
                    var result = await Files.Serialization.Json.DeSerializeMultipleAsync<T>(storageFile);
                    if (result == null)
                    {
                        Logs.Log(m, "Le flux n'a pas été ouvert correctement.");
                        return Enumerable.Empty<T>();
                    }
                    return result;
                }

                return Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }


        [Obsolete]
        public async Task<OperationStateVM> RemoveFileAsync(string baseName, StorageFolder Folder, SearchOptions options = SearchOptions.StartWith)
        {
            try
            {
                if (baseName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = "Le chemin d'accès au fichier ne peut pas être vide ou ne contenir que des espaces blancs.",
                    };
                }


                var files = await Folder.GetFilesAsync(CommonFileQuery.OrderByName);
                if (files == null || files.Count == 0)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = true,
                        Message = "Aucun fichier n'a été trouvé.",
                    };
                }

                foreach (var file in files)
                {
                    switch (options)
                    {
                        case SearchOptions.StartWith:
                            if (file.Name.Trim().StartsWith(baseName)) await file.DeleteAsync();
                            break;
                        case SearchOptions.Contains:
                            if (file.Name.Contains(baseName)) await file.DeleteAsync();
                            break;
                        case SearchOptions.EndWith:
                            if (file.Name.Trim().EndsWith(baseName)) await file.DeleteAsync();
                            break;
                        case SearchOptions.Egal:
                            if (file.Name.Trim().ToUpper() == baseName.Trim().ToUpper()) await file.DeleteAsync();
                            break;
                        default:
                            break;
                    }
                }

                return new OperationStateVM()
                {
                    IsSuccess = true,
                };

            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return new OperationStateVM()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }



        /// <summary>
        /// Crée le dossier d'une bibliothèque dans le dossier "Libraries" et/ou renvoie l'objet <see cref="StorageFolder"/> 
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public async Task<StorageFolder> GetChildItemFolderAsync(Guid guid, MainPathEnum mainPathEnum)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    return null;
                }

                string folderName = null;
                switch (mainPathEnum)
                {
                    case MainPathEnum.Libraries:
                        folderName = DefaultPathName.Libraries;
                        break;
                    case MainPathEnum.Books:
                        folderName = DefaultPathName.Books;
                        break;
                    case MainPathEnum.Contacts:
                        folderName = DefaultPathName.Contacts;
                        break;
                    case MainPathEnum.Authors:
                        folderName = DefaultPathName.Authors;
                        break;
                    case MainPathEnum.Editors:
                        break;
                    default:
                        break;
                }

                if (folderName == null)
                {
                    return null;
                }

                var mainFolder = await GetFolderInLocalAppDirAsync(folderName);
                if (mainFolder == null)
                {
                    return null;
                }

                var libraryFolder = await mainFolder.CreateFolderAsync(guid.ToString(), CreationCollisionOption.OpenIfExists);
                if (libraryFolder == null)
                {
                    return null;
                }

                return libraryFolder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Crée ou (obtient si existe déjà) le dossier d'un item particulier tel une bibliothèque, un livre, un contact, etc... via un objet <see cref="StorageFolder"/>
        /// </summary>
        /// <typeparam name="T">Représente un modèle de vue. Exemple : <see cref="BibliothequeVM"/> ou <see cref="LivreVM" /> ou <see cref="ContactVM" /></typeparam>
        /// <param name="newDefaultFolderGuid">Représente le nom du dossier</param>
        /// <returns></returns>
        public async Task<StorageFolder> CreateItemFolderAsync<T>(Guid newItemFolderGuid) where T : class
        {
            try
            {
                if (newItemFolderGuid == Guid.Empty || newItemFolderGuid == default)
                {
                    return null;
                }

                var defaultFolder = await this.GetFolderInLocalAppDirAsync<T>();
                if (defaultFolder == null)
                {
                    return null;
                }

                var bookFolder = await defaultFolder.CreateFolderAsync(newItemFolderGuid.ToString(), CreationCollisionOption.OpenIfExists);
                if (bookFolder == null)
                {
                    return null;
                }

                return bookFolder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtient un dossier dans le dossier local de l'application via un objet <see cref="StorageFolder"/>
        /// </summary>
        /// <typeparam name="T">
        /// Représente un modèle de vue. Exemple : <see cref="BibliothequeVM"/> ou <see cref="LivreVM" /> ou <see cref="ContactVM" />
        /// </typeparam>
        /// <remarks>Remarques : Ces dossiers retournés font intrèsinquement partie de la structure de l'application.</remarks>
        /// <returns></returns>
        public async Task<StorageFolder> GetFolderInLocalAppDirAsync<T>() where T : class
        {
            try
            {
                StorageFolder folder = null;

                if (typeof(T) == typeof(BibliothequeVM))
                {
                    folder = await this.CreateFolderInLocalAppDirAsync(DefaultPathName.Libraries, CreationCollisionOption.OpenIfExists);
                }
                else if (typeof(T) == typeof(LivreVM))
                {
                    folder = await this.CreateFolderInLocalAppDirAsync(DefaultPathName.Books, CreationCollisionOption.OpenIfExists);
                }
                else if (typeof(T) == typeof(ContactVM))
                {
                    folder = await this.CreateFolderInLocalAppDirAsync(DefaultPathName.Contacts, CreationCollisionOption.OpenIfExists);
                }

                return folder;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }
        /// <summary>
        /// Obtient un dossier dans le dossier local de l'application via un objet <see cref="StorageFolder"/>
        /// </summary>
        /// <param name="folderName">Nom du dossier</param>
        /// <remarks>Remarques : Ces dossiers retournés font intrèsinquement partie de la structure de l'application.</remarks>
        /// <returns></returns>
        public async Task<StorageFolder> GetFolderInLocalAppDirAsync(string folderName)
        {
            try
            {
                if (folderName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                StorageFolder mediaStorage = ApplicationData.Current.LocalFolder;
                if (mediaStorage == null)
                {
                    return null;
                }

                IStorageItem item = await mediaStorage.TryGetItemAsync(folderName);
                if (item == null || !item.IsOfType(StorageItemTypes.Folder))
                {
                    return null;
                }

                StorageFolder folder = await mediaStorage.GetFolderAsync(item.Name);
                return folder ?? null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        /// <summary>
        /// Crée un dossier dans le dossier local de l'application puis retourne un objet <see cref="StorageFolder"/>
        /// </summary>
        /// <param name="folderName">Nom du dossier à créer</param>
        /// <param name="creationCollisionOption">Action si le dossier existe déjà</param>
        /// <remarks>Remarques : Ces dossiers retournés font intrèsinquement partie de la structure de l'application.</remarks>
        /// <returns></returns>
        public async Task<StorageFolder> CreateFolderInLocalAppDirAsync(string folderName, CreationCollisionOption creationCollisionOption = CreationCollisionOption.OpenIfExists)
        {
            try
            {
                if (folderName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                StorageFolder mediaStorage = ApplicationData.Current.LocalFolder;
                if (mediaStorage == null)
                {
                    return null;
                }

                StorageFolder folder = await mediaStorage.CreateFolderAsync(folderName, creationCollisionOption);
                return folder ?? null;
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
