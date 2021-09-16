﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Diagnostics.Tools.Counters
{
    public class CounterPayload
    {
        public CounterPayload(string providerName, string name, string displayName, string displayUnits, string tags, double value, DateTime timestamp, string type)
        {
            ProviderName = providerName;
            Name = name;
            Tags = tags;
            Value = value;
            Timestamp = timestamp;
            CounterType = type;
        }

        public string ProviderName { get; private set; }
        public string Name { get; private set; }
        public double Value { get; private set; }
        public virtual string DisplayName { get; protected set; }
        public string CounterType { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Tags { get; private set; }
    }

    class GaugePayload : CounterPayload
    {
        public GaugePayload(string providerName, string name, string displayName, string displayUnits, string tags, double value, DateTime timestamp) :
            base(providerName, name, displayName, displayUnits, tags, value, timestamp, "Metric")
        {
            // In case these properties are not provided, set them to appropriate values.
            string counterName = string.IsNullOrEmpty(displayName) ? name : displayName;
            DisplayName = !string.IsNullOrEmpty(displayUnits) ? $"{counterName} ({displayUnits})" : counterName;
        }
    }
}