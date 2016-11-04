using System.ComponentModel;
using PartsUnlimited.Resources;
using Xamarin.Forms;

namespace PartsUnlimited.ViewModels
{
    public class ShoppingItemViewModel : INotifyPropertyChanged
    {
        private readonly string _path = Device.OnPlatform("images/", "", "Assets/images/");
        private string _imageSource;
        private decimal _discountPercentage;
        private decimal _price;
        private string _itemName;

        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        public string ImageSource
        {
            get
            {
                if (_imageSource.StartsWith("http://") || _imageSource.StartsWith("https://"))
                    return _imageSource;
                return _path + _imageSource;
            }
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        /// <summary>
        /// Gets or sets the discount percentage.
        /// </summary>
        public decimal DiscountPercentage
        {
            get { return _discountPercentage; }
            set
            {
                _discountPercentage = value;
                OnPropertyChanged(nameof(DiscountPercentage));
                OnPropertyChanged(nameof(PriceAfterDiscount));
            }
        }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(PriceAfterDiscount));
            }
        }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }


        public decimal PriceAfterDiscount => Price * (100M - DiscountPercentage) / 100M;


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
