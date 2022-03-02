using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels.General;
using Windows.Storage;

namespace LibraryProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        internal const string DbName = "LibraryDB.db";
        internal const string RecordNotExistMessage = "Cet enregistrement n'existe pas.";
        internal const string RecordAlreadyExistMessage = "Cet enregistrement existe déjà.";
        internal const string ViewModelNullOrEmptyMessage = "Le modèle de vue est null ou ne contient aucun élément.";
        internal const string ParentIdNullOrEmptyMessage = "L'id parent n'est pas renseigné.";
        internal const string UnsupportedTParameter = "Le type du paramètre T n'est pas supporté.";
        //internal static readonly LibraryDbContext Context = new LibraryDbContext();

        internal static OperationStateVM ViewModelEmpty
        {
            get => new OperationStateVM()
            {
                IsSuccess = false,
                Message = ViewModelNullOrEmptyMessage
            };
        }

        internal static OperationStateVM ParentIdEmpty
        {
            get => new OperationStateVM()
            {
                IsSuccess = false,
                Message = ParentIdNullOrEmptyMessage
            };
        }

        internal static async Task<string> DbFileAsync()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var destinatedDbFile = await localFolder.TryGetItemAsync(DbName);
                if (destinatedDbFile != null && destinatedDbFile.IsOfType(StorageItemTypes.File))
                {
                    return destinatedDbFile.Path;
                }

                StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var originalDbFile = await installedLocation.TryGetItemAsync(DbName);
                if (originalDbFile != null && originalDbFile.IsOfType(StorageItemTypes.File))
                {
                    var file = await installedLocation.GetFileAsync(DbName);
                    if (file != null)
                    {
                        var copiedDbFile = await file.CopyAsync(localFolder);
                        if (copiedDbFile != null)
                        {
                            return copiedDbFile.Path;
                        }
                    }
                }

                throw new Exception("Le chemin d'accès à la base de données n'est pas valide ou n'a pas été trouvé. Veuillez vérifier vos paramètres.");
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine(Logs.GetLog(ex, m));
                return null;
            }
        }

        internal static string DbFile()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var destinatedDbFile = localFolder.TryGetItemAsync(DbName).AsTask().GetAwaiter().GetResult();
                if (destinatedDbFile != null && destinatedDbFile.IsOfType(StorageItemTypes.File))
                {
                    return destinatedDbFile.Path;
                }

                StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var originalDbFile = installedLocation.TryGetItemAsync(DbName).AsTask().GetAwaiter().GetResult();
                if (originalDbFile != null && originalDbFile.IsOfType(StorageItemTypes.File))
                {
                    var file = installedLocation.GetFileAsync(DbName).AsTask().GetAwaiter().GetResult();
                    if (file != null)
                    {
                        var copiedDbFile = file.CopyAsync(localFolder).AsTask().GetAwaiter().GetResult();
                        if (copiedDbFile != null)
                        {
                            return copiedDbFile.Path;
                        }
                    }
                }

                throw new Exception("Le chemin d'accès à la base de données n'est pas valide ou n'a pas été trouvé. Veuillez vérifier vos paramètres.");
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine(Logs.GetLog(ex, m));
                return null;
            }
        }

        #region Aide-mémoire
        //public LibraryDbContext()
        //{
        //    Database.EnsureCreated();
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlite($"Data Source={DbServices.DbFile}");
        //    }
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        var dbFile = DbServices.DbFile();
        //        optionsBuilder.UseSqlite($"Data Source={dbFile}");
        //    }
        //}

        //ValueGeneratedNever ==> ValueGeneratedOnAdd 
        #endregion
    }

}
