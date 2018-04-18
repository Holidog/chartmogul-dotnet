using System.Collections.Generic;
using Newtonsoft.Json;

namespace chartmoguldotnet.models
{
    public class CustomerCollection
    {
        [JsonProperty(PropertyName = "entries")]
        public List<Customer> Customers { get; set; }

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
            return Customers.Count == 0;
        }
    }
}