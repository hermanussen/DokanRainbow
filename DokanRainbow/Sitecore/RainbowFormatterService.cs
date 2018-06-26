namespace DokanRainbow.Sitecore
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using global::Rainbow.Storage.Yaml;

    public class RainbowFormatterService
    {
        public MemoryStream GetRainbowContents(IDictionary<string, IDictionary<int, dynamic>> items, string databaseName)
        {
            var memoryStream = new MemoryStream();
            if (!items.Any())
            {
                return memoryStream;
            }

            var itemData = new ItemServiceItemData(items)
            {
                DatabaseName = databaseName
            };
            new YamlSerializationFormatter(null, null).WriteSerializedItem(itemData, memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
