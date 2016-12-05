using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using PartsUnlimited.Resources;

namespace PartsUnlimited.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = UwpColorConverter.Convert(ApplicationColors.MainDarkColour);
                    statusBar.ForegroundColor = UwpColorConverter.Convert(ApplicationColors.MainLightColour);
                }
            }

            LoadApplication(new Views.App());
        }
    }
}
