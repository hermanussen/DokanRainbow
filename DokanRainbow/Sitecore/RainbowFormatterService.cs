namespace DokanRainbow.Sitecore
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using global::Rainbow.Storage.Yaml;
    
    public class RainbowFormatterService
    {

        /// <summary>
        /// Turn a list of items into a file stream that contains the file contents.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
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
