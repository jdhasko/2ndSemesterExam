﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AdministratorApp.Annotations;
using AdministratorApp.Models;
using CommonLibrary.Models;
using GalaSoft.MvvmLight.Command;

namespace AdministratorApp.ViewModels
{
    class RequestsVM : INotifyPropertyChanged
    {
        private Tuple<Invoice, string, string> _selectedInvoice;
        private List<Store> _selectedStores = new List<Store>();
        private string _filterString = "";


        public RequestsVM()
        {
            LoadData();
            DoAcceptInvoice = new RelayCommand(AcceptInvoice);
            DoDenyInvoice = new RelayCommand(DenyInvoice);
        }

        public RelayCommand DoAcceptInvoice { get; set; }
        public RelayCommand DoDenyInvoice { get; set; }

        public string AdminComment { get; set; }

        public bool EnableButtons
        {
            get { return SelectedInvoice != null; }
        }

        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                OnPropertyChanged(nameof(FilteredInvoices));
            }
        }

        public ObservableCollection<Store> AllStores
        {
            get => new ObservableCollection<Store>(Data.AllStores.Values);
        }

        public List<Store> SelectedStores
        {
            get { return _selectedStores; }
            set
            {
                _selectedStores = value;
                OnPropertyChanged(nameof(FilteredInvoices));
            }
        }

        public User SelectedInvoiceAuthor
        {
            get
            {
                if (SelectedInvoice != null)
                    return Data.AllUsers[SelectedInvoice.Item1.AuthorID];
                return null;
            }
        }

        public ObservableCollection<Tuple<Invoice, string, string>> FilteredInvoices
        {
            get
            {
                ObservableCollection<Tuple<Invoice, string, string>> invoices = new ObservableCollection<Tuple<Invoice, string, string>>();
                List<Invoice> filteredList = new List<Invoice>();
                List<Tuple<Invoice, string>> listToFilter = new List<Tuple<Invoice, string>>();


                if (SelectedStores.Count == 0)
                {
                    foreach (KeyValuePair<int, Invoice> pair in Data.AllInvoices
                        .Where(i => i.Value.InvoiceStatusID == (int) Constants.InvoiceStatus.Open).ToList())
                    {
                        listToFilter.Add(
                            new Tuple<Invoice, string>(pair.Value, Data.AllUsers[pair.Value.AuthorID].Name));
                    }

                    filteredList = CommonMethods.FilterListByString(listToFilter, FilterString);
                }
                else
                {
                    List<Tuple<Invoice, int>> invoicesToFilter = new List<Tuple<Invoice, int>>();
                    foreach (Invoice invoice in Data.AllInvoices.Values.Where(i => i.InvoiceStatusID == (int)Constants.InvoiceStatus.Open).ToList())
                    {
                        invoicesToFilter.Add(new Tuple<Invoice, int>(invoice, invoice.StoreID));
                    }

                    foreach (Tuple<Invoice, int> pair in invoicesToFilter)
                    {
                        listToFilter.Add(new Tuple<Invoice, string>(pair.Item1, Data.AllUsers[pair.Item1.AuthorID].Name));
                    }

                    filteredList = CommonMethods.FilterListByString(CommonMethods.FilterListByStores(invoicesToFilter, SelectedStores), FilterString);
                }

                foreach (Invoice invoice in filteredList)
                {
                    invoices.Add(new Tuple<Invoice, string, string>(invoice, Data.AllUsers[invoice.AuthorID].Name, Data.AllStores[invoice.StoreID].Name));
                }

                return invoices;
            }
        }

        public Tuple<Invoice, string, string> SelectedInvoice
        {
            get { return _selectedInvoice; }
            set
            {
                _selectedInvoice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedInvoiceItems));
                OnPropertyChanged(nameof(SelectedInvoiceAuthor));
                OnPropertyChanged(nameof(EnableButtons));
            }
        }

        public ObservableCollection<Tuple<Item, int>> SelectedInvoiceItems
        {
            get
            {
                ObservableCollection<Tuple<Item, int>> items = new ObservableCollection<Tuple<Item, int>>();
                if (SelectedInvoice != null)
                {
                    foreach (KeyValuePair<int, int> value in Data.InvoiceHasItems[SelectedInvoice.Item1.ID])
                    {
                        items.Add(new Tuple<Item, int>(Data.AllItems[value.Key], value.Value));
                    }
                }

                return items;
            }
        }

        public async void AcceptInvoice()
        {
            Data.AllInvoices[SelectedInvoice.Item1.ID].InvoiceStatusID = (int) Constants.InvoiceStatus.Accepted;
            Data.AllInvoices[SelectedInvoice.Item1.ID].AdminComment = AdminComment;
            await APIHandler<Invoice>.PutOne($"Invoices/{SelectedInvoice.Item1.ID}", Data.AllInvoices[SelectedInvoice.Item1.ID]);
            SelectedInvoice = null;
            AdminComment = "";
            OnPropertyChanged(nameof(AdminComment));
            OnPropertyChanged(nameof(FilteredInvoices));
        }

        public async void DenyInvoice()
        {
            Data.AllInvoices[SelectedInvoice.Item1.ID].InvoiceStatusID = (int)Constants.InvoiceStatus.Denied;
            Data.AllInvoices[SelectedInvoice.Item1.ID].AdminComment = AdminComment;
            await APIHandler<Invoice>.PutOne($"Invoices/{SelectedInvoice.Item1.ID}", Data.AllInvoices[SelectedInvoice.Item1.ID]);
            AdminComment = "";
            SelectedInvoice = null;
            OnPropertyChanged(nameof(AdminComment));
            OnPropertyChanged(nameof(FilteredInvoices));
        }

        public async Task LoadData()
        {
            await Data.UpdateUsers();
            await Data.UpdateItems();
            await Data.UpdateInvoices();
            await Data.UpdateInvoiceHasItems();
            await Data.UpdateStore();
            OnPropertyChanged(nameof(FilteredInvoices));
            SelectedInvoice = FilteredInvoices[0];
            OnPropertyChanged(nameof(AllStores));
            OnPropertyChanged(nameof(SelectedInvoice));
            OnPropertyChanged(nameof(SelectedInvoiceItems));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
