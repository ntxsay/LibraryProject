using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RostalProjectUWP.Code.Helpers
{
    public class VisualViewHelpers
    {
        public class MainControlsUI
        {
            public static MainPage MainPage => (Window.Current.Content as Frame).Content as MainPage;
            public MainPage GetMainPage => (Window.Current.Content as Frame).Content as MainPage;
            public Frame GetMainFrameContainer => GetMainPage.MainFrameContainer;
            public static Frame MainFrameContainer => MainPage.MainFrameContainer;
        }

        public static T FindVisualChild<T>(DependencyObject elementCible) where T : DependencyObject
        {
            try
            {
                var count = VisualTreeHelper.GetChildrenCount(elementCible);
                if (count == 0) return null;

                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(elementCible, i);
                    if (child != null && child is T t)
                    {
                        return t;
                    }
                    else
                    {
                        T childOfChild = FindVisualChild<T>(child);
                        if (childOfChild != null)
                            return childOfChild;
                    }
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static T FindVisualAncestor<T>(DependencyObject elementCible) where T : DependencyObject
        {
            try
            {
                DependencyObject parent = elementCible;
                while (parent != null)
                {
                    parent = VisualTreeHelper.GetParent(parent);
                    if (parent == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (parent is T t)
                        {
                            return t;
                        }
                    }
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }

}
