namespace Nihei.SC4Buddy.Control.UserFolders
{
    using System;
    using System.Collections.ObjectModel;
    using Moq;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.UserFolders.Control;
    using Nihei.SC4Buddy.UserFolders.DataAccess;
    using Should;
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

            result.ShouldBeFalse();
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

            result.ShouldBeFalse();
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

            result.ShouldBeFalse();
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

            result.ShouldBeFalse();
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

            result.ShouldBeFalse();
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

            result.ShouldBeFalse();
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

            result.ShouldBeFalse();
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

            result.ShouldBeFalse();
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

            result.ShouldBeTrue();
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
                        new UserFolder { Id = id, Alias = "example", FolderPath = @"C:\example" }
                    });
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object); 
            
            const string Path = @"C:\Windows";

            var result = instance.ValidatePath(Path, id);

            result.ShouldBeTrue();
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

            result.ShouldBeFalse();
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
                            new UserFolder { Id = id, Alias = "bar", FolderPath = @"C:\Windows" }
                        });
            var userFolderController = new Mock<IUserFolderController>();
            var instance = new UserFoldersController(
                userFoldersDataAccess.Object,
                userFolderController.Object);

            const string Path = @"C:\Windows";

            var result = instance.ValidatePath(Path, id);

            result.ShouldBeTrue();
            userFoldersDataAccess.VerifyAll();
        }
    }
}
