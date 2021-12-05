using RostalProjectUWP.Code;
using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.Code.Services.Db;
using RostalProjectUWP.Models.Local;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using RostalProjectUWP.Views;
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

namespace RostalProjectUWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModelPage { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            ViewModelPage = new MainPageViewModel(this);
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

                if (itemTag == "AddNewElement")
                {
                    await AddElementNavigationAsync();
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
                Type view = Assembly.GetExecutingAssembly().GetType($"RostalProjectUWP.Views.{clickedView}");

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
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
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
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
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
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }
        #endregion
        #region Navigation-Method

        private async Task AddElementNavigationAsync()
        {
            try
            {
                var dialog = new NewElementCD();

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (dialog.ManagePage == ManagePage.Library)
                    {
                        var libraryList = await DbServices.AllVMAsync<Tlibrary, BibliothequeVM>();

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
            catch (Exception)
            {

                throw;
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
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
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
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return false;
            }
        }

        //public bool OpenCollection(IEnumerable<AnimeVM> viewModelList)
        //{
        //    MethodBase m = MethodBase.GetCurrentMethod();
        //    try
        //    {
        //        if (viewModelList == null)
        //        {
        //            Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : Le ViewModel est null.");
        //            return false;
        //        }

        //        #region Header
        //        Run runTitle = new Run()
        //        {
        //            Text = "Les animes",
        //            FontWeight = FontWeights.Medium,
        //        };

        //        TextBlock header = new TextBlock();
        //        header.Inlines.Add(runTitle);
        //        #endregion

        //        object settingViewDisplay = settingsServices.GetSetting(SettingsServices.Names.Display.ViewDisplayMode);
        //        if (settingViewDisplay is string stringDisplayMode)
        //        {
        //            _displayViewModeUI.UpdateWindowTitle(settingsServices);
        //            if (stringDisplayMode == SettingsServices.Values.Display.OnePageView)
        //            {
        //                MainPage mainPage = GetMainPage;
        //                if (mainPage == null)
        //                {
        //                    Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : mainPage est null.");
        //                    return false;
        //                }
        //                return _navigationUI.NavigateToView(NavigationUI.Path.Anime.AnimeList, viewModelList);
        //            }
        //            else if (stringDisplayMode == SettingsServices.Values.Display.TabView)
        //            {
        //                TabsUI tabs = new TabsUI("anime-local-collection", header, new AnimeCollectionPage(viewModelList),
        //                    new Microsoft.UI.Xaml.Controls.SymbolIconSource()
        //                    {
        //                        Symbol = Symbol.List
        //                    }, true);
        //                tabs.OpenTab();
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
        //        return false;
        //    }
        //}
        #endregion
    }

    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private readonly MainPage _mainPage;
        public MainPageViewModel()
        {

        }

        public MainPageViewModel(MainPage mainPage)
        {
            _mainPage = mainPage;
        }


        //public string AnimeList => NavigationUI.Path.Anime.AnimeList;
        //public string AnimeNew => NavigationUI.Path.Anime.NewAnime;
        //public string AnimeDashBoard => NavigationUI.Path.Anime.DashBoard;



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
