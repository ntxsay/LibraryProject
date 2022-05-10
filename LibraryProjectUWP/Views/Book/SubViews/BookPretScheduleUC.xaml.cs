using LibraryProjectUWP.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Book.SubViews
{
    public sealed partial class BookPretScheduleUC : UserControl
    {
        public LivrePretDayCellVM ViewModel { get; private set; }
        public BookPretScheduleSubPage ParentPage { get; private set; }

        public BookPretScheduleUC()
        {
            this.InitializeComponent();
        }

        public BookPretScheduleUC(LivrePretDayCellVM _ViewModel, BookPretScheduleSubPage bookPretScheduleSubPage)
        {
            ParentPage = bookPretScheduleSubPage;
            ViewModel = _ViewModel;
            this.InitializeComponent();
        }

        private void UserControl_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down) && ParentPage.BookPretScheduleUCDateStart != null && ParentPage.BookPretScheduleUCDateStart != this)
            {
                ParentPage.BookPretScheduleUCDateEnd = this;
                ParentPage.SelectCellsDay();
            }
            else
            {
                ParentPage.BookPretScheduleUCDateStart = this;
            }
        }

        private void UserControl_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }
    }
}
