using LibraryProjectUWP.Code.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace LibraryProjectUWP.Code.Services.ES
{
    public partial class Files
    {
        public enum FileSearchNameOption
        {
            StartWith,
            EndWith,
            Default
        }

        public static IEnumerable<string> ImageExtensions
        {
            get => new List<string>()
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".svg",
                };
        }

        public static IEnumerable<string> ExcelExtensions
        {
            get => new List<string>()
                {
                    ".xls",
                    ".xlsx",
                };
        }

        public static IEnumerable<string> LibraryExtensions
        {
            get => new List<string>()
                {
                    ".rtl",
                    ".json",
                };
        }

        public static IEnumerable<string> BookExtensions
        {
            get => new List<string>()
                {
                    ".rtb",
                    ".json",
                };
        }

        public static IEnumerable<string> VideosExtensions
        {
            get => new List<string>()
                {
                    ".mp4",
                    ".wmv",
                    ".avi",
                };
        }

        public struct DefaultFile
        {
            public const string ThumbnailV1 = "ms-appx:///Assets/Square150x150Logo.scale-200.png";
        }

        public static async Task<BitmapImage> BitmapImageFromFileAsync(string imageFileName)
        {
            try
            {
                if (imageFileName.IsStringNullOrEmptyOrWhiteSpace())
                {
#warning "Logging"
                    return null;
                }

                BitmapImage image = new BitmapImage();
                
                if (imageFileName.StartsWith("ms-appx:///"))
                {
                    var uri = new System.Uri(imageFileName);
                    StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                    using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
                    {
                        await image.SetSourceAsync(stream);
                    }
                    return image;
                    //return new BitmapImage(new Uri(imageFileName));
                }
                else if (imageFileName.StartsWith("http") || imageFileName.StartsWith("ftp"))
                {
                    var uri = new System.Uri(imageFileName);
                    var randomAccessStreamReference = RandomAccessStreamReference.CreateFromUri(uri);
                    using (IRandomAccessStream stream = await randomAccessStreamReference.OpenReadAsync())
                    {
                        await image.SetSourceAsync(stream);
                    }
                    return image;
                }
                else if (await ExistsAsync(imageFileName))
                {
                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(imageFileName);
                    using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
                    {
                        await image.SetSourceAsync(stream);
                    }
                    return image;
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static async Task<string> OpenFileNameAsync(IEnumerable<string> extensions, PickerViewMode viewMode = PickerViewMode.Thumbnail, PickerLocationId SuggestedStartLocation = PickerLocationId.PicturesLibrary)
        {
            try
            {
                if (extensions == null || extensions.Count() == 0)
                {
                    return null;
                }

                FileOpenPicker picker = new FileOpenPicker
                {
                    ViewMode = viewMode,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };

                foreach (var ext in extensions)
                {
                    picker.FileTypeFilter.Add(ext);
                }

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    return file.Path;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<StorageFile> OpenStorageFileAsync(IEnumerable<string> extensions, PickerViewMode viewMode = PickerViewMode.Thumbnail, PickerLocationId SuggestedStartLocation = PickerLocationId.PicturesLibrary)
        {
            try
            {
                if (extensions == null || extensions.Count() == 0)
                {
                    return null;
                }

                FileOpenPicker picker = new FileOpenPicker
                {
                    ViewMode = viewMode,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };

                foreach (var ext in extensions)
                {
                    picker.FileTypeFilter.Add(ext);
                }

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    return file;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<StorageFile[]> OpenStorageFilesAsync(IEnumerable<string> extensions, PickerViewMode viewMode = PickerViewMode.Thumbnail, PickerLocationId SuggestedStartLocation = PickerLocationId.PicturesLibrary)
        {
            try
            {
                if (extensions == null || extensions.Count() == 0)
                {
                    return Array.Empty<StorageFile>();
                }

                FileOpenPicker picker = new FileOpenPicker
                {
                    ViewMode = viewMode,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };

                foreach (var ext in extensions)
                {
                    picker.FileTypeFilter.Add(ext);
                }

                IEnumerable<StorageFile> files = await picker.PickMultipleFilesAsync();
                if (files != null && files.Any())
                {
                    return files.ToArray();
                }
                else
                {
                    return Array.Empty<StorageFile>();
                }
            }
            catch (Exception)
            {
                return Array.Empty<StorageFile>();
            }
        }

        public static async Task<StorageFile> SaveStorageFileAsync(IDictionary<string, IList<string>> extensions, string suggestFileName = null, PickerLocationId SuggestedStartLocation = PickerLocationId.Downloads)
        {
            try
            {
                if (extensions == null || extensions.Count() == 0)
                {
                    return null;
                }

                FileSavePicker picker = new FileSavePicker
                {
                    SuggestedFileName = suggestFileName,
                    SuggestedStartLocation = SuggestedStartLocation
                };

                foreach (var ext in extensions)
                {
                    picker.FileTypeChoices.Add(ext.Key, ext.Value);
                }

                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    return file;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<bool> UploadFileAsync(StorageFile file, string WebApiPath)
        {
            try
            {
                if (file == null)
                {
                    return false;
                }

                var http = new Windows.Web.Http.HttpClient();
                var formContent = new HttpMultipartFormDataContent();
                var fileContent = new HttpStreamContent(await file.OpenReadAsync());
                formContent.Add(fileContent, "allfiles", file.Name);
                var response = await http.PostAsync(new Uri(WebApiPath), formContent);
                string filepath = Convert.ToString(response.Content); //Give path in which file is uploaded
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static async Task<bool> ExistsAsync(string filePath)
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(filePath);
                string fileName = System.IO.Path.GetFileName(filePath);

                StorageFolder accessFolder = await StorageFolder.GetFolderFromPathAsync(path);
                if (accessFolder == null)
                {
                    return false;
                }
                IStorageItem _file = await accessFolder.TryGetItemAsync(fileName);
                if (_file == null || !_file.IsOfType(StorageItemTypes.File))
                {
                    return false;
                }

                path = null;
                fileName = null;
                accessFolder = null;
                _file = null;
                return true;
            }
            catch (FileNotFoundException ex1)
            {
                Debug.WriteLine(ex1.Message);
                return false;
            }
            catch (UnauthorizedAccessException ex2)
            {
                Debug.WriteLine(ex2.Message);
                return false;
            }
            catch (ArgumentException ex3)
            {
                Debug.WriteLine(ex3.Message);
                return false;
            }
            catch (Exception ex4)
            {
                Debug.WriteLine(ex4.Message);
                return false;
            }
        }

        public static bool Exists(string filePath)
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(filePath);
                var fileName = System.IO.Path.GetFileName(filePath);
                StorageFolder accessFolder = StorageFolder.GetFolderFromPathAsync(path).AsTask().GetAwaiter().GetResult();
                StorageFile file = accessFolder.GetFileAsync(fileName).AsTask().GetAwaiter().GetResult();
                return file != null;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<StorageFile> GetFileAsync(string filePath)
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(filePath);
                var fileName = System.IO.Path.GetFileName(filePath);
                StorageFolder accessFolder = await StorageFolder.GetFolderFromPathAsync(path);
                StorageFile file = await accessFolder.GetFileAsync(fileName);
                return file;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<bool> DeleteAllFilesAsync(StorageFolder parentFolder)
        {
            try
            {
                if (parentFolder == null)
                {
                    return false;
                }

                var files = await parentFolder.GetFilesAsync();
                if (files == null || files.Count == 0) return true;

                foreach (var file in files)
                {
                    await file.DeleteAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtient une énumération d'objets <see cref="StorageFile"/> représentant les fichiers trouvés en fonction du dossier spécifié et du nom de fichier à rechercher
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="Name"></param>
        /// <param name="SearchInSubFolder"></param>
        /// <returns></returns>
        public static async Task<IReadOnlyList<StorageFile>> GetFilesByNameAsync(StorageFolder parentFolder, string Name, FileSearchNameOption Option = FileSearchNameOption.StartWith, bool SearchInSubFolder = true)
        {
            try
            {
                if (parentFolder == null)
                {
                    return null;
                }

                if (Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                List<StorageFolder> FolderList = new List<StorageFolder>();
                if (SearchInSubFolder == true)
                {
                    var folderlist = await parentFolder.GetFoldersAsync();
                    if (folderlist != null && folderlist.Count > 0)
                    {
                        FolderList.AddRange(folderlist);
                    }
                }

                FolderList.Add(parentFolder);

                if (FolderList == null || FolderList.Count == 0)
                {
                    return null;
                }

                List<StorageFile> StorageFilesList = new List<StorageFile>();
                foreach (StorageFolder Folder in FolderList)
                {
                    var FolderFiles = await Folder.GetFilesAsync();
                    if (FolderFiles == null || FolderFiles.Count() == 0)
                    {
                        continue;
                    }

                    IEnumerable<StorageFile> filteredFiles;
                    switch (Option)
                    {
                        case FileSearchNameOption.StartWith:
                            filteredFiles = FolderFiles.Where(w => w.Name.ToLower().StartsWith(Name.ToLower()));
                            break;
                        case FileSearchNameOption.EndWith:
                            filteredFiles = FolderFiles.Where(w => w.Name.ToLower().EndsWith(Name.ToLower()));
                            break;
                        case FileSearchNameOption.Default:
                            filteredFiles = FolderFiles.Where(w => w.Name.ToLower().Contains(Name.ToLower()));
                            break;
                        default:
                            filteredFiles = FolderFiles.Where(w => w.Name.ToLower().Contains(Name.ToLower()));
                            break;
                    }

                    StorageFilesList.AddRange(filteredFiles);
                }

                return StorageFilesList.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<IEnumerable<StorageFile>> GetFilesAsync(StorageFolder parentFolder, string Extension)
        {
            try
            {
                if (parentFolder == null)
                {
                    return Enumerable.Empty<StorageFile>();
                }

                if (Extension.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return Enumerable.Empty<StorageFile>();
                }

                IReadOnlyList<StorageFolder> FolderList = await parentFolder.GetFoldersAsync();
                if (FolderList == null || FolderList.Count == 0)
                {
                    return Enumerable.Empty<StorageFile>();
                }

                List<StorageFile> StorageFilesList = new List<StorageFile>();
                foreach (StorageFolder Folder in FolderList)
                {
                    var FolderFiles = await Folder.GetFilesAsync();
                    if (FolderFiles == null || FolderFiles.Count() == 0)
                    {
                        continue;
                    }

                    var filteredFiles = FolderFiles.Where(w => w.FileType.ToLower() == Extension.ToLower());
                    StorageFilesList.AddRange(filteredFiles);
                }

                return StorageFilesList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        

        public static async Task<byte[]> ConvertFileToByte(StorageFile file)
        {
            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();

                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }

        }


    }

}
