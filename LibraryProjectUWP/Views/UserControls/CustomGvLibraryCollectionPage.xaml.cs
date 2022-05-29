using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.ES;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Library;
using LibraryProjectUWP.Views.Book;
using LibraryProjectUWP.Views.Book.SubViews;
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
    public sealed partial class CustomGvLibraryCollectionPage : Page
    {
        public CustomGvLibraryCollectionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is List<LivreVM> parameters)
            {
                if (GridViewItems.ItemsSource != null && GridViewItems.ItemsSource is List<LivreVM> existingList)
                {
                    var changed = PropertyHelpers.GetChangedProperties(parameters, existingList);
                    if (changed != null && changed.Any())
                    {
                        GridViewItems.ItemsSource = e.Parameter;
                    }
                }
                else
                {
                    GridViewItems.ItemsSource = e.Parameter;
                }
                //ViewModelPage = new BookCollectionSubPageVM(parameters);
                //ParentPage = parameters;
                //CommonView = new CommonView(ParentPage, this);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //GridViewItems.ItemsSource = null;
            //DeleteBookXUiCmd = null;
            //GC.Collect();
        }


        public IEnumerable<T> GetViewModelList<T>() where T : class
        {
            try
            {
                if (typeof(T) == typeof(LivreVM) && GridViewItems.ItemsSource is List<LivreVM> livreVms)
                {
                    return livreVms.Select(s => (T)(object)s);
                }
                else if (typeof(T) == typeof(BibliothequeVM) && GridViewItems.ItemsSource is List<LivreVM> bibliothequeVms)
                {
                    return bibliothequeVms.Select(s => (T)(object)s);
                }

                return Enumerable.Empty<T>();
            }
            catch (Exception)
            {

                throw;
            }
        }

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
    }
}
