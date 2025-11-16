using ProstoMulti.Models;
using ProstoMulti.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProstoMulti.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; } = new();
        private readonly ApiService _apiService = new();

        private int sellerId;
        public int SellerId
        {
            get => sellerId;
            set
            {
                if (sellerId == value) return;
                sellerId = value;
                OnPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                OnPropertyChanged();
            }
        }

        private string description;
        public string Description
        {
            get => description;
            set
            {
                if (description == value) return;
                description = value;
                OnPropertyChanged();
            }
        }

        private string category;
        public string Category
        {
            get => category;
            set
            {
                if (category == value) return;
                category = value;
                OnPropertyChanged();
            }
        }

        private decimal price;
        public decimal Price
        {
            get => price;
            set
            {
                if (price == value) return;
                price = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteItemCommand { get; }
        public ICommand AddItemCommand { get; }

        public ItemsViewModel()
        {
            DeleteItemCommand = new Command<Item>(async (item) => await OnDeleteItem(item));
            AddItemCommand = new Command(async () => await OnAddItem());
        }


        public async Task LoadItemsAsync()
        {
            if (IsBusy) 
            { 
                return;
            }
            try
            {
                IsBusy = true;
                Items.Clear();
                var items = await _apiService.GetItemsAsync();
                foreach (var item in items)
                    Items.Add(item);
            }
            finally 
            { 
                IsBusy = false; 
            }
        }

        private async Task OnDeleteItem(Item item)
        {
            if (item == null) return;

            IsBusy = true;
            try
            {
                bool success = await _apiService.DeleteItemAsync(item.ItemId);
                if (success) Items.Remove(item);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnAddItem()
        {
            if (IsBusy) return;

            if (SellerId <= 0 || string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description)
               || string.IsNullOrWhiteSpace(Category) || Price <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Будь ласка, заповніть всі поля коректно", "OK");
                return;
            }

            var newItem = new Item
            {
                SellerId = SellerId,
                Name = Name,
                Description = Description,
                Category = Category,
                Price = Price
            };

            IsBusy = true;
            try
            {
                var addedItem = await _apiService.AddItemAsync(newItem);
                if (addedItem != null)
                {
                    Items.Add(addedItem);

                    SellerId = 0;
                    Name = string.Empty;
                    Description = string.Empty;
                    Category = string.Empty;
                    Price = 0;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}