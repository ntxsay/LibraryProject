using LibraryProjectUWP.ViewModels.Book;
using System;
using System.Collections.Generic;
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

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class DeleteBookCD : ContentDialog
    {
        private List<LivreVM> ViewModelList { get; set; } = new List<LivreVM>();
        public DeleteBookCD()
        {
            this.InitializeComponent();
        }

        public DeleteBookCD(IEnumerable<LivreVM> _viewModelList)
        {
            this.InitializeComponent();
            ViewModelList = new List<LivreVM>(_viewModelList);
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                
                if (ViewModelList.Count == 1)
                {
                    this.Title = "Supprimer un livre";
                    Run run1 = new Run()
                    {
                        Text = "Êtes-vous sûr de vouloir supprimer le livre « ",
                    };

                    tbkName.Inlines.Add(run1);

                    Run run2 = new Run()
                    {
                        Text = $"{ViewModelList[0].MainTitle}",
                        FontWeight = FontWeights.Medium,
                    };

                    tbkName.Inlines.Add(run2);

                    Run run3 = new Run()
                    {
                        Text = $" » ?",
                    };

                    tbkName.Inlines.Add(run3);
                }
                else if (ViewModelList.Count > 1)
                {
                    this.Title = "Supprimer des livres";
                    Run run1 = new Run()
                    {
                        Text = "Êtes-vous sûr de vouloir supprimer « ",
                    };

                    tbkName.Inlines.Add(run1);

                    Run run2 = new Run()
                    {
                        Text = $"{ViewModelList.Count} livres",
                        FontWeight = FontWeights.Medium,
                    };

                    tbkName.Inlines.Add(run2);

                    Run run3 = new Run()
                    {
                        Text = $" » ?",
                    };

                    tbkName.Inlines.Add(run3);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
