namespace NIHEI.SC4Buddy.Control
{
    using System;

    using Should;

    using Xunit;

    public class PluginFolderControllerTest
    {
        private readonly PluginFolderController instance = new PluginFolderController();
        #region ValidatePathExists

        [Fact]
        public void ValidatePathExists_ValidPath_ReturnTrue()
        {
            const string Value = @"C:\Windows";
            var result = instance.ValidatePathExists(Value);

            result.ShouldBeTrue();
        }

        [Fact]
        public void ValidatePathExists_InvalidPath_ReturnFalse()
        {
            const string Value = @"C:\NIHEISC4BUDDYUNKNOWNFOLDER";
            var result = instance.ValidatePathExists(Value);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidatePathExists_EmptyString_ReturnFalse()
        {
            var value = string.Empty;
            var result = instance.ValidatePathExists(value);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidatePathExists_SingleSpaceString_ReturnFalse()
        {
            const string Value = " ";
            var result = instance.ValidatePathExists(Value);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidatePathExists_Null_ReturnFalse()
        {
            string value = null;
            var result = instance.ValidatePathExists(value);

            result.ShouldBeFalse();
        }

        #endregion


        [Fact]
        public void ValidateAlias_ValidAlias_ReturnTrue()
        {
            const string Value = "example";
            var result = instance.ValidateAlias(Value);

            result.ShouldBeTrue();

        }

        [Fact]
        public void ValidateAlias_EmptyString_ReturnFalse()
        {
            var value = string.Empty;
            var result = instance.ValidateAlias(value);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidateAlias_Null_ReturnFalse()
        {
            string value = null;
            var result = instance.ValidateAlias(value);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidateAlias_SingleSpaceString_ReturnFalse()
        {
            const string Value = " ";
            var result = instance.ValidateAlias(Value);

            result.ShouldBeFalse();
        }
    }
}
