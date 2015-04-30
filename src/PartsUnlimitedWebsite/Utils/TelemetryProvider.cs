using System;
using System.Collections.Generic;

namespace PartsUnlimited.Utils
{
    public class TelemetryProvider : ITelemetryProvider
    {
        public void TrackEvent(string message)
        {
        }

        public void TrackEvent(string message, Dictionary<string, string> properties, Dictionary<string, double> measurements)
        {
        }

        public void TrackTrace(string message)
        {
        }

        public void TrackException (Exception exception)
        {
        }
    }
}