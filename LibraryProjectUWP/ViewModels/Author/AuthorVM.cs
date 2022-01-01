﻿using LibraryProjectUWP.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.Author
{
    internal class AuthorVM : ContactVM
    {
        private DateTime? _DateNaissance = DateTime.UtcNow.AddYears(-20);
        public DateTime? DateNaissance
        {
            get => _DateNaissance;
            set
            {
                if (_DateNaissance != value)
                {
                    _DateNaissance = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _DateDeces = DateTime.UtcNow;
        public DateTime? DateDeces
        {
            get => _DateDeces;
            set
            {
                if (_DateDeces != value)
                {
                    _DateDeces = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _LieuNaissance;
        public string LieuNaissance
        {
            get => _LieuNaissance;
            set
            {
                if (_LieuNaissance != value)
                {
                    _LieuNaissance = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _LieuDeces;
        public string LieuDeces
        {
            get => _LieuDeces;
            set
            {
                if (_LieuDeces != value)
                {
                    _LieuDeces = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Notes;
        public string Notes
        {
            get => _Notes;
            set
            {
                if (_Notes != value)
                {
                    _Notes = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Biographie;
        public string Biographie
        {
            get => _Biographie;
            set
            {
                if (_Biographie != value)
                {
                    _Biographie = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}