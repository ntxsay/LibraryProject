﻿using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LibraryProjectUWP.Code.Services.ES
{
    internal class EsBook
    {
        internal const string BookDefaultJaquette = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string baseJaquetteFile = "Book_Jaquette";

        readonly EsGeneral _EsGeneral = new EsGeneral();

        public async Task<OperationStateVM> ChangeBookItemJaquetteAsync(LivreVM viewModel)
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

                var folderItem = await this.GetBookItemFolderAsync(viewModel.Guid);
                if (folderItem == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Le répertoire du livre \"{viewModel.TitresOeuvre?.FirstOrDefault()}\" n'a pas pû être trouvé.",
                    };
                }

                var deleteResult = await _EsGeneral.RemoveFileAsync(baseJaquetteFile, folderItem, EsGeneral.SearchOptions.StartWith);
                if (!deleteResult.IsSuccess)
                {
                    return deleteResult;
                }

                var newCopyFile = await storageFile.CopyAsync(folderItem, baseJaquetteFile + System.IO.Path.GetExtension(storageFile.Path), NameCollisionOption.ReplaceExisting);
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

        public async Task<string> GetBookItemJaquettePathAsync(LivreVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel == null)
                {
                    return null;
                }

                var folderItem = await this.GetBookItemFolderAsync(viewModel.Guid);
                if (folderItem == null)
                {
                    return null;
                }

                foreach (var ext in Files.ImageExtensions)
                {
                    string fileName = $"{baseJaquetteFile}{ext}";
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
        /// Crée le dossier d'un livre dans le dossier "Books" et/ou renvoie l'objet <see cref="StorageFolder"/> 
        /// </summary>
        /// <returns></returns>
        public async Task<StorageFolder> GetBookItemFolderAsync(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    return null;
                }

                var booksFolder = await GetBooksFolderAsync();
                if (booksFolder == null)
                {
                    return null;
                }

                var bookFolder = await booksFolder.CreateFolderAsync(guid.ToString(), CreationCollisionOption.OpenIfExists);
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