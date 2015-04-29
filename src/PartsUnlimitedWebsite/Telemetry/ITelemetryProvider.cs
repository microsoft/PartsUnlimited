// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace PartsUnlimited.Telemetry
{
    public interface ITelemetryProvider
    {
        void TrackTrace(string message);
        void TrackEvent(string message);
        void TrackEvent(string message, Dictionary<string, string> properties, Dictionary<string, double> measurements);
        void TrackException(Exception exception);
    }
}