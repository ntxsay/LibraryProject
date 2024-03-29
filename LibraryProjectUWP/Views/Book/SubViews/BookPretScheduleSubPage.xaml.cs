﻿using LibraryProjectUWP.Code.Services;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.Views.PrincipalPages;
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
                CurrentMonth = new DateTime(now.Year, now.Month, 1, 23, 59, 59);
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
                int rowIndex = 0, columnIndex = 0;
                if (GridDayCells.Children.Any())
                {
                    GridDayCells.Children.Clear();
                }

                int daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
                int dayOfWeek = Convert.ToInt32(CurrentMonth.DayOfWeek.ToString("d"));

                for (int i = 0; i < dayOfWeek; i++)
                {
                    Border border = new Border()
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                    };

                    GridDayCells.Children.Add(border);
                    border.SetValue(Grid.RowProperty, rowIndex);
                    border.SetValue(Grid.ColumnProperty, columnIndex);

                    if (columnIndex == 6)
                    {
                        columnIndex = 0;
                        rowIndex++;
                    }
                    else
                    {
                        columnIndex++;
                    }
                }

                var prets = DbServices.Book.AllPretsVMAsync().GetAwaiter().GetResult()?.ToList();

                var now = DateTime.Now;
                for (int i = 1; i <= daysInMonth; i++)
                {
                    var date = new DateTime(CurrentMonth.Year, CurrentMonth.Month, i);
                    var viewModel = new LivrePretDayCellVM()
                    {
                        Day = i,
                        Date = date,
                        DayColor = CurrentMonth.Year == now.Year && CurrentMonth.Month == now.Month && i == now.Day ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Transparent),
                    };

                    if (prets != null && prets.Count > 0)
                    {
                        NameToColorService nameToColorService = new NameToColorService();
                        prets.ForEach(p => p.EventColor = nameToColorService.GetColor(p));


                        List<LivrePretVM> nPrets = new List<LivrePretVM>();
                        foreach(var item in prets)
                        {
                            var book = DbServices.Book.SingleAsync(item.Exemplary.IdBook, null).GetAwaiter().GetResult();
                            item.IdBook = book.Id;
                            item.BookTitle = book.MainTitle;

                            var itemDayDateStart = new DateTime(item.DatePret.Year, item.DatePret.Month, item.DatePret.Day);
                            var itemDayDateStartComparaison = DateTime.Compare(date, itemDayDateStart);
                            if (item.DateRemise.HasValue)
                            {
                                var itemDayDateEnd = new DateTime(item.DateRemise.Value.Year, item.DateRemise.Value.Month, item.DateRemise.Value.Day);
                                var itemDayDateEndComparaison = DateTime.Compare(date, itemDayDateEnd);
                                if (itemDayDateStartComparaison >= 0 && itemDayDateEndComparaison <= 0)
                                {
                                    nPrets.Add(item);
                                }
                            }
                            else
                            {
                                if (itemDayDateStartComparaison == 0)
                                {
                                    nPrets.Add(item);
                                }
                            }
                        }

                        if (nPrets != null && nPrets.Count > 0)
                        {
                            viewModel.Prets = new ObservableCollection<LivrePretVM>(nPrets);
                        }
                    }

                    BookPretScheduleUC dayUC = new BookPretScheduleUC(viewModel, this);

                    GridDayCells.Children.Add(dayUC);
                    dayUC.SetValue(Grid.RowProperty, rowIndex);
                    dayUC.SetValue(Grid.ColumnProperty, columnIndex);

                    if (columnIndex == 6)
                    {
                        columnIndex = 0;
                        rowIndex++;
                    }
                    else
                    {
                        columnIndex++;
                    }
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
            //try
            //{
            //    foreach (var item in GridDayCells.Children)
            //    {
            //        if (item is Border border)
            //        {
            //            border.Width = GridDayNames.ColumnDefinitions[0].ActualWidth;
            //            border.Height = GridDayNames.ColumnDefinitions[0].ActualWidth;
            //            border.Background = new SolidColorBrush(Colors.Transparent);
            //        }
            //        else if (item is BookPretScheduleUC dayUC)
            //        {
            //            dayUC.Width = GridDayNames.ColumnDefinitions[0].ActualWidth - 2;
            //            dayUC.Height = GridDayNames.ColumnDefinitions[0].ActualWidth - 2;
            //        }
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
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
                var selectedDays = GridDayCells.Children.Where(s => s is BookPretScheduleUC scheduleUC && DateTime.Compare(scheduleUC.ViewModel.Date, BookPretScheduleUCDateStart.ViewModel.Date) >= 0 && DateTime.Compare(scheduleUC.ViewModel.Date, BookPretScheduleUCDateEnd.ViewModel.Date) <= 0).Select(q => (BookPretScheduleUC)q).ToList();
                if (selectedDays != null && selectedDays.Count > 0)
                {

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.OpenBookCollection(ParentPage.Parameters.ParentLibrary);
        }
    }
}
