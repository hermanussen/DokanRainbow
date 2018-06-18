namespace DokanRainbow.Sitecore
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using global::Rainbow.Model;

    public class ItemServiceItemData : IItemData
    {
        private dynamic item;

        public ItemServiceItemData(dynamic item)
        {
            this.item = item;
        }

        public Guid Id
        {
            get
            {
                Guid result;
                if (Guid.TryParse(this.item.ItemID?.ToString().Trim(new [] {'{', '}'}), out result))
                {
                    return result;
                }

                return Guid.Empty;
            }
        }

        public Guid ParentId
        {
            get
            {
                Guid result;
                if (Guid.TryParse(this.item.ParentID?.ToString().Trim(new[] { '{', '}' }), out result))
                {
                    return result;
                }

                return Guid.Empty;
            }
        }

        public Guid TemplateId
        {
            get
            {
                Guid result;
                if (Guid.TryParse(this.item.TemplateID?.ToString().Trim(new[] { '{', '}' }), out result))
                {
                    return result;
                }

                return Guid.Empty;
            }
        }

        public string Path
        {
            get { return this.item.ItemPath; }
        }

        public string SerializedItemId
        {
            get { return Id.ToString(); }
        }

        public IEnumerable<IItemData> GetChildren()
        {
            yield break;
        }

        public string DatabaseName { get; set; }

        public string Name
        {
            get { return this.item.ItemName; }
        }

        public Guid BranchId
        {
            get
            {
                Guid result;
                if (Guid.TryParse(this.item.BranchID?.ToString().Trim(new[] { '{', '}' }), out result))
                {
                    return result;
                }

                return Guid.Empty;
            }
        }

        public IEnumerable<IItemFieldValue> SharedFields
        {
            get
            {
                return new IItemFieldValue[0];
            }
        }

        public IEnumerable<IItemLanguage> UnversionedFields
        {
            get { return new IItemLanguage[0]; }
        }

        public IEnumerable<IItemVersion> Versions
        {
            get
            {
                return new IItemVersion[0];
            }
        }
    }
}
