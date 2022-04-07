using LibraryProjectUWP.Code.Helpers;
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
    public sealed partial class SideBarItemHeader : UserControl
    {
        public Guid Guid { get; private set; }
        public SideBarItemHeader()
        {
            this.InitializeComponent();
        }

        public Guid HeaderGuid
        {
            get { return (Guid)GetValue(HeaderGuidProperty); }
            set { SetValue(HeaderGuidProperty, value); }
        }

        public static readonly DependencyProperty HeaderGuidProperty = DependencyProperty.Register(nameof(HeaderGuid), typeof(Guid),
                                                                typeof(SideBarItemHeader), new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderGuidChanged)));

        private static void OnHeaderGuidChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SideBarItemHeader parent && e.NewValue is Guid guid)
            {
                parent.Guid = guid;
            }
        }

        public String Title
        {
            get { return (String)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(String),
                                                                typeof(SideBarItemHeader), new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SideBarItemHeader parent && e.NewValue is string title)
            {
                if (!title.IsStringNullOrEmptyOrWhiteSpace())
                {
                    parent.TbcTitle.Text = title.Trim();
                }
            }
        }

        public String Glyph
        {
            get { return (String)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(String),
                                                                typeof(SideBarItemHeader), new PropertyMetadata(null, new PropertyChangedCallback(OnGlyphChanged)));
        private static void OnGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SideBarItemHeader parent && e.NewValue is string glyph)
            {
                parent.MyFontIcon.Glyph = glyph;
            }
        }
    }
}
