using System.ComponentModel;

namespace PartsUnlimited.Resources
{
    public class GlobalResources : INotifyPropertyChanged
    {
        // Singleton
        public static GlobalResources Current = new GlobalResources();
        
        public string Website { get; } = "http://YourPartsUnlimitedWebsite";
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
