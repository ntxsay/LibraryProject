﻿using LibraryProjectUWP.Views.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.General
{
    public class LibraryBookNavigationDriverVM
    {
        public MainPage MainPage { get; set; }
        public BibliothequeVM ParentLibrary { get; set; }
        public BookCollectionPage ParentPage { get; set; }
    }

}
