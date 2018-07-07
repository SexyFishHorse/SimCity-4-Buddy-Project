namespace Nihei.SC4Buddy.Installer.FileHandlers
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Nihei.SC4Buddy.Plugins.Installer.FileHandlers;
    using Xunit;

    public class InstallHandlerTest
    {
        private const string PathToTestMaterial = @"..\..\..\TEST\multirar";

        private readonly string archivePath = Path.Combine(PathToTestMaterial, @"archive.part1.rar");

        private readonly string archivePart2Path = Path.Combine(PathToTestMaterial, @"archive.part2.rar");

        private readonly string outputFolder = Path.Combine(PathToTestMaterial, @"Output");

        private readonly string tempFolder = Path.Combine(PathToTestMaterial, @"Temp");

        [Fact]
        public void ExtractFilesToTempTest1()
        {
            var instance = new ArchiveHandler { FileInfo = new FileInfo(archivePath), TempFolder = tempFolder };

            var infos = instance.ExtractFilesToTemp().ToList();

            infos.Count.Should().Be(7);
            infos.Any(x => x.Name.Equals("Floating_hotel", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            infos.Any(x => x.Name.Equals("Floating_hotel.jpg", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            infos.Any(x => x.Name.Equals("Floating_hotel-0x5ad0e817_0x1112e585_0x30000.SC4Model", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            infos.Any(x => x.Name.Equals("Floating_hotel-0x6534284a-0x304a15b2-0x513da7c8.SC4Desc", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            infos.Any(x => x.Name.Equals("Floating_hotel-dummy-0x5ad0e817_0x511378c7_0x30000.SC4Model", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            infos.Any(x => x.Name.Equals("Floating_hotel-dummy-0x6534284a-0x304a15b2-0x513da8f0.SC4Desc", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
            infos.Any(x => x.Name.Equals("LM1x1_somy-Floating_hotel----------_b13dada1.SC4Lot", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }
    }
}
