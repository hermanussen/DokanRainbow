namespace DokanRainbow.Sitecore
{
    using System;
    using System.Collections.Generic;
    using DokanNet;

    public class CachedItemList
    {
        public DateTime Created { get; private set; }

        public IList<FileInformation> Items { get; private set; }
        
        private const int CacheTimeSeconds = 5;

        public bool IsExpired
        {
            get { return (DateTime.Now - this.Created).TotalSeconds > CacheTimeSeconds; }
        }

        public CachedItemList(IList<FileInformation> items)
        {
            this.Items = items;
            this.Created = DateTime.Now;
        }
    }
}
