using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Publishers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Book
{
    public sealed partial class NewEditBookUC : PivotItem
    {
        public readonly ManageBookParametersDriverVM _parameters;

        public NewEditBookUCVM ViewModelPage { get; set; } = new NewEditBookUCVM();


        public delegate void CancelModificationEventHandler(NewEditBookUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(NewEditBookUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(NewEditBookUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;


        public NewEditBookUC()
        {
            this.InitializeComponent();
        }

        public NewEditBookUC(ManageBookParametersDriverVM parameters)
        {
            this.InitializeComponent();
            _parameters = parameters;
            ViewModelPage.EditMode = parameters.EditMode;
            ViewModelPage.Header = $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Editer")} un livre";
            ViewModelPage.ViewModel = parameters?.CurrentViewModel;
            InitializeActionInfos();
        }

        private async void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateAuthorListAsync();
            await UpdateCollectionListAsync();
            await UpdateEditeurListAsync();
        }

        private void InitializeActionInfos()
        {
            try
            {
                Run runTitle = new Run()
                {
                    Text = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter un " : "d'éditer le")} livre",
                    //FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runTitle);

                if (_parameters != null)
                {
                    if (ViewModelPage.EditMode == EditMode.Edit)
                    {
                        Run runCategorie = new Run()
                        {
                            Text = " " + _parameters?.CurrentViewModel?.MainTitle,
                            FontWeight = FontWeights.Medium,
                        };
                        TbcInfos.Inlines.Add(runCategorie);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Titles
        private void AddTitleToBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (!this.TBX_TitlesOeuvre.Text.IsStringNullOrEmptyOrWhiteSpace())
                {
                    string value = this.TBX_TitlesOeuvre.Text.Trim();
                    if (ViewModelPage.ViewModel.TitresOeuvre.Any())
                    {
                        bool IsAlreadyExist = ViewModelPage.ViewModel.TitresOeuvre.Any(c => c == value);
                        if (!IsAlreadyExist)
                        {
                            ViewModelPage.ViewModel.TitresOeuvre.Add(value);
                            this.TBX_TitlesOeuvre.Text = String.Empty;
                        }
                    }
                    else
                    {
                        ViewModelPage.ViewModel.TitresOeuvre.Add(value);
                        this.TBX_TitlesOeuvre.Text = String.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void RemoveTitleToBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.LBX_TitresOeuvre.SelectedIndex > -1)
                {
                    ViewModelPage.ViewModel.TitresOeuvre.RemoveAt(this.LBX_TitresOeuvre.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Authors
        private async Task UpdateAuthorListAsync()
        {
            try
            {
                var authorsList = await DbServices.Author.AllVMAsync();
                ViewModelPage.AuthorViewModelList = authorsList?.ToList();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchAuthor_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.AuthorViewModelList == null)
                {
                    return;
                }

                var FilteredItems = new List<AuthorVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in ViewModelPage.AuthorViewModelList)
                {
                    if (!value.NomNaissance.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.NomNaissance.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                        }
                    }
                    else if (!value.NomUsage.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.NomUsage.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                        }
                    }
                    else if (!value.Prenom.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.Prenom.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                        }
                    }
                    else if (!value.AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.AutresPrenoms.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                        }
                    }
                }

                if (!FilteredItems.Any())
                {
                    FilteredItems.Add(new AuthorVM()
                    {
                        Id = -1,
                        NomNaissance = "Ajouter un auteur",
                    });
                }

                sender.ItemsSource = FilteredItems;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchAuthor_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem != null && args.SelectedItem is AuthorVM value)
                {
                    if (value.Id != -1)
                    {
                        sender.Text = value.NomNaissance + " " + value.Prenom;
                        return;
                    }
                }
                sender.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ASB_SearchAuthor_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is AuthorVM viewModel)
                {
                    if (viewModel.Id != -1)
                    {
                        if (ViewModelPage.ViewModel.Auteurs.Any())
                        {
                            bool IsAlreadyExist = ViewModelPage.ViewModel.Auteurs.Any(c => c.Id == viewModel.Id);
                            if (!IsAlreadyExist)
                            {
                                ViewModelPage.ViewModel.Auteurs.Add(viewModel);
                                sender.Text = String.Empty;
                            }
                        }
                        else
                        {
                            ViewModelPage.ViewModel.Auteurs.Add(viewModel);
                            sender.Text = String.Empty;
                        }
                    }
                    else
                    {
                        //Ajoute un nouvel auteur
                        if (_parameters.ParentPage != null)
                        {
                            await _parameters.ParentPage.NewAuthorAsync();
                        }
                    }
                }
                else
                {
                    //
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void UpdateAuthorToBookXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await UpdateAuthorListAsync();
        }

        private void RemoveAuthorToBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.LBX_Author.SelectedIndex > -1)
                {
                    ViewModelPage.ViewModel.Auteurs.RemoveAt(this.LBX_Author.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Collection
        private async Task UpdateCollectionListAsync()
        {
            try
            {
                var itemList = await DbServices.Collection.AllVMAsync();
                ViewModelPage.CollectionViewModelList = itemList?.ToList();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void UpdateCollectionToBookXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await UpdateCollectionListAsync();
        }

        private void RemoveCollectionToBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.ListViewCollection.SelectedIndex > -1)
                {
                    ViewModelPage.ViewModel.Publication.Collections.RemoveAt(this.ListViewCollection.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchCollection_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.CollectionViewModelList == null)
                {
                    return;
                }

                var FilteredItems = new List<CollectionVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in ViewModelPage.CollectionViewModelList)
                {
                    if (!value.Name.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.Name.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                        }
                    }
                }

                if (!FilteredItems.Any())
                {
                    FilteredItems.Add(new CollectionVM()
                    {
                        Id = -1,
                        Name = "Ajouter une collection",
                    });
                }

                sender.ItemsSource = FilteredItems;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchCollection_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem != null && args.SelectedItem is CollectionVM value)
                {
                    if (value.Id != -1)
                    {
                        sender.Text = value.Name;
                        return;
                    }
                }
                sender.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ASB_SearchCollection_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is CollectionVM viewModel)
                {
                    if (viewModel.Id != -1)
                    {
                        if (ViewModelPage.ViewModel.Publication.Collections.Any())
                        {
                            bool IsAlreadyExist = ViewModelPage.ViewModel.Publication.Collections.Any(c => c.Id == viewModel.Id);
                            if (!IsAlreadyExist)
                            {
                                ViewModelPage.ViewModel.Publication.Collections.Add(viewModel);
                                sender.Text = String.Empty;
                            }
                        }
                        else
                        {
                            ViewModelPage.ViewModel.Publication.Collections.Add(viewModel);
                            sender.Text = String.Empty;
                        }
                    }
                    else
                    {
                        //Ajoute un nouvel auteur
                        if (_parameters.ParentPage != null)
                        {
                            await _parameters.ParentPage.NewCollectionAsync();
                        }
                    }
                }
                else
                {
                    //
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Editeurs
        private async Task UpdateEditeurListAsync()
        {
            try
            {
                var itemList = await DbServices.Editors.AllVMAsync();
                ViewModelPage.EditorsViewModelList = itemList?.ToList();
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void UpdateEditorToBookXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await UpdateEditeurListAsync();
        }

        private void RemoveEditorToBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.ListViewEditeur.SelectedIndex > -1)
                {
                    ViewModelPage.ViewModel.Publication.Editeurs.RemoveAt(this.ListViewEditeur.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchEditor_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.EditorsViewModelList == null)
                {
                    return;
                }

                var FilteredItems = new List<PublisherVM>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in ViewModelPage.EditorsViewModelList)
                {
                    if (!value.Name.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.Name.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                        }
                    }
                }

                if (!FilteredItems.Any())
                {
                    FilteredItems.Add(new PublisherVM()
                    {
                        Id = -1,
                        Name = "Ajouter un éditeur",
                    });
                }

                sender.ItemsSource = FilteredItems;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }

        }

        private void ASB_SearchEditor_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem != null && args.SelectedItem is PublisherVM value)
                {
                    if (value.Id != -1)
                    {
                        sender.Text = value.Name;
                        return;
                    }
                }
                sender.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private async void ASB_SearchEditor_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is PublisherVM viewModel)
                {
                    if (viewModel.Id != -1)
                    {
                        if (ViewModelPage.ViewModel.Publication.Editeurs.Any())
                        {
                            bool IsAlreadyExist = ViewModelPage.ViewModel.Publication.Editeurs.Any(c => c.Id == viewModel.Id);
                            if (!IsAlreadyExist)
                            {
                                ViewModelPage.ViewModel.Publication.Editeurs.Add(viewModel);
                                sender.Text = String.Empty;
                            }
                        }
                        else
                        {
                            ViewModelPage.ViewModel.Publication.Editeurs.Add(viewModel);
                            sender.Text = String.Empty;
                        }
                    }
                    else
                    {
                        //Ajoute un nouvel auteur
                        if (_parameters.ParentPage != null)
                        {
                            await _parameters.ParentPage.NewEditorAsync();
                        }
                    }
                }
                else
                {
                    //
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        #region Date Parution
        private void MfiClearParutionDate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModelPage.ViewModel.Publication.DateParution != null)
                {
                    ViewModelPage.ViewModel.Publication.DateParution = null;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void TmfiDayKnow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleMenuFlyoutItem toggle)
                {
                    if (toggle.IsChecked)
                    {
                        if (ViewModelPage.ViewModel.Publication.IsMoisParutionKnow == false || ViewModelPage.ViewModel.Publication.DateParution  == null)
                        {
                            ViewModelPage.ViewModel.Publication.IsJourParutionKnow = false;
                            ViewModelPage.ViewModel.Publication.IsJourParutionVisible = false;
                            toggle.IsChecked = false;
                        }
                        else
                        {
                            ViewModelPage.ViewModel.Publication.IsJourParutionKnow = true;
                            ViewModelPage.ViewModel.Publication.IsJourParutionVisible = true;
                        }
                    }
                    else
                    {
                        ViewModelPage.ViewModel.Publication.IsJourParutionKnow = false;
                        ViewModelPage.ViewModel.Publication.IsJourParutionVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void TmfiMonthKnow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleMenuFlyoutItem toggle)
                {
                    if (toggle.IsChecked)
                    {
                        if (ViewModelPage.ViewModel.Publication.DateParution == null)
                        {
                            ViewModelPage.ViewModel.Publication.IsJourParutionVisible = true;
                            ViewModelPage.ViewModel.Publication.IsMoisParutionVisible = true;
                            ViewModelPage.ViewModel.Publication.IsMoisParutionKnow = false;
                            ViewModelPage.ViewModel.Publication.IsJourParutionKnow = false;
                        }
                        else
                        {
                            ViewModelPage.ViewModel.Publication.IsMoisParutionVisible = true;
                            ViewModelPage.ViewModel.Publication.IsMoisParutionKnow = true;
                        }
                    }
                    else
                    {
                        ViewModelPage.ViewModel.Publication.IsJourParutionKnow = false;
                        ViewModelPage.ViewModel.Publication.IsJourParutionVisible = false;
                        ViewModelPage.ViewModel.Publication.IsMoisParutionKnow = false;
                        ViewModelPage.ViewModel.Publication.IsMoisParutionVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DP_DateParution_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            try
            {
                if (args.NewDate != null)
                {
                    ViewModelPage.ViewModel.Publication.IsJourParutionVisible = true;
                    ViewModelPage.ViewModel.Publication.IsMoisParutionVisible = true;
                    ViewModelPage.ViewModel.Publication.IsJourParutionKnow = true;
                    ViewModelPage.ViewModel.Publication.IsMoisParutionKnow = true;
                }
                else
                {
                    ViewModelPage.ViewModel.Publication.IsJourParutionVisible = true;
                    ViewModelPage.ViewModel.Publication.IsMoisParutionVisible = true;
                    ViewModelPage.ViewModel.Publication.IsJourParutionKnow = false;
                    ViewModelPage.ViewModel.Publication.IsMoisParutionKnow = false;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void DateParution_MenuFlyout_Opening(object sender, object e)
        {
            try
            {
                if (ViewModelPage.ViewModel.Publication.DateParution == null)
                {
                    BtnDateParution.Flyout.Hide();
                    MyTeachingTip.Target = BtnDateParution;
                    MyTeachingTip.Title = "Date de parution";
                    MyTeachingTip.Subtitle = "Sélectionnez tout d'abord une date puis cliquez de nouveau sur ce bouton.";
                    MyTeachingTip.IsOpen = true;
                    
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }
        #endregion

        private void PreviousStepXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (FlipViewStep.SelectedIndex > 0)
                {
                    FlipViewStep.SelectedIndex--;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void NextStepXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                if (FlipViewStep.SelectedIndex < FlipViewStep.Items.Count)
                {
                    FlipViewStep.SelectedIndex++;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
        }

        private void ImportBookXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }

        private void CreateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                CreateItemRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void UpdateItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                UpdateItemRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }


        private bool IsModelValided()
        {
            try
            {
                if (ViewModelPage.ViewModel.MainTitle.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ResultMessage = $"Le titre principal du livre ne peut pas être vide ou ne contenir que des espaces blancs.";
                    ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
                    return false;
                }

                if (ViewModelPage.ViewModel.ClassificationAge.MinAge > ViewModelPage.ViewModel.ClassificationAge.MaxAge)
                {
                    ViewModelPage.ResultMessage = $"L'âge minimum ne peut pas être supérieur à l'âge maximum.";
                    ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
                    return false;
                }

                if (ViewModelPage.ViewModel.ClassificationAge.MaxAge < ViewModelPage.ViewModel.ClassificationAge.MinAge)
                {
                    ViewModelPage.ResultMessage = $"L'âge maximum ne peut pas être inférieur à l'âge maximum.";
                    ViewModelPage.ResultMessageForeGround = new SolidColorBrush(Colors.OrangeRed);
                    return false;
                }

                //if (_parameters.ViewModelList != null && _parameters.ViewModelList.Any(a => a.Name.ToLower() == ViewModelPage.Value.Trim().ToLower()))
                //{
                //    var isError = !(_parameters.EditMode == Code.EditMode.Edit && _parameters.CurrentLibrary?.Name?.Trim().ToLower() == ViewModelPage.Value?.Trim().ToLower());
                //    if (isError)
                //    {
                //        TbxErrorMessage.Text = $"Cette bibliothèque existe déjà.";
                //        return false;
                //    }
                //}

                ViewModelPage.ResultMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CancelModificationRequested != null)
                {
                    CancelModificationRequested = null;
                }

                if (CreateItemRequested != null)
                {
                    CreateItemRequested = null;
                }

                if (UpdateItemRequested != null)
                {
                    UpdateItemRequested = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DeleteItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }

        private void MenuFlyout_Opened(object sender, object e)
        {

        }

        
    }

    public class NewEditBookUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string _Header;
        public string Header
        {
            get => this._Header;
            set
            {
                if (this._Header != value)
                {
                    this._Header = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ResultMessage;
        public string ResultMessage
        {
            get => this._ResultMessage;
            set
            {
                if (this._ResultMessage != value)
                {
                    this._ResultMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ArgName;
        public string ArgName
        {
            get => this._ArgName;
            set
            {
                if (this._ArgName != value)
                {
                    this._ArgName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public readonly IEnumerable<string> civilityList = CivilityHelpers.CiviliteListShorted();

        private LivreVM _ViewModel;
        public LivreVM ViewModel
        {
            get => this._ViewModel;
            set
            {
                if (this._ViewModel != value)
                {
                    this._ViewModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Brush _ResultMessageForeGround;
        public Brush ResultMessageForeGround
        {
            get => this._ResultMessageForeGround;
            set
            {
                if (this._ResultMessageForeGround != value)
                {
                    this._ResultMessageForeGround = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private EditMode _EditMode;
        public EditMode EditMode
        {
            get => this._EditMode;
            set
            {
                if (this._EditMode != value)
                {
                    this._EditMode = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private IEnumerable<AuthorVM> _AuthorViewModelList = Enumerable.Empty<AuthorVM>();
        public IEnumerable<AuthorVM> AuthorViewModelList
        {
            get => this._AuthorViewModelList;
            set
            {
                if (_AuthorViewModelList != value)
                {
                    this._AuthorViewModelList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private IEnumerable<CollectionVM> _CollectionViewModelList = Enumerable.Empty<CollectionVM>();
        public IEnumerable<CollectionVM> CollectionViewModelList
        {
            get => this._CollectionViewModelList;
            set
            {
                if (_CollectionViewModelList != value)
                {
                    this._CollectionViewModelList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private IEnumerable<PublisherVM> _EditorsViewModelList = Enumerable.Empty<PublisherVM>();
        public IEnumerable<PublisherVM> EditorsViewModelList
        {
            get => this._EditorsViewModelList;
            set
            {
                if (_EditorsViewModelList != value)
                {
                    this._EditorsViewModelList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
