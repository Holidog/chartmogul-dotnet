using System;
using Newtonsoft.Json;

namespace chartmoguldotnet.models
{
    public class Transaction
    {
        [JsonIgnore]
        public string Uuid { get; set; }
        
        [JsonProperty(PropertyName = "external_id")]
        public string ExternalId { get; set; }
        
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }
        
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        
        [JsonProperty(PropertyName = "result")]
        public string Result { get; set; }
    }
}
