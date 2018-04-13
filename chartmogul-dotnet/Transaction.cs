﻿using System;
using Newtonsoft.Json;

namespace OConnors.ChartMogul
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Result { get; set; }

        [JsonProperty(PropertyName = "external_id")]
        public string ExternalId { get; set; }
    }
}