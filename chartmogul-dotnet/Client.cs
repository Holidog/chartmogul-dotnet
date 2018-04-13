using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using chartmoguldotnet.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace chartmoguldotnet
{
    public class Client
    {
        private readonly string _baseUrl = "https://api.chartmogul.com/v1/";
        private readonly string _credentials;

        public Client(Config config)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes($"{config.AccountToken}:{config.SecretKey}");
            _credentials = Convert.ToBase64String(plainTextBytes);
        }

        public Client(string accountToken, string secretKey) : this (new Config { AccountToken = accountToken, SecretKey = secretKey }) {}

        public List<DataSource> GetDataSources()
        {
            string urlPath = $"import/data_sources";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<DataSource>();
            if (resp.Success)
            {
                JToken root = JObject.Parse(resp.Json);
                JToken sources = root["data_sources"];
                IEnumerable<DataSource> dataSources = JsonConvert.DeserializeObject<IEnumerable<DataSource>>(sources.ToString());
                list = dataSources.ToList();
            }
            return list;
        }

        public bool AddDataSource(DataSource dataSource)
        {
            string urlPath = $"data_sources";
            string json = JsonConvert.SerializeObject(dataSource);

            ApiResponse resp = CallApi(urlPath, "POST", json);
            return resp.Success;
        }

        public string DeleteDataSource()
        {
            throw new NotImplementedException();
        }

        public List<Customer> GetCustomers()
        {
            string urlPath = $"import/customers";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<Customer>();
            if (resp.Success)
            {
                IEnumerable<Customer> customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(resp.Json);
                list = customers.ToList();
            }

            return list;
        }

        public bool AddCustomer(Customer cust, DataSource ds)
        {
            if (cust == null)
                return false;

            string urlPath = $"import/customers";
            cust.DataSource = ds.Uuid;
            string json = JsonConvert.SerializeObject(cust);

            ApiResponse resp = CallApi(urlPath, "POST", json);
            return resp.Success;
        }

        public void DeleteCustomer(Customer cust)
        {
            throw new NotImplementedException();
        }

        public List<Plan> GetPlans()
        {
            string urlPath = $"import/plans";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<Plan>();
            if (resp.Success)
            {
                IEnumerable<Plan> plans = JsonConvert.DeserializeObject<IEnumerable<Plan>>(resp.Json);
                list = plans.ToList();
            }

            return list;
        }

        public bool AddPlan(Plan plan, DataSource ds)
        {
            string urlPath = $"import/plans";
            plan.DataSource = ds.Uuid;
            string json = JsonConvert.SerializeObject(plan);

            ApiResponse resp = CallApi(urlPath, "POST", json);
            return resp.Success;
        }

        public List<Invoice> GetInvoices(bool includeAll = false)
        {
            string urlPath = $"invoices";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<Invoice>();
            if (resp.Success)
            {
                var invoices = JsonConvert.DeserializeObject<InvoiceCollection>(resp.Json);
                list.Concat(invoices.Invoices);

                if(includeAll) {
                    for (int i = invoices.CurrentPage + 1; i < invoices.TotalPages; i++)
                    {
                        var invoicesPage = CallApiPageable<InvoiceCollection>(urlPath, i);
                        list.Concat(invoicesPage.Invoices);
                    }
                }
            }

            return list;
        }

        public bool AddInvoice(List<Invoice> invoices, Customer customer)
        {
            string urlPath = $"import/customers/{customer.Uuid}/invoices";
            string json = JsonConvert.SerializeObject(invoices);

            ApiResponse resp = CallApi(urlPath, "POST", json);
            return resp.Success;
        }

        public List<Subscription> GetSubscriptions(Customer cust)
        {
            string urlPath = $"import/customers/{cust.Uuid}/subscriptions";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<Subscription>();
            if (resp.Success)
            {
                IEnumerable<Subscription> subscriptions = JsonConvert.DeserializeObject<IEnumerable<Subscription>>(resp.Json);
                list = subscriptions.ToList();
            }

            return list;
        }

        public bool CancelSubscription(Subscription sub, DateTime cancelledAt)
        {
            string urlPath = $"import/subscriptions/{sub.Uuid}";
            sub.CancellationDates = cancelledAt;
            string json = JsonConvert.SerializeObject(sub);

            ApiResponse resp = CallApi(urlPath, "PATCH", json);
            return resp.Success;
        }

        private O CallApiPageable<O>(string urlPath, int page = 1)
        {
            ApiResponse resp = CallApi(urlPath, "GET", string.Empty, $"?page={page}");
            return JsonConvert.DeserializeObject<O>(resp.Json);
        }

        private ApiResponse CallApi(string urlPath, string httpMethod, string jsonToWrite = "", string queryString = "")
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(_baseUrl + urlPath);
                httpRequest.Headers.Add("Authorization", "Basic " + _credentials);
                httpRequest.Accept = "*/*";
                httpRequest.Method = httpMethod.ToUpper();

                if (httpMethod == "POST")
                {
                    httpRequest.ContentType = "application/json";
                }

                // Only write the json file when it is a add call or a update call
                if (!string.IsNullOrEmpty(jsonToWrite))
                {
                    using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                    {
                        streamWriter.Write(jsonToWrite);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                WebResponse response = (HttpWebResponse)httpRequest.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
                {
                    var responseText = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    return new ApiResponse { Success = true, Json = responseText };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"ApiCall could not be done: {ex.Message}." };
            }
        }
    }  
}
