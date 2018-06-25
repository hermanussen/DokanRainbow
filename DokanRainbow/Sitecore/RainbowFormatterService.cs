namespace DokanRainbow.Sitecore
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using global::Rainbow.Storage.Yaml;

    public class RainbowFormatterService
    {
        public MemoryStream GetRainbowContents(IDictionary<string, dynamic> item, string databaseName)
        {
            var memoryStream = new MemoryStream();
            if (!item.Any())
            {
                return memoryStream;
            }

            var itemData = new ItemServiceItemData(item)
            {
                DatabaseName = databaseName
            };
            new YamlSerializationFormatter(null, null).WriteSerializedItem(itemData, memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
