﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using AdministratorApp.Annotations;
using AdministratorApp.Models;
using CommonLibrary.Models;

namespace AdministratorApp.ViewModels
{
    public class CreateNewCategoryVM : INotifyPropertyChanged
    {

        //Instance fields
        private string _category;
        private string _errorText;


        public CreateNewCategoryVM()
        {}

        //Primitive properties
        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Method which is responsible for checking errors.
        /// </summary>
        /// <returns>Returns a true or false value. Returns true if no error was found</returns>
        public bool CheckErrors()
        {
            if (string.IsNullOrEmpty(Category))
            {
                ErrorText = "The category must have a name!";
                return false;
            }

            foreach (var c in Data.AllCategories.Values)
            {
                if (Category == c.Name)
                {
                    ErrorText = "Category with this name already exist!";
                    return false;
                }
            }


            return true;
        }

        
        /// <summary>
        /// Method for loading data from Database. It updates the categories
        /// </summary>
        public async void LoadDataAsync()
        {
            await Data.UpdateCategories();
        }




        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }
}
