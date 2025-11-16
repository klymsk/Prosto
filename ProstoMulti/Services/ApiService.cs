using ProstoMulti.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProstoMulti.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient();

            #if ANDROID
                _client.BaseAddress = new Uri("http://10.0.2.2:5297/api/v1/");
            #else
                _client.BaseAddress = new Uri("http://localhost:5297/api/v1/");
            #endif
        }

        public async Task<List<Item>> GetItemsAsync()
            => await _client.GetFromJsonAsync<List<Item>>("Item");

        public async Task<List<Customer>> GetCustomersAsync()
            => await _client.GetFromJsonAsync<List<Customer>>("UserProfile");

        public async Task<List<CartItem>> GetCartItemsAsync()
            => await _client.GetFromJsonAsync<List<CartItem>>("CartItem");


        public async Task<bool> DeleteItemAsync(int id)
        {
            var response = await _client.DeleteAsync($"Item/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCartItemAsync(int id)
        {
            var response = await _client.DeleteAsync($"Cartitem/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var response = await _client.DeleteAsync($"Userprofile/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<Item> AddItemAsync(Item item)
        {
            var response = await _client.PostAsJsonAsync("Item", item);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Item>();
            }
            return null;
        }

        public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
        {
            var response = await _client.PostAsJsonAsync("Cartitem", cartItem);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CartItem>();
            }
            return null;
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            var response = await _client.PostAsJsonAsync("Userprofile", customer);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Customer>();
            }
            return null;
        }
    }
}
