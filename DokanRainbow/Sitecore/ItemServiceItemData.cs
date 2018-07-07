namespace DokanRainbow.Sitecore
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using global::Rainbow.Model;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides the implementation of the data that Rainbow needs to serialize the item.
    /// </summary>
    public class ItemServiceItemData : IItemData
    {
        /// <summary>
        /// A dictionary of the same item in different languages and versions.
        /// The key of the first dictionary contains the language.
        /// The item versions are the key of the second dictionary.
        /// </summary>
        private readonly IDictionary<string, IDictionary<int, dynamic>> items;

        /// <summary>
        /// These fields are excluded by default.
        /// </summary>
        private readonly string[] includedStandardFields = { "__Created", "__Created by" };

        /// <summary>
        /// Holds a list of all field values, so they can be queried when requested as regular, unversioned and shared fields.
        /// </summary>
        public IEnumerable<dynamic> AllFields { get; }

        public ItemServiceItemData(IDictionary<string, IDictionary<int, dynamic>> items)
        {
            this.items = items;
            this.AllFields = this.GetFieldValues();
        }

        public Guid Id
        {
            get
            {
                Guid result;
                if (Guid.TryParse(this.items.Values.SelectMany(v => v.Values).FirstOrDefault()?.ItemID?.ToString().Trim(new [] {'{', '}'}), out result))
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
                if (Guid.TryParse(this.items.Values.SelectMany(v => v.Values).FirstOrDefault()?.ParentID?.ToString().Trim(new[] { '{', '}' }), out result))
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
                if (Guid.TryParse(this.items.Values.SelectMany(v => v.Values).FirstOrDefault()?.TemplateID?.ToString().Trim(new[] { '{', '}' }), out result))
                {
                    return result;
                }

                return Guid.Empty;
            }
        }

        public string Path
        {
            get { return this.items.Values.SelectMany(v => v.Values).FirstOrDefault()?.ItemPath; }
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
            get { return this.items.Values.SelectMany(v => v.Values).FirstOrDefault()?.ItemName; }
        }

        public Guid BranchId
        {
            get
            {
                Guid result;
                if (Guid.TryParse(this.items.Values.SelectMany(v => v.Values).FirstOrDefault()?.BranchID?.ToString().Trim(new[] { '{', '}' }), out result))
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
                return items.Select(l => new ProxyItemLanguage(CultureInfo.GetCultureInfo(l.Key))
                {
                    Fields = this.AllFields
                        .Where(f => l.Key.Equals(f.Language) && !f.IsStandardValue && !(f.IsShared ?? false) && f.IsUnversioned)
                        .Select(f => new ProxyFieldValue(f.FieldId, f.FieldValue)
                        {
                            NameHint = f.FieldName,
                            FieldType = f.FieldType
                        })
                })
                .Where(v => v.Fields.Any(f => !includedStandardFields.Contains(f.NameHint)))
                .ToArray();
            }
        }

        public IEnumerable<IItemVersion> Versions
        {
            get
            {
                return items.SelectMany(l => l.Value.Select(v => new ProxyItemVersion(CultureInfo.GetCultureInfo(l.Key), v.Key)
                {
                    Fields = this.AllFields
                        .Where(f => l.Key.Equals(f.Language) && v.Key == f.Version && !f.IsStandardValue && !(f.IsUnversioned ?? false) && !(f.IsShared ?? false))
                        .Select(f => new ProxyFieldValue(f.FieldId, f.FieldValue)
                        {
                            NameHint = f.FieldName,
                            FieldType = f.FieldType
                        })
                })
                .Where(v => v.Fields.Any(f => !includedStandardFields.Contains(f.NameHint)))).ToArray();
            }
        }

        private IEnumerable<dynamic> GetFieldValues()
        {
            foreach (var item in items)
            {
                string language = item.Key;
                foreach (KeyValuePair<int, dynamic> itemVersion in item.Value)
                {
                    JObject metaData = itemVersion.Value.metadata as JObject;
                    JObject itemObject = itemVersion.Value as JObject;
                    if (metaData == null || itemObject == null)
                    {
                        yield break;
                    }

                    foreach (var datum in metaData)
                    {
                        dynamic val = datum.Value as dynamic;
                        if (val == null
                            || itemObject[datum.Key]?.Value<string>() == null
                            || (datum.Key.StartsWith("__") && !includedStandardFields.Contains(datum.Key)))
                        {
                            continue;
                        }

                        yield return new
                        {
                            FieldName = datum.Key,
                            FieldId = Guid.Parse(val.ID.ToString().Trim(new[] {'{', '}'})),
                            FieldType = datum.Value["Type"].Value<string>(),
                            IsShared = datum.Value["Shared"].Value<bool>(),
                            IsUnversioned = datum.Value["Unversioned"].Value<bool>(),
                            IsStandardValue = datum.Value["ContainsStandardValue"].Value<bool>(),
                            FieldValue = itemObject[datum.Key]?.Value<string>(),
                            Language = language,
                            Version = itemVersion.Key
                        };
                    }
                }
            }
        }
    }
}
