using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LibraryProjectUWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModelPage { get; set; } = new MainPageViewModel();
        public MainPage()
        {
            this.InitializeComponent();
            this.TitleBarCustomization();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (PrincipalNaviguation.MenuItems[2] is Microsoft.UI.Xaml.Controls.NavigationViewItem first)
                {
                    PrincipalNaviguation.SelectedItem = first;
                }
                
                LibraryCollectionNavigationAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
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
        private async void PrincipalNaviguation_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            try
            {
                if (!(args.InvokedItemContainer is Microsoft.UI.Xaml.Controls.NavigationViewItem item) || item == _lastItemMUCX)
                {
                    //sender.Content = null;
                    return;
                }

                if (item.Tag != null)
                {
                    await NavigateToViewAsync(item.Tag.ToString(), item);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task NavigateToViewAsync(string itemTag, Microsoft.UI.Xaml.Controls.NavigationViewItem item = null)
        {
            try
            {
                if (itemTag.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return;
                }

                if (itemTag == ViewModelPage.NewElementMenuItem.Tag)
                {
                    await AddElementNavigationAsync();
                }
                else if (itemTag == ViewModelPage.LibraryCollectionMenuItem.Tag)
                {
                    LibraryCollectionNavigationAsync();
                }
                else if (itemTag == ViewModelPage.ContactCollectionMenuItem.Tag)
                {
                    ContactCollectionNavigationAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void PrincipalNaviguation_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            this.GoToBack();
        }

        public bool NavigateToView(string clickedView, object parameters)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                Type view = Assembly.GetExecutingAssembly().GetType($"LibraryProjectUWP.Views.{clickedView}");

                if (string.IsNullOrWhiteSpace(clickedView) || view == null)
                {
                    return false;
                }

                bool isSuccess = this.MainFrameContainer.Navigate(view, parameters, new EntranceNavigationTransitionInfo());
                CheckBackArrowVisibility();
                return isSuccess;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return false;
            }
        }

        public void CheckBackArrowVisibility()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.ViewModelPage.IsBackArrowVisible = this.MainFrameContainer.CanGoBack
                    ? Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Visible
                    : Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public void GoToBack()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.MainFrameContainer.CanGoBack)
                {
                    this.MainFrameContainer.GoBack();
                }

                CheckBackArrowVisibility();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion
        #region Navigation-Method

        private async Task AddElementNavigationAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var dialog = new NewElementCD();

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (dialog.ManagePage == ManagePage.Library)
                    {
                        var libraryList = await DbServices.Library.AllVMAsync();

                        ManageLibraryParametersVM manageBookParameters = new ManageLibraryParametersVM()
                        {
                            EditMode = EditMode.Create,
                            ViewModelList = libraryList,
                        };
                        OpenManageLibraryPage(manageBookParameters);
                    }
                    else if (dialog.ManagePage == ManagePage.Book)
                    {
                        ManageBookParametersVM manageBookParameters = new ManageBookParametersVM()
                        {
                            EditMode = EditMode.Create,
                            ViewModel = new LivreVM()
                            {

                            },
                        };
                        OpenManageBookPage(manageBookParameters);
                    }
                }
                else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private bool LibraryCollectionNavigationAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                return NavigateToView("Library.LibraryCollectionPage", null);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return false;
            }
        }

        private bool ContactCollectionNavigationAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                return NavigateToView("Contact.ContactCollectionPage", null);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return false;
            }
        }

        internal bool BookCollectionNavigationAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                return NavigateToView("Book.BookCollectionPage", null);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return false;
            }
        }

        public bool OpenManageLibraryPage(ManageLibraryParametersVM parameters)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (parameters == null)
                {
                    Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : Le ViewModel est null.");
                    return false;
                }

                return NavigateToView("ManageContainerPage", parameters);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return false;
            }
        }

        public bool OpenManageBookPage(ManageBookParametersVM manageBookParameters)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (manageBookParameters == null || manageBookParameters.ViewModel == null)
                {
                    Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : Le ViewModel est null.");
                    return false;
                }

                return NavigateToView("ManageContainerPage", manageBookParameters);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return false;
            }
        }

        #endregion

        
    }

    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public MainPageViewModel()
        {

        }

        public ItemTagContentVM NewElementMenuItem => new ItemTagContentVM()
        {
            Text = "Nouvel élément",
            Tag = "AddNewElement"
        };

        public ItemTagContentVM LibraryCollectionMenuItem => new ItemTagContentVM()
        {
            Text = "Bibliothèques",
            Tag = "library-collection"
        };

        public ItemTagContentVM ContactCollectionMenuItem => new ItemTagContentVM()
        {
            Text = "Contacts",
            Tag = "contact-collection"
        };


        private Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible _IsBackArrowVisible;
        public Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible IsBackArrowVisible
        {
            get => _IsBackArrowVisible;
            set
            {
                if (_IsBackArrowVisible != value)
                {
                    _IsBackArrowVisible = value;
                    OnPropertyChanged();
                }
            }
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
