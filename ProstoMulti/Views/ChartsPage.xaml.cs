using ProstoMulti.ViewModels;

namespace ProstoMulti.Views;

public partial class ChartsPage : ContentPage
{
    public ChartsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ChartsViewModel vm)
            await vm.LoadAsync();
    }
}