// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace PartsUnlimited.Telemetry
{
    public class EmptyTelemetryProvider : ITelemetryProvider
    {
        private readonly TelemetryClient _telemetry = new TelemetryClient();

        public void TrackEvent(string message)
        {
            _telemetry.TrackEvent(message);
        }

        public void TrackEvent(string message, Dictionary<string, string> properties, Dictionary<string, double> measurements)
        {
            _telemetry.TrackEvent(message, properties, measurements);
        }

        public void TrackException(Exception exception)
        {
            _telemetry.TrackException(exception);
        }

        public void TrackTrace(string message)
        {
            _telemetry.TrackTrace(message);
        }
    }
}