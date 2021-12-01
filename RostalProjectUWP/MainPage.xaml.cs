using RostalProjectUWP.Code.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RostalProjectUWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.TitleBarCustomization();
        }

        #region Title Bar
        private void TitleBarCustomization()
        {
            try
            {
                CustomDragRegion.Visibility = Visibility.Visible;
                //// change the button background to transparent
                var titlebar = CoreApplication.GetCurrentView().TitleBar;
                titlebar.ExtendViewIntoTitleBar = true;
                titlebar.LayoutMetricsChanged += Titlebar_LayoutMetricsChanged;
                titlebar.IsVisibleChanged += Titlebar_IsVisibleChanged;
                // Set XAML element as a draggable region.
                Window.Current.SetTitleBar(CustomDragRegion);
                ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Titlebar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        private void Titlebar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            this.UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = coreTitleBar.SystemOverlayRightInset;
                AppTitleBar.MinWidth = coreTitleBar.SystemOverlayLeftInset;
            }
            else
            {
                CustomDragRegion.MinWidth = coreTitleBar.SystemOverlayLeftInset;
                AppTitleBar.MinWidth = coreTitleBar.SystemOverlayRightInset;
            }

            CustomDragRegion.Height = AppTitleBar.Height = coreTitleBar.Height;
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(0);// new GridLength(coreTitleBar.SystemOverlayRightInset);
                                                         //TitleBarButton.Margin = new Thickness(0, 0, coreTitleBar.SystemOverlayRightInset, 0);

            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;
        }
        #endregion

        #region Navigation
        public Microsoft.UI.Xaml.Controls.NavigationViewItem _lastItemMUCX;
        private void PrincipalNaviguation_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            try
            {
                if (!(args.InvokedItemContainer is Microsoft.UI.Xaml.Controls.NavigationViewItem item) || item == _lastItemMUCX)
                {
                    //sender.Content = null;
                    return;
                }

                if (item.Tag != null && item.Tag.ToString() != "animesParentItem")
                {
                    NavigateToView(item.Tag.ToString(), item);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void NavigateToView(string itemTag, Microsoft.UI.Xaml.Controls.NavigationViewItem item = null)
        {
            try
            {
                if (itemTag.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return;
                }

                if (itemTag == "proposeGenre")
                {
                    //
                }
                
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void PrincipalNaviguation_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        } 
        #endregion
    }
}
