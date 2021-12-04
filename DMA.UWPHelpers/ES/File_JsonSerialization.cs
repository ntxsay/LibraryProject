using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMA.UWPHelpers.Helpers;
using Newtonsoft.Json;
using Windows.Storage;

namespace DMA.UWPHelpers.ES
{
    public partial class File
    {
        public struct Serialization
        {
            public class Json
            {

                public static async Task<bool> SerializeAsync<T>(T value, StorageFile configFileName)
                {
                    try
                    {
                        if (configFileName == null)
                        {
                            return false;
                        }

                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        string dataString = JsonConvert.SerializeObject(value, typeof(T), Formatting.Indented, settings);
                        if (dataString.IsStringNullOrEmptyOrWhiteSpace())
                        {
                            return false;
                        }

                        await FileIO.WriteTextAsync(configFileName, dataString);

                        return true;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                public static async Task<T> DeserializeAsync<T>(StorageFile configFileName)
                {
                    try
                    {
                        if (configFileName == null)
                        {
                            return default;
                        }

                        string dataString = await Windows.Storage.FileIO.ReadTextAsync(configFileName);
                        if (dataString.IsStringNullOrEmptyOrWhiteSpace())
                        {
                            return default;
                        }

                        var settings = new JsonSerializerSettings();
                        return JsonConvert.DeserializeObject<T>(dataString, settings);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }
    }
}
