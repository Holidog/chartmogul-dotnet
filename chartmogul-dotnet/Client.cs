using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using chartmoguldotnet.Exceptions;
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
            string urlPath = $"data_sources";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<DataSource>();
            if (resp.Success)
            {
                JToken root = JObject.Parse(resp.Json);
                JToken sources = root["data_sources"];
                var dataSources = JsonConvert.DeserializeObject<IEnumerable<DataSource>>(sources.ToString());
                list = dataSources.ToList();
            }
            return list;
        }

        public DataSource AddDataSource(DataSource dataSource)
        {
            string urlPath = $"data_sources";
            string json = JsonConvert.SerializeObject(dataSource);

            ApiResponse resp = CallApi(urlPath, "POST", json);
            if (resp.Success)
            {
                return JsonConvert.DeserializeObject<DataSource>(resp.Json);
            }

            throw new ChartMogulException($"DataSource cannot be created: {resp.Message}");
        }

        public string DeleteDataSource()
        {
            throw new NotImplementedException();
        }

        public List<Customer> GetCustomers(bool includeAll = false)
        {
            string urlPath = $"customers";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<Customer>();
            if (resp.Success)
            {
                var customers = JsonConvert.DeserializeObject<CustomerCollection>(resp.Json);
                list = list.Concat(customers.Customers).ToList();

                if(includeAll) {
                    for (var i = customers.CurrentPage + 1; i < customers.TotalPages; i++)
                    {
                        var customersPage = CallApiPageable<CustomerCollection>(urlPath, i);
                        list = list.Concat(customersPage.Customers).ToList();
                    }
                }
            }

            return list;
        }

        public Customer AddCustomer(Customer cust, DataSource ds)
        {
            string urlPath = $"customers";
            cust.DataSource = ds.Uuid;
            string json = JsonConvert.SerializeObject(cust);

            ApiResponse resp = CallApi(urlPath, "POST", json);
            if (resp.Success)
            {
                return JsonConvert.DeserializeObject<Customer>(resp.Json);
            }

            throw new ChartMogulException($"Customer cannot be created: {resp.Message}");
        }

        public void DeleteCustomer(Customer cust)
        {
            throw new NotImplementedException();
        }

        public List<Plan> GetPlans(bool includeAll = true)
        {
            string urlPath = $"plans";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<Plan>();
            if (resp.Success)
            {
                var plans = JsonConvert.DeserializeObject<PlanCollection>(resp.Json);
                list = list.Concat(plans.Plans).ToList();

                if(includeAll) {
                    for (var i = plans.CurrentPage + 1; i < plans.TotalPages; i++)
                    {
                        var plansPage = CallApiPageable<PlanCollection>(urlPath, i);
                        list = list.Concat(plansPage.Plans).ToList();
                    }
                }
            }

            return list;
        }

        public Plan AddPlan(Plan plan, DataSource ds)
        {
            string urlPath = $"plans";
            plan.DataSource = ds.Uuid;
            string json = JsonConvert.SerializeObject(plan);

            ApiResponse resp = CallApi(urlPath, "POST", json);
            if (resp.Success)
            {
                return JsonConvert.DeserializeObject<Plan>(resp.Json);
            }

            throw new ChartMogulException($"Plan cannot be created: {resp.Message}");
        }
        
        public void DeletePlan(Plan plan)
        {
            throw new NotImplementedException();
        }

        public List<Invoice> GetInvoices(bool includeAll = false)
        {
            string urlPath = $"invoices";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<Invoice>();
            if (resp.Success)
            {
                var invoices = JsonConvert.DeserializeObject<InvoiceCollection>(resp.Json);
                list = list.Concat(invoices.Invoices).ToList();

                if(includeAll) {
                    for (var i = invoices.CurrentPage + 1; i < invoices.TotalPages; i++)
                    {
                        var invoicesPage = CallApiPageable<InvoiceCollection>(urlPath, i);
                        list = list.Concat(invoicesPage.Invoices).ToList();
                    }
                }
            }

            return list;
        }

        public Invoice AddInvoice(List<Invoice> invoices, Customer customer)
        {
            string urlPath = $"import/customers/{customer.Uuid}/invoices";
            string json = JsonConvert.SerializeObject(invoices);

            ApiResponse resp = CallApi(urlPath, "POST", json);
            if (resp.Success)
            {
                return JsonConvert.DeserializeObject<Invoice>(resp.Json);
            }

            throw new ChartMogulException($"Invoice cannot be created: {resp.Message}");
        }
        
        public void DeleteInvoice(Invoice invoice)
        {
            throw new NotImplementedException();
        }
        
        public Transaction AddTransaction(Invoice invoice, Transaction transaction)
        {
            string urlPath = $"import/invoices/{invoice.Uuid}/transactions";
            string json = JsonConvert.SerializeObject(transaction);

            ApiResponse resp = CallApi(urlPath, "POST", json);
            if (resp.Success)
            {
                return JsonConvert.DeserializeObject<Transaction>(resp.Json);
            }

            throw new ChartMogulException($"Transaction cannot be created: {resp.Message}");
        }

        public List<Subscription> GetSubscriptions(Customer cust)
        {
            string urlPath = $"import/customers/{cust.Uuid}/subscriptions";
            ApiResponse resp = CallApi(urlPath, "GET");
            var list = new List<Subscription>();
            if (resp.Success)
            {
                var subscriptions = JsonConvert.DeserializeObject<IEnumerable<Subscription>>(resp.Json);
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

        private TO CallApiPageable<TO>(string urlPath, int page = 1)
        {
            ApiResponse resp = CallApi(urlPath, "GET", string.Empty, $"?page={page}");
            if (resp.Success)
            {
                return JsonConvert.DeserializeObject<TO>(resp.Json);
            }
            
            throw new ChartMogulException($"Pageable API couldn't be retrieved: {resp.Message}");
        }

        private ApiResponse CallApi(string urlPath, string httpMethod, string jsonToWrite = "", string queryString = "")
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create($"{_baseUrl}{urlPath}{queryString}");
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
