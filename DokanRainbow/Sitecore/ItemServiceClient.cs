namespace DokanRainbow.Sitecore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using DokanNet;
    using Newtonsoft.Json;
    using RestSharp;

    public class ItemServiceClient
    {
        private readonly string domain;
        private readonly string userName;
        private readonly string password;

        private readonly RestClient client;

        private readonly IDictionary<string, CachedItemList> cachedItemLists;

        public ItemServiceClient(string instanceUrl, string domain, string userName, string password)
        {
            this.domain = domain;
            this.userName = userName;
            this.password = password;
            this.cachedItemLists = new Dictionary<string, CachedItemList>();
            this.client = new RestClient(instanceUrl)
                {
                    CookieContainer = new CookieContainer()
                };
        }
        
        public IList<FileInformation> FindFilesWithPattern(string fileName, string searchPattern)
        {
            string itemPath = GetItemPath(fileName, searchPattern);
            lock (cachedItemLists)
            {
                if (cachedItemLists.ContainsKey(itemPath) && !cachedItemLists[itemPath].IsExpired)
                {
                    return cachedItemLists[itemPath].Items;
                }

                List<FileInformation> result = new List<FileInformation>();

                if (this.EnsureLoggedIn())
                {
                    var request = new RestRequest($"/sitecore/api/ssc/item/?path={itemPath}&database=core");
                    var response = this.client.Execute(request);

                    if (response.IsSuccessful)
                    {
                        dynamic item = JsonConvert.DeserializeObject(response.Content);
                        string itemId = item.ItemID.Value;

                        request = new RestRequest($"/sitecore/api/ssc/item/{itemId}/children?database=core");
                        response = this.client.Execute(request);

                        if (response.IsSuccessful)
                        {
                            dynamic children = JsonConvert.DeserializeObject(response.Content);
                            foreach (dynamic child in children)
                            {
                                result.Add(new FileInformation()
                                {
                                    FileName = child.ItemName,
                                    CreationTime = child.__Created,
                                    Attributes = FileAttributes.Directory,
                                    LastWriteTime = child.__Updated,
                                    Length = child.ToString().ToCharArray().Length
                                });
                                result.Add(new FileInformation()
                                {
                                    FileName = $"{child.ItemName}.yml",
                                    CreationTime = child.__Created,
                                    Attributes = FileAttributes.Normal,
                                    LastWriteTime = child.__Updated,
                                    Length = child.ToString().ToCharArray().Length
                                });
                            }

                            cachedItemLists[itemPath] = new CachedItemList(result);
                        }
                    }
                }

                return result;
            }
        }

        public string GetItemPath(string filePath, string searchPattern)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(searchPattern))
            {
                return "/sitecore";
            }

            string result = filePath.Replace(@"\", "/");
            if (!"*".Equals(searchPattern))
            {
                result = $"{result}/{searchPattern}";
            }

            if (result.StartsWith("/sitecore"))
            {
                result = result.Substring(9);
            }

            return $"/sitecore{result}".TrimEnd(new[] { '/' });
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
