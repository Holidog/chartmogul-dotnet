using System;
using Newtonsoft.Json;

namespace chartmoguldotnet.models
{
    public class Transaction : IChartMogulModel
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Result { get; set; }

        [JsonProperty(PropertyName = "external_id")]
        public string ExternalId { get; set; }
    }
}
