using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.General;
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
    internal partial class EsGeneral
    {
        internal const string LibraryDefaultJaquette = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string BookDefaultJaquette = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string BookDefaultBackgroundImage = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string BookCollectionDefaultBackgroundImage = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string LibraryCollectionDefaultBackgroundImage = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        public enum SearchOptions
        {
            StartWith,
            Contains,
            EndWith,
            Egal
        }

        public class DefaultPathName
        {
            public const string Libraries = "Libraries";
            public const string Books = "Books";
            public const string Contacts = "Contacts";
            public const string Authors = "Authors";
        }

        public enum MainPathEnum
        {
            Libraries,
            Books,
            Contacts,
            Authors,
            Editors
        }


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

        public async Task<StorageFolder> CreateFolderInLocalFolderAppAsync(string NomDuDossierACreer, CreationCollisionOption creationCollisionOption = CreationCollisionOption.OpenIfExists)
        {
            try
            {
                if (NomDuDossierACreer.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                StorageFolder mediaStorage = ApplicationData.Current.LocalFolder;
                if (mediaStorage == null)
                {
                    return null;
                }

                StorageFolder folder = await mediaStorage.CreateFolderAsync(NomDuDossierACreer, creationCollisionOption);
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
        /// Crée le dossier d'une bibliothèque dans le dossier "Libraries" et/ou renvoie l'objet <see cref="StorageFolder"/> 
        /// </summary>
        /// <returns></returns>
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

                var mainFolder = await GetParentItemFolderAsync(folderName);
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
        /// Crée le dossier en question et/ou renvoie l'objet <see cref="StorageFolder"/> 
        /// </summary>
        /// <returns></returns>
        public async Task<StorageFolder> GetParentItemFolderAsync(string pathName)
        {
            try
            {
                var folder = await CreateFolderInLocalFolderAppAsync(pathName, CreationCollisionOption.OpenIfExists);
                return folder;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
