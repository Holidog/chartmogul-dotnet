using System.Collections.Generic;
using Newtonsoft.Json;

namespace chartmoguldotnet.models
{
    public class SubscriptionCollection
    {
        [JsonProperty(PropertyName = "customer_uuid")]
        public string CustomerId { get; set; }
        
        [JsonProperty(PropertyName = "subscriptions")]
        public List<Subscription> Subscriptions { get; set; }

        [JsonProperty(PropertyName = "current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty(PropertyName = "total_pages")]
        public int TotalPages { get; set; }

        public bool HasMorePages()
        {
            return CurrentPage < TotalPages;
        }
        
        public bool IsEmpty()
        {
            return Subscriptions.Count == 0;
        }
    }
}