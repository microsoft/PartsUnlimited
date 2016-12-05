using System;
using PartsUnlimited.Resources;
using Xamarin.Forms;

namespace PartsUnlimited.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void Settings_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private void ShoppingItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) { return; }

            Device.OpenUri(new Uri(GlobalResources.Current.Website));

            // Unselect item.
            ((ListView)sender).SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // BindingContext needs to be readded to update UI.
            var bindingContext = BindingContext;
            BindingContext = null;
            BindingContext = bindingContext;

        }
    }
}
