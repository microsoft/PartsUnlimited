using System;
using System.Linq;
using Windows.UI;

namespace PartsUnlimited.UWP
{
    public static class UwpColorConverter
    {
        public static Color Convert(Xamarin.Forms.Color xamarinColor)
        {
            return Color.FromArgb(DoubleToByte(xamarinColor.A), DoubleToByte(xamarinColor.R), DoubleToByte(xamarinColor.G), DoubleToByte(xamarinColor.B));
        }

        private static byte DoubleToByte(double input)
        {
            return BitConverter.GetBytes((int)(input * 255)).First();
        }
    }
}
