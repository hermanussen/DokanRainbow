namespace DokanRainbow.Tests.Sitecore
{
    using DokanRainbow.Sitecore;
    using FluentAssertions;
    using Xunit;

    public class ItemServiceClientTest
    {
        [Theory]
        [InlineData("\\", "*", "/sitecore")]
        [InlineData("\\sitecore", "content", "/sitecore/content")]
        [InlineData("\\sitecore\\templates", "common", "/sitecore/templates/common")]
        public void ShouldGetItemPath(string filePath, string searchPattern, string expectedResult)
        {
            new ItemServiceClient("http://localhost", "sitecore", "admin", "b").GetItemPath(filePath, searchPattern)
                .Should().BeEquivalentTo(expectedResult);
        }
    }
}
