using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using AppHelpers.Strings;
using AppHelpers;
using Windows.ApplicationModel;

namespace DMA.UWPHelpers.Logging
{
    public class UwpLoggerVM
    {
        public DateTime Date { get; set; }
        public string Version { get; set; }
        public UwpLoggerState TypeLog { get; set; }
        public string Message { get; set; }
        public string InnerMessage { get; set; }
        public string ClassMethod { get; set; }
        public int Line { get; set; }
    }

    public enum UwpLoggerState
    {
        Warning,
        Error,
        Information
    }

    public class UwpLogger
    {
        public static async Task CreateLogAsync(string mainMessage, string innerMessage, UwpLoggerState typeOfLog, string version, int line, string classMethod)
        {
            try
            {
                StorageFolder dataStorage = ApplicationData.Current.LocalFolder;
                if (dataStorage == null)
                {
                    Debug.WriteLine("Impossible de récupérer le dossier de données de l'application.");
                    return;
                }

                StorageFolder configurationSf = await dataStorage.CreateFolderAsync("Configuration", CreationCollisionOption.OpenIfExists);
                if (configurationSf == null)
                {
                    Debug.WriteLine("Impossible de récupérer le dossier de configuration de l'application.");
                    return;
                }

                StorageFolder loggerSf = await configurationSf.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);
                if (loggerSf == null)
                {
                    Debug.WriteLine("Impossible de récupérer le dossier des logs de l'application.");
                    return;
                }

                if (classMethod.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Debug.WriteLine($"{nameof(CreateLogAsync)} : Le nom de la classe et/ou de la méthode , ne doit pas être vide ou ne contenir que des espaces blancs.");
                    return;
                }

                UwpLoggerVM viewModel = new UwpLoggerVM()
                {
                    Date = DateTime.Now,
                    Version = version,
                    TypeLog = typeOfLog,
                    ClassMethod = classMethod,
                    Message = mainMessage,
                    InnerMessage = innerMessage,
                    Line = line,
                };

                await CreateLogAsync(viewModel, loggerSf);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(CreateLogAsync)} : {ex.Message}");
                return;
            }
        }

        private static async Task CreateLogAsync(UwpLoggerVM viewModel, StorageFolder storageFolder)
        {
            try
            {
                if (viewModel == null)
                {
                    Debug.WriteLine($"{nameof(CreateLogAsync)} : L'objet logger ne doit pas être null.");
                    return;
                }

                if (storageFolder == null)
                {
                    Debug.WriteLine($"{nameof(CreateLogAsync)} : Le dossier de sauvegarde ne doit pas être null.");
                    return;
                }

                StorageFile logFile = await storageFolder.CreateFileAsync($"{viewModel.Date:yyyyMMdd_HHmmss}_{viewModel.ClassMethod}_{IdHelpers.GenerateMalaxedGUID(4)}.json");
                if (logFile == null)
                {
                    Debug.WriteLine($"{nameof(CreateLogAsync)} : Le fichier json ne doit pas être null.");
                    return;
                }

                bool isSuccess = await ES.File.Serialization.Json.SerializeAsync(viewModel, logFile);

                if (!isSuccess)
                {
                    Debug.WriteLine($"{nameof(CreateLogAsync)} : Une erreur s'est produite lors de la sérialization du fichier.");
                    return;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(CreateLogAsync)} : {ex.Message}");
                return;
            }
        }


        public static string GetLineError(Exception ex)
        {
            try
            {

                int linenumber = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                return linenumber.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetCurrentMethodName()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            return stackFrame.GetMethod().DeclaringType + " :: " + stackFrame.GetMethod().Name;
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
