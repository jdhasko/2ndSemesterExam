﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;
using Windows.UI.Xaml.Controls;
using AdministratorApp.Annotations;
using AdministratorApp.Models;
using AdministratorApp.Views;
using CommonLibrary.Models;
using GalaSoft.MvvmLight.Command;

namespace AdministratorApp.ViewModels
{
    public class StockPageVM : INotifyPropertyChanged
    {
       
        //Instance fields
        private Tuple<Item, string> _selectedItem;
        private List<Item> _items;
        private float _priceAfterDiscount;
        private string _errorMessage;
        private List<Stock> _selectedStocks = new List<Stock>();
        private string _filterString = "";
        private List<Category> _selectedCategories = new List<Category>();
        private Category _selectedItemCategory;
        private ObservableCollection<Tuple<Stock, StockHasItems>> _selectedItemInStocks = new ObservableCollection<Tuple<Stock, StockHasItems>>();


        /// <summary>
        /// Constructor to Load data from database, Update the ViewModel in VMHandler and to initialise the RelayCommands;
        /// </summary>
        public StockPageVM()
        {
            LoadDataAsync();
            VMHandler.StockPageVm = this;
            GoToAddItem = new RelayCommand(NavigateToAddItemPage);
            DeselectItemCommand = new RelayCommand(DeselectItem);
            DeleteItemCommand=new RelayCommand(DeleteItem);
            NavigateToAddItemToStockCommand = new RelayCommand(NavigateToAddItemToStock);
            DoToggleIDSort = new RelayCommand(ToggleIDSort);
            DoToggleNameSort = new RelayCommand(ToggleNameSort);
            DoToggleTypeSort = new RelayCommand(ToggleTypeSort);
            DoTogglePriceSort = new RelayCommand(TogglePriceSort);
        }

        
        /// Properties
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }
        public RelayCommand DeleteItemCommand { get; }
        public RelayCommand DeselectItemCommand { get; }
        public RelayCommand GoToAddItem { get; }
        public RelayCommand NavigateToAddItemToStockCommand { get; }

        public static Dictionary<int, Dictionary<int, int>> StockHasItems
        {
            get => Data.StockHasItems;
            set => Data.StockHasItems = value;
        }

        public ObservableCollection<Category> AllCategories
        {
            get => new ObservableCollection<Category>(Data.AllCategories.Values);
        }

        public List<Stock> SelectedStocks
        {
            get => _selectedStocks;
            set { _selectedStocks = value; OnPropertyChanged(); OnPropertyChanged(nameof(FilteredItems)); }
        }

        public Tuple<Item, string> SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                UpdateSelectedItemInStocks();
                if (SelectedItem == null)
                {
                    _selectedItemCategory = null;
                }
                else _selectedItemCategory = new Category(_selectedItem.Item1.CategoryId, SelectedItem.Item2); 
                OnPropertyChanged(nameof(SelectedItemInStocks));
                OnPropertyChanged(nameof(PriceAfterDiscount));
                OnPropertyChanged(nameof(SelectedItemCategory));

            }
        }

        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                OnPropertyChanged(nameof(FilteredItems));
            }
        }

        public Constants.SortBy SortBy { get; set; } = Constants.SortBy.IDDescending;

        public RelayCommand DoToggleIDSort { get; set; }
        public RelayCommand DoToggleNameSort { get; set; }
        public RelayCommand DoToggleTypeSort { get; set; }
        public RelayCommand DoTogglePriceSort { get; set; }

        public async void ToggleIDSort()
        {
            if (SortBy != Constants.SortBy.IDDescending && SortBy != Constants.SortBy.IDAscending)
            {
                SortBy = Constants.SortBy.IDDescending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
                OnPropertyChanged(nameof(SelectedItem));
                return;
            }

            if (SortBy == Constants.SortBy.IDDescending)
            {
                SortBy = Constants.SortBy.IDAscending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
            }

            else
            {
                SortBy = Constants.SortBy.IDDescending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
            }
        }

        public async void ToggleNameSort()
        {
            if (SortBy != Constants.SortBy.NameDescending && SortBy != Constants.SortBy.NameAscending)
            {
                SortBy = Constants.SortBy.NameDescending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
                return;
            }

            if (SortBy == Constants.SortBy.NameDescending) 
            { 
                SortBy = Constants.SortBy.NameAscending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
            }


            else
            {
                SortBy = Constants.SortBy.NameDescending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
            }
               
        }

        public async void ToggleTypeSort()
        {
            if (SortBy != Constants.SortBy.TypeDescending && SortBy != Constants.SortBy.TypeAscending)
            {
                SortBy = Constants.SortBy.TypeDescending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
                return;
            }

            if (SortBy == Constants.SortBy.TypeDescending)
            {
                SortBy = Constants.SortBy.TypeAscending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
            }


            else
            {
                SortBy = Constants.SortBy.TypeDescending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
            }
                
        }

        public async void TogglePriceSort()
        {
            if (SortBy != Constants.SortBy.PriceDescending && SortBy != Constants.SortBy.PriceAscending)
            {
                SortBy = Constants.SortBy.PriceDescending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];

                return;
            }

            if (SortBy == Constants.SortBy.PriceDescending)
            {
                SortBy = Constants.SortBy.PriceAscending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
            }


            else
            {
                SortBy = Constants.SortBy.PriceDescending;
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];

            }
             
        }

        public ObservableCollection<Tuple<Item, string>> FilteredItems
        {
            get
            {
                ObservableCollection<Tuple<Item, string>> items = new ObservableCollection<Tuple<Item, string>>();
                List<Item> filteredList = new List<Item>();
                List<Tuple<Item, int>> itemsToFilter = new List<Tuple<Item, int>>();
                foreach (Item item in Data.AllItems.Values)
                {
                    itemsToFilter.Add(new Tuple<Item, int>(item, item.CategoryId));
                }
                filteredList = CommonMethods.FilterListByCategories(itemsToFilter, SelectedCategories);
                List<Tuple<Item, List<int>>> itemsToFilter2 = new List<Tuple<Item, List<int>>>();
                foreach (Item item in filteredList)
                {
                    if (Data.ItemsInStocks.Count == 0)
                        break;
                    List<int> itemStocks = new List<int>();

                    if (Data.ItemsInStocks.ContainsKey(item.Id))
                    {
                        foreach (KeyValuePair<int, int> pair in Data.ItemsInStocks[item.Id])
                        {
                            itemStocks.Add(pair.Key);
                        }
                    }

                    itemsToFilter2.Add(new Tuple<Item, List<int>>(item, itemStocks));
                }

                filteredList = CommonMethods.FilterListByStock(itemsToFilter2, SelectedStocks);

                filteredList = CommonMethods.FilterListByString(filteredList, FilterString);

                foreach (Item item in filteredList)
                {
                    items.Add(new Tuple<Item, string>(item, Data.AllCategories.Count != 0 ? Data.AllCategories[item.CategoryId]?.Name : "No Category."));
                }

                switch (SortBy)
                {
                    case Constants.SortBy.IDAscending:
                        items = new ObservableCollection<Tuple<Item, string>>(items.OrderBy(x => x.Item1.Id).ToList());
                        break;
                    case Constants.SortBy.NameAscending:
                        items = new ObservableCollection<Tuple<Item, string>>(items.OrderBy(x => x.Item1.Name).ToList());
                        break;
                    case Constants.SortBy.TypeAscending:
                        items = new ObservableCollection<Tuple<Item, string>>(items.OrderBy(x => Data.AllCategories[x.Item1.CategoryId].Name).ToList());
                        break;
                    case Constants.SortBy.PriceAscending:
                        items = new ObservableCollection<Tuple<Item, string>>(items.OrderBy(x => x.Item1.Price).ToList());
                        break;
                    case Constants.SortBy.IDDescending:
                        items = new ObservableCollection<Tuple<Item, string>>(items.OrderByDescending(x => x.Item1.Id).ToList());
                        break;
                    case Constants.SortBy.NameDescending:
                        items = new ObservableCollection<Tuple<Item, string>>(items.OrderByDescending(x => x.Item1.Name).ToList());
                        break;
                    case Constants.SortBy.TypeDescending:
                        items = new ObservableCollection<Tuple<Item, string>>(items.OrderByDescending(x => Data.AllCategories[x.Item1.CategoryId].Name).ToList());
                        break;
                    case Constants.SortBy.PriceDescending:
                        items = new ObservableCollection<Tuple<Item, string>>(items.OrderByDescending(x => x.Item1.Price).ToList());
                        break;
                    default:
                        items = new ObservableCollection<Tuple<Item, string>>(items.OrderByDescending(x => x.Item1.Id).ToList());
                        break;
                }

                return items;
            }
        }

        public float PriceAfterDiscount
        {
            get
            {
                if (SelectedItem != null)
                {
                    //You can only pay integer amounts in hungary therefore it's rounded to int.
                    return _priceAfterDiscount =
                        Convert.ToInt32(SelectedItem.Item1.Price * (1 - SelectedItem.Item1.DiscountPercentage / 100));
                }

                return 0;
            }
        }



        public ObservableCollection<Store> Stores
        {
            get => new ObservableCollection<Store>(Data.AllStores.Values);
        }

        public ObservableCollection<Stock> Stocks
        {
            get => new ObservableCollection<Stock>(Data.AllStocks.Values);
        }

        public Category SelectedItemCategory
        {
            get => _selectedItemCategory;
            
            set { _selectedItemCategory = value; OnPropertyChanged(); }
        }

        public List<Category> SelectedCategories
        {
            get { return _selectedCategories; }
            set
            {
                _selectedCategories = value;
                OnPropertyChanged(nameof(FilteredItems));
            }
        }

        public ObservableCollection<Tuple<Stock, StockHasItems>> SelectedItemInStocks
        {
            get
            { 
                return _selectedItemInStocks;
            }
            set
            {
                _selectedItemInStocks = value; 
                OnPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get { return AuthHandler.ShowAdministratorFunctions; }
        }


        /// <summary>
        /// Method which is responsible for updating selected item in stock.
        /// </summary>
        public void UpdateSelectedItemInStocks()
        {

            ObservableCollection<Tuple<Stock, StockHasItems>> stocks = new ObservableCollection<Tuple<Stock, StockHasItems>>();

            if (SelectedItem == null)
            {
                SelectedItemInStocks = stocks;
                return;
            }

            if (!Data.ItemsInStocks.ContainsKey(SelectedItem.Item1.Id))
                stocks.Add(new Tuple<Stock, StockHasItems>(new Stock(0, "This item has not been placed to any store yet.\t\t\t\t"), new StockHasItems(0, SelectedItem.Item1.Id, 0)));
            
            if (Data.ItemsInStocks.ContainsKey(SelectedItem.Item1.Id))
            {
                foreach (KeyValuePair<int, int> pair in Data.ItemsInStocks[SelectedItem.Item1.Id])
                {
                    stocks.Add(new Tuple<Stock, StockHasItems>(Data.AllStocks[pair.Key], new StockHasItems(pair.Key, SelectedItem.Item1.Id, pair.Value)));
                }

            }
            SelectedItemInStocks = stocks;

        }

        /// <summary>
        /// Method which deselects the selected item.
        /// </summary>
        private void DeselectItem()
        {
            SelectedItem = null;
        }

        /// <summary>
        /// Method for saving edited item to database.
        /// </summary>
        /// <returns>Returns a boolean value. It returns true value if the save was successful. </returns>
        public async Task<bool> SaveEdit()
        {
            try
            {
                if (CheckFields())
                {
                    if (SelectedItem.Item1.DiscountPercentage <= 100)
                    {
                        await APIHandler<Item>.PutOne($"items/{SelectedItem.Item1.Id}",
                            new Item(SelectedItem.Item1.Id, SelectedItem.Item1.Name, SelectedItem.Item1.Price,
                                SelectedItem.Item1.Comment, SelectedItem.Item1.PictureSource,
                                SelectedItem.Item1.Barcode,
                                SelectedItem.Item1.Color, SelectedItem.Item1.Size, SelectedItemCategory.ID,
                                SelectedItem.Item1.DiscountPercentage));
                        foreach (var ItemInStock in SelectedItemInStocks )
                        {
                            if (!ItemInStock.Equals( new Tuple<Stock, StockHasItems>(new Stock(0, "This item has not been placed to any store yet.\t\t\t\t"), new StockHasItems(0, SelectedItem.Item1.Id, 0))))
                            {
                                await APIHandler<StockHasItems>.PutOne($"StockHasItems/{ItemInStock.Item1.ID}/{SelectedItem.Item1.Id}", ItemInStock.Item2);

                            }
                        }  

                        await LoadDataAsync();
                        return true;
                    }
                    else ErrorMessage = "Discount cannot be more than 100%";

                    return false;
                }

               else ErrorMessage = "All required fields must be filled out!"; return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        /// <summary>
        /// Method responsible for checking if all the required fields are filled out
        /// </summary>
        /// <returns>Returns a boolean value, if all the fields are filled out returns true</returns>
        private bool CheckFields()
        {
            bool experssion = !string.IsNullOrEmpty(SelectedItem.Item1.Name) && SelectedItem.Item1.Price != 0 &&
                              SelectedItem.Item1.Barcode != 0 && !string.IsNullOrEmpty(SelectedItem.Item1.Color) &&
                              !string.IsNullOrEmpty(SelectedItem.Item1.Size) && SelectedItemCategory != null;

            if (experssion)
            {
                return true;
            }

            return false;
        }



        /// <summary>
        /// Method responsible for deleting item from the database. It used the API handler.
        /// </summary>
        public async void DeleteItem()
        {
            if (SelectedItem != null)
            {
                await APIHandler<Item>.DeleteOne($"items/{SelectedItem.Item1.Id}");
                await Data.UpdateItems();
                OnPropertyChanged(nameof(FilteredItems));
                SelectedItem = FilteredItems[0];
            }

        }


        /// <summary>
        /// Method responsible for loading all necessary data from the database, and updates properties.
        /// </summary>
        /// <returns></returns>
        private async Task LoadDataAsync()
        {
            await Data.UpdateCategories();
            await Data.UpdateItems();
            await Data.UpdateStock();
            await Data.UpdateStore();
            await Data.UpdateItemsInStocks();
            OnPropertyChanged(nameof(FilteredItems));
            OnPropertyChanged(nameof(AllCategories));
            SelectedItem = FilteredItems[0];
            OnPropertyChanged(nameof(SelectedItemCategory));
            OnPropertyChanged(nameof(Stores));
            OnPropertyChanged(nameof(Stocks));
            OnPropertyChanged(nameof(SelectedItemInStocks));
            OnPropertyChanged(nameof(PriceAfterDiscount));

            UpdateSelectedItemInStocks();
            
        }

        /// <summary>
        /// Method which navigates the frame to AddItemToStock page
        /// </summary>
        private void NavigateToAddItemToStock()
        {
            NavigationHandler.NavigateToPage(typeof(AddItemToStockPage));
        }

        /// <summary>
        /// Method which navigates the frame to AddItem page
        /// </summary>
        private void NavigateToAddItemPage()
        {
            NavigationHandler.NavigateToPage(typeof(AddItemPage));
        }

        

        


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
