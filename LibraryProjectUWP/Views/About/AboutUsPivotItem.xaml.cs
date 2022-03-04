using LibraryProjectUWP.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.About
{
    public sealed partial class AboutUsPivotItem : PivotItem
    {
        private readonly AppInfo appInfo = new AppInfo();
        private string MajorV { get; set; }
        private string MinorV { get; set; }
        private string RevisionV { get; set; }
        private string BuildV { get; set; }
        public AboutUsPivotItem()
        {
            this.InitializeComponent();
            MajorV = appInfo.AppVersion.Major.ToString();
            MinorV = appInfo.AppVersion.Minor.ToString();
            RevisionV = appInfo.AppVersion.Revision.ToString();
            BuildV = appInfo.AppVersion.Build.ToString();
        }
    }
}
