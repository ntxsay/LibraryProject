﻿using LibraryProjectUWP.Code;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.Library.Manage
{
    public sealed partial class NewEditCategoryUC : UserControl
    {
        public readonly ManageCategorieDialogParametersVM _categorieParameters;
        public readonly ManageSubCategorieDialogParametersVM _subCategorieParameters;
        
        public NewEditCategoryUCVM ViewModelPage { get; set; } = new NewEditCategoryUCVM();

        public delegate void CancelModificationEventHandler(NewEditCategoryUC sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        public delegate void UpdateItemEventHandler(NewEditCategoryUC sender, ExecuteRequestedEventArgs e);
        public event UpdateItemEventHandler UpdateItemRequested;

        public delegate void CreateItemEventHandler(NewEditCategoryUC sender, ExecuteRequestedEventArgs e);
        public event CreateItemEventHandler CreateItemRequested;

        public NewEditCategoryUC()
        {
            this.InitializeComponent();
        }

        public NewEditCategoryUC(ManageCategorieDialogParametersVM parameters)
        {
            this.InitializeComponent();
            _categorieParameters = parameters;
            ViewModelPage.EditMode = parameters.EditMode;
            ViewModelPage.Header = $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Editer")} une catégorie";
            ViewModelPage.NamePlaceHolderText = "Nom de la catégorie";
            ViewModelPage.DescriptionPlaceHolderText = "Description facultative de la catégorie";
            ViewModelPage.ArgName = "catégorie";
            ViewModelPage.Value = parameters?.CurrentCategorie?.Name;
            ViewModelPage.Description = parameters?.CurrentCategorie?.Description;
            InitializeActionInfos();
        }

        public NewEditCategoryUC(ManageSubCategorieDialogParametersVM parameters)
        {
            this.InitializeComponent();
            _subCategorieParameters = parameters;
            ViewModelPage.EditMode = parameters.EditMode;
            ViewModelPage.Header = $"{(parameters.EditMode == Code.EditMode.Create ? "Ajouter" : "Editer")} une sous-catégorie";
            ViewModelPage.NamePlaceHolderText = "Nom de la sous-catégorie";
            ViewModelPage.DescriptionPlaceHolderText = "Description facultative de la sous-catégorie";
            ViewModelPage.ArgName = "sous-catégorie";
            ViewModelPage.Value = parameters?.CurrentSubCategorie?.Name;
            ViewModelPage.Description = parameters?.CurrentSubCategorie?.Description;
            InitializeActionInfos();
        }

        private void InitializeActionInfos()
        {
            try
            {
                Run runTitle = new Run()
                {
                    Text = $"Vous êtes en train {(ViewModelPage.EditMode == EditMode.Create ? "d'ajouter une nouvelle" : "d'éditer la")} {ViewModelPage.ArgName}",
                    //FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runTitle);
                
                if (_categorieParameters != null)
                {
                    if (ViewModelPage.EditMode == EditMode.Edit)
                    {
                        Run runCategorie = new Run()
                        {
                            Text = " " + _categorieParameters?.CurrentCategorie?.Name,
                            FontWeight = FontWeights.Medium,
                        };
                        TbcInfos.Inlines.Add(runCategorie);
                    }

                    Run runInLibrary = new Run()
                    {
                        Text = " dans la bibliothèque",
                    };
                    TbcInfos.Inlines.Add(runInLibrary);
                    
                    Run runLibrary = new Run()
                    {
                        Text = " « " + _categorieParameters?.ParentLibrary?.Name + " »",
                        FontWeight = FontWeights.Medium,
                    };
                    TbcInfos.Inlines.Add(runLibrary);
                }
                else if (_subCategorieParameters != null)
                {
                    if (ViewModelPage.EditMode == EditMode.Edit)
                    {
                        Run runSubCategorie = new Run()
                        {
                            Text = " " + _subCategorieParameters?.CurrentSubCategorie?.Name,
                            FontWeight = FontWeights.Medium,
                        };
                        TbcInfos.Inlines.Add(runSubCategorie);
                    }

                    Run runInCategorie = new Run()
                    {
                        Text = " dans la catégorie",
                    };
                    TbcInfos.Inlines.Add(runInCategorie);

                    Run runCategorieName = new Run()
                    {
                        Text = " « " + _subCategorieParameters?.Categorie?.Name + " »",
                        FontWeight = FontWeights.Medium,
                    };
                    TbcInfos.Inlines.Add(runCategorieName);

                    Run runInLibrary = new Run()
                    {
                        Text = " dans la bibliothèque",
                    };
                    TbcInfos.Inlines.Add(runInLibrary);

                    Run runLibrary = new Run()
                    {
                        Text = " « " + _subCategorieParameters?.Categorie?.Name + " »",
                        FontWeight = FontWeights.Medium,
                    };
                    TbcInfos.Inlines.Add(runLibrary);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void CancelModificationXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CancelModificationRequested?.Invoke(this, args);
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

                if (CreateItemRequested != null)
                {
                    CreateItemRequested(this, args);
                }
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
                if (ViewModelPage.Value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    ViewModelPage.ErrorMessage = $"Le nom de la {(_subCategorieParameters != null ? "sous-catégorie" : "catégorie")} ne peut pas être vide ou ne contenir que des espaces blancs.";
                    return false;
                }

                if (_subCategorieParameters != null && !_subCategorieParameters.Categorie.Name.IsStringNullOrEmptyOrWhiteSpace() && _subCategorieParameters.Categorie.Name.ToLower() == ViewModelPage.Value.Trim().ToLower())
                {
                    ViewModelPage.ErrorMessage = $"Le nom de la sous-catégorie ne peut pas avoir le même nom que son parent.";
                    return false;
                }

                if (_categorieParameters != null)
                {
                    if (_categorieParameters.ParentLibrary.Categories != null && _categorieParameters.ParentLibrary.Categories.Any(a => a.Name.ToLower() == ViewModelPage.Value.Trim().ToLower()))
                    {
                        ViewModelPage.ErrorMessage = $"Cette catégorie existe déjà.";
                        return false;
                    }
                }
                else if (_subCategorieParameters != null)
                {
                    if (_subCategorieParameters.Categorie.SubCategorieLivres != null && _subCategorieParameters.Categorie.SubCategorieLivres.Any(a => a.Name.ToLower() == ViewModelPage.Value.Trim().ToLower()))
                    {
                        ViewModelPage.ErrorMessage = $"Cette sous-catégorie existe déjà.";
                        return false;
                    }
                }

                ViewModelPage.ErrorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return false;
            }
        }


        private void DeleteItemXUiCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

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
    }

    public class NewEditCategoryUCVM : INotifyPropertyChanged
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

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get => this._ErrorMessage;
            set
            {
                if (this._ErrorMessage != value)
                {
                    this._ErrorMessage = value;
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

        private string _Value;
        public string Value
        {
            get => this._Value;
            set
            {
                if (this._Value != value)
                {
                    this._Value = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Description;
        public string Description
        {
            get => this._Description;
            set
            {
                if (this._Description != value)
                {
                    this._Description = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _NamePlaceHolderText;
        public string NamePlaceHolderText
        {
            get => this._NamePlaceHolderText;
            set
            {
                if (this._NamePlaceHolderText != value)
                {
                    this._NamePlaceHolderText = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _DescriptionPlaceHolderText;
        public string DescriptionPlaceHolderText
        {
            get => this._DescriptionPlaceHolderText;
            set
            {
                if (this._DescriptionPlaceHolderText != value)
                {
                    this._DescriptionPlaceHolderText = value;
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}