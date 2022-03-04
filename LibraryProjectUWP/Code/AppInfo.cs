using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace LibraryProjectUWP.Code
{
    public class AppInfo
    {
        public AppInfo()
        {

        }

        public PackageId GetAppInfo()
        {
            try
            {
                Package package = Package.Current;
                PackageId packageId = package.Id;

                return packageId;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public PackageVersion AppVersion
        {
            get
            {
                try
                {
                    Package package = Package.Current;
                    PackageId packageId = package.Id;
                    PackageVersion version = packageId.Version;

                    return version;
                }
                catch (Exception)
                {
                    return default;
                }
            }
        }

        public string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        public string GetAppName()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            string name = packageId.Name;

            return name;
        }

        public string GetAppNameAndVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            string name = packageId.Name;

            return $"{name}_{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }

}
