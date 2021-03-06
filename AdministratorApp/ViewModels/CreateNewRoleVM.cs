﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using AdministratorApp.Annotations;
using AdministratorApp.Models;
using CommonLibrary.Models;
using GalaSoft.MvvmLight.Command;

namespace AdministratorApp.ViewModels
{
    class CreateNewRoleVM : INotifyPropertyChanged
    {
        private string _role;
        private string _errorText;
        

        public CreateNewRoleVM()
        {
        }

        public string Role
        {
            get { return _role; }
            set { _role = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Method that when called updates the roles in Data.cs
        /// </summary>
        public async void LoadDataAsync()
        {
           await Data.UpdateRoles();
        }

        /// <summary>
        /// A method that checks the inputted string for errors
        /// </summary>
        /// <returns>returns an error from the enum RoleErrors that is located within Constants.cs</returns>
        public Constants.RoleErrors CheckRoleForErrors()
        {
            LoadDataAsync();
            if (!Role.All(Constants.AllowedCharacters.Contains))
            {
                return Constants.RoleErrors.INCORRECT_FORMAT;
            }
            foreach (Role r in Data.AllRoles.Values)
            {
                if (r.Name.ToLower() == Role.ToLower())
                {
                    return Constants.RoleErrors.ROLE_EXISTS;
                }
            }

            return Constants.RoleErrors.OK;
        }

        /// <summary>
        /// A method that when called will return a string for error text depending on the provided error
        /// </summary>
        /// <param name="roleErrors">a RoleError that comes from Constants.RoleErrors</param>
        /// <returns>A string that says what the error is</returns>
        public string SetErrorText(Constants.RoleErrors roleErrors)
        {
            if (roleErrors == Constants.RoleErrors.INCORRECT_FORMAT)
            {
                return "The entered role name contains characters \noutside of the English and Hungarian alphabet.";
            }

            if (roleErrors == Constants.RoleErrors.ROLE_EXISTS)
            {
                return "The entered role name already exists";
            }

            return "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
