using ProstoMulti.ViewModels;

namespace ProstoMulti.Views;

public partial class CustomersPage : ContentPage
{
    private CustomersViewModel _vm;

    public CustomersPage()
    {
        InitializeComponent();
        _vm = BindingContext as CustomersViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCustomersAsync();
    }
}