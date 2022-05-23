using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.General;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace LibraryProjectUWP.Code.Services.ES
{
    internal partial class EsAppBaseApi
    {
        internal const string LibraryDefaultJaquette = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string BookDefaultJaquette = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string BookDefaultBackgroundImage = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string BookCollectionDefaultBackgroundImage = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        internal const string LibraryCollectionDefaultBackgroundImage = "ms-appx:///Assets/Backgrounds/polynesia-3021072.jpg";
        public enum SearchOptions
        {
            StartWith,
            Contains,
            EndWith,
            Egal
        }

        public class DefaultPathName
        {
            public const string Libraries = "Libraries";
            public const string Books = "Books";
            public const string Contacts = "Contacts";
            [Obsolete]
            public const string Authors = "Authors";
        }

        public enum MainPathEnum
        {
            Libraries,
            Books,
            Contacts,
            Authors,
            Editors
        }
    }
}
