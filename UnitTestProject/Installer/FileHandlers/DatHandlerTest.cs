namespace NIHEI.SC4Buddy.Installer.FileHandlers
{
    using System;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Model;

    using Should;

    using Xunit;

    public class DatHandlerTest
    {
        private const string PathToTestMaterial = @"D:\users\asbjorn\SkyDrive\Code\Projects\SC4Buddy\TEST";

        private readonly string archivePath = Path.Combine(PathToTestMaterial, @"dat\file.dat");

        private readonly string outputFolder = Path.Combine(PathToTestMaterial, @"dat\Output");

        private readonly string tempFolder = Path.Combine(PathToTestMaterial, @"dat\Temp");

        private readonly string outputFile1 = Path.Combine(PathToTestMaterial, @"dat\Output\Plugins", "file.dat");

        [Fact(DisplayName = "set_FileInfo, FileInfo for non-dat file, ArgumentException")]
        public void FileInfoSetterTest()
        {
            var instance = new DatHandler();
            var exception =
                Assert.Throws<ArgumentException>(() => instance.FileInfo = new FileInfo(
                    Path.Combine(PathToTestMaterial, @"zip\archive.zip")));
            exception.Message.ShouldStartWith("FileInfo must point to a .dat file.");
        }

        [Fact(DisplayName = "ExtractFilesToTemp(), No FileInfo, Throws InvalidOperationException")]
        public void MoveFilesToTempTest1()
        {
            var instance = new DatHandler();
            Assert.Throws<InvalidOperationException>(() => instance.ExtractFilesToTemp());
        }

        [Fact(DisplayName = "ExtractFilesToTemp(), Valid datfile, Files moved to temp folder")]
        public void ExtractFilesTioTempTest2()
        {
            var instance = new DatHandler
                               {
                                   FileInfo = new FileInfo(archivePath),
                                   TempFolder = tempFolder
                               };

            var infos = instance.ExtractFilesToTemp().ToList();

            infos.Count.ShouldEqual(1);
            infos[0].Name.ShouldEqual("file.dat");
        }

        [Fact(DisplayName = "MoveFilesToUserFolder(), Temp file not present & UserFolder is null, "
                            + "Throw InvalidOperationException")]
        public void MoveFilesToUserFolder1()
        {
            var instance = new DatHandler();

            var exception = Assert.Throws<ArgumentNullException>(() => instance.MoveToPluginFolder(null));
            exception.ParamName.ShouldEqual("userFolder");
            exception.Message.ShouldStartWith("UserFolder may not be null.");
        }

        [Fact(DisplayName = "MoveFilesToUserFolder(), Temp file not present & UserFolder is valid, "
                            + "Throw InvalidOperationException")]
        public void MoveFilesToUserFolderTest2()
        {
            var instance = new DatHandler();
            var userFolder = new UserFolder
            {
                Alias = "Main plugin folder",
                Path =
                    Path.Combine(PathToTestMaterial, "MoveFilesToUserFolderOutput")
            };

            var exception = Assert.Throws<InvalidOperationException>(() => instance.MoveToPluginFolder(userFolder));
            exception.Message.ShouldEqual("The archive has not been extracted to the temp folder.");
        }

        [Fact(DisplayName = "MoveFilesToUserFolder(), Temp file present, UserFolder is null, "
            + "Throw ArgumentException")]
        public void MoveFilesToUserFolderTest3()
        {
            var instance = new DatHandler { FileInfo = new FileInfo(archivePath), TempFolder = tempFolder };

            Assert.DoesNotThrow(() => instance.ExtractFilesToTemp());

            var exception = Assert.Throws<ArgumentNullException>(() => instance.MoveToPluginFolder(null));
            exception.ParamName.ShouldEqual("userFolder");
            exception.Message.ShouldStartWith("UserFolder may not be null.");
        }

        [Fact(DisplayName = "MoveFilesToUserFolder(), Temp file not present & UserFolder is valid, "
                            + "Files in plugin folder & Return list of installed files")]
        public void MoveFilesToUserFolderTest4()
        {
            if (Directory.Exists(Path.Combine(PathToTestMaterial, "MoveFilesToUserFolderOutput")))
            {
                Directory.Delete(Path.Combine(PathToTestMaterial, "MoveFilesToUserFolderOutput"), true);
            }

            var folderCreated =
                Directory.CreateDirectory(Path.Combine(PathToTestMaterial, "MoveFilesToUserFolderOutput"));
            if (!folderCreated.Exists)
            {
                throw new InvalidOperationException("output folder not created. (" + folderCreated.FullName + ")");
            }

            var instance = new DatHandler { FileInfo = new FileInfo(archivePath), TempFolder = tempFolder };
            var userFolder = new UserFolder { Alias = "Main plugin folder", Path = outputFolder };

            Assert.DoesNotThrow(() => instance.ExtractFilesToTemp());
            var installedFiles = instance.MoveToPluginFolder(userFolder).ToList();

            installedFiles.ShouldContain(
                new PluginFile { Path = outputFile1, Checksum = "ce54d1157f2cea1d77bb0e3aef45b37c" },
                new BaseHandlerTest.PluginFileTestComparer());

            File.Exists(outputFile1).ShouldBeTrue("File 1 not in plugin folder.");
        }
    }
}
