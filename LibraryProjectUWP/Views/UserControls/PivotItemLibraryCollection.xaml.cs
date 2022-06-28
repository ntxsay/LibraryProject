using LibraryProjectUWP.Code.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.UserControls
{
    public sealed partial class PivotItemLibraryCollection : PivotItem
    {
        public PivotItemLibraryCollection()
        {
            this.InitializeComponent();
        }

        public void NavigateToView(Type page, object parameters)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                ClearAllNavigationStack();
                _ = FrameContainer.Navigate(page, parameters, new EntranceNavigationTransitionInfo());
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void ClearAllNavigationStack()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FrameContainer.CanGoBack)
                    FrameContainer.BackStack.Clear();

                if (FrameContainer.CanGoForward)
                    FrameContainer.ForwardStack.Clear();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
    }
}
