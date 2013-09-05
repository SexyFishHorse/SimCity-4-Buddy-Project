﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[assembly: EdmSchemaAttribute()]
#region EDM Relationship Metadata

[assembly: EdmRelationshipAttribute("NIHEI.SC4Buddy.Entities", "FK_Plugin_File", "Plugin", System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(NIHEI.SC4Buddy.Entities.Plugin), "File", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(NIHEI.SC4Buddy.Entities.PluginFile), true)]
[assembly: EdmRelationshipAttribute("NIHEI.SC4Buddy.Entities", "FK_PluginGroup_Plugin", "PluginGroup", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(NIHEI.SC4Buddy.Entities.PluginGroup), "Plugin", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(NIHEI.SC4Buddy.Entities.Plugin), true)]
[assembly: EdmRelationshipAttribute("NIHEI.SC4Buddy.Entities", "FK_UserFolder_Plugin", "UserFolder", System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(NIHEI.SC4Buddy.Entities.UserFolder), "Plugin", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(NIHEI.SC4Buddy.Entities.Plugin), true)]

#endregion

namespace NIHEI.SC4Buddy.Entities
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class DatabaseEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new DatabaseEntities object using the connection string found in the 'DatabaseEntities' section of the application configuration file.
        /// </summary>
        public DatabaseEntities() : base("name=DatabaseEntities", "DatabaseEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new DatabaseEntities object.
        /// </summary>
        public DatabaseEntities(string connectionString) : base(connectionString, "DatabaseEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new DatabaseEntities object.
        /// </summary>
        public DatabaseEntities(EntityConnection connection) : base(connection, "DatabaseEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<PluginFile> PluginFiles
        {
            get
            {
                if ((_PluginFiles == null))
                {
                    _PluginFiles = base.CreateObjectSet<PluginFile>("PluginFiles");
                }
                return _PluginFiles;
            }
        }
        private ObjectSet<PluginFile> _PluginFiles;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Plugin> Plugins
        {
            get
            {
                if ((_Plugins == null))
                {
                    _Plugins = base.CreateObjectSet<Plugin>("Plugins");
                }
                return _Plugins;
            }
        }
        private ObjectSet<Plugin> _Plugins;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<PluginGroup> PluginGroups
        {
            get
            {
                if ((_PluginGroups == null))
                {
                    _PluginGroups = base.CreateObjectSet<PluginGroup>("PluginGroups");
                }
                return _PluginGroups;
            }
        }
        private ObjectSet<PluginGroup> _PluginGroups;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<UserFolder> UserFolders
        {
            get
            {
                if ((_UserFolders == null))
                {
                    _UserFolders = base.CreateObjectSet<UserFolder>("UserFolders");
                }
                return _UserFolders;
            }
        }
        private ObjectSet<UserFolder> _UserFolders;

        #endregion

        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the PluginFiles EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToPluginFiles(PluginFile pluginFile)
        {
            base.AddObject("PluginFiles", pluginFile);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Plugins EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToPlugins(Plugin plugin)
        {
            base.AddObject("Plugins", plugin);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the PluginGroups EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToPluginGroups(PluginGroup pluginGroup)
        {
            base.AddObject("PluginGroups", pluginGroup);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the UserFolders EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToUserFolders(UserFolder userFolder)
        {
            base.AddObject("UserFolders", userFolder);
        }

        #endregion

    }

    #endregion

    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="NIHEI.SC4Buddy.Entities", Name="Plugin")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Plugin : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Plugin object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        /// <param name="userFolderId">Initial value of the UserFolderId property.</param>
        public static Plugin CreatePlugin(global::System.Int64 id, global::System.String name, global::System.Int64 userFolderId)
        {
            Plugin plugin = new Plugin();
            plugin.Id = id;
            plugin.Name = name;
            plugin.UserFolderId = userFolderId;
            return plugin;
        }

        #endregion

        #region Simple Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value, "Id");
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int64 _Id;
        partial void OnIdChanging(global::System.Int64 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, false, "Name");
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Author
        {
            get
            {
                return _Author;
            }
            set
            {
                OnAuthorChanging(value);
                ReportPropertyChanging("Author");
                _Author = StructuralObject.SetValidValue(value, true, "Author");
                ReportPropertyChanged("Author");
                OnAuthorChanged();
            }
        }
        private global::System.String _Author;
        partial void OnAuthorChanging(global::System.String value);
        partial void OnAuthorChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                OnDescriptionChanging(value);
                ReportPropertyChanging("Description");
                _Description = StructuralObject.SetValidValue(value, true, "Description");
                ReportPropertyChanged("Description");
                OnDescriptionChanged();
            }
        }
        private global::System.String _Description;
        partial void OnDescriptionChanging(global::System.String value);
        partial void OnDescriptionChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int64> PluginGroupId
        {
            get
            {
                return _PluginGroupId;
            }
            set
            {
                OnPluginGroupIdChanging(value);
                ReportPropertyChanging("PluginGroupId");
                _PluginGroupId = StructuralObject.SetValidValue(value, "PluginGroupId");
                ReportPropertyChanged("PluginGroupId");
                OnPluginGroupIdChanged();
            }
        }
        private Nullable<global::System.Int64> _PluginGroupId;
        partial void OnPluginGroupIdChanging(Nullable<global::System.Int64> value);
        partial void OnPluginGroupIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Link
        {
            get
            {
                return _Link;
            }
            set
            {
                OnLinkChanging(value);
                ReportPropertyChanging("Link");
                _Link = StructuralObject.SetValidValue(value, true, "Link");
                ReportPropertyChanged("Link");
                OnLinkChanged();
            }
        }
        private global::System.String _Link;
        partial void OnLinkChanging(global::System.String value);
        partial void OnLinkChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int64> RemotePluginId
        {
            get
            {
                return _RemotePluginId;
            }
            set
            {
                OnRemotePluginIdChanging(value);
                ReportPropertyChanging("RemotePluginId");
                _RemotePluginId = StructuralObject.SetValidValue(value, "RemotePluginId");
                ReportPropertyChanged("RemotePluginId");
                OnRemotePluginIdChanged();
            }
        }
        private Nullable<global::System.Int64> _RemotePluginId;
        partial void OnRemotePluginIdChanging(Nullable<global::System.Int64> value);
        partial void OnRemotePluginIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 UserFolderId
        {
            get
            {
                return _UserFolderId;
            }
            set
            {
                OnUserFolderIdChanging(value);
                ReportPropertyChanging("UserFolderId");
                _UserFolderId = StructuralObject.SetValidValue(value, "UserFolderId");
                ReportPropertyChanged("UserFolderId");
                OnUserFolderIdChanged();
            }
        }
        private global::System.Int64 _UserFolderId;
        partial void OnUserFolderIdChanging(global::System.Int64 value);
        partial void OnUserFolderIdChanged();

        #endregion

        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("NIHEI.SC4Buddy.Entities", "FK_Plugin_File", "File")]
        public EntityCollection<PluginFile> Files
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<PluginFile>("NIHEI.SC4Buddy.Entities.FK_Plugin_File", "File");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<PluginFile>("NIHEI.SC4Buddy.Entities.FK_Plugin_File", "File", value);
                }
            }
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("NIHEI.SC4Buddy.Entities", "FK_PluginGroup_Plugin", "PluginGroup")]
        public PluginGroup Group
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<PluginGroup>("NIHEI.SC4Buddy.Entities.FK_PluginGroup_Plugin", "PluginGroup").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<PluginGroup>("NIHEI.SC4Buddy.Entities.FK_PluginGroup_Plugin", "PluginGroup").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<PluginGroup> GroupReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<PluginGroup>("NIHEI.SC4Buddy.Entities.FK_PluginGroup_Plugin", "PluginGroup");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<PluginGroup>("NIHEI.SC4Buddy.Entities.FK_PluginGroup_Plugin", "PluginGroup", value);
                }
            }
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("NIHEI.SC4Buddy.Entities", "FK_UserFolder_Plugin", "UserFolder")]
        public UserFolder UserFolder
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<UserFolder>("NIHEI.SC4Buddy.Entities.FK_UserFolder_Plugin", "UserFolder").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<UserFolder>("NIHEI.SC4Buddy.Entities.FK_UserFolder_Plugin", "UserFolder").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<UserFolder> UserFolderReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<UserFolder>("NIHEI.SC4Buddy.Entities.FK_UserFolder_Plugin", "UserFolder");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<UserFolder>("NIHEI.SC4Buddy.Entities.FK_UserFolder_Plugin", "UserFolder", value);
                }
            }
        }

        #endregion

    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="NIHEI.SC4Buddy.Entities", Name="PluginFile")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class PluginFile : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new PluginFile object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        /// <param name="path">Initial value of the Path property.</param>
        /// <param name="checksum">Initial value of the Checksum property.</param>
        /// <param name="pluginId">Initial value of the PluginId property.</param>
        public static PluginFile CreatePluginFile(global::System.Int64 id, global::System.String path, global::System.String checksum, global::System.Int64 pluginId)
        {
            PluginFile pluginFile = new PluginFile();
            pluginFile.Id = id;
            pluginFile.Path = path;
            pluginFile.Checksum = checksum;
            pluginFile.PluginId = pluginId;
            return pluginFile;
        }

        #endregion

        #region Simple Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value, "Id");
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int64 _Id;
        partial void OnIdChanging(global::System.Int64 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Path
        {
            get
            {
                return _Path;
            }
            set
            {
                OnPathChanging(value);
                ReportPropertyChanging("Path");
                _Path = StructuralObject.SetValidValue(value, false, "Path");
                ReportPropertyChanged("Path");
                OnPathChanged();
            }
        }
        private global::System.String _Path;
        partial void OnPathChanging(global::System.String value);
        partial void OnPathChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Checksum
        {
            get
            {
                return _Checksum;
            }
            set
            {
                OnChecksumChanging(value);
                ReportPropertyChanging("Checksum");
                _Checksum = StructuralObject.SetValidValue(value, false, "Checksum");
                ReportPropertyChanged("Checksum");
                OnChecksumChanged();
            }
        }
        private global::System.String _Checksum;
        partial void OnChecksumChanging(global::System.String value);
        partial void OnChecksumChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 PluginId
        {
            get
            {
                return _PluginId;
            }
            set
            {
                OnPluginIdChanging(value);
                ReportPropertyChanging("PluginId");
                _PluginId = StructuralObject.SetValidValue(value, "PluginId");
                ReportPropertyChanged("PluginId");
                OnPluginIdChanged();
            }
        }
        private global::System.Int64 _PluginId;
        partial void OnPluginIdChanging(global::System.Int64 value);
        partial void OnPluginIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Boolean> Quarantined
        {
            get
            {
                return _Quarantined;
            }
            set
            {
                OnQuarantinedChanging(value);
                ReportPropertyChanging("Quarantined");
                _Quarantined = StructuralObject.SetValidValue(value, "Quarantined");
                ReportPropertyChanged("Quarantined");
                OnQuarantinedChanged();
            }
        }
        private Nullable<global::System.Boolean> _Quarantined;
        partial void OnQuarantinedChanging(Nullable<global::System.Boolean> value);
        partial void OnQuarantinedChanged();

        #endregion

        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("NIHEI.SC4Buddy.Entities", "FK_Plugin_File", "Plugin")]
        public Plugin Plugin
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Plugin>("NIHEI.SC4Buddy.Entities.FK_Plugin_File", "Plugin").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Plugin>("NIHEI.SC4Buddy.Entities.FK_Plugin_File", "Plugin").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<Plugin> PluginReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Plugin>("NIHEI.SC4Buddy.Entities.FK_Plugin_File", "Plugin");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<Plugin>("NIHEI.SC4Buddy.Entities.FK_Plugin_File", "Plugin", value);
                }
            }
        }

        #endregion

    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="NIHEI.SC4Buddy.Entities", Name="PluginGroup")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class PluginGroup : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new PluginGroup object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        public static PluginGroup CreatePluginGroup(global::System.Int64 id, global::System.String name)
        {
            PluginGroup pluginGroup = new PluginGroup();
            pluginGroup.Id = id;
            pluginGroup.Name = name;
            return pluginGroup;
        }

        #endregion

        #region Simple Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value, "Id");
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int64 _Id;
        partial void OnIdChanging(global::System.Int64 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, false, "Name");
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();

        #endregion

        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("NIHEI.SC4Buddy.Entities", "FK_PluginGroup_Plugin", "Plugin")]
        public EntityCollection<Plugin> Plugins
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Plugin>("NIHEI.SC4Buddy.Entities.FK_PluginGroup_Plugin", "Plugin");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Plugin>("NIHEI.SC4Buddy.Entities.FK_PluginGroup_Plugin", "Plugin", value);
                }
            }
        }

        #endregion

    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="NIHEI.SC4Buddy.Entities", Name="UserFolder")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class UserFolder : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new UserFolder object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        /// <param name="path">Initial value of the Path property.</param>
        /// <param name="alias">Initial value of the Alias property.</param>
        public static UserFolder CreateUserFolder(global::System.Int64 id, global::System.String path, global::System.String alias)
        {
            UserFolder userFolder = new UserFolder();
            userFolder.Id = id;
            userFolder.Path = path;
            userFolder.Alias = alias;
            return userFolder;
        }

        #endregion

        #region Simple Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value, "Id");
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int64 _Id;
        partial void OnIdChanging(global::System.Int64 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Path
        {
            get
            {
                return _Path;
            }
            set
            {
                OnPathChanging(value);
                ReportPropertyChanging("Path");
                _Path = StructuralObject.SetValidValue(value, false, "Path");
                ReportPropertyChanged("Path");
                OnPathChanged();
            }
        }
        private global::System.String _Path;
        partial void OnPathChanging(global::System.String value);
        partial void OnPathChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Alias
        {
            get
            {
                return _Alias;
            }
            set
            {
                OnAliasChanging(value);
                ReportPropertyChanging("Alias");
                _Alias = StructuralObject.SetValidValue(value, false, "Alias");
                ReportPropertyChanged("Alias");
                OnAliasChanged();
            }
        }
        private global::System.String _Alias;
        partial void OnAliasChanging(global::System.String value);
        partial void OnAliasChanged();

        #endregion

        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("NIHEI.SC4Buddy.Entities", "FK_UserFolder_Plugin", "Plugin")]
        public EntityCollection<Plugin> Plugins
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Plugin>("NIHEI.SC4Buddy.Entities.FK_UserFolder_Plugin", "Plugin");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Plugin>("NIHEI.SC4Buddy.Entities.FK_UserFolder_Plugin", "Plugin", value);
                }
            }
        }

        #endregion

    }

    #endregion

}
