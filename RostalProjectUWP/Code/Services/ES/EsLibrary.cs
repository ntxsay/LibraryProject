using RostalProjectUWP.Code.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RostalProjectUWP.Code.Services.ES
{
    internal class EsLibrary
    {
        public class DefaultPath
        {
            public const string Libraries = "Libraries";
            public const string Books = "Books";
        }

        /// <summary>
        /// Crée le dossier d'une bibliothèque dans le dossier "Libraries" et/ou renvoie l'objet <see cref="StorageFolder"/> 
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFolder> GetLibraryItemFolderAsync(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    return null;
                }

                var librariesFolder = await GetLibrariesFolderAsync();
                if (librariesFolder == null)
                {
                    return null;
                }

                var libraryFolder = await librariesFolder.CreateFolderAsync(guid.ToString(), CreationCollisionOption.OpenIfExists);
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
        /// Crée le dossier "Libraries" et/ou renvoie l'objet <see cref="StorageFolder"/> 
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFolder> GetLibrariesFolderAsync()
        {
            try
            {
                var folder = await CreateFolderInLocalFolderAppAsync(DefaultPath.Libraries, CreationCollisionOption.OpenIfExists);
                return folder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Crée le dossier "Books" et/ou renvoie l'objet <see cref="StorageFolder"/> 
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFolder> GetBooksFolderAsync()
        {
            try
            {
                var folder = await CreateFolderInLocalFolderAppAsync(DefaultPath.Books, CreationCollisionOption.OpenIfExists);
                return folder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<StorageFolder> CreateFolderInLocalFolderAppAsync(string NomDuDossierACreer, CreationCollisionOption creationCollisionOption = CreationCollisionOption.OpenIfExists)
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
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}\nInner Exception : {ex.InnerException?.Message}");
                return null;
            }
        }
    }
}
