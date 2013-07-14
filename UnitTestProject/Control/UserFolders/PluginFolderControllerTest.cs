namespace NIHEI.SC4Buddy.Control.UserFolders
{
    using System.Collections.Generic;

    using Moq;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;

    using Should;

    using Xunit;

    public class UserFolderControllerTest
    {
        [Fact(DisplayName = "ValidatePath(), Empty string & no current id, Return False")]
        public void ValidatePath1()
        {
            var registryMock = new Mock<IUserFolderRegistry>();

            var value = string.Empty;
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(value);

            result.ShouldBeFalse();
            registryMock.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Empty string & with current id, Return False")]
        public void ValidatePath2()
        {
            var registryMock = new Mock<IUserFolderRegistry>();

            var path = string.Empty;
            const int CurrentId = 1;
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(path, CurrentId);

            result.ShouldBeFalse();
            registryMock.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Null as string & no current id, Return False")]
        public void ValidatePath3()
        {
            var registryMock = new Mock<IUserFolderRegistry>();

            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(null);

            result.ShouldBeFalse();
            registryMock.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Null as string & with current id, Return False")]
        public void ValidatePath4()
        {
            var registryMock = new Mock<IUserFolderRegistry>();

            const int CurrentId = 1;
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(null, CurrentId);

            result.ShouldBeFalse();
            registryMock.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Whitespace as string & no current id, Return False")]
        public void ValidatePath5()
        {
            var registryMock = new Mock<IUserFolderRegistry>();

            const string Path = " ";
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(Path);

            result.ShouldBeFalse();
            registryMock.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Whitespace as string & with current id, Return False")]
        public void ValidatePath6()
        {
            var registryMock = new Mock<IUserFolderRegistry>();

            const string Path = " ";
            const int CurrentId = 1;
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(Path, CurrentId);

            result.ShouldBeFalse();
            registryMock.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact]
        public void ValidatePath_InvalidPath_NoCurrentId_ReturnFalse()
        {
            var registryMock = new Mock<IUserFolderRegistry>();

            const string Path = "example";
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(Path);

            result.ShouldBeFalse();
            registryMock.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact]
        public void ValidatePath_InvalidPath_CurrentId_ReturnFalse()
        {
            var registryMock = new Mock<IUserFolderRegistry>();

            const string Path = "example";
            const int Id = 1;
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(Path, Id);

            result.ShouldBeFalse();
            registryMock.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact]
        public void ValidatePath_ValidPath_NoCurrentId_EmptyRegistry_ReturnTrue()
        {
            var registryMock = new Mock<IUserFolderRegistry>();
            registryMock.Setup(x => x.UserFolders).Returns(new List<UserFolder>());

            const string Path = @"C:\Windows";
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(Path);

            result.ShouldBeTrue();
            registryMock.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_NoCurrentId_PopulatedRegistryWithNoMatches_ReturnTrue()
        {
            var registryMock = new Mock<IUserFolderRegistry>();
            registryMock.Setup(x => x.UserFolders).Returns(new List<UserFolder>
                                                                 {
                                                                     new UserFolder
                                                                         {
                                                                             Id = 1, 
                                                                             Alias = "example", 
                                                                             Path = @"C:\example"
                                                                         }, 
                                                                     new UserFolder
                                                                         {
                                                                             Id = 2, 
                                                                             Alias = "foo", 
                                                                             Path = @"C:\foo"
                                                                         }
                                                                 });

            const string Path = @"C:\Windows";
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(Path);

            result.ShouldBeTrue();
            registryMock.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchOnIdButNotPath_ReturnTrue()
        {
            var registryMock = new Mock<IUserFolderRegistry>();
            registryMock.Setup(x => x.UserFolders).Returns(new List<UserFolder>
                                                                 {
                                                                     new UserFolder
                                                                         {
                                                                             Id = 1, 
                                                                             Alias = "example", 
                                                                             Path = @"C:\example"
                                                                         }, 
                                                                     new UserFolder
                                                                         {
                                                                             Id = 2, 
                                                                             Alias = "foo", 
                                                                             Path = @"C:\foo"
                                                                         }
                                                                 });

            const string Path = @"C:\Windows";
            const int Id = 1;
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(Path, Id);

            result.ShouldBeTrue();
            registryMock.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchInPathButNotId_ReturnFalse()
        {
            var registryMock = new Mock<IUserFolderRegistry>();
            registryMock.Setup(x => x.UserFolders).Returns(new List<UserFolder>
                                                                 {
                                                                     new UserFolder
                                                                         {
                                                                             Id = 1, 
                                                                             Alias = "example", 
                                                                             Path = @"C:\example"
                                                                         }, 
                                                                     new UserFolder
                                                                         {
                                                                             Id = 2, 
                                                                             Alias = "foo", 
                                                                             Path = @"C:\foo"
                                                                         },
                                                                     new UserFolder
                                                                         {
                                                                             Id = 3,
                                                                             Alias = "bar",
                                                                             Path = @"C:\Windows"
                                                                         }
                                                                 });

            const string Path = @"C:\Windows";
            const int Id = 1;
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(Path, Id);

            result.ShouldBeFalse();
            registryMock.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchInPathAndId_ReturnTrue()
        {
            var registryMock = new Mock<IUserFolderRegistry>();
            registryMock.Setup(x => x.UserFolders).Returns(new List<UserFolder>
                                                                 {
                                                                     new UserFolder
                                                                         {
                                                                             Id = 1, 
                                                                             Alias = "example", 
                                                                             Path = @"C:\example"
                                                                         }, 
                                                                     new UserFolder
                                                                         {
                                                                             Id = 2, 
                                                                             Alias = "foo", 
                                                                             Path = @"C:\foo"
                                                                         },
                                                                     new UserFolder
                                                                         {
                                                                             Id = 3,
                                                                             Alias = "bar",
                                                                             Path = @"C:\Windows"
                                                                         }
                                                                 });

            const string Path = @"C:\Windows";
            const int Id = 3;
            var instance = new UserFolderController(registryMock.Object);

            var result = instance.ValidatePath(Path, Id);

            result.ShouldBeTrue();
            registryMock.Verify(x => x.UserFolders, Times.Once());
        }
    }
}
