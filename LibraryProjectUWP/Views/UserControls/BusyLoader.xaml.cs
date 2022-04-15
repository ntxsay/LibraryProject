using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.ViewModels.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace LibraryProjectUWP.Views.UserControls
{
    public sealed partial class BusyLoader : Grid
    {
        public BusyLoaderParametersVM Parameters { get; private set; }
        public BusyLoader(BusyLoaderParametersVM parameters) 
        {
            this.InitializeComponent();
            Parameters = parameters;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Parameters != null && Parameters.OpenedLoaderCallback != null)
                {
                    Parameters.OpenedLoaderCallback();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Parameters != null && Parameters.CancelButtonCallback != null)
            {
                Parameters.CancelButtonCallback();
            }
        }

        private void BtnPrimary_Click(object sender, RoutedEventArgs e)
        {
            if (Parameters != null && Parameters.PrimaryButtonCallback != null)
            {
                Parameters.PrimaryButtonCallback();
            }
        }

        private void BtnSecondary_Click(object sender, RoutedEventArgs e)
        {
            if (Parameters != null && Parameters.SecondaryButtonCallback != null)
            {
                Parameters.SecondaryButtonCallback();
            }
        }


        #region Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
                                                                typeof(BusyLoader), new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BusyLoader parent && e.NewValue is string value)
            {
                if (parent.TbcTitle.Text != value)
                    parent.TbcTitle.Text = value;
            }
        }
        #endregion

        #region CancelButtonVisibility
        public Visibility CancelButtonVisibility
        {
            get { return (Visibility)GetValue(CancelButtonVisibilityProperty); }
            set { SetValue(CancelButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty CancelButtonVisibilityProperty = DependencyProperty.Register(nameof(CancelButtonVisibility), typeof(Visibility),
                                                                typeof(BusyLoader), new PropertyMetadata(null, new PropertyChangedCallback(OnCancelButtonVisibilityChanged)));

        private static void OnCancelButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BusyLoader parent && e.NewValue is Visibility value)
            {
                if (parent.BtnPrimary.Visibility != value)
                    parent.BtnCancel.Visibility = value;
            }
        }
        #endregion

        #region PrimaryButtonVisibility
        public Visibility PrimaryButtonVisibility
        {
            get { return (Visibility)GetValue(PrimaryButtonVisibilityProperty); }
            set { SetValue(PrimaryButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty PrimaryButtonVisibilityProperty = DependencyProperty.Register(nameof(PrimaryButtonVisibility), typeof(Visibility),
                                                                typeof(BusyLoader), new PropertyMetadata(null, new PropertyChangedCallback(OnPrimaryButtonVisibilityChanged)));

        private static void OnPrimaryButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BusyLoader parent && e.NewValue is Visibility value)
            {
                if (parent.BtnPrimary.Visibility != value)
                    parent.BtnPrimary.Visibility = value;
            }
        }
        #endregion

        #region SecondaryButtonVisibility
        public Visibility SecondaryButtonVisibility
        {
            get { return (Visibility)GetValue(SecondaryButtonVisibilityProperty); }
            set { SetValue(SecondaryButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty SecondaryButtonVisibilityProperty = DependencyProperty.Register(nameof(SecondaryButtonVisibility), typeof(Visibility),
                                                                typeof(BusyLoader), new PropertyMetadata(null, new PropertyChangedCallback(OnSecondaryButtonVisibilityChanged)));

        private static void OnSecondaryButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BusyLoader parent && e.NewValue is Visibility value)
            {
                if (parent.BtnPrimary.Visibility != value)
                    parent.BtnSecondary.Visibility = value;
            }
        }
        #endregion

        #region CancelButtonTitle
        public string CancelButtonTitle
        {
            get { return (string)GetValue(CancelButtonTitleProperty); }
            set { SetValue(CancelButtonTitleProperty, value); }
        }

        public static readonly DependencyProperty CancelButtonTitleProperty = DependencyProperty.Register(nameof(CancelButtonTitle), typeof(string),
                                                                typeof(BusyLoader), new PropertyMetadata(null, new PropertyChangedCallback(OnCancelButtonTitleChanged)));

        private static void OnCancelButtonTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BusyLoader parent && e.NewValue is string value)
            {
                if (parent.TbcCancel.Text != value)
                    parent.TbcCancel.Text = value;
            }
        }
        #endregion

        #region PrimaryButtonTitle
        public string PrimaryButtonTitle
        {
            get { return (string)GetValue(PrimaryButtonTitleProperty); }
            set { SetValue(PrimaryButtonTitleProperty, value); }
        }

        public static readonly DependencyProperty PrimaryButtonTitleProperty = DependencyProperty.Register(nameof(PrimaryButtonTitle), typeof(string),
                                                                typeof(BusyLoader), new PropertyMetadata(null, new PropertyChangedCallback(OnPrimaryButtonTitleChanged)));

        private static void OnPrimaryButtonTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BusyLoader parent && e.NewValue is string value)
            {
                if (parent.TbcPrimary.Text != value)
                    parent.TbcPrimary.Text = value;
            }
        }
        #endregion

        #region SecondaryButtonTitle
        public string SecondaryButtonTitle
        {
            get { return (string)GetValue(SecondaryButtonTitleProperty); }
            set { SetValue(SecondaryButtonTitleProperty, value); }
        }

        public static readonly DependencyProperty SecondaryButtonTitleProperty = DependencyProperty.Register(nameof(SecondaryButtonTitle), typeof(string),
                                                                typeof(BusyLoader), new PropertyMetadata(null, new PropertyChangedCallback(OnSecondaryButtonTitleChanged)));

        private static void OnSecondaryButtonTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BusyLoader parent && e.NewValue is string value)
            {
                if (parent.TbcSecondary.Text != value)
                    parent.TbcSecondary.Text = value;
            }
        }
        #endregion
    }
}
