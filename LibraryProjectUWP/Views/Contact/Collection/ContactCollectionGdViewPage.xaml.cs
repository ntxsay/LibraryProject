using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Contact;
using LibraryProjectUWP.ViewModels.General;
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
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Contact.Collection
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ContactCollectionGdViewPage : Page
    {
        public ContactCollectionGdViewPageVM ViewModelPage { get; set; } = new ContactCollectionGdViewPageVM();
        private ContactCollectionParentChildParamsVM _contactParameters;
        ContactCollectionCommonViewVM _commonView;
        public ContactCollectionGdViewPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is ContactCollectionParentChildParamsVM contactParameters)
            {
                _contactParameters = contactParameters;
                _commonView = new ContactCollectionCommonViewVM(this, contactParameters.ParentPage);
                ViewModelPage.ViewModelList = _contactParameters.ViewModelList?.ToList();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeModelList();
        }

        private void InitializeModelList()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _contactParameters.ParentPage.RefreshItemsGrouping();
                this.PivotItems.SelectedIndex = _contactParameters.ParentPage.ViewModelPage.SelectedPivotIndex;
                this.PivotItems.SelectionChanged += PivotItems_SelectionChanged;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void PivotItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is Pivot pivot)
                {
                    _contactParameters.ParentPage.ViewModelPage.SelectedItems = new List<ContactVM>();
                    _contactParameters.ParentPage.ViewModelPage.SelectedPivotIndex = pivot.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void GridViewItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is GridView gridView)
                {
                    _contactParameters.ParentPage.ViewModelPage.SelectedItems = gridView.SelectedItems.Cast<ContactVM>().ToList();
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        /// <summary>
        /// Ouvre la liste des livre de la bibliothèque
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (sender is Viewbox viewbox && viewbox.Tag is ContactVM viewModel)
                {
                    VisualViewHelpers.MainControlsUI mainControlsUI = new VisualViewHelpers.MainControlsUI();
                    var mainPage = mainControlsUI.GetMainPage;
                    if (mainPage != null)
                    {
                        mainPage.BookCollectionNavigationAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Image imageCtrl && imageCtrl.Tag is string path)
                {
                    var bitmapImage = await Files.BitmapImageFromFileAsync(path);
                    imageCtrl.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        #region Groups
        public void GroupItemsByNone()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, _contactParameters.ParentPage.ViewModelPage.OrderedBy, _contactParameters.ParentPage.ViewModelPage.SortedBy).Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos contacts").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Any())
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, ContactVM>>(GroupingItems);
                    _contactParameters.ParentPage.ViewModelPage.GroupedBy = ContactGroupVM.GroupBy.None;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        public void GroupItemsByLetterNomNaissance()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, _contactParameters.ParentPage.ViewModelPage.OrderedBy, _contactParameters.ParentPage.ViewModelPage.SortedBy).Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.NomNaissance.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, ContactVM>>(GroupingItems);
                    _contactParameters.ParentPage.ViewModelPage.GroupedBy = ContactGroupVM.GroupBy.LetterNomNaissance;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupItemsByLetterPrenom()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, _contactParameters.ParentPage.ViewModelPage.OrderedBy, _contactParameters.ParentPage.ViewModelPage.SortedBy).Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.Prenom.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, ContactVM>>(GroupingItems);
                    _contactParameters.ParentPage.ViewModelPage.GroupedBy = ContactGroupVM.GroupBy.LetterPrenom;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void GroupByCreationYear()
        {
            try
            {
                if (ViewModelPage.ViewModelList == null || !ViewModelPage.ViewModelList.Any())
                {
                    return;
                }

                var GroupingItems = this.OrderItems(ViewModelPage.ViewModelList, _contactParameters.ParentPage.ViewModelPage.OrderedBy, _contactParameters.ParentPage.ViewModelPage.SortedBy).Where(w => !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(s => s.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => s);
                if (GroupingItems != null && GroupingItems.Count() > 0)
                {
                    ViewModelPage.GroupedRelatedViewModel.Collection = new ObservableCollection<IGrouping<string, ContactVM>>(GroupingItems);
                    _contactParameters.ParentPage.ViewModelPage.GroupedBy = ContactGroupVM.GroupBy.CreationYear;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Group-Orders
        private IEnumerable<ContactVM> OrderItems(IEnumerable<ContactVM> Collection, ContactGroupVM.OrderBy OrderBy = ContactGroupVM.OrderBy.Croissant, ContactGroupVM.SortBy SortBy = ContactGroupVM.SortBy.Prenom)
        {
            try
            {
                if (Collection == null || Collection.Count() == 0)
                {
                    return null;
                }

                if (SortBy == ContactGroupVM.SortBy.Prenom)
                {
                    if (OrderBy == ContactGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.Prenom);
                    }
                    else if (OrderBy == ContactGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.Prenom);
                    }
                }
                else if (SortBy == ContactGroupVM.SortBy.NomNaissance)
                {
                    if (OrderBy == ContactGroupVM.OrderBy.Croissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.NomNaissance);
                    }
                    else if (OrderBy == ContactGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.Where(w => w != null && !w.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() && !w.Prenom.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.NomNaissance);
                    }
                }
                else if (SortBy == ContactGroupVM.SortBy.DateCreation)
                {
                    if (OrderBy == ContactGroupVM.OrderBy.Croissant)
                    {
                        return Collection.OrderBy(o => o.DateAjout);
                    }
                    else if (OrderBy == ContactGroupVM.OrderBy.DCroissant)
                    {
                        return Collection.OrderByDescending(o => o.DateAjout);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return Enumerable.Empty<ContactVM>();
            }
        }

        #endregion

        private async void ChangeJaquetteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is ContactVM viewModel)
                {
                    //EsLibrary esLibrary = new EsLibrary();
                    //var result = await esLibrary.ChangeLibraryItemJaquetteAsync(viewModel);
                    //if (!result.IsSuccess)
                    //{
                    //    return;
                    //}

                    //viewModel.JaquettePath = result.Result?.ToString() ?? "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
                    //var image = GetSelectedThumbnailImage(viewModel);
                    //if (image != null)
                    //{
                    //    var bitmapImage = await Files.BitmapImageFromFileAsync(viewModel.JaquettePath);
                    //    image.Source = bitmapImage;
                    //}
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ExportLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is ContactVM viewModel)
                {
                    //await _commonView.ExportLibraryAsync(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DeleteLibraryXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is ContactVM viewModel)
                {
                    //_commonView.DeleteLibrary(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        private void EditLibraryInfosXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is ContactVM viewModel)
                {
                    //_commonView.EditLibrary(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        public void SearchViewModel(ContactVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return;
                }

                foreach (var pivotItem in PivotItems.Items)
                {
                    if (pivotItem is IGrouping<string, ContactVM> group && group.Any(f => f == viewModel))
                    {
                        if (this.PivotItems.SelectedItem != pivotItem)
                        {
                            this.PivotItems.SelectedItem = pivotItem;
                        }

                        var _container = this.PivotItems.ContainerFromItem(pivotItem);
                        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
                        while (gridView != null && gridView.Name != "GridViewItems")
                        {
                            gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
                            if (gridView == null)
                            {
                                return;
                            }
                            else
                            {
                                if (gridView.Name == "GridViewItems")
                                {
                                    break;
                                }
                            }
                        }

                        if (gridView != null)
                        {
                            foreach (var gridViewItem in gridView.Items)
                            {
                                if (gridViewItem is ContactVM _viewModel && _viewModel == viewModel)
                                {
                                    if (gridView.SelectedItem != gridViewItem)
                                    {
                                        gridView.SelectedItem = gridViewItem;
                                    }

                                    var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                                    OpenFlyoutSearchedItem(_gridViewItemContainer);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void OpenFlyoutSearchedItem(DependencyObject _gridViewItemContainer)
        {
            try
            {
                if (_gridViewItemContainer == null)
                {
                    return;
                }

                var grid = VisualViewHelpers.FindVisualChild<Grid>(_gridViewItemContainer);
                if (grid != null)
                {
                    Grid gridActions = grid.Children.FirstOrDefault(f => f is Grid _gridActions && _gridActions.Name == "GridActions") as Grid;
                    if (gridActions != null)
                    {
                        Button buttonActions = gridActions.Children.FirstOrDefault(f => f is Button _buttonActions && _buttonActions.Name == "BtnActions") as Button;
                        if (buttonActions != null)
                        {
                            buttonActions.Flyout.ShowAt(buttonActions, new FlyoutShowOptions()
                            {
                                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                                ShowMode = FlyoutShowMode.Auto
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private Image GetSelectedThumbnailImage(ContactVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    return null;
                }

                if (this.PivotItems.SelectedItem != null)
                {
                    if (this.PivotItems.SelectedItem is IGrouping<string, ContactVM> group && group.Any(f => f == viewModel))
                    {

                        var _container = this.PivotItems.ContainerFromItem(this.PivotItems.SelectedItem);
                        var gridView = VisualViewHelpers.FindVisualChild<GridView>(_container);
                        while (gridView != null && gridView.Name != "GridViewItems")
                        {
                            gridView = VisualViewHelpers.FindVisualChild<GridView>(gridView);
                            if (gridView == null)
                            {
                                return null;
                            }
                            else
                            {
                                if (gridView.Name == "GridViewItems")
                                {
                                    break;
                                }
                            }
                        }

                        if (gridView != null)
                        {
                            foreach (var gridViewItem in gridView.Items)
                            {
                                if (gridViewItem is ContactVM _viewModel && _viewModel == viewModel)
                                {
                                    if (gridView.SelectedItem != gridViewItem)
                                    {
                                        gridView.SelectedItem = gridViewItem;
                                    }

                                    var _gridViewItemContainer = gridView.ContainerFromItem(gridViewItem);
                                    return SelectImageFromContainer(_gridViewItemContainer);
                                }
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        private Image SelectImageFromContainer(DependencyObject _gridViewItemContainer)
        {
            try
            {
                if (_gridViewItemContainer == null)
                {
                    return null;
                }

                var grid = VisualViewHelpers.FindVisualChild<Grid>(_gridViewItemContainer);
                if (grid != null)
                {
                    Viewbox viewboxThumbnailContainer = grid.Children.FirstOrDefault(f => f is Viewbox _viewboxThumbnailContainer && _viewboxThumbnailContainer.Name == "ViewboxSimpleThumnailDatatemplate") as Viewbox;
                    if (viewboxThumbnailContainer != null)
                    {
                        Border border = viewboxThumbnailContainer.Child as Border;
                        if (border != null)
                        {
                            Grid gridImageContainer = border.Child as Grid;
                            if (gridImageContainer != null)
                            {
                                Image image = gridImageContainer.Children.FirstOrDefault(f => f is Image _image) as Image;
                                return image;
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        private void MenuFlyoutSubItem_Categorie_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is MenuFlyoutSubItem subItem && subItem.Tag is ContactVM viewModel)
                {
                    //_commonView.PopulateCategoriesMenuItems(subItem, viewModel);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class ContactCollectionGdViewPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ContactGroupVM _GroupedRelatedViewModel = new ContactGroupVM();
        public ContactGroupVM GroupedRelatedViewModel
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

        private bool _IsSplitViewOpen;
        public bool IsSplitViewOpen
        {
            get => this._IsSplitViewOpen;
            set
            {
                if (_IsSplitViewOpen != value)
                {
                    this._IsSplitViewOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private UserControl _SplitViewContent;
        public UserControl SplitViewContent
        {
            get => this._SplitViewContent;
            set
            {
                if (_SplitViewContent != value)
                {
                    this._SplitViewContent = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ContactGroupVM.GroupBy _GroupedBy = ContactGroupVM.GroupBy.None;
        public ContactGroupVM.GroupBy GroupedBy
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

        private List<ContactVM> _ViewModelList;
        public List<ContactVM> ViewModelList
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
