namespace NIHEI.SC4Buddy.View.Elements
{
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Entities;

    public class PluginListViewItem : ListViewItem
    {
        public PluginListViewItem(Plugin plugin, ListViewGroup @group)
            : base(plugin.Name, @group)
        {
            Plugin = plugin;
        }

        public Plugin Plugin { get; private set; }
    }
}
