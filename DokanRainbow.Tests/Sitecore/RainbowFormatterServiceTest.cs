namespace DokanRainbow.Tests.Sitecore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using DokanRainbow.Sitecore;
    using FluentAssertions;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class RainbowFormatterServiceTest
    {
        [Fact]
        public void ShouldGetRainbowContentsWithNoFields()
        {
            var input = new Dictionary<string, IDictionary<int,dynamic>>();
            input.Add("en",
                new Dictionary<int, dynamic>()
                {
                    {
                        1,
                        JObject.FromObject(new
                            {
                                ItemID = "a4960f3b-7250-42bb-8df5-2789dcf613ea",
                                ItemName = "Example item",
                                ItemPath = "/sitecore/templates/Example item",
                                TemplateID = "292eb157-5305-4da3-9916-3015d8d32855",
                                ParentID = "3c1715fe-6a13-4fcf-845f-de308ba9741d",
                                metadata = new
                                {
                                }
                            })
                    }
                });

            var memoryStream = new RainbowFormatterService().GetRainbowContents(input, "master");
            using (StreamReader sr = new StreamReader(memoryStream))
            {
                sr.ReadToEnd().Replace("\r", string.Empty).Should().BeEquivalentTo(@"---
ID: ""a4960f3b-7250-42bb-8df5-2789dcf613ea""
Parent: ""3c1715fe-6a13-4fcf-845f-de308ba9741d""
Template: ""292eb157-5305-4da3-9916-3015d8d32855""
Path: /sitecore/templates/Example item
DB: master
Languages:
- Language: en
  Versions:
  - Version: 1
".Replace("\r", string.Empty));
            }
        }

        [Fact]
        public void ShouldGetRainbowContentsWithMultipleLanguages()
        {
            var input = new Dictionary<string, IDictionary<int, dynamic>>();
            input.Add("en",
                new Dictionary<int, dynamic>()
                {
                    {
                        1,
                        JObject.FromObject(new
                        {
                            ItemID = "a4960f3b-7250-42bb-8df5-2789dcf613ea",
                            ItemName = "Example item",
                            ItemPath = "/sitecore/templates/Example item",
                            TemplateID = "292eb157-5305-4da3-9916-3015d8d32855",
                            ParentID = "3c1715fe-6a13-4fcf-845f-de308ba9741d",
                            metadata = new
                            {
                            }
                        })
                    }
                });
            input.Add("nl-NL", input.First().Value);

            var memoryStream = new RainbowFormatterService().GetRainbowContents(input, "master");
            using (StreamReader sr = new StreamReader(memoryStream))
            {
                sr.ReadToEnd().Replace("\r", string.Empty).Should().BeEquivalentTo(@"---
ID: ""a4960f3b-7250-42bb-8df5-2789dcf613ea""
Parent: ""3c1715fe-6a13-4fcf-845f-de308ba9741d""
Template: ""292eb157-5305-4da3-9916-3015d8d32855""
Path: /sitecore/templates/Example item
DB: master
Languages:
- Language: en
  Versions:
  - Version: 1
- Language: ""nl-NL""
  Versions:
  - Version: 1
".Replace("\r", string.Empty));
            }
        }

        [Fact]
        public void ShouldGetRainbowContentsWithSharedField()
        {
            var input = new Dictionary<string, IDictionary<int, dynamic>>();
            input.Add("en",
                new Dictionary<int, dynamic>()
                {
                    {
                        1,
                        JObject.FromObject(new
                        {
                            ItemID = "a4960f3b-7250-42bb-8df5-2789dcf613ea",
                            ItemName = "Example item",
                            ItemPath = "/sitecore/templates/Example item",
                            TemplateID = "292eb157-5305-4da3-9916-3015d8d32855",
                            ParentID = "3c1715fe-6a13-4fcf-845f-de308ba9741d",
                            SharedExample = "Field value shared",
                            metadata = new
                            {
                                SharedExample = new
                                {
                                    ID = "db380f9c-13ef-4866-8e10-5889fb0209ed",
                                    DisplayName = "Shared example",
                                    Titel = string.Empty,
                                    Type = "Single-Line Text",
                                    Unversioned = false,
                                    Shared = true,
                                    Source = string.Empty,
                                    ContainsStandardValue = false
                                }
                            }
                        })
                    }
                });

            var memoryStream = new RainbowFormatterService().GetRainbowContents(input, "master");
            using (StreamReader sr = new StreamReader(memoryStream))
            {
                sr.ReadToEnd().Replace("\r", string.Empty).Should().BeEquivalentTo(@"---
ID: ""a4960f3b-7250-42bb-8df5-2789dcf613ea""
Parent: ""3c1715fe-6a13-4fcf-845f-de308ba9741d""
Template: ""292eb157-5305-4da3-9916-3015d8d32855""
Path: /sitecore/templates/Example item
DB: master
SharedFields:
- ID: ""db380f9c-13ef-4866-8e10-5889fb0209ed""
  Hint: SharedExample
  Value: Field value shared
Languages:
- Language: en
  Versions:
  - Version: 1
".Replace("\r", string.Empty));
            }
        }

        [Fact]
        public void ShouldGetRainbowContents()
        {
            var input = new Dictionary<string, IDictionary<int, dynamic>>();
            input.Add("en",
                new Dictionary<int, dynamic>()
                {
                    {
                        1,
                        JObject.FromObject(new
                        {
                            ItemID = "a4960f3b-7250-42bb-8df5-2789dcf613ea",
                            ItemName = "Example item",
                            ItemPath = "/sitecore/templates/Example item",
                            TemplateID = "292eb157-5305-4da3-9916-3015d8d32855",
                            ParentID = "3c1715fe-6a13-4fcf-845f-de308ba9741d",
                            metadata = new
                            {

                            }
                        })
                    }
                });

            var memoryStream = new RainbowFormatterService().GetRainbowContents(input, "master");
            using (StreamReader sr = new StreamReader(memoryStream))
            {
                sr.ReadToEnd().Replace("\r", string.Empty).Should().BeEquivalentTo(@"---
ID: ""a4960f3b-7250-42bb-8df5-2789dcf613ea""
Parent: ""3c1715fe-6a13-4fcf-845f-de308ba9741d""
Template: ""292eb157-5305-4da3-9916-3015d8d32855""
Path: /sitecore/templates/Example item
DB: master
SharedFields:
- ID: ""db380f9c-13ef-4866-8e10-5889fb0209ed""
  Hint: Shared example
  Value: Field value shared
Languages:
- Language: en
  Fields:
  - ID: ""ed2d94b1-827f-4467-b430-26c7e6c061ac""
    Hint: Unversioned example
    Value: Field value unversioned in en
  Versions:
  - Version: 1
    Fields:
    - ID: ""25bed78c-4957-4165-998a-ca1b52f67497""
      Hint: __Created
      Value: 20180623T151336Z
    - ID: ""5dd74568-4d4b-44c1-b513-0af5f4cda34f""
      Hint: __Created by
      Value: |
        sitecore\Admin
    - ID: ""a88d1296-5f29-47fe-a8ca-163ef65cbbed""
      Hint: Regular example
      Value: Field value in en
- Language: ""nl-NL""
  Fields:
  - ID: ""ed2d94b1-827f-4467-b430-26c7e6c061ac""
    Hint: Unversioned example
    Value: ""Field value unversioned in nl-NL""
  Versions:
  - Version: 1
    Fields:
    - ID: ""25bed78c-4957-4165-998a-ca1b52f67497""
      Hint: __Created
      Value: 20180623T151426Z
    - ID: ""5dd74568-4d4b-44c1-b513-0af5f4cda34f""
      Hint: __Created by
      Value: |
        sitecore\Admin
    - ID: ""a88d1296-5f29-47fe-a8ca-163ef65cbbed""
      Hint: Regular example
      Value: ""Field value in nl-NL v1""
  - Version: 2
    Fields:
    - ID: ""25bed78c-4957-4165-998a-ca1b52f67497""
      Hint: __Created
      Value: 20180623T151506Z
    - ID: ""5dd74568-4d4b-44c1-b513-0af5f4cda34f""
      Hint: __Created by
      Value: |
        sitecore\Admin
    - ID: ""a88d1296-5f29-47fe-a8ca-163ef65cbbed""
      Hint: Regular example
      Value: ""Field value in nl-NL v2""
    - ID: ""c8f93afe-bfd4-4e8f-9c61-152559854661""
      Hint: __Valid from
      Value: 20180623T151506Z
".Replace("\r", string.Empty));
            }
        }
    }
}
