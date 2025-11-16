using ProstoMulti.ViewModels;

namespace ProstoMulti.Views;

public partial class CartItemsPage : ContentPage
{
    private CartItemsViewModel _vm;

    public CartItemsPage()
    {
        InitializeComponent();
        _vm = BindingContext as CartItemsViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCartItemsAsync();
    }
}