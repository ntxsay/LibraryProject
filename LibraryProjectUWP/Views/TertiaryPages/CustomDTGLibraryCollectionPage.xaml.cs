using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.General;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.Book.SubViews;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace LibraryProjectUWP.Views.UserControls
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class CustomDTGLibraryCollectionPage : Page
    {
        public Type Type { get; set; }
        public CustomDTGLibraryCollectionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is IntermediatePageToTertiaryPageDriverVM<LivreVM> parameters)
            {
                Type = typeof(LivreVM);
                InitializeData(parameters.Tables);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //DeleteBookXUiCmd = null;
            //GC.Collect();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            DataGridItems.ItemsSource = Enumerable.Empty<LivreVM>();
        }

        private void InitializeData<T>(IEnumerable<T> data) where T : class
        {
            try
            {
                if (DataGridItems.ItemsSource != null)
                {
                    return;
                }
                if (Type == typeof(LivreVM))
                {
                    ////Id
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "Id",
                    //    Binding = new Binding { Path = new PropertyPath("Id"), Mode = BindingMode.OneWay },
                    //});

                    //Titre
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Titre",
                        Binding = new Binding { Path = new PropertyPath("MainTitle"), Mode = BindingMode.OneWay },
                    });

                    //Autre(s) titre(s)
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Autre(s) titre(s)",
                        Binding = new Binding { Path = new PropertyPath("TitresOeuvreStringList"), Mode = BindingMode.OneWay },
                    });

                    //Auteur(s)
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Auteur(s)",
                        Binding = new Binding { Path = new PropertyPath("AuteursStringList"), Mode = BindingMode.OneWay },
                    });

                    //Maison(s) d'édition
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Maison(s) d'édition",
                        Binding = new Binding { Path = new PropertyPath("Publication.EditeursStringList"), Mode = BindingMode.OneWay },
                    });

                    //Date de parution
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Date de parution",
                        Binding = new Binding { Path = new PropertyPath("Publication.DateParution"), Mode = BindingMode.OneWay },
                    });

                    //Collection(s)
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Collection(s)",
                        Binding = new Binding { Path = new PropertyPath("Publication.CollectionsStringList"), Mode = BindingMode.OneWay },
                    });

                    ////Classification âge
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "Classification âge",
                    //    Binding = new Binding { Path = new PropertyPath("ClassificationAge.StringClassification"), Mode = BindingMode.OneWay },
                    //});

                    ////Format
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "Format",
                    //    Binding = new Binding { Path = new PropertyPath("Format.Format"), Mode = BindingMode.OneWay },
                    //});

                    ////Dimensions (L × l × E)
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "Dimensions (L × l × E)",
                    //    Binding = new Binding { Path = new PropertyPath("Format.Dimensions"), Mode = BindingMode.OneWay },
                    //});

                    ////Pages
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "Pages",
                    //    Binding = new Binding { Path = new PropertyPath("Format.NbOfPages"), Mode = BindingMode.OneWay },
                    //});

                    //Langue
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Langue",
                        Binding = new Binding { Path = new PropertyPath("Publication.Langue"), Mode = BindingMode.OneWay },
                    });

                    //Nombre d'exemplaires
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Nombre d'exemplaires",
                        Binding = new Binding { Path = new PropertyPath("NbExemplaires"), Mode = BindingMode.OneWay },
                    });

                    //Nombre de prêts
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Nombre de prêts",
                        Binding = new Binding { Path = new PropertyPath("NbPrets"), Mode = BindingMode.OneWay },
                    });

                    ////Cotation
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "Cotation",
                    //    Binding = new Binding { Path = new PropertyPath("Identification.Cotation"), Mode = BindingMode.OneWay },
                    //});

                    ////ISBN
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "ISBN",
                    //    Binding = new Binding { Path = new PropertyPath("Identification.ISBN"), Mode = BindingMode.OneWay },
                    //});

                    ////ISBN-10
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "ISBN-10",
                    //    Binding = new Binding { Path = new PropertyPath("Identification.ISBN10"), Mode = BindingMode.OneWay },
                    //});

                    ////ISBN13
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "ISBN13",
                    //    Binding = new Binding { Path = new PropertyPath("Identification.ISBN13"), Mode = BindingMode.OneWay },
                    //});

                    ////ISSN
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "ISSN",
                    //    Binding = new Binding { Path = new PropertyPath("Identification.ISSN"), Mode = BindingMode.OneWay },
                    //});

                    ////ASIN
                    //DataGridItems.Columns.Add(new DataGridTextColumn()
                    //{
                    //    Header = "ASIN",
                    //    Binding = new Binding { Path = new PropertyPath("Identification.ASIN"), Mode = BindingMode.OneWay },
                    //});

                    //Date d'ajout
                    DataGridItems.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "Date d'ajout",
                        Binding = new Binding { Path = new PropertyPath("DateAjout"), Mode = BindingMode.OneWay },
                    });
                    DataGridItems.ItemsSource = data;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        //public IEnumerable<T> GetViewModelList<T>() where T : class
        //{
        //    try
        //    {
        //        if (typeof(T) == typeof(LivreVM) && GridViewItems.ItemsSource is List<LivreVM> livreVms)
        //        {
        //            return livreVms.Select(s => (T)(object)s);
        //        }
        //        else if (typeof(T) == typeof(BibliothequeVM) && GridViewItems.ItemsSource is List<LivreVM> bibliothequeVms)
        //        {
        //            return bibliothequeVms.Select(s => (T)(object)s);
        //        }

        //        return Enumerable.Empty<T>();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        #region Context Menu Item
        private async void ChangeJaquetteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    //var result = await esAppBaseApi.ReplaceJaquetteAsync<LivreVM>(viewModel.Guid);
                    //if (!result.IsSuccess)
                    //{
                    //    return;
                    //}

                    //viewModel.JaquettePath = result.Result?.ToString() ?? EsGeneral.BookDefaultJaquette;
                    //var image = uiServices.GetSelectedThumbnailImage<LivreVM>(viewModel.Id, PivotItems, "GridViewItems");
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

        private async void DeleteJaquetteXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    //_ = await esAppBaseApi.RemoveJaquetteAsync<LivreVM>(viewModel.Guid);
                    //viewModel.JaquettePath = EsGeneral.BookDefaultJaquette;
                    //var image = uiServices.GetSelectedThumbnailImage<LivreVM>(viewModel.Id, PivotItems, "GridViewItems");
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

        private async void EditBookInfosXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is LivreVM viewModel)
            {
                //await ParentPage.NewEditBookAsync(viewModel, EditMode.Edit);
            }
        }

        private async void ExportThisBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    // await ParentPage.ExportThisBookAsync(viewModel);
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void DeleteBookXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    //await ParentPage.DeleteBookAsync(new LivreVM[] { viewModel });
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void BookExemplaryListXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    //ParentPage.OpenBookExemplaryList(viewModel);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void NewBookPretXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (args.Parameter is LivreVM viewModel)
                {
                    //ParentPage.OpenBookPretList(viewModel);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        #endregion

        private void DataGridItems_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void DataGridItems_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void DataGridItems_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }
    }
}
