using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppHelpers.Strings;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace DMA.UWPHelpers.ES
{
    public partial class File
    {
        public struct Images
        {
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
                    else if (await ES.File.ExistsAsync(imageFileName))
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
        }
        public struct ImagesRessources
        {
            public const string Gift = "ms-appx:///Assets/Icons/Society/iconfinder_gift-box-present-surprise-wrap_3209344.png";
            public const string MultipleGift = "ms-appx:///Assets/Icons/Society/iconfinder_christmas-07_3927512.png";
            public const string MangaBulled = "ms-appx:///Assets/Icons/Society/iconfinder_CDisplay_Manga_177005.png";
            public const string Naruto = "ms-appx:///Assets/Icons/JapanAnimation/icons8-naruto-512.png";

            public static Dictionary<string, string> ImagesReSourcesDictionary = new Dictionary<string, string>()
                {
                    { nameof(Gift), Gift },
                    { nameof(MultipleGift), MultipleGift },
                    { nameof(MangaBulled), MangaBulled },
                    { nameof(Naruto), Naruto },
                };

            public static string GetImageSourceByName(string ResourceName)
            {
                try
                {
                    if (ResourceName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return null;
                    }

                    var value = ImagesReSourcesDictionary.SingleOrDefault(s => s.Key.ToLower() == ResourceName.ToLower()).Value;
                    return value;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
