﻿namespace Nihei.SC4Buddy.Installer.FileHandlers
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.Plugins.Installer.FileHandlers;
    using Xunit;

    public class ZipHandlerTest
    {
        #region Strings

        private const string PathToTestMaterial = @"C:\users\asbjorn\git\sc4buddy\TEST";

        private const int ExpectedNumberOfFiles = 2;

        private readonly string archivePath = Path.Combine(PathToTestMaterial, @"zip\archive.zip");

        private readonly string outputFolder = Path.Combine(PathToTestMaterial, @"zip\Output");

        private readonly string tempFolder = Path.Combine(PathToTestMaterial, @"zip\Temp");

        private readonly string outputFile1 = Path.Combine(PathToTestMaterial, @"zip\Output\Plugins", "readme.txt");

        private readonly string outputFile2 = Path.Combine(
            PathToTestMaterial, @"zip\Output\Plugins", "PEG_Mem-Park-Kit_106.dat");

        #endregion

        [Fact(DisplayName = "set_FileInfo, FileInfo for non-zip file, ArgumentException")]
        public void FileInfoSetterTest()
        {
            var instance = new ArchiveHandler();
            var exception =
                Assert.Throws<ArgumentException>(
                    () => instance.FileInfo = new FileInfo(Path.Combine(PathToTestMaterial, "rar/archive.rar")));
            exception.Message.Should().StartWith("FileInfo must point to a .zip file.");
        }

        #region ExtractFilesToTemp

        [Fact(DisplayName = "ExtractFilesToTemp(), No FileInfo, Throws InvalidOperationException")]
        public void ExtractFilesToTempTest1()
        {
            var instance = new ArchiveHandler();

            var exception = Assert.Throws<InvalidOperationException>(() => instance.ExtractFilesToTemp());
            exception.Message.Should().Be("FileInfo is not set.");
        }

        [Fact(DisplayName = "ExtractFilesToTemp(), Valid zipfile, Files moved to temp folder")]
        public void ExtractFilesToTempTest2()
        {
            var instance = new ArchiveHandler
                               {
                                   FileInfo =
                                       new FileInfo(archivePath),
                                   TempFolder = tempFolder
                               };

            instance.ExtractFilesToTemp();

            var installPath = Path.Combine(Path.GetTempPath(), "archive");

            Directory.Exists(installPath).Should().BeTrue("Install directory does not exist.");

            var entries = Directory.GetFileSystemEntries(installPath);
            entries.Length.Should().Be(
                ExpectedNumberOfFiles,
                "archive should only contain two file which should both be copied over.");

            entries.Should().Contain(Path.Combine(Path.GetTempPath(), "archive", "PEG_Mem-Park-Kit_106.dat"));
            entries.Should().Contain(Path.Combine(Path.GetTempPath(), "archive", "readme.txt"));
        }

        #endregion

        #region MoveFilesToUserFolder

        [Fact(DisplayName = "MoveFilesToUserFolder(), Temp file not present & UserFolder is null, "
                            + "Throw InvalidOperationException")]
        public void MoveFilesToUserFolder1()
        {
            var instance = new ArchiveHandler();

            var exception = Assert.Throws<ArgumentNullException>(() => instance.MoveToPluginFolder(null));
            exception.ParamName.Should().Be("userFolder");
            exception.Message.Should().StartWith("UserFolder may not be null.");
        }

        [Fact(DisplayName = "MoveFilesToUserFolder(), Temp file not present & UserFolder is valid, "
                            + "Throw InvalidOperationException")]
        public void MoveFilesToUserFolderTest2()
        {
            var instance = new ArchiveHandler();
            var userFolder = new UserFolder
                                   {
                                       Alias = "Main plugin folder",
                                       FolderPath =
                                           Path.Combine(PathToTestMaterial, "MoveFilesToUserFolderOutput")
                                   };

            var exception = Assert.Throws<InvalidOperationException>(() => instance.MoveToPluginFolder(userFolder));
            exception.Message.Should().Be("The archive has not been extracted to the temp folder.");
        }

        [Fact(DisplayName = "MoveFilesToUserFolder(), Temp file present, UserFolder is null, "
            + "Throw ArgumentException")]
        public void MoveFilesToUserFolderTest3()
        {
            var instance = new ArchiveHandler { FileInfo = new FileInfo(archivePath), TempFolder = tempFolder };

            Action act = () => instance.ExtractFilesToTemp();

            act.Should().NotThrow();

            var exception = Assert.Throws<ArgumentNullException>(() => instance.MoveToPluginFolder(null));
            exception.ParamName.Should().Be("userFolder");
            exception.Message.Should().StartWith("UserFolder may not be null.");
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

            var instance = new ArchiveHandler { FileInfo = new FileInfo(archivePath), TempFolder = tempFolder };
            var userFolder = new UserFolder { Alias = "Main plugin folder", FolderPath = outputFolder };

            Action act = () => instance.ExtractFilesToTemp();

            act.Should().NotThrow();

            var installedFiles = instance.MoveToPluginFolder(userFolder).ToList();

            installedFiles.Should().Contain(new PluginFile { Path = outputFile1, Checksum = "7c1e41fd4219c43db85ca75bbba9d1ad" });
            installedFiles.Should().Contain(new PluginFile { Path = outputFile2, Checksum = "95f09d6e18bc1775b487eaf11909776c" });

            File.Exists(outputFile1).Should().BeTrue("File 1 not in plugin folder.");
            File.Exists(outputFile2).Should().BeTrue("File 2 not in plugin folder.");
        }

        #endregion
    }
}
