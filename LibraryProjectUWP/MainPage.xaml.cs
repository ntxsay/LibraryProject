using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.Models.Local;
using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Settings;
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
using LibraryProjectUWP.Views.UserControls.TitleBar;
using LibraryProjectUWP.Views.Library;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.Icons;
using Windows.UI.Core;
using LibraryProjectUWP.Views.UserControls;

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
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
                ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(840, 600));
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
                else if (itemTag == ViewModelPage.AboutMenuItem.Tag)
                {
                    OpenAboutPage();
                    //await AboutDialogNavigationAsync();
                }
                else if (itemTag == ViewModelPage.LibraryCollectionMenuItem.Tag)
                {
                    LibraryCollectionNavigationAsync();
                }
                else if (itemTag == ViewModelPage.ContactCollectionMenuItem.Tag)
                {
                    ContactCollectionNavigationAsync();
                }
                else if (itemTag == ViewModelPage.SettingsMenuItem.Tag)
                {
                    OpenSettingsPage();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        private async void PrincipalNaviguation_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            await this.GoToBack();
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

                if (this.MainFrameContainer.CanGoBack)
                {
                    AppTitleBar.Margin = new Thickness(40, 0, 0, 0);
                }
                else
                {
                    AppTitleBar.Margin = new Thickness(0, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        public async Task GoToBack()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.MainFrameContainer.CanGoBack)
                {
                    if (this.MainFrameContainer.Content is BookCollectionPage bookCollection && bookCollection.Parameters.ParentLibrary.Books.Count > 100)
                    {
                        var dialog = new BookBeforeGoBackCD()
                        {
                            Title = $"Quitter la bibliothèque {bookCollection.Parameters.ParentLibrary.Name}"
                        };

                        var result = await dialog.ShowAsync();
                        if (result == ContentDialogResult.Secondary)
                        {
                            return;
                        }

                        this.ChangeAppTitle(ViewModelPage.MainTitleBar);
                    }
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
        private async Task AboutDialogNavigationAsync()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var dialog = new AboutCd();

                var result = await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }


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
                this.ChangeAppTitle(ViewModelPage.MainTitleBar);
                return NavigateToView("Library.LibraryCollectionPage", this);
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

        internal bool BookCollectionNavigationAsync(BibliothequeVM parentLibrary, LibraryCollectionPage parentPage)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.ChangeAppTitle(new TitleBarLibraryName(parentLibrary) 
                { 
                    Margin = new Thickness(0, 14, 0, 0),
                });
                return NavigateToView("Book.BookCollectionPage", new LibraryToBookNavigationDriverVM() { ParentLibrary = parentLibrary, ParentPage = parentPage, MainPage = this, });
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

        public bool OpenSettingsPage()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                

                return NavigateToView("ManageContainerPage", new ManageParametersDriverVM() { });
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

        private bool OpenAboutPage()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                this.ChangeAppTitle(ViewModelPage.MainTitleBar);
                return NavigateToView("About.AboutPage", null);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return false;
            }
        }

        public void ChangeAppTitle(UIElement element)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.AppTitleContainer.Children.Any())
                {
                    this.AppTitleContainer.Children.Clear();
                }

                this.AppTitleContainer.Children.Add(element);
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        public async Task<bool> OpenLoadingAsync()
        {
            try
            {
                return await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (!LoadingControl.IsLoading)
                    {
                        LoadingControl.IsLoading = true;
                    }
                });
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void OpenBusyLoader(BusyLoaderParametersVM parametersVM)
        {
            try
            {
                if (!gridMainContainer.Children.Any(a => a is BusyLoader))
                {
                    gridMainContainer.Children.Add(new BusyLoader(parametersVM)
                    {
                        Title = parametersVM.ProgessText,
                        PrimaryButtonTitle = parametersVM.PrimaryButtonText,
                        PrimaryButtonVisibility = parametersVM.PrimaryButtonVisibility,
                        SecondaryButtonTitle = parametersVM.SecondaryButtonText,
                        SecondaryButtonVisibility = parametersVM.SecondaryButtonVisibility,
                        CancelButtonTitle = parametersVM.CancelButtonText,
                        CancelButtonVisibility = parametersVM.CancelButtonVisibility,
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateBusyLoader(BusyLoaderParametersVM parametersVM)
        {
            try
            {
                if (gridMainContainer.Children.FirstOrDefault(a => a is BusyLoader) is BusyLoader busyLoader)
                {
                    if (busyLoader.TbcTitle.Text != parametersVM.ProgessText)
                        busyLoader.TbcTitle.Text = parametersVM.ProgessText;
                    if (busyLoader.TbcPrimary.Text != parametersVM.PrimaryButtonText)
                        busyLoader.TbcPrimary.Text = parametersVM.PrimaryButtonText;
                    if (busyLoader.BtnPrimary.Visibility != parametersVM.PrimaryButtonVisibility)
                        busyLoader.BtnPrimary.Visibility = parametersVM.PrimaryButtonVisibility;
                    if (busyLoader.TbcSecondary.Text != parametersVM.SecondaryButtonText)
                        busyLoader.TbcSecondary.Text = parametersVM.SecondaryButtonText;
                    if (busyLoader.BtnSecondary.Visibility != parametersVM.SecondaryButtonVisibility)
                        busyLoader.BtnSecondary.Visibility = parametersVM.SecondaryButtonVisibility;
                    if (busyLoader.TbcCancel.Text != parametersVM.CancelButtonText)
                        busyLoader.TbcCancel.Text = parametersVM.CancelButtonText;
                    if (busyLoader.BtnCancel.Visibility != parametersVM.CancelButtonVisibility)
                        busyLoader.BtnCancel.Visibility = parametersVM.CancelButtonVisibility;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public BusyLoader GetBusyLoader => gridMainContainer.Children.FirstOrDefault(a => a is BusyLoader) as BusyLoader;

        public void CloseBusyLoader()
        {
            try
            {
                if (gridMainContainer.Children.FirstOrDefault(a => a is BusyLoader) is BusyLoader busyLoader)
                {
                    gridMainContainer.Children.Remove(busyLoader);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public MainPageViewModel()
        {

        }

        public ItemTagContentVM AboutMenuItem => new ItemTagContentVM()
        {
            Text = "À propos de ...",
            Tag = "About"
        };

        public ItemTagContentVM SettingsMenuItem => new ItemTagContentVM()
        {
            Text = "Paramètres",
            Tag = "settings"
        };

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
            Text = "Adhérants",
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

        private object _AppBarTitle;
        public object AppBarTitle
        {
            get => _AppBarTitle;
            set
            {
                if (_AppBarTitle != value)
                {
                    _AppBarTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        public Viewbox MainTitleBar
        {
            get => new Viewbox()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 16,
                Margin = new Thickness(0, 12, 0, 0),
                Child = new LibraryLongLogo(),
            };
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
