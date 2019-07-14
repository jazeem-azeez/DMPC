using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using DMC.CacheProviders;

namespace DMC.Models
{
    public class EventMessage
    {
        public EventMessage() => this.EventHeaders = new Dictionary<string, string> { 
                {nameof(CacheEventTypes),string.Empty },
                {nameof(this.ExpiryInMins),0.ToString() }
            };

        public List<string> EventKeyCollection { get; set; }
        public List<string> EventFilterCollection { get; set; }
        public Dictionary<string, string> EventHeaders { get; }
        public DateTime EventTimeStamp { get; set; }
        public string OldEventData { get; set; }
        public string EventData { get; set; }
        [JsonIgnore]
        public CacheEventTypes EventType { get => (CacheEventTypes)Enum.Parse(typeof(CacheEventTypes), this.EventHeaders[nameof(CacheEventTypes)]); set => this.EventHeaders[nameof(CacheEventTypes)] = value.ToString(); }
        [JsonIgnore]
        public int ExpiryInMins { get => Convert.ToInt32(this.EventHeaders[nameof(this.ExpiryInMins)]); set => this.EventHeaders[nameof(this.ExpiryInMins)] = value.ToString(); }
        public string CorrelationId { get; set; }
    }
}