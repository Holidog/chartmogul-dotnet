using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace chartmoguldotnet.models
{
    public class Invoice
    {
        [JsonIgnore]
        public string Uuid { get; set; }

        [JsonProperty(PropertyName = "external_id")]
        public string ExternalId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "due_date")]
        public DateTime DueDate { get; set; }

        [JsonProperty(PropertyName = "line_items")]
        public List<LineItem> Items { get; set; }
        
        [JsonProperty(PropertyName = "transactions")]
        public List<Transaction> Transactions { get; set; }
    }
}
