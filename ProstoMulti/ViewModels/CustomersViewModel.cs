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
    public class CustomersViewModel : BaseViewModel
    {
        public ObservableCollection<Customer> Customers { get; } = new();
        private readonly ApiService _apiService = new();

        private string? phoneNumber;
        public string? PhoneNumber
        {
            get => phoneNumber;
            set
            {
                if (phoneNumber == value) return;
                phoneNumber = value;
                OnPropertyChanged();
            }
        }

        private string? password;
        public string? Password
        {
            get => password;
            set
            {
                if (password == value) return;
                password = value;
                OnPropertyChanged();
            }
        }

        private string? fullName;
        public string? FullName
        {
            get => fullName;
            set
            {
                if (fullName == value) return;
                fullName = value;
                OnPropertyChanged();
            }
        }

        private string? email;
        public string? Email
        {
            get => email;
            set
            {
                if (email == value) return;
                email = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteCustomerCommand { get; }
        public ICommand AddCustomerCommand { get; }

        public CustomersViewModel()
        {
            DeleteCustomerCommand = new Command<Customer>(async (customer) => await OnDeleteCustomer(customer));
            AddCustomerCommand = new Command(async () => await OnAddCustomer());
        }

        public async Task LoadCustomersAsync()
        {
            if (IsBusy)
            {
                return;
            }
            try
            {
                IsBusy = true;
                Customers.Clear();

                var customers = await _apiService.GetCustomersAsync();
                foreach (var customer in customers)
                    Customers.Add(customer);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnDeleteCustomer(Customer customer)
        {
            if (customer == null) return;

            IsBusy = true;
            try
            {
                bool success = await _apiService.DeleteCustomerAsync(customer.UserId);
                if (success) Customers.Remove(customer);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnAddCustomer()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(Password)
               || string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Будь ласка, заповніть всі поля коректно", "OK");
                return;
            }

            var newCustomer = new Customer
            {
                PhoneNumber = PhoneNumber,
                Password = Password,
                FullName = FullName,
                Email = Email
            };

            IsBusy = true;
            try
            {
                var addedCustomer = await _apiService.AddCustomerAsync(newCustomer);
                if (addedCustomer != null)
                {
                    Customers.Add(addedCustomer);

                    PhoneNumber = string.Empty;
                    Password = string.Empty;
                    FullName = string.Empty;
                    Email = string.Empty;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
