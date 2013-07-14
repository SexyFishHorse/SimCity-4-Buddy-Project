namespace NIHEI.SC4Buddy.ArchiveExtractors
{
    using System.IO;

    public class ExtracterTestBase
    {
        private const int ExpectedNumberOfFiles = 2;

        private const string PathToTestMaterial = @"D:\users\asbjorn\SkyDrive\Code\Projects\SC4Buddy\TEST";

        private readonly string archivePath = Path.Combine(PathToTestMaterial, @"zip\archive.zip");

        private readonly string outputFolder = Path.Combine(PathToTestMaterial, @"zip\Output");

        private readonly string outputFile1 = Path.Combine(PathToTestMaterial, @"zip\Output", "readme.txt");

        private readonly string outputFile2 = Path.Combine(
            PathToTestMaterial, @"zip\Output", "PEG_Mem-Park-Kit_106.dat");
    }
}