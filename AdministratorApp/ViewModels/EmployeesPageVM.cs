﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using AdministratorApp.Models;
using CommonLibrary.Models;
using GalaSoft.MvvmLight.Command;

namespace AdministratorApp.ViewModels
{
    class EmployeesPageVM : INotifyPropertyChanged
    {
        ObservableCollection<User> _users = new ObservableCollection<User>();
        private User _sEmp =  new User();
        private ObservableCollection<string> _stores = new ObservableCollection<string>();
        private ObservableCollection<Role> _roles = new ObservableCollection<Role>();
        private Salary _objSalary;

        private string _name = "";
        private string _address = "";
        private int _telephone = 0;
        private string _role = "";
        private bool _IES = false;
        private string _selectedRole = "";
        private float _salary;
        private float _salaryWTax;
        private int _userId = -1;
        private float _tajNumber;
        private float _taxNumber;
        private float _workingHours;
        private string _selectedStore;
        private string _userName;
        private string _email;

        private string _errorText = "You must first select a user\nbefore trying to delete one";

        public EmployeesPageVM()
        {
            LoadDataAsync();
            DoShowUserName = new RelayCommand(GetUserName);
            DoCancel = new RelayCommand(Cancel);
            DoDelete = new RelayCommand(DeleteUser);
        }

        public Dictionary<int, User> DictUsers
        {
            get { return Data.AllUsers; }
            set { Data.AllUsers = value; 
                OnPropertyChanged(); }
        }

        public RelayCommand DoShowUserName { get; set; }
        public RelayCommand DoCancel { get; set; }
        public RelayCommand DoDelete { get; set; }
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value; OnPropertyChanged(); }
        }
        public Dictionary<int, Salary> DictSalaries
        {
            get { return Data.AllSalaries; }
            set { Data.AllSalaries = value; OnPropertyChanged(); }
        }

        public Dictionary<int, Store> DictStore
        {
            get { return Data.AllStores; }
        }

        public ObservableCollection<User> Users
        {
            get
            {
                ObservableCollection<User> users = new ObservableCollection<User>(Data.AllUsers.Values);
                return users;
            }
            set
            {
                _users = value; OnPropertyChanged();
            }
        }

        public User SelectedEmp
        {
            set { _sEmp = value;
                if (_sEmp != null)
                {
                    Name = _sEmp.Name;
                    Telephone = _sEmp.Phone;
                    Address = _sEmp.Address;
                    SelectedRole = null;
                    _userId = _sEmp.Id;
                    _objSalary = CommonMethods.GetSalary(_userId, DictSalaries);
                    Role = CommonMethods.GetRole(_sEmp.RoleId, Data.AllRoles).Name;
                    Salary = _objSalary.BeforeTax;
                    SalaryWTax = _objSalary.BeforeTax - (_objSalary.BeforeTax * (_objSalary.TaxPercentage / 100));
                    IsEmployeeSelected = true;
                    TajNumber = _sEmp.TAJNumber;
                    TaxNumber = _sEmp.TAXNumber;
                    WorkingHours = _sEmp.WorkingHours;
                    SelectedStore = DictStore[_sEmp.StoreId].Name;
                    Email = _sEmp.Email;
                    UserName = "";
                }
                OnPropertyChanged();}
            get { return _sEmp; }
        }

        public string Name
        {
            set { _name = value; OnPropertyChanged(); }
            get { return _name; }
        }
        public string Address
        {
            set { _address = value; OnPropertyChanged(); }
            get { return _address; }
        }

        public int Telephone
        {
            set { _telephone = value; OnPropertyChanged();}
            get { return _telephone; }
        }

        public string Role
        {
            get { return _role; }
            set { _role = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Stores
        {
            get { return _stores; }
            set { _stores = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Role> Roles
        {
            get { return _roles; }
            set { _roles = value; OnPropertyChanged(); }

        }

        public bool IsEmployeeSelected
        {
            get { return _IES; }
            set { _IES = value; OnPropertyChanged(); }
        }

        public string SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                Role = value;
                _selectedRole = value; OnPropertyChanged(); }
        }

        public float Salary
        {
            get { return _salary; }
            set { _salary = value; OnPropertyChanged(); }
        }

        public float SalaryWTax
        {
            get { return _salaryWTax; }
            set { _salaryWTax = value; OnPropertyChanged(); }
        }

        public float TajNumber
        {
            get { return _tajNumber; }
            set { _tajNumber = value; OnPropertyChanged();}
        }

        public float TaxNumber
        {
            get { return _taxNumber; }
            set { _taxNumber = value; OnPropertyChanged();}
        }

        public float WorkingHours
        {
            get { return _workingHours; }
            set { _workingHours = value; OnPropertyChanged();}
        }

        public string SelectedStore
        {
            get { return _selectedStore; }
            set { _selectedStore = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }
        }

        public string ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; OnPropertyChanged(); }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task LoadDataAsync()
        {
            await Data.UpdateUsers();
            await Data.UpdateSalaries();
            await Data.UpdateRoles();
            await Data.UpdateStore();
            Roles = new ObservableCollection<Role>(Data.AllRoles.Values);

            foreach (Store s in DictStore.Values)
            {
                Stores.Add(s.Name);
            }
            OnPropertyChanged(nameof(Users));
            OnPropertyChanged(nameof(DictUsers));
        }

        private async void GetUserName()
        {
            if (_userId != -1)
            {
                UserName = await APIHandler<string>.GetOne($"auth/getusername/{_userId}");
            }
        }

        public void Cancel()
        {
            Name = "";
            Telephone = 0;
            Address = "";
            SelectedRole = null;
            _userId = 0;
            _objSalary = null;
            Role = null;
            Salary = 0;
            SalaryWTax = 0;
            IsEmployeeSelected = false;
            TajNumber = 0;
            TaxNumber = 0;
            WorkingHours = 0;
            SelectedStore = "";
            Email = "";
            UserName = "";
            SelectedEmp = null;
            SelectedStore = null;
        }

        public async void DeleteUser()
        {
            int id = SelectedEmp.Id;
            if (SelectedEmp != null)
            {
                if (SelectedEmp.Id != AuthHandler.UserID)
                {
                    if (SelectedEmp.UserLevelId != Data.OwnerID)
                    {
                        if (AuthHandler.ActiveUser.UserLevelId > SelectedEmp.UserLevelId)
                        {
                            //Auth auth = await APIHandler<Auth>.DeleteOne($"Auths/DeleteUserAuth/{id}");
                            //Salary salary = await APIHandler<Salary>.DeleteOne($"Salaries/DeleteSalary/{id}");
                            User user = await APIHandler<User>.DeleteOne($"Users/DeleteUser/{id}");
                            ErrorText = CommonMethods.SetErrorTextOnDelete(user.Id == -1 
                                ? Constants.UserDeleteErorrs.DELETE_ID_0
                                : Constants.UserDeleteErorrs.OK);
                            Cancel();
                            await LoadDataAsync();
                        }
                        else
                            ErrorText = CommonMethods.SetErrorTextOnDelete(Constants.UserDeleteErorrs.LOW_ACCESS_LEVEL);
                    }
                    else
                        ErrorText = CommonMethods.SetErrorTextOnDelete(Constants.UserDeleteErorrs.DELETE_OWNER);
                }
                else
                    ErrorText = CommonMethods.SetErrorTextOnDelete(Constants.UserDeleteErorrs.USER_LOGGED_IN);
            }
            else
                ErrorText = CommonMethods.SetErrorTextOnDelete(Constants.UserDeleteErorrs.NO_SELECTED_USER);
        }


    }
}
