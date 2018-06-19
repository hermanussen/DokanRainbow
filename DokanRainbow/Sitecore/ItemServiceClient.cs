﻿namespace DokanRainbow.Sitecore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Newtonsoft.Json;
    using RestSharp;

    public class ItemServiceClient
    {
        private readonly string domain;
        private readonly string userName;
        private readonly string password;

        private readonly RestClient client;

        public string DatabaseName { get; private set; }

        public string HostName { get; private set; }

        public ItemServiceClient(string instanceUrl, string domain, string userName, string password, string databaseName)
        {
            this.domain = domain;
            this.userName = userName;
            this.password = password;
            this.client = new RestClient(instanceUrl)
                {
                    CookieContainer = new CookieContainer()
                };
            this.HostName = this.client.BaseUrl.Host;
            this.DatabaseName = databaseName;
        }
        
        public IEnumerable<dynamic> GetChildren(string itemPath)
        {
            dynamic item = GetItem(itemPath);
            if (item != null)
            {
                string itemId = item.ItemID.Value;

                var request = new RestRequest($"/sitecore/api/ssc/item/{itemId}/children?database={DatabaseName}");
                var response = this.client.Execute(request);

                if (response.IsSuccessful)
                {
                    dynamic children = JsonConvert.DeserializeObject(response.Content);
                    foreach (dynamic child in children)
                    {
                        yield return child;
                    }
                }
            }
        }

        public dynamic GetItem(string itemPath)
        {
            if (this.EnsureLoggedIn())
            {
                var request = new RestRequest($"/sitecore/api/ssc/item/?path=/sitecore{itemPath}&database={DatabaseName}&includeMetadata=true");
                var response = this.client.Execute(request);

                if (response.IsSuccessful)
                {
                    return JsonConvert.DeserializeObject(response.Content);
                }
            }

            return null;
        }
        
        private bool EnsureLoggedIn()
        {
            // Check if we are already logged in
            if (this.client.CookieContainer.GetCookies(this.client.BaseUrl)[".ASPXAUTH"]?.Expired ?? true)
            {
                var request = new RestRequest("/sitecore/api/ssc/auth/login", Method.POST);

                request.AddParameter("domain", this.domain);
                request.AddParameter("username", this.userName);
                request.AddParameter("password", this.password);

                var response = this.client.Execute(request);
                return response.IsSuccessful;
            }

            return true;
        }
    }
}
