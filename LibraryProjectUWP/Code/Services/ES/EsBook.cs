using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using Newtonsoft.Json.Linq;
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
        internal const string baseBackgroundFile = "Book_Collection_Bacground_Image";
        internal const string baseJaquetteFile = "Book_Jaquette";
        readonly EsGeneral _EsGeneral = new EsGeneral();


        public async Task SaveBookViewModelAsync(LivreVM viewModel, StorageFolder folderLocation = null)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel == null)
                {
                    Logs.Log(m, "Le modèle de vue est null.");
                    return;
                }

                var folderItem = folderLocation ?? await this.GetBookItemFolderAsync(viewModel.Guid);
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

        public async Task<bool> SaveBookViewModelAsAsync(IEnumerable<LivreVM> viewModelList)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModelList == null || !viewModelList.Any())
                {
                    Logs.Log(m, "Le modèle de vue est null ou ne contient aucun élément.");
                    return false;
                }

                var suggestedFileName = $"model_{viewModelList.Count()}_livre_s_{DateTime.Now:yyyyMMddHHmmss}";

                var savedFile = await Files.SaveStorageFileAsync(new Dictionary<string, IList<string>>()
                    {
                        {"JavaScript Object Notation", new List<string>() { ".json" } }
                    }, suggestedFileName);

                if (savedFile == null)
                {
                    Logs.Log(m, "Le fichier n'a pas pû être créé.");
                    return false;
                }

                bool isFileSaved = await Files.Serialization.Json.SerializeAsync(viewModelList, savedFile);
                if (isFileSaved == false)
                {
                    Logs.Log(m, "Le flux n'a pas été enregistré dans le fichier.");
                    return false;
                }

                return isFileSaved;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return false;
            }
        }

        /// <summary>
        /// Supprime le dossier d'un livre dans le dossier "Books" et/ou renvoie l'objet <see cref="StorageFolder"/> 
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public async Task<bool> DeleteBookItemFolderAsync(Guid guid)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    return false;
                }

                var bookFolder = await GetBookItemFolderAsync(guid);
                if (bookFolder == null)
                {
                    return true;
                }

                await bookFolder.DeleteAsync(StorageDeleteOption.Default);
                return true;
            }
            catch (Exception)
            {
                return false;
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
                var folder = await _EsGeneral.CreateFolderInLocalFolderAppAsync(EsGeneral.DefaultPathName.Books, CreationCollisionOption.OpenIfExists);
                return folder;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
