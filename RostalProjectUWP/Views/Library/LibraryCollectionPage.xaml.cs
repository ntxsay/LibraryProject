using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.Code.Services.Db;
using RostalProjectUWP.Code.Services.Logging;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using RostalProjectUWP.ViewModels.Library;
using RostalProjectUWP.Views.Library.Collection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RostalProjectUWP.Views.Library
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class LibraryCollectionPage : Page
    {
        public LibraryCollectionPageVM ViewModelPage { get; set; } = new LibraryCollectionPageVM();
        public LibraryCollectionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var libraryList = await DbServices.Library.AllVMAsync();
                ViewModelPage.ViewModelList = libraryList?.ToList();
                ViewModelPage.SearchingLibraryVisibility = Visibility.Collapsed;
                NavigateToView(typeof(LibraryCollectionGridViewPage), new LibraryCollectionParentChildParamsVM() { ParentPage = this, ViewModelList = ViewModelPage.ViewModelList, });
                ViewModelPage.IsGridView = true;
                ViewModelPage.IsDataGridView = false;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #region Navigation
        public void NavigateToView(Type page, object parameters)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _ = FramePartialView.Navigate(page, parameters, new EntranceNavigationTransitionInfo());
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewCollectionXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (!(FramePartialView.Content is LibraryCollectionGridViewPage))
                {
                    NavigateToView(typeof(LibraryCollectionGridViewPage), new LibraryCollectionParentChildParamsVM() { ParentPage = this, ViewModelList = ViewModelPage.ViewModelList, });
                }

                ViewModelPage.IsGridView = true;
                ViewModelPage.IsDataGridView = false;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void DataGridViewCollectionXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (!(FramePartialView.Content is LibraryCollectionDataGridViewPage))
                {
                    NavigateToView(typeof(LibraryCollectionDataGridViewPage), new LibraryCollectionParentChildParamsVM() { ParentPage = this, ViewModelList = ViewModelPage.ViewModelList, });
                }

                ViewModelPage.IsGridView = false;
                ViewModelPage.IsDataGridView = true;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        private void GroupByLetterXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FramePartialView.Content is LibraryCollectionGridViewPage libraryCollectionGridViewPage)
                {
                    libraryCollectionGridViewPage.GroupItemsByAlphabetic();
                }
                else if (FramePartialView.Content is LibraryCollectionDataGridViewPage libraryCollectionDataGridViewPage)
                {
                    libraryCollectionDataGridViewPage.GroupItemsByAlphabetic();
                }

                ViewModelPage.CountSelectedItems = 0;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GroupByCreationYearXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FramePartialView.Content is LibraryCollectionGridViewPage libraryCollectionGridViewPage)
                {
                    libraryCollectionGridViewPage.GroupByCreationYear();
                }
                else if (FramePartialView.Content is LibraryCollectionDataGridViewPage libraryCollectionDataGridViewPage)
                {
                    libraryCollectionDataGridViewPage.GroupByCreationYear();
                }

                ViewModelPage.CountSelectedItems = 0;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void GroupByNoneXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (FramePartialView.Content is LibraryCollectionGridViewPage libraryCollectionGridViewPage)
                {
                    libraryCollectionGridViewPage.GroupItemsByNone();
                }
                else if (FramePartialView.Content is LibraryCollectionDataGridViewPage libraryCollectionDataGridViewPage)
                {
                    libraryCollectionDataGridViewPage.GroupItemsByNone();
                }

                ViewModelPage.CountSelectedItems = 0;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        //private void RefreshItemsGrouping()
        //{
        //    MethodBase m = MethodBase.GetCurrentMethod();
        //    try
        //    {
        //        if (ViewModelPage.GroupedRelatedViewModel.IsGroupedByNone)
        //        {
        //            this.GroupItemsByNone();
        //        }
        //        else if (this.GroupedRelatedViewModel.IsGroupedByDateDebutDiffusionYear)
        //        {
        //            this.GroupByDebutDiffusionYear();
        //        }
        //        else if (this.GroupedRelatedViewModel.IsGroupedByLetter)
        //        {
        //            this.GroupItemsByAlphabetic();
        //        }
        //        else if (this.GroupedRelatedViewModel.IsGroupedByPays)
        //        {
        //            this.GroupSocietysByPaysProduction();
        //        }
        //        else if (this.GroupedRelatedViewModel.IsGroupedByGenre)
        //        {
        //            this.GroupByGenre();
        //        }
        //        else if (this.GroupedRelatedViewModel.IsGroupedBySeason)
        //        {
        //            this.GroupBySeason();
        //        }
        //        else if (this.GroupedRelatedViewModel.IsGroupedByStudio)
        //        {
        //            this.GroupByStudio();
        //        }
        //        else if (this.GroupedRelatedViewModel.IsGroupedByEditeur)
        //        {
        //            this.GroupByEditeur();
        //        }
        //        else
        //        {
        //            this.GroupItemsByNone();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.Log(ex, m);
        //        return;
        //    }
        //}
    }

    public class LibraryCollectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private LibraryGroupVM _GroupedRelatedViewModel = new LibraryGroupVM();
        public LibraryGroupVM GroupedRelatedViewModel
        {
            get => this._GroupedRelatedViewModel;
            set
            {
                if (this._GroupedRelatedViewModel != value)
                {
                    this._GroupedRelatedViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private LibraryGroupVM.GroupBy _GroupedBy = LibraryGroupVM.GroupBy.None;
        public LibraryGroupVM.GroupBy GroupedBy
        {
            get => this._GroupedBy;
            set
            {
                if (this._GroupedBy != value)
                {
                    this._GroupedBy = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsDataGridView;
        public bool IsDataGridView
        {
            get => this._IsDataGridView;
            set
            {
                if (_IsDataGridView != value)
                {
                    this._IsDataGridView = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsGridView;
        public bool IsGridView
        {
            get => this._IsGridView;
            set
            {
                if (_IsGridView != value)
                {
                    this._IsGridView = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private double _loadItemPurcentValue;

        public double LoadItemPurcentValue
        {
            get => this._loadItemPurcentValue;
            set
            {
                if (_loadItemPurcentValue != value)
                {
                    this._loadItemPurcentValue = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _SearchingLibraryVisibility = Visibility.Visible;
        public Visibility SearchingLibraryVisibility
        {
            get => this._SearchingLibraryVisibility;
            set
            {
                if (_SearchingLibraryVisibility != value)
                {
                    this._SearchingLibraryVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _countItems;
        public int CountItems
        {
            get => this._countItems;
            set
            {
                if (_countItems != value)
                {
                    this._countItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _countSelectedItems;
        public int CountSelectedItems
        {
            get => this._countSelectedItems;
            set
            {
                if (_countSelectedItems != value)
                {
                    this._countSelectedItems = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private List<BibliothequeVM> _ViewModelList;
        public List<BibliothequeVM> ViewModelList
        {
            get => this._ViewModelList;
            set
            {
                if (_ViewModelList != value)
                {
                    this._ViewModelList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
