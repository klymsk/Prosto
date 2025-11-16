using ProstoMulti.ViewModels;

namespace ProstoMulti.Views;

public partial class ItemsPage : ContentPage
{
    private ItemsViewModel _vm;

    public ItemsPage()
    {
        InitializeComponent();
        _vm = BindingContext as ItemsViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadItemsAsync();
    }
}