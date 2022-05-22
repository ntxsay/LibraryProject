using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
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

namespace LibraryProjectUWP.Code.Services.ES
{
    internal class EsLibrary
    {
        internal const string LibraryDefaultJaquette = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string JaquetteBaseFileName = "Library_Jaquette";
        internal const string baseBackgroundFile = "Book_Collection_Bacground_Image";
        readonly EsGeneral _EsGeneral = new EsGeneral();

        public async Task<string> GetBookCollectionBackgroundImagePathAsync(Guid guidLibrary)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var libraryFolder = await this.GetLibraryItemFolderAsync(guidLibrary);
                if (libraryFolder == null)
                {
                    return EsGeneral.BookCollectionDefaultBackgroundImage;
                }

                foreach (var ext in Files.ImageExtensions)
                {
                    string fileName = $"{baseBackgroundFile}{ext}";
                    var storageItem = await libraryFolder.TryGetItemAsync(fileName);
                    if (storageItem == null || !storageItem.IsOfType(StorageItemTypes.File))
                    {
                        continue;
                    }

                    return storageItem.Path;
                }

                return EsGeneral.BookCollectionDefaultBackgroundImage;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return EsGeneral.BookCollectionDefaultBackgroundImage;
            }
        }

        public async Task<OperationStateVM> ChangeBookCollectionBackgroundImageAsync(Guid guidLibrary)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var storageFile = await Files.OpenStorageFileAsync(Files.ImageExtensions);
                if (storageFile == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Le fichier n'a pas pas pû être récupéré par le sélecteur de fichier.",
                    };
                }

                var libraryFolder = await this.GetLibraryItemFolderAsync(guidLibrary);
                if (libraryFolder == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Le répertoire des livres n'a pas pû être trouvé.",
                    };
                }

                var deleteResult = await _EsGeneral.RemoveFileAsync(baseBackgroundFile, libraryFolder, EsGeneral.SearchOptions.StartWith);
                if (!deleteResult.IsSuccess)
                {
                    return deleteResult;
                }

                var newCopyFile = await storageFile.CopyAsync(libraryFolder, baseBackgroundFile + System.IO.Path.GetExtension(storageFile.Path), NameCollisionOption.ReplaceExisting);
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

        public async Task<string> GetLibraryCollectionBackgroundImagePathAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var libraryFolder = await this.GetLibrariesFolderAsync();
                if (libraryFolder == null)
                {
                    return EsGeneral.LibraryCollectionDefaultBackgroundImage;
                }

                foreach (var ext in Files.ImageExtensions)
                {
                    string fileName = $"{baseBackgroundFile}{ext}";
                    var storageItem = await libraryFolder.TryGetItemAsync(fileName);
                    if (storageItem == null || !storageItem.IsOfType(StorageItemTypes.File))
                    {
                        continue;
                    }

                    return storageItem.Path;
                }

                return EsGeneral.LibraryCollectionDefaultBackgroundImage;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return EsGeneral.LibraryCollectionDefaultBackgroundImage;
            }
        }

        public async Task<OperationStateVM> ChangeLibraryCollectionBackgroundImageAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var storageFile = await Files.OpenStorageFileAsync(Files.ImageExtensions);
                if (storageFile == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Le fichier n'a pas pas pû être récupéré par le sélecteur de fichier.",
                    };
                }

                var libraryFolder = await this.GetLibrariesFolderAsync();
                if (libraryFolder == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Le répertoire des bibliothèques n'a pas pû être trouvé.",
                    };
                }

                var deleteResult = await _EsGeneral.RemoveFileAsync(baseBackgroundFile, libraryFolder, EsGeneral.SearchOptions.StartWith);
                if (!deleteResult.IsSuccess)
                {
                    return deleteResult;
                }

                var newCopyFile = await storageFile.CopyAsync(libraryFolder, baseBackgroundFile + System.IO.Path.GetExtension(storageFile.Path), NameCollisionOption.ReplaceExisting);
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


                var deleteResult = await _EsGeneral.RemoveFileAsync(JaquetteBaseFileName, folderItem, EsGeneral.SearchOptions.StartWith);
                if (!deleteResult.IsSuccess)
                {
                    return deleteResult;
                }

                var newCopyFile = await storageFile.CopyAsync(folderItem, JaquetteBaseFileName + System.IO.Path.GetExtension(storageFile.Path), NameCollisionOption.ReplaceExisting);
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

        public async Task SaveLibraryViewModelAsync(BibliothequeVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel == null)
                {
                    Logs.Log(m, "Le modèle de vue est null.");
                    return;
                }

                var folderItem = await this.GetLibraryItemFolderAsync(viewModel.Guid);
                if (folderItem == null)
                {
                    return;
                }

                var savedFile = await folderItem.CreateFileAsync("model.json", CreationCollisionOption.OpenIfExists);
                if (savedFile == null)
                {
                    Logs.Log(m, "Le fichier n'a pas pû être créé.");
                    return;
                }

                bool isFileSaved = await Files.Serialization.Json.SerializeAsync(viewModel, savedFile);
                if (isFileSaved == false)
                {
                    Logs.Log(m, "Le flux n'a pas été enregistré dans le fichier.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
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
                var folder = await _EsGeneral.CreateFolderInLocalFolderAppAsync(EsGeneral.DefaultPathName.Libraries, CreationCollisionOption.OpenIfExists);
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
                var folder = await _EsGeneral.CreateFolderInLocalFolderAppAsync(EsGeneral.DefaultPathName.Books, CreationCollisionOption.OpenIfExists);
                return folder;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<StorageFile> GetLibrarySettingsFile(BibliothequeVM viewModel)
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

                var storageItem = await folderItem.TryGetItemAsync($"settings.json");
                if (storageItem == null || !storageItem.IsOfType(StorageItemTypes.File))
                {
                    var file = await folderItem.GetFileAsync($"settings.json");
                    return file;
                }

                

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return null;
            }
        }

    }
}
