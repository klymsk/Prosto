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
    public class CartItemsViewModel : BaseViewModel
    {
        public ObservableCollection<CartItem> CartItems { get; } = new();
        private readonly ApiService _apiService = new();

        private int userId;
        public int UserId
        {
            get => userId;
            set
            {
                if (userId == value) return;
                userId = value;
                OnPropertyChanged();
            }
        }

        private int itemId;
        public int ItemId
        {
            get => itemId;
            set
            {
                if (itemId == value) return;
                itemId = value;
                OnPropertyChanged();
            }
        }

        private int quantity;
        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity == value) return;
                quantity = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteCartItemCommand { get; }
        public ICommand AddCartItemCommand { get; }

        public CartItemsViewModel()
        {
            DeleteCartItemCommand = new Command<CartItem>(async (cartItem) => await OnDeleteCartItem(cartItem));
            AddCartItemCommand = new Command(async () => await OnAddCartItem());
        }

        public async Task LoadCartItemsAsync()
        {
            if (IsBusy)
            {
                return;
            }
            try
            {
                IsBusy = true;
                CartItems.Clear();

                var cartItems = await _apiService.GetCartItemsAsync();
                foreach (var cartItem in cartItems)
                    CartItems.Add(cartItem);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnDeleteCartItem(CartItem cartItem)
        {
            if (cartItem == null) return;

            IsBusy = true;
            try
            {
                bool success = await _apiService.DeleteCartItemAsync(cartItem.Id);
                if (success) CartItems.Remove(cartItem);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnAddCartItem()
        {
            if (IsBusy) return;

            if (UserId <= 0 ||  ItemId <= 0 || Quantity <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Будь ласка, заповніть всі поля коректно", "OK");
                return;
            }

            var newCartItem = new CartItem
            {
                UserId = UserId,
                ItemId = ItemId,
                Quantity = Quantity
            };

            IsBusy = true;
            try
            {
                var addedCartItem = await _apiService.AddCartItemAsync(newCartItem);
                if (addedCartItem != null)
                {
                    CartItems.Add(addedCartItem);

                    UserId = 0;
                    ItemId = 0;
                    Quantity = 0;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}