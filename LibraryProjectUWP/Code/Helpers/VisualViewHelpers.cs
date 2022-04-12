using LibraryProjectUWP.Code.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace LibraryProjectUWP.Code.Helpers
{
    public class VisualViewHelpers
    {
        public class MainControlsUI
        {
            public static MainPage MainPage => (Window.Current.Content as Frame).Content as MainPage;
            public MainPage GetMainPage
            {
                get 
                {
                    try
                    {
                        if (Window.Current == null)
                        {
                            return null;
                        }

                        if (Window.Current.Content is Frame frame)
                        {
                            return frame.Content as MainPage;
                        }

                        return null;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
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

        public static T FindVisualChild<T>(DependencyObject elementCible, string childName) where T : DependencyObject
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
                        // If the child's name is set for search
                        if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                        {
                            return t;
                        }
                        else
                        {
                            T childOfChild = FindVisualChild<T>(child, childName);
                            if (childOfChild != null)
                                return childOfChild;
                        }
                    }
                    else
                    {
                        T childOfChild = FindVisualChild<T>(child, childName);
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

        public static IEnumerable<T> FindVisualChilds<T>(DependencyObject elementCible) where T : DependencyObject
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            try
            {
                var count = VisualTreeHelper.GetChildrenCount(elementCible);
                if (count == 0) return Enumerable.Empty<T>();

                List<T> result = new List<T>();
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(elementCible, i);
                    if (child != null && child is T t)
                    {
                        result.Add(t);
                    }
                    else
                    {
                        var elList = FindVisualChilds<T>(child);
                        if (elList != null && elList.Any())
                        {
                            result.AddRange(elList);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return Enumerable.Empty<T>();
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
