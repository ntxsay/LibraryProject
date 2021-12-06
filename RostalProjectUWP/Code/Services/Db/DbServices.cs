using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RostalProjectUWP.Models.Local;
using RostalProjectUWP.ViewModels.General;
using Windows.Storage;

namespace RostalProjectUWP.Code.Services.Db
{
    internal partial class DbServices
    {
        internal const string RecordNotExistMessage = "Cet enregistrement n'existe pas.";
        internal const string RecordAlreadyExistMessage = "Cet enregistrement existe déjà.";
        internal const string ViewModelNullOrEmptyMessage = "Le modèle de vue est null ou ne contient aucun élément.";
        internal const string ParentIdNullOrEmptyMessage = "L'id parent n'est pas renseigné.";
        internal const string UnsupportedTParameter = "Le type du paramètre T n'est pas supporté.";
        //internal static readonly RostalDbContext Context = new RostalDbContext();

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

        internal static string DbFile
        {
            get
            {
                try
                {
                    //StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                    StorageFile file = localFolder.GetFileAsync("RostalDB.db").AsTask().GetAwaiter().GetResult();
                    if (file == null)
                    {
                        //ToastServices.PopToast("Le chemin d'accès à la base de données n'est pas valide ou n'a pas été trouvé. Veuillez vérifier vos paramètres.");
                        throw new Exception("Le chemin d'accès à la base de données n'est pas valide ou n'a pas été trouvé. Veuillez vérifier vos paramètres.");
                        //return null;
                    }
                    Debug.WriteLine(file.Path);
                    return file.Path;
                }
                catch (Exception ex)
                {
                    MethodBase m = MethodBase.GetCurrentMethod();
                    Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                    return null;
                }
            }
        }

        //public RostalDbContext()
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

        //ValueGeneratedNever ==> ValueGeneratedOnAdd
    }

}
