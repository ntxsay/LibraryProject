using RostalProjectUWP.Code;
using RostalProjectUWP.Code.Helpers;
using RostalProjectUWP.ViewModels;
using RostalProjectUWP.ViewModels.General;
using RostalProjectUWP.Views.Library;
using RostalProjectUWP.Views.Library.Manage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    public sealed partial class ManageLibraryPage : Page
    {
        private ManageContainerPage _parentPage;
        public ManageLibraryPageViewModel PageViewModel { get; set; } = new ManageLibraryPageViewModel();
        private BibliothequeVM ViewModel { get; set; }
        public EditMode Mode { get; set; }
        public string Title { get; set; }
        public string _actionButtonName;
        public ManageLibraryPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is ManageLibraryParametersVM parameters)
            {
                ViewModel = parameters.ViewModel;
                Mode = parameters.EditMode;
                _parentPage = parameters.ParentPage;
                Initialize(parameters.ViewModel, parameters.EditMode);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeFirstItem();
        }

        private void Initialize(BibliothequeVM viewModel, EditMode editMode)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                //ThemeListener.ThemeChanged += Listener_ThemeChanged;

                if (viewModel != null)
                {
                    ViewModel = viewModel;
                    Mode = editMode;
                    switch (Mode)
                    {
                        case EditMode.Create:
                            Title = "Ajouter une bibliothèque";
                            _actionButtonName = "Créer";
                            break;
                        case EditMode.Edit:
                            Title = $"Editer le livre {(!viewModel.Name.IsStringNullOrEmptyOrWhiteSpace() ? viewModel.Name : "sans nom")}";
                            _actionButtonName = "Mettre à jour";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }


        #region Navigation
        private void InitializeFirstItem()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                //PageViewModel.ErrorList.CollectionChanged += ErrorList_CollectionChanged;

                if (MyNavigationView.MenuItems[0] is Microsoft.UI.Xaml.Controls.NavigationViewItem first)
                {
                    MyNavigationView.SelectedItem = first;
                }

                this.NavigateToView(typeof(ManageLibraryGeneralPage), new ManageLibraryParentChildVM() { ViewModel = ViewModel, ParentPage = this });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }
        private void MyNavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (!(args.InvokedItemContainer is Microsoft.UI.Xaml.Controls.NavigationViewItem item))
                {
                    return;
                }

                string itemTag = item.Tag.ToString();
                if (itemTag.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return;
                }

                if (itemTag == PageViewModel.GeneralMenuItem.Tag)
                {
                    this.NavigateToView(typeof(ManageLibraryGeneralPage), new ManageLibraryParentChildVM() { ViewModel = ViewModel, ParentPage = this });
                }
                //else if (itemTag == PageViewModel.CategorieMenuItem.Tag)
                //{
                //    this.NavigateToView(typeof(ManageBookCategorie), new ManageBookParentChildVM() { ViewModel = ViewModel, ParentPage = this });
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        public void NavigateToView(Type page, object parameters)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                _ = FrameContainer.Navigate(page, parameters, new EntranceNavigationTransitionInfo());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }
        #endregion

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class ManageLibraryPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        public ItemTagContentVM LibraryManagementMenuItem => new ItemTagContentVM()
        {
            Text = "Gestionnaire de bibliothèque",
            Tag = "library-management"
        };

        public ItemTagContentVM GeneralMenuItem => new ItemTagContentVM()
        {
            Text = "Général",
            Tag = "general"
        };

        public ItemTagContentVM CategorieMenuItem => new ItemTagContentVM()
        {
            Text = "Catégories",
            Tag = "categorie-sous-categorie"
        };

        public ItemTagContentVM DescriptionMenuItem => new ItemTagContentVM()
        {
            Text = "Description",
            Tag = "description"
        };

        public ItemTagContentVM EditeursMenuItem => new ItemTagContentVM()
        {
            Text = "Editeurs",
            Tag = "editeurs"
        };

        private ObservableCollection<OperationStateVM> _ErrorList = new ObservableCollection<OperationStateVM>();
        public ObservableCollection<OperationStateVM> ErrorList
        {
            get => _ErrorList;
            set
            {
                if (_ErrorList != value)
                {
                    _ErrorList = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _MessageState = "Erreur(s) détectées, vous pouvez valider la fiche";
        public string MessageState
        {
            get => _MessageState;
            set
            {
                if (_MessageState != value)
                {
                    _MessageState = value;
                    OnPropertyChanged();
                }
            }
        }

        private SolidColorBrush _BrushColorState = new SolidColorBrush(Windows.UI.Colors.Green);
        public SolidColorBrush BrushColorState
        {
            get => _BrushColorState;
            set
            {
                if (_BrushColorState != value)
                {
                    _BrushColorState = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsProgressRingIndeterminate;
        public bool IsProgressRingIndeterminate
        {
            get => _IsProgressRingIndeterminate;
            set
            {
                if (_IsProgressRingIndeterminate != value)
                {
                    _IsProgressRingIndeterminate = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _DefaultBackgroundImageVisibility = Visibility.Collapsed;
        public Visibility DefaultBackgroundImageVisibility
        {
            get => _DefaultBackgroundImageVisibility;
            set
            {
                if (_DefaultBackgroundImageVisibility != value)
                {
                    _DefaultBackgroundImageVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _TopCloseButtonVisibility = Visibility.Visible;
        public Visibility TopCloseButtonVisibility
        {
            get => _TopCloseButtonVisibility;
            set
            {
                if (_TopCloseButtonVisibility != value)
                {
                    _TopCloseButtonVisibility = value;
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
