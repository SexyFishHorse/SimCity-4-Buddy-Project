namespace Nihei.SC4Buddy.Control.UserFolders
{
    using System;
    using System.Collections.ObjectModel;
    using FluentAssertions;
    using Moq;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.UserFolders.Control;
    using Nihei.SC4Buddy.UserFolders.DataAccess;
    using Xunit;

    public class UserFoldersControllerTest
    {
        [Fact(DisplayName = "ValidatePath(), Empty string & no current id, Return False")]
        public void ValidatePath1()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            var value = string.Empty;

            var result = instance.ValidatePath(value, Guid.Empty);

            result.Should().BeFalse();
        }

        [Fact(DisplayName = "ValidatePath(), Empty string & with current id, Return False")]
        public void ValidatePath2()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            var path = string.Empty;
            var currentId = Guid.NewGuid();

            var result = instance.ValidatePath(path, currentId);

            result.Should().BeFalse();
        }

        [Fact(DisplayName = "ValidatePath(), Null as string & no current id, Return False")]
        public void ValidatePath3()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            var result = instance.ValidatePath(null, Guid.Empty);

            result.Should().BeFalse();
        }

        [Fact(DisplayName = "ValidatePath(), Null as string & with current id, Return False")]
        public void ValidatePath4()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            var currentId = Guid.NewGuid();
            var result = instance.ValidatePath(null, currentId);

            result.Should().BeFalse();
        }

        [Fact(DisplayName = "ValidatePath(), Whitespace as string & no current id, Return False")]
        public void ValidatePath5()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            const string Path = " ";

            var result = instance.ValidatePath(Path, Guid.Empty);

            result.Should().BeFalse();
        }

        [Fact(DisplayName = "ValidatePath(), Whitespace as string & with current id, Return False")]
        public void ValidatePath6()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            const string Path = " ";
            var currentId = Guid.NewGuid();

            var result = instance.ValidatePath(Path, currentId);

            result.Should().BeFalse();
        }

        [Fact]
        public void ValidatePath_InvalidPath_NoCurrentId_ReturnFalse()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            const string Path = "example";

            var result = instance.ValidatePath(Path, Guid.Empty);

            result.Should().BeFalse();
        }

        [Fact]
        public void ValidatePath_InvalidPath_CurrentId_ReturnFalse()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            const string Path = "example";
            var id = Guid.NewGuid();

            var result = instance.ValidatePath(Path, id);

            result.Should().BeFalse();
        }

        [Fact]
        public void ValidatePath_ValidPath_NoCurrentId_ReturnTrue()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            userFoldersDataAccess.Setup(x => x.LoadUserFolders()).Returns(new Collection<UserFolder>());
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            const string Path = @"C:\Windows";

            var result = instance.ValidatePath(Path, Guid.Empty);

            result.Should().BeTrue();
            userFoldersDataAccess.VerifyAll();
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchOnIdButNotPath_ReturnTrue()
        {
            var id = Guid.NewGuid();

            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            userFoldersDataAccess.Setup(x => x.LoadUserFolders())
                .Returns(
                    new Collection<UserFolder>
                    {
                        new UserFolder(id) { Alias = "example", FolderPath = @"C:\example" }
                    });
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            const string Path = @"C:\Windows";

            var result = instance.ValidatePath(Path, id);

            result.Should().BeTrue();
            userFoldersDataAccess.VerifyAll();
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchInPathButNotId_ReturnFalse()
        {
            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            userFoldersDataAccess.Setup(x => x.LoadUserFolders()).Returns(new Collection<UserFolder> { new UserFolder { Alias = "bar", FolderPath = @"C:\Windows" } });
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            const string Path = @"C:\Windows";
            var id = Guid.NewGuid();

            var result = instance.ValidatePath(Path, id);

            result.Should().BeFalse();
            userFoldersDataAccess.VerifyAll();
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchInPathAndId_ReturnTrue()
        {
            var id = Guid.NewGuid();

            var userFoldersDataAccess = new Mock<IUserFoldersDataAccess>();
            userFoldersDataAccess.Setup(x => x.LoadUserFolders())
                    .Returns(
                        new Collection<UserFolder>
                        {
                            new UserFolder(id) { Alias = "bar", FolderPath = @"C:\Windows" }
                        });
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            const string Path = @"C:\Windows";

            var result = instance.ValidatePath(Path, id);

            result.Should().BeTrue();
            userFoldersDataAccess.VerifyAll();
        }
    }
}
