using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace chartmoguldotnet.models
{
    public class InvoiceCollection
    {
        [JsonProperty(PropertyName = "invoices")]
        public List<Invoice> Invoices { get; set; }

        [JsonProperty(PropertyName = "current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty(PropertyName = "total_pages")]
        public int TotalPages { get; set; }

        public bool HasMorePages()
        {
            return CurrentPage < TotalPages;
        }
    }
}
