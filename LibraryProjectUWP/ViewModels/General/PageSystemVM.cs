using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LibraryProjectUWP.ViewModels.General
{
    public class PageSystemVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private int _CurrentPage;
        public int CurrentPage
        {
            get => this._CurrentPage;
            set
            {
                if (_CurrentPage != value)
                {
                    this._CurrentPage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Brush _BackgroundColor = Application.Current.Resources["PageSelectedBackground"] as SolidColorBrush;
        public Brush BackgroundColor
        {
            get => this._BackgroundColor;
            set
            {
                if (_BackgroundColor != value)
                {
                    this._BackgroundColor = value;
                    this.OnPropertyChanged();
                }
            }
        }

        
        private bool _IsPageSelected;
        public bool IsPageSelected
        {
            get => this._IsPageSelected;
            set
            {
                if (_IsPageSelected != value)
                {
                    this._IsPageSelected = value;
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
