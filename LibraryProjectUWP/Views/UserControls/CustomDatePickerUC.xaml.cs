using LibraryProjectUWP.Code.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
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

                if (this.Jour != value)
                {
                    this.Jour = value;
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

                if (this.Mois != value)
                {
                    this.Mois = value;
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

                if (this.Annee != value)
                {
                    this.Annee = value;
                }
            }
        }

        public string GetDate(out string messageError, out DateTime? exactDate)
        {
            try
            {
                if (!Month.IsStringNullOrEmptyOrWhiteSpace() && Month != DatesHelpers.NoAnswer &&
                    Year.IsStringNullOrEmptyOrWhiteSpace() || Year == DatesHelpers.NoAnswer)
                {
                    messageError = $"Vous devez spécifier l'année avant de valider le mois.";
                    exactDate = null;
                    return "--/--/--/";
                }
                else if (!Day.IsStringNullOrEmptyOrWhiteSpace() && Day != DatesHelpers.NoAnswer &&
                    Month.IsStringNullOrEmptyOrWhiteSpace() || Month == DatesHelpers.NoAnswer)
                {
                    messageError = $"Vous devez spécifier le mois avant de valider le jour.";
                    exactDate = null;
                    return "--/--/--/";
                }
                else
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
                            messageError = $"La date renseignée n'est pas valide.";
                            exactDate = null;
                            return "--/--/--/";
                        }
                        else
                        {
                            messageError = null;
                            exactDate = date;
                            return date.ToString("dd/MM/yyyy");
                        }
                    }
                    else if (!Month.IsStringNullOrEmptyOrWhiteSpace() && Month != DatesHelpers.NoAnswer &&
                            !Year.IsStringNullOrEmptyOrWhiteSpace() && Year != DatesHelpers.NoAnswer)
                    {
                        var month = DatesHelpers.ChooseMonth().ToList().IndexOf(Month);
                        var year = Convert.ToInt32(Year);
                        var result = $"--/{month:00}/{year:0000}";

                        messageError = null;
                        exactDate = null;
                        return result;
                    }
                    else if (!Year.IsStringNullOrEmptyOrWhiteSpace() && Year != DatesHelpers.NoAnswer)
                    {
                        var result = $"--/--/{Year}";

                        messageError = null;
                        exactDate = null;
                        return result;
                    }
                }

                messageError = null;
                exactDate = null;
                return null;
            }
            catch (Exception)
            {
                messageError = $"Une erreur inconnue s'est produite.";
                exactDate = null;
                return "--/--/--/";
            }
        }

        #region Jour
        public string Jour
        {
            get { return (string)GetValue(JourProperty); }
            set { SetValue(JourProperty, value); }
        }

        public static readonly DependencyProperty JourProperty = DependencyProperty.Register(nameof(Jour), typeof(string),
                                                                typeof(CustomDatePickerUC), new PropertyMetadata(null, new PropertyChangedCallback(OnJourChanged)));

        private static void OnJourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomDatePickerUC parent && e.NewValue is string value)
            {
                parent.Day = value.Trim();
            }
        }
        #endregion

        #region Mois
        public string Mois
        {
            get { return (string)GetValue(MoisProperty); }
            set { SetValue(MoisProperty, value); }
        }

        public static readonly DependencyProperty MoisProperty = DependencyProperty.Register(nameof(Mois), typeof(string),
                                                                typeof(CustomDatePickerUC), new PropertyMetadata(null, new PropertyChangedCallback(OnMoisChanged)));

        private static void OnMoisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomDatePickerUC parent && e.NewValue is string value)
            {
                parent.Month = value.Trim();
            }
        }
        #endregion

        #region Annee
        public string Annee
        {
            get { return (string)GetValue(AnneeProperty); }
            set { SetValue(AnneeProperty, value); }
        }

        public static readonly DependencyProperty AnneeProperty = DependencyProperty.Register(nameof(Annee), typeof(string),
                                                                typeof(CustomDatePickerUC), new PropertyMetadata(null, new PropertyChangedCallback(OnAnneeChanged)));

        private static void OnAnneeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomDatePickerUC parent && e.NewValue is string value)
            {
                parent.Year = value.Trim();
            }
        }
        #endregion

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
