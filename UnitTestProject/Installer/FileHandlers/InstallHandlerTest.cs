namespace NIHEI.SC4Buddy.Installer.FileHandlers
{
    using System.IO;

    using Xunit;

    public class InstallHandlerTest
    {
        private const string PathToTestMaterial = @"..\..\TEST";

        private readonly string archivePath = Path.Combine(PathToTestMaterial, @"multirar\archive.part1.rar");

        private readonly string outputFolder = Path.Combine(PathToTestMaterial, @"multirar\Output");

        private readonly string tempFolder = Path.Combine(PathToTestMaterial, @"multirar\Temp");

        [Fact]
        public void ExtractFilesToTempTest1()
        {

        }
    }
}
