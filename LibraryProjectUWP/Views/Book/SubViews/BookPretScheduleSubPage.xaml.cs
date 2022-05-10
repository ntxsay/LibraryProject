using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views.Book.SubViews
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class BookPretScheduleSubPage : Page
    {
        public BookPretScheduleUC BookPretScheduleUCDateStart { get; internal set; }
        public BookPretScheduleUC BookPretScheduleUCDateEnd { get; internal set; }
        DateTime CurrentMonth { get; set; }
        public BookCollectionPage ParentPage { get; private set; }
        
        public BookPretScheduleSubPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is BookCollectionPage parameters)
            {
                ParentPage = parameters;
                //InitializeDataAsync(true);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var now = DateTime.Now;
                CurrentMonth = new DateTime(now.Year, now.Month, 1);
                DisplayDays();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DisplayDays()
        {
            try
            {
                if (VSWGridDays.Children.Any())
                {
                    VSWGridDays.Children.Clear();
                }

                int daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
                int dayOfWeek = Convert.ToInt32(CurrentMonth.DayOfWeek.ToString("d"));

                for (int i = 0; i < dayOfWeek; i++)
                {
                    Border border = new Border()
                    {
                        Width = GridDayNames.ColumnDefinitions[0].ActualWidth,
                        Height = GridDayNames.ColumnDefinitions[0].ActualWidth,
                        Background = new SolidColorBrush(Colors.Transparent),
                    };

                    VSWGridDays.Children.Add(border);
                }

                var now = DateTime.Now;
                for (int i = 1; i <= daysInMonth; i++)
                {
                    BookPretScheduleUC dayUC = new BookPretScheduleUC(new LivrePretDayCellVM()
                    {
                        Day = i,
                        Date = new DateTime(CurrentMonth.Year, CurrentMonth.Month, i),
                        DayColor = CurrentMonth.Year == now.Year && CurrentMonth.Month == now.Month &&  i == now.Day ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Transparent),
                    }, this)
                    {
                        Width = GridDayNames.ColumnDefinitions[0].ActualWidth - 2,
                        Height = GridDayNames.ColumnDefinitions[0].ActualWidth - 2,
                    };

                    VSWGridDays.Children.Add(dayUC);
                }

                TbkMonthName.Text = $"{DateTimeFormatInfo.CurrentInfo.GetMonthName(CurrentMonth.Month)} {CurrentMonth.Year}";
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                foreach (var item in VSWGridDays.Children)
                {
                    if (item is Border border)
                    {
                        border.Width = GridDayNames.ColumnDefinitions[0].ActualWidth;
                        border.Height = GridDayNames.ColumnDefinitions[0].ActualWidth;
                        border.Background = new SolidColorBrush(Colors.Transparent);
                    }
                    else if (item is BookPretScheduleUC dayUC)
                    {
                        dayUC.Width = GridDayNames.ColumnDefinitions[0].ActualWidth - 2;
                        dayUC.Height = GridDayNames.ColumnDefinitions[0].ActualWidth - 2;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void BtnPreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            CurrentMonth = CurrentMonth.AddMonths(-1);
            DisplayDays();
        }

        private void BtnToday_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            CurrentMonth = new DateTime(now.Year, now.Month, 1);
            DisplayDays();
        }

        private void BtnNextMonth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentMonth = CurrentMonth.AddMonths(1);
                DisplayDays();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SelectCellsDay()
        {
            try
            {
                var selectedDays = VSWGridDays.Children.Where(s => s is BookPretScheduleUC scheduleUC && DateTime.Compare(scheduleUC.ViewModel.Date, BookPretScheduleUCDateStart.ViewModel.Date) >= 0 && DateTime.Compare(scheduleUC.ViewModel.Date, BookPretScheduleUCDateEnd.ViewModel.Date) <= 0).Select(q => (BookPretScheduleUC)q).ToList();
                if (selectedDays != null && selectedDays.Count > 0)
                {

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
