using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PartsUnlimited.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ShoppingItemViewModel> _shoppingItems;

        public ObservableCollection<ShoppingItemViewModel> ShoppingItems
        {
            get { return _shoppingItems; }
            set
            {
                _shoppingItems = value;
                OnPropertyChanged(nameof(ShoppingItems));
            }
        }

        public MainPageViewModel()
        {
            _shoppingItems = new ObservableCollection<ShoppingItemViewModel>
            {
                new ShoppingItemViewModel
                {
                    ItemName = "Filter Set",
                    Price = 28.99M,
                    DiscountPercentage = 20,
                    ImageSource = "product_oil_filters.jpg"
                },
                new ShoppingItemViewModel
                {
                    ItemName = "Oil and Filter Combo",
                    Price = 34.49M,
                    DiscountPercentage = 20,
                    ImageSource = "product_oil_oil_filter_combo.jpg"
                },
                new ShoppingItemViewModel
                {
                    ItemName = "Synthetic Engine Oil",
                    Price = 36.49M,
                    DiscountPercentage = 20,
                    ImageSource = "product_oil_premium_oil.jpg"
                }
            };

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
