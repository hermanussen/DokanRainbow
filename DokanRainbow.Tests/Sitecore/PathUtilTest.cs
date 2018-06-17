namespace DokanRainbow.Tests.Sitecore
{
    using DokanRainbow.Sitecore;
    using FluentAssertions;
    using Xunit;

    public class PathUtilTest
    {
        [Theory]
        [InlineData("\\", "*", "/", true)]
        [InlineData("\\", " * ", "/", true)]
        [InlineData("\\", "sitecore", "/sitecore", false)]
        [InlineData("\\", "sitecore.yml", "/sitecore", false)]
        [InlineData("\\sitecore", "templates", "/sitecore/templates", false)]
        [InlineData("\\sitecore\\templates", "*", "/sitecore/templates", true)]
        [InlineData("\\sitecore", "templates.yml", "/sitecore/templates", false)]
        [InlineData("\\client\\Speak.yml", null, "/client/speak", false)]
        public void ShouldGetItemPath(string filePath, string searchPattern, string expectedResult, bool expectedChildrenOf)
        {
            var result = PathUtil.GetItemPath(filePath, searchPattern);
            result.path.Should().BeEquivalentTo(expectedResult);
            result.childrenOf.Should().Be(expectedChildrenOf);
        }
    }
}
