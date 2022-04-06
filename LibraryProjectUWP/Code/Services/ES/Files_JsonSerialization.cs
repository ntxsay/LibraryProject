using Newtonsoft.Json;
using LibraryProjectUWP.Code.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Reflection;
using LibraryProjectUWP.Code.Services.Logging;

namespace LibraryProjectUWP.Code.Services.ES
{
    public partial class Files
    {
        public struct Serialization
        {
            public class Json
            {
                public enum DeserializeMode
                {
                    Single,
                    Multiple,
                    UnKnow
                }

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
                    catch (Exception ex)
                    {
                        MethodBase m = MethodBase.GetCurrentMethod();
                        Logs.Log(ex, m);
                        return false;
                    }
                }

                public static async Task<string> GetDataStringAsync(StorageFile configFileName)
                {
                    try
                    {
                        if (configFileName == null)
                        {
                            return null;
                        }

                        string dataString = await FileIO.ReadTextAsync(configFileName);
                        return dataString;
                    }
                    catch (Exception ex)
                    {
                        MethodBase m = MethodBase.GetCurrentMethod();
                        Logs.Log(ex, m);
                        return null;
                    }
                }

                public static async Task<DeserializeMode> GetDeSerializationModeAsync(StorageFile configFileName)
                {
                    try
                    {
                        if (configFileName == null)
                        {
                            return DeserializeMode.UnKnow;
                        }

                        string dataString = await GetDataStringAsync(configFileName);
                        if (dataString.IsStringNullOrEmptyOrWhiteSpace())
                        {
                            return DeserializeMode.UnKnow;
                        }

                        if (dataString.StartsWith("["))
                        {
                            return DeserializeMode.Multiple;
                        }
                        else if (dataString.StartsWith("{"))
                        {
                            return DeserializeMode.Single;
                        }

                        return DeserializeMode.UnKnow;
                    }
                    catch (Exception ex)
                    {
                        MethodBase m = MethodBase.GetCurrentMethod();
                        Logs.Log(ex, m);
                        return DeserializeMode.UnKnow;
                    }
                }

                public static async Task<T> DeSerializeSingleAsync<T>(StorageFile configFileName)
                {
                    try
                    {
                        if (configFileName == null)
                        {
                            return default;
                        }

                        string dataString = await FileIO.ReadTextAsync(configFileName);
                        if (dataString.IsStringNullOrEmptyOrWhiteSpace())
                        {
                            return default;
                        }

                        var settings = new JsonSerializerSettings();
                        return JsonConvert.DeserializeObject<T>(dataString, settings);
                    }
                    catch (Exception ex)
                    {
                        MethodBase m = MethodBase.GetCurrentMethod();
                        Logs.Log(ex, m);
                        return default;
                    }
                }

                public static async Task<IEnumerable<T>> DeSerializeMultipleAsync<T>(StorageFile configFileName)
                {
                    try
                    {
                        if (configFileName == null)
                        {
                            return default;
                        }

                        string dataString = await FileIO.ReadTextAsync(configFileName);
                        if (dataString.IsStringNullOrEmptyOrWhiteSpace())
                        {
                            return default;
                        }

                        var settings = new JsonSerializerSettings();
                        return JsonConvert.DeserializeObject<IEnumerable<T>>(dataString, settings);
                    }
                    catch (Exception ex)
                    {
                        MethodBase m = MethodBase.GetCurrentMethod();
                        Logs.Log(ex, m);
                        return Enumerable.Empty<T>();
                    }
                }
            }
        }
    }

}
