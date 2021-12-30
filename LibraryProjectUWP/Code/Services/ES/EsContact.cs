using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Contact;
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
    internal class EsContact
    {
        internal const string DefaultJaquette = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        readonly EsGeneral _EsGeneral = new EsGeneral();
        readonly string baseFile = "Contact_Jaquette";

        public async Task<OperationStateVM> ChangeItemJaquetteAsync(ContactVM viewModel)
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

                var folderItem = await _EsGeneral.GetChildItemFolderAsync(viewModel.Guid, EsGeneral.MainPathEnum.Contacts);
                if (folderItem == null)
                {
                    return new OperationStateVM()
                    {
                        IsSuccess = false,
                        Message = $"Le répertoire du contact \"{viewModel.TitreCivilite} {viewModel.NomNaissance} {viewModel.Prenom}\" n'a pas pû être trouvé.",
                    };
                }

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

        public async Task<string> GetItemJaquettePathAsync(ContactVM viewModel)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (viewModel == null)
                {
                    return null;
                }

                var folderItem = await _EsGeneral.GetChildItemFolderAsync(viewModel.Guid, EsGeneral.MainPathEnum.Contacts);
                if (folderItem == null)
                {
                    return null;
                }

                foreach (var ext in Files.ImageExtensions)
                {
                    string fileName = $"{baseFile}{ext}";
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
    }

}
