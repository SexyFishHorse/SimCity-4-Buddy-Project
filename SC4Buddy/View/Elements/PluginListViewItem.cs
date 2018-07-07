namespace Nihei.SC4Buddy.View.Elements
{
    using System.Windows.Forms;
    using Nihei.SC4Buddy.Model;

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
