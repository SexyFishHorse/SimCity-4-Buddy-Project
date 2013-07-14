namespace NIHEI.SC4Buddy.Entities
{
    public partial class UserFolder
    {
        public string PluginFolderPath
        {
            get
            {
                return System.IO.Path.Combine(Path, "Plugins");
            }
        }

        public string RegionFolderPath
        {
            get
            {
                return System.IO.Path.Combine(Path, "Regions");
            }
        }
    }
}
