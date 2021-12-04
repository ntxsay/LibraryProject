using RostalProjectUWP.Code.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace RostalProjectUWP.Code.Services.ES
{
    public partial class Folders
    {
        public static async Task<StorageFolder> PickFolderAsync()
        {
            try
            {
                var folderPicker = new Windows.Storage.Pickers.FolderPicker
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
                };
                folderPicker.FileTypeFilter.Add("*");

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    // Application now has read/write access to all contents in the picked folder
                    // (including other sub-folder contents)
                    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                    return folder;
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static async Task<decimal?> GetFreeSpaceAsync(StorageFolder folder)
        {
            try
            {
                if (folder == null)
                {
                    return null;
                }

                IDictionary<string, object> retrivedProperties = await folder.Properties.RetrievePropertiesAsync(new string[] { "System.FreeSpace" });
                if (retrivedProperties == null || retrivedProperties.Count == 0)
                {
                    return null;
                }
                object nameSpace = retrivedProperties["System.FreeSpace"];
                if (nameSpace == null || !(nameSpace is ulong freeSpace))
                {
                    return null;
                }

                //var freeSpace = (ulong)retrivedProperties["System.FreeSpace"];
                var freeSpaceDecimal = Convert.ToDecimal(freeSpace);
                decimal GB = freeSpaceDecimal / 1024 / 1024 / 1024;
                var rounded = Math.Round(GB, 2);

                return rounded;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");
                return null;
            }
        }

        public static async Task<long?> GetFolderSizeAsync(StorageFolder folder)
        {
            try
            {
                if (folder == null)
                {
                    return null;
                }

                // Query all files in the folder. Make sure to add the CommonFileQuery
                // So that it goes through all sub-folders as well
                StorageFileQueryResult folders = folder.CreateFileQuery(CommonFileQuery.OrderByName);

                // Await the query, then for each file create a new Task which gets the size
                IEnumerable<Task<ulong>> fileSizeTasks = (await folders.GetFilesAsync()).Select(async file => (await file.GetBasicPropertiesAsync()).Size);

                // Wait for all of these tasks to complete. WhenAll thankfully returns each result
                // as a whole list
                ulong[] sizes = await Task.WhenAll(fileSizeTasks);

                // Sum all of them up. You have to convert it to a long because Sum does not accept ulong.
                long folderSize = sizes.Sum(l => (long)l);
                return folderSize;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");
                return null;
            }
        }

        public static async Task<EsOperationState> CopyFolderAsync(StorageFolder source, StorageFolder destinationContainer, string desiredName = null)
        {
            try
            {
                StorageFolder destinationFolder = await destinationContainer.CreateFolderAsync(desiredName ?? source.Name, CreationCollisionOption.ReplaceExisting);

                foreach (StorageFile file in await source.GetFilesAsync())
                {
                    _ = await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
                }
                foreach (StorageFolder folder in await source.GetFoldersAsync())
                {
                    _ = await CopyFolderAsync(folder, destinationFolder);
                }

                return new EsOperationState()
                {
                    IsSuccess = true,
                    Folder = destinationFolder,
                };
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");
                return new EsOperationState()
                {
                    IsSuccess = false,
                    Message = $"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}",
                };
            }
        }

        public static async Task<StorageFolder> CreateFolderInLocalFolderAppAsync(string NomDuDossierACreer, CreationCollisionOption creationCollisionOption = CreationCollisionOption.GenerateUniqueName)
        {
            try
            {
                if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(NomDuDossierACreer))
                {
                    return null;
                }

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder folder = await localFolder.CreateFolderAsync(NomDuDossierACreer, creationCollisionOption);
                return folder ?? null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> GetFolderPathInLocalFolderAppAsync(string NomDuDossier)
        {
            try
            {
                if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(NomDuDossier))
                {
                    return null;
                }

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                if (localFolder == null)
                {
                    return null;
                }

                var folder = await localFolder.TryGetItemAsync(NomDuDossier);
                if (folder != null && folder.IsOfType(StorageItemTypes.Folder))
                {
                    return folder.Path;

                }
                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");
                return null;
            }
        }

        public static async Task<StorageFolder> GetFolderAsync(string folderPath)
        {
            try
            {
                if (folderPath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                StorageFolder accessFolder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                return accessFolder;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");
                //ToastServices.PopToast(ex.Message);
                return null;
            }
        }

        public static StorageFolder GetFolder(string folderPath)
        {
            try
            {
                if (folderPath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                StorageFolder accessFolder = StorageFolder.GetFolderFromPathAsync(folderPath).AsTask().GetAwaiter().GetResult();
                return accessFolder;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");
                return null;
            }
        }

        public static async Task<bool> ExistsAsync(string NomDuDossier)
        {
            try
            {
                if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(NomDuDossier))
                {
                    return false;
                }
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder folder = await localFolder.GetFolderAsync(NomDuDossier);
                return folder != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Exists(string NomDuDossier)
        {
            try
            {
                if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(NomDuDossier))
                {
                    return false;
                }
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder folder = localFolder.GetFolderAsync(NomDuDossier).AsTask().GetAwaiter().GetResult();
                return folder != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}
