using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.Code.Services.Logging;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
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
        readonly EsGeneral _EsGeneral = new EsGeneral();

        public async Task<OperationStateVM> ChangeLibraryItemJaquetteAsync(BibliothequeVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Le modèle de vue est null.",
                    };
                }

                var storageFile = await Files.OpenStorageFileAsync(Files.ImageExtensions);
                if (storageFile == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Le fichier n'a pas pas pû être récupéré par le sélecteur de fichier.",
                    };
                }

                var folderItem = await this.GetLibraryItemFolderAsync(viewModel.Guid);
                if (folderItem == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Le répertoire de la bibliothèque \"{viewModel.Name}\" n'a pas pû être trouvé.",
                    };
                }

                string baseFile = "Library_Jaquette";

                var deleteResult = await _EsGeneral.RemoveFileAsync(baseFile, folderItem, EsGeneral.SearchOptions.StartWith);
                if (!deleteResult.IsSuccess)
                {
                    return deleteResult;
                }

                var newCopyFile = await storageFile.CopyAsync(folderItem, baseFile + System.IO.Path.GetExtension(storageFile.Path), NameCollisionOption.ReplaceExisting);
                if (newCopyFile == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = "Le fichier n'a pû être copié dans le répertoire de l'application.",
                    };
                }

                return new OperationStateVM()
                {
                    IsSuccess = true,
                    Result = newCopyFile.Path,
                };
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return new OperationStateVM()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<string> GetLibraryItemJaquettePathAsync(BibliothequeVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel == null)
                {
                    return null;
                }

                var folderItem = await this.GetLibraryItemFolderAsync(viewModel.Guid);
                if (folderItem == null)
                {
                    return null;
                }

                foreach (var ext in Files.ImageExtensions)
                {
                    string fileName = $"Library_Jaquette{ext}";
                    var storageItem = await folderItem.TryGetItemAsync(fileName);
                    if (storageItem == null || !storageItem.IsOfType(StorageItemTypes.File))
                    {
                        continue;
                    }

                    return storageItem.Path;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

        /// <summary>
        /// Crée le dossier d'une bibliothèque dans le dossier "Libraries" et/ou renvoie l'objet <see cref="StorageFolder"/> 
        /// </summary>
        /// <returns></returns>
        public async Task<StorageFolder> GetLibraryItemFolderAsync(Guid guid)
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
        public async Task<StorageFolder> GetLibrariesFolderAsync()
        {
            try
            {
                var folder = await _EsGeneral.CreateFolderInLocalFolderAppAsync(EsGeneral.DefaultPath.Libraries, CreationCollisionOption.OpenIfExists);
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
        public async Task<StorageFolder> GetBooksFolderAsync()
        {
            try
            {
                var folder = await _EsGeneral.CreateFolderInLocalFolderAppAsync(EsGeneral.DefaultPath.Books, CreationCollisionOption.OpenIfExists);
                return folder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        
    }
}
