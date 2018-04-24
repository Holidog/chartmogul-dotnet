using System;
using Newtonsoft.Json;

namespace chartmoguldotnet.models
{
    public class Customer
    {
        [JsonProperty(PropertyName = "data_source_uuid")]
        public string DataSource { get; set; }

        [JsonProperty(PropertyName = "external_id")]
        public string ExternalId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "company")]
        public string Company { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }

        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }
        
        [JsonProperty(PropertyName = "lead_created_at")]
        public DateTime CreatedDate { get; set; }
        
        [JsonProperty(PropertyName = "free_trial_started_at")]
        public DateTime FreeTrialDate { get; set; }
    }
}
