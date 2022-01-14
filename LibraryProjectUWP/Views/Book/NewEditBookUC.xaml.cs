using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Author;
using LibraryProjectUWP.ViewModels.Book;
using LibraryProjectUWP.ViewModels.Collection;
using LibraryProjectUWP.ViewModels.Publishers;
using Microsoft.UI.Xaml.Controls;
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

        #region LanGuages
        private void ASB_SearchLanguage_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (sender.Text.IsStringNullOrEmptyOrWhiteSpace() || ViewModelPage.languagesList == null)
                {
                    return;
                }

                var FilteredItems = new List<string>();
                var splitSearchTerm = sender.Text.ToLower().Split(" ");

                foreach (var value in ViewModelPage.languagesList)
                {
                    if (!value.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                            continue;
                        }
                    }
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

        private void ASB_SearchLanguage_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                if (args.SelectedItem != null && args.SelectedItem is string value)
                {
                    sender.Text = value;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void ASB_SearchLanguage_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null && args.ChosenSuggestion is string lang)
                {
                    if (ViewModelPage.ViewModel.Publication.Langues.Any())
                    {
                        bool IsAlreadyExist = ViewModelPage.ViewModel.Publication.Langues.Any(c => c.ToLower() == lang.ToLower());
                        if (!IsAlreadyExist)
                        {
                            ViewModelPage.ViewModel.Publication.Langues.Add(lang);
                        }
                    }
                    else
                    {
                        ViewModelPage.ViewModel.Publication.Langues.Add(lang);
                    }
                    sender.Text = String.Empty;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return;
            }
        }

        private void RemoveLanguageXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                if (this.ListViewLanguages.SelectedIndex > -1)
                {
                    ViewModelPage.ViewModel.Publication.Langues.RemoveAt(this.ListViewLanguages.SelectedIndex);
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
                            continue;
                        }
                    }
                    
                    if (!value.NomUsage.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.NomUsage.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                        }
                    }

                    if (!value.Prenom.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.Prenom.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                            continue;
                        }
                    }
                    
                    if (!value.AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var found = splitSearchTerm.All((key) => {
                            return value.AutresPrenoms.ToLower().Contains(key.ToLower());
                        });

                        if (found)
                        {
                            FilteredItems.Add(value);
                            continue;
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
                            if (!sender.Text.IsStringNullOrEmptyOrWhiteSpace()) 
                            {
                                var split = StringHelpers.SplitWord(sender.Text, new string[] { " " });
                                if (split.Length == 1)
                                {
                                    await _parameters.ParentPage.NewAuthorAsync(split[0], string.Empty, ViewModelPage.Guid);
                                }
                                else if (split.Length >= 2)
                                {
                                    await _parameters.ParentPage.NewAuthorAsync(split[0], split[1], ViewModelPage.Guid);
                                }
                            }
                            else
                            {
                                await _parameters.ParentPage.NewAuthorAsync(string.Empty, string.Empty, ViewModelPage.Guid);
                            }
                            sender.Text = String.Empty;
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
                            await _parameters.ParentPage.NewCollectionAsync(sender.Text, ViewModelPage.Guid);
                            sender.Text = String.Empty;
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
                            await _parameters.ParentPage.NewEditorAsync(sender.Text, ViewModelPage.Guid);
                            sender.Text = String.Empty;
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

        private void PipsPager_SelectedIndexChanged(Microsoft.UI.Xaml.Controls.PipsPager sender, Microsoft.UI.Xaml.Controls.PipsPagerSelectedIndexChangedEventArgs args)
        {
            try
            {
                if (sender.SelectedPageIndex == -1)
                {
                    return;
                }

                if (sender.SelectedPageIndex == 0)
                {
                    ViewModelPage.GeneralStepVisibility = Visibility.Visible;
                    ViewModelPage.PublicationStepVisibility = Visibility.Collapsed;
                    ViewModelPage.IdentificationStepVisibility = Visibility.Collapsed;
                    ViewModelPage.FormatStepVisibility = Visibility.Collapsed;
                    ViewModelPage.DescriptionStepVisibility = Visibility.Collapsed;
                }
                else if (sender.SelectedPageIndex == 1)
                {
                    ViewModelPage.GeneralStepVisibility = Visibility.Collapsed;
                    ViewModelPage.PublicationStepVisibility = Visibility.Visible;
                    ViewModelPage.IdentificationStepVisibility = Visibility.Collapsed;
                    ViewModelPage.FormatStepVisibility = Visibility.Collapsed;
                    ViewModelPage.DescriptionStepVisibility = Visibility.Collapsed;
                }
                else if (sender.SelectedPageIndex == 2)
                {
                    ViewModelPage.GeneralStepVisibility = Visibility.Collapsed;
                    ViewModelPage.PublicationStepVisibility = Visibility.Collapsed;
                    ViewModelPage.IdentificationStepVisibility = Visibility.Visible;
                    ViewModelPage.FormatStepVisibility = Visibility.Collapsed;
                    ViewModelPage.DescriptionStepVisibility = Visibility.Collapsed;
                }
                else if (sender.SelectedPageIndex == 3)
                {
                    ViewModelPage.GeneralStepVisibility = Visibility.Collapsed;
                    ViewModelPage.PublicationStepVisibility = Visibility.Collapsed;
                    ViewModelPage.IdentificationStepVisibility = Visibility.Collapsed;
                    ViewModelPage.FormatStepVisibility = Visibility.Visible;
                    ViewModelPage.DescriptionStepVisibility = Visibility.Collapsed;
                }
                else if (sender.SelectedPageIndex == 4)
                {
                    ViewModelPage.GeneralStepVisibility = Visibility.Collapsed;
                    ViewModelPage.PublicationStepVisibility = Visibility.Collapsed;
                    ViewModelPage.IdentificationStepVisibility = Visibility.Collapsed;
                    ViewModelPage.FormatStepVisibility = Visibility.Collapsed;
                    ViewModelPage.DescriptionStepVisibility = Visibility.Visible;
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
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"Le titre principal du livre ne peut pas être vide ou ne contenir que des espaces blancs.";
                    ViewModelPage.CurrentPipsPagerIndex = 0;
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                if (ViewModelPage.ViewModel.ClassificationAge.MinAge > ViewModelPage.ViewModel.ClassificationAge.MaxAge)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"L'âge minimum ne peut pas être supérieur à l'âge maximum.";
                    ViewModelPage.CurrentPipsPagerIndex = 0;
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
                    return false;
                }

                if (ViewModelPage.ViewModel.ClassificationAge.MaxAge < ViewModelPage.ViewModel.ClassificationAge.MinAge)
                {
                    ViewModelPage.ResultMessageTitle = "Vérifiez vos informations";
                    ViewModelPage.ResultMessage = $"L'âge maximum ne peut pas être inférieur à l'âge maximum.";
                    ViewModelPage.CurrentPipsPagerIndex = 0;
                    ViewModelPage.ResultMessageSeverity = InfoBarSeverity.Warning;
                    ViewModelPage.IsResultMessageOpen = true;
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

                ViewModelPage.IsResultMessageOpen = false;
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

        public Guid Guid { get; private set; } = Guid.NewGuid();


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

        public readonly IEnumerable<string> languagesList = CountryHelpers.LanguagesList();
        //public readonly IEnumerable<string> civilityList = CivilityHelpers.CiviliteListShorted();

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

        private InfoBarSeverity _ResultMessageSeverity = InfoBarSeverity.Informational;
        public InfoBarSeverity ResultMessageSeverity
        {
            get => this._ResultMessageSeverity;
            set
            {
                if (this._ResultMessageSeverity != value)
                {
                    this._ResultMessageSeverity = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsResultMessageOpen;
        public bool IsResultMessageOpen
        {
            get => this._IsResultMessageOpen;
            set
            {
                if (this._IsResultMessageOpen != value)
                {
                    this._IsResultMessageOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ResultMessageTitle;
        public string ResultMessageTitle
        {
            get => this._ResultMessageTitle;
            set
            {
                if (this._ResultMessageTitle != value)
                {
                    this._ResultMessageTitle = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _CurrentPipsPagerIndex;
        public int CurrentPipsPagerIndex
        {
            get => this._CurrentPipsPagerIndex;
            set
            {
                if (this._CurrentPipsPagerIndex != value)
                {
                    this._CurrentPipsPagerIndex = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _GeneralStepVisibility = Visibility.Visible;
        public Visibility GeneralStepVisibility
        {
            get => this._GeneralStepVisibility;
            set
            {
                if (this._GeneralStepVisibility != value)
                {
                    this._GeneralStepVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _IdentificationStepVisibility = Visibility.Collapsed;
        public Visibility IdentificationStepVisibility
        {
            get => this._IdentificationStepVisibility;
            set
            {
                if (this._IdentificationStepVisibility != value)
                {
                    this._IdentificationStepVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _PublicationStepVisibility = Visibility.Collapsed;
        public Visibility PublicationStepVisibility
        {
            get => this._PublicationStepVisibility;
            set
            {
                if (this._PublicationStepVisibility != value)
                {
                    this._PublicationStepVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _FormatStepVisibility = Visibility.Collapsed;
        public Visibility FormatStepVisibility
        {
            get => this._FormatStepVisibility;
            set
            {
                if (this._FormatStepVisibility != value)
                {
                    this._FormatStepVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Visibility _DescriptionStepVisibility = Visibility.Collapsed;
        public Visibility DescriptionStepVisibility
        {
            get => this._DescriptionStepVisibility;
            set
            {
                if (this._DescriptionStepVisibility != value)
                {
                    this._DescriptionStepVisibility = value;
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
