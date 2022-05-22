using LibraryProjectUWP.ViewModels;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Boîte de dialogue de contenu, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryProjectUWP.Views
{
    public sealed partial class CheckModificationsStateCD : ContentDialog
    {
        private object ViewModel { get; set; }
        private ObservableCollection<PropertiesChangedVM> ChangedProperties { get; set; } = new ObservableCollection<PropertiesChangedVM>();
        public CheckModificationsStateCD()
        {
            this.InitializeComponent();
        }

        public CheckModificationsStateCD(object _viewModel, IEnumerable<PropertiesChangedVM> _changedProperties)
        {
            ViewModel = _viewModel;
            ChangedProperties = new ObservableCollection<PropertiesChangedVM>(_changedProperties);
            this.InitializeComponent();
            InitializeActionInfos();
        }

        private void InitializeActionInfos()
        {
            try
            {
                tbkName.Inlines.Clear();

                Run run1 = new Run()
                {
                    Text = $"Souhaitez-vous enregistrer les modifications apportées ",
                };
                tbkName.Inlines.Add(run1);

                if (ViewModel is LivreVM livreVM)
                {
                    Run run2 = new Run()
                    {
                        Text = $"au livre « ",
                    };
                    tbkName.Inlines.Add(run2);

                    Run run3 = new Run()
                    {
                        Text = livreVM.MainTitle ?? "nouveau livre",
                        FontWeight = FontWeights.SemiBold,
                    };
                    tbkName.Inlines.Add(run3);
                }
                else if (ViewModel is BibliothequeVM bibliothequeVM)
                {
                    Run run2 = new Run()
                    {
                        Text = $"à la bibliothèque « ",
                    };
                    tbkName.Inlines.Add(run2);

                    Run run3 = new Run()
                    {
                        Text = bibliothequeVM.Name ?? "nouvelle bibliothèque",
                        FontWeight = FontWeights.SemiBold,
                    };
                    tbkName.Inlines.Add(run3);
                }

                Run run4 = new Run()
                {
                    Text = $" » ?",
                };
                tbkName.Inlines.Add(run4);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
