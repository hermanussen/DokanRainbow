namespace DokanRainbow.Sitecore
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using global::Rainbow.Model;
    using Newtonsoft.Json.Linq;

    public class ItemServiceItemData : IItemData
    {
        private readonly dynamic item;

        public IEnumerable<dynamic> AllFields { get; private set; }

        public ItemServiceItemData(dynamic item)
        {
            this.item = item;
            this.AllFields = this.GetFieldValues();
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
                return this.AllFields
                    .Where(f => !f.IsStandardValue && f.IsShared ?? false)
                    .Select(f => new ProxyFieldValue(f.FieldId, f.FieldValue)
                        {
                            NameHint = f.FieldName,
                            FieldType = f.FieldType
                        }).ToArray();
            }
        }

        public IEnumerable<IItemLanguage> UnversionedFields
        {
            get
            {
                return new IItemLanguage[]
                {
                    new ProxyItemVersion(CultureInfo.GetCultureInfo("en"), 1)
                    {
                        Fields = this.AllFields
                            .Where(f => !f.IsStandardValue && f.IsShared ?? false)
                            .Select(f => new ProxyFieldValue(f.FieldId, f.FieldValue)
                                {
                                    NameHint = f.FieldName,
                                    FieldType = f.FieldType
                                })
                    }
                };
            }
        }

        public IEnumerable<IItemVersion> Versions
        {
            get
            {
                return new IItemVersion[]
                {
                    new ProxyItemVersion(CultureInfo.GetCultureInfo("en"), 1)
                    {
                        Fields = this.AllFields
                            .Where(f => !f.IsStandardValue && !(f.IsUnversioned ?? false))
                            .Select(f => new ProxyFieldValue(f.FieldId, f.FieldValue)
                            {
                                NameHint = f.FieldName,
                                FieldType = f.FieldType
                            })
                    }
                };
            }
        }

        private IEnumerable<dynamic> GetFieldValues()
        {
            JObject metaData = this.item.metadata as JObject;
            JObject itemObject = this.item as JObject;
            if (metaData == null || itemObject == null)
            {
                yield break;
            }

            foreach (var datum in metaData)
            {
                dynamic val = datum.Value as dynamic;
                if (val == null || itemObject[datum.Key]?.Value<string>() == null)
                {
                    continue;
                }

                yield return new
                {
                    FieldName = datum.Key,
                    FieldId = Guid.Parse(val.ID.ToString().Trim(new[] { '{', '}' })),
                    FieldType = datum.Value["Type"].Value<string>(),
                    IsShared = datum.Value["Shared"].Value<bool>(),
                    IsUnversioned = datum.Value["Unversioned"].Value<bool>(),
                    IsStandardValue = datum.Value["ContainsStandardValue"].Value<bool>(),
                    FieldValue = itemObject[datum.Key]?.Value<string>()
                };
            }
        }
    }
}
