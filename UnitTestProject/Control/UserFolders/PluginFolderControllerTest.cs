namespace NIHEI.SC4Buddy.Control.UserFolders
{
    using System.Data.Objects;
    using System.Linq;

    using Moq;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Model;

    using Should;

    using Xunit;

    public class UserFolderControllerTest
    {
        [Fact(DisplayName = "ValidatePath(), Empty string & no current id, Return False")]
        public void ValidatePath1()
        {
            var entities = new Mock<IEntities>();

            var value = string.Empty;
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(value);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Empty string & with current id, Return False")]
        public void ValidatePath2()
        {
            var entities = new Mock<IEntities>();

            var path = string.Empty;
            const int CurrentId = 1;
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(path, CurrentId);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Null as string & no current id, Return False")]
        public void ValidatePath3()
        {
            var entities = new Mock<IEntities>();

            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(null);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Null as string & with current id, Return False")]
        public void ValidatePath4()
        {
            var entities = new Mock<IEntities>();

            const int CurrentId = 1;
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(null, CurrentId);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Whitespace as string & no current id, Return False")]
        public void ValidatePath5()
        {
            var entities = new Mock<IEntities>();

            const string Path = " ";
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Whitespace as string & with current id, Return False")]
        public void ValidatePath6()
        {
            var entities = new Mock<IEntities>();

            const string Path = " ";
            const int CurrentId = 1;
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, CurrentId);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact]
        public void ValidatePath_InvalidPath_NoCurrentId_ReturnFalse()
        {
            var entities = new Mock<IEntities>();

            const string Path = "example";
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact]
        public void ValidatePath_InvalidPath_CurrentId_ReturnFalse()
        {
            var entities = new Mock<IEntities>();

            const string Path = "example";
            const int Id = 1;
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, Id);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact]
        public void ValidatePath_ValidPath_NoCurrentId_ReturnTrue()
        {
            var entities = new Mock<IEntities>();
            var objectSetMock = new Mock<IObjectSet<UserFolder>>();
            objectSetMock.Setup(x => x.FirstOrDefault()).Returns(() => null);

            const string Path = @"C:\Windows";
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path);

            result.ShouldBeTrue();
            entities.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchOnIdButNotPath_ReturnTrue()
        {
            var entities = new Mock<IEntities>();
            var objectSetMock = new Mock<IObjectSet<UserFolder>>();
            objectSetMock.Setup(x => x.FirstOrDefault())
                .Returns(() => new UserFolder { Id = 1, Alias = "example", Path = @"C:\example" });

            const string Path = @"C:\Windows";
            const int Id = 1;
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, Id);

            result.ShouldBeTrue();
            entities.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchInPathButNotId_ReturnFalse()
        {
            var entities = new Mock<IEntities>();
            var objectSetMock = new Mock<IObjectSet<UserFolder>>();
            objectSetMock.Setup(x => x.FirstOrDefault())
                .Returns(() => new UserFolder { Id = 3, Alias = "bar", Path = @"C:\Windows" });

            const string Path = @"C:\Windows";
            const int Id = 1;
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, Id);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchInPathAndId_ReturnTrue()
        {
            var entities = new Mock<IEntities>();
            var objectSetMock = new Mock<IObjectSet<UserFolder>>();
            objectSetMock.Setup(x => x.FirstOrDefault())
                .Returns(new UserFolder { Id = 3, Alias = "bar", Path = @"C:\Windows" });

            const string Path = @"C:\Windows";
            const int Id = 3;
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, Id);

            result.ShouldBeTrue();
            entities.Verify(x => x.UserFolders, Times.Once());
        }
    }
}
