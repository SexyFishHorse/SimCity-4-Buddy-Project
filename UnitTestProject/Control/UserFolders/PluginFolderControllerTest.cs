namespace NIHEI.SC4Buddy.Control.UserFolders
{
    using System;
    using System.Data.Objects;
    using System.Linq;

    using Moq;

    using NIHEI.SC4Buddy.DataAccess;
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

            var result = instance.ValidatePath(value, Guid.Empty);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Empty string & with current id, Return False")]
        public void ValidatePath2()
        {
            var entities = new Mock<IEntities>();

            var path = string.Empty;
            var currentId = Guid.NewGuid();
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(path, currentId);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Null as string & no current id, Return False")]
        public void ValidatePath3()
        {
            var entities = new Mock<IEntities>();

            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(null, Guid.Empty);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Null as string & with current id, Return False")]
        public void ValidatePath4()
        {
            var entities = new Mock<IEntities>();

            var currentId = Guid.NewGuid();
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(null, currentId);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Whitespace as string & no current id, Return False")]
        public void ValidatePath5()
        {
            var entities = new Mock<IEntities>();

            const string Path = " ";
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, Guid.Empty);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact(DisplayName = "ValidatePath(), Whitespace as string & with current id, Return False")]
        public void ValidatePath6()
        {
            var entities = new Mock<IEntities>();

            const string Path = " ";
            var currentId = Guid.NewGuid();
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, currentId);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact]
        public void ValidatePath_InvalidPath_NoCurrentId_ReturnFalse()
        {
            var entities = new Mock<IEntities>();

            const string Path = "example";
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, Guid.Empty);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Never());
        }

        [Fact]
        public void ValidatePath_InvalidPath_CurrentId_ReturnFalse()
        {
            var entities = new Mock<IEntities>();

            const string Path = "example";
            var id = Guid.NewGuid();
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, id);

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

            var result = instance.ValidatePath(Path, Guid.Empty);

            result.ShouldBeTrue();
            entities.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchOnIdButNotPath_ReturnTrue()
        {
            var id = Guid.NewGuid();
            var entities = new Mock<IEntities>();
            var objectSetMock = new Mock<IObjectSet<UserFolder>>();
            objectSetMock.Setup(x => x.FirstOrDefault())
                .Returns(() => new UserFolder(id) { Alias = "example", FolderPath = @"C:\example" });

            const string Path = @"C:\Windows";
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, id);

            result.ShouldBeTrue();
            entities.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchInPathButNotId_ReturnFalse()
        {
            var entities = new Mock<IEntities>();
            var objectSetMock = new Mock<IObjectSet<UserFolder>>();
            objectSetMock.Setup(x => x.FirstOrDefault())
                .Returns(() => new UserFolder(Guid.NewGuid()) { Alias = "bar", FolderPath = @"C:\Windows" });

            const string Path = @"C:\Windows";
            var id = Guid.NewGuid();
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, id);

            result.ShouldBeFalse();
            entities.Verify(x => x.UserFolders, Times.Once());
        }

        [Fact]
        public void ValidatePath_ValidPath_CurrentId_PopulatedRegistryWithMatchInPathAndId_ReturnTrue()
        {
            var id = Guid.NewGuid();

            var entities = new Mock<IEntities>();
            var objectSetMock = new Mock<IObjectSet<UserFolder>>();
            objectSetMock.Setup(x => x.FirstOrDefault())
                .Returns(new UserFolder(id) { Alias = "bar", FolderPath = @"C:\Windows" });

            const string Path = @"C:\Windows";
            var instance = new UserFolderController(entities.Object);

            var result = instance.ValidatePath(Path, id);

            result.ShouldBeTrue();
            entities.Verify(x => x.UserFolders, Times.Once());
        }
    }
}
