using LibraryProjectUWP.Code.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.UserControls
{
    public sealed partial class CustomDatePickerUC : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public IEnumerable<string> chooseDays = DatesHelpers.ChooseDays();
        public IEnumerable<string> chooseMonths = DatesHelpers.ChooseMonth();
        public List<string> chooseYear = new List<string>();

        public CustomDatePickerUC()
        {
            this.InitializeComponent();
            chooseYear.Add(DatesHelpers.NoAnswer);
            chooseYear.AddRange(DatesHelpers.ChooseYear());
        }

        private string _Day;
        public string Day
        {
            get => this._Day;
            set
            {
                if (this._Day != value)
                {
                    this._Day = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Month;
        public string Month
        {
            get => this._Month;
            set
            {
                if (this._Month != value)
                {
                    this._Month = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Year;
        public string Year
        {
            get => this._Year;
            set
            {
                if (this._Year != value)
                {
                    this._Year = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string GetDate(out DateTime? exactDate)
        {
            try
            {
                if (!Day.IsStringNullOrEmptyOrWhiteSpace() && Day != DatesHelpers.NoAnswer &&
                    !Month.IsStringNullOrEmptyOrWhiteSpace() && Month != DatesHelpers.NoAnswer &&
                    !Year.IsStringNullOrEmptyOrWhiteSpace() && Year != DatesHelpers.NoAnswer)
                {
                    var day = Convert.ToInt32(Day);
                    var month = DatesHelpers.ChooseMonth().ToList().IndexOf(Month);
                    var year = Convert.ToInt32(Year);
                    var isDateCorrect = DateTime.TryParseExact($"{day:00}/{month:00}/{year:0000}", "dd/MM/yyyy", new CultureInfo("fr-FR"), DateTimeStyles.AssumeLocal, out DateTime date);
                    if (!isDateCorrect)
                    {
                        exactDate = null;
                        return null;
                    }
                    else
                    {
                        exactDate = date;
                        return date.ToString("dd/MM/yyyy");
                    }
                }
                else if (!Month.IsStringNullOrEmptyOrWhiteSpace() && Month != DatesHelpers.NoAnswer &&
                        !Year.IsStringNullOrEmptyOrWhiteSpace() && Year != DatesHelpers.NoAnswer)
                {
                    var month = DatesHelpers.ChooseMonth().ToList().IndexOf(Month);
                    var year = Convert.ToInt32(Year);
                    var result = $"{month:00}/{year:0000}";
                    exactDate = null;
                    return result;
                }
                else if (!Year.IsStringNullOrEmptyOrWhiteSpace() && Year != DatesHelpers.NoAnswer)
                {
                    var result = $"{Year}";
                    exactDate = null;
                    return result;
                }

                exactDate = null;
                return null;
            }
            catch (Exception)
            {
                exactDate = null;
                return null;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
