namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;

    using NIHEI.SC4Buddy.Entities;

    public class Entities : IEntities
    {
        private readonly DatabaseEntities entities;

        public Entities(DatabaseEntities entities)
        {
            this.entities = entities;
        }

        public IObjectSet<Plugin> Plugins
        {
            get
            {
                return entities.Plugins;
            }
        }

        public IObjectSet<PluginFile> Files
        {
            get
            {
                return entities.PluginFiles;
            }
        }

        public IObjectSet<UserFolder> UserFolders
        {
            get
            {
                return entities.UserFolders;
            }
        }

        public IObjectSet<PluginGroup> Groups
        {
            get
            {
                return entities.PluginGroups;
            }
        }

        public IObjectSet<QuarantinedFile> QuarantinedFiles
        {
            get
            {
                return entities.QuarantinedFile;
            }
        } 

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public void RevertChanges(EntityObject entityObject)
        {
            entities.Refresh(RefreshMode.StoreWins, entityObject);
        }

        public void RevertChanges(IEnumerable<EntityObject> entityCollection)
        {
            entities.Refresh(RefreshMode.StoreWins, entityCollection);
        }
    }
}
