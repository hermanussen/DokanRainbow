namespace DokanRainbow.Sitecore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class PathUtil
    {
        /// <summary>
        /// Turns a file system search query into a sitecore path
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static (string path, bool childrenOf) GetItemPath(string filePath, string searchPattern = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return ("/",true);
            }

            List<string> resultPath = filePath.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            bool childrenOf = !string.IsNullOrWhiteSpace(searchPattern) && searchPattern.Trim().Equals("*");

            if (!childrenOf)
            {
                if (string.IsNullOrWhiteSpace(searchPattern))
                {
                    if (resultPath.Any())
                    {
                        resultPath = resultPath
                            .Take(resultPath.Count - 1)
                            .Append(resultPath.Last().EndsWith(".yml") ? resultPath.Last().Substring(0, resultPath.Last().Length - 4) : resultPath.Last()).ToList();
                    }
                }
                else
                {
                    resultPath.Add(searchPattern.EndsWith(".yml") ? searchPattern.Substring(0, searchPattern.Length - 4) : searchPattern);
                }
            }

            return (string.Concat("/", string.Join("/", resultPath)), childrenOf);
        }
    }
}
