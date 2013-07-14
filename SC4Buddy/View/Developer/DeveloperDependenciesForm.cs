namespace NIHEI.SC4Buddy.View.Developer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.Common;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class DeveloperDependenciesForm : Form
    {
        private readonly RemotePluginRegistry registry;

        public DeveloperDependenciesForm()
        {
            InitializeComponent();

            registry = RemoteRegistryFactory.RemotePluginRegistry;

            Dependencies = new List<RemotePlugin>();
        }

        public List<RemotePlugin> Dependencies { get; set; }

        private void SearchTbTextChanged(object sender, EventArgs e)
        {
            var txt = searchTB.Text;

            resultsLB.BeginUpdate();
            resultsLB.Items.Clear();

            if (txt.Length > 2)
            {
                var results =
                    registry.RemotePlugins.Where(
                        x => x.Name.ToUpper().Contains(txt.ToUpper()) || x.Author.ToUpper().Contains(txt.ToUpper()));

                foreach (var result in from result in results
                                       let plugin = result
                                       where
                                           !depLB.Items.Cast<ToStringDecorator<RemotePlugin>>()
                                                 .Any(x => x.ObjectValue.Name.Equals(plugin.Name))
                                       select result)
                {
                    resultsLB.Items.Add(
                        new ToStringDecorator<RemotePlugin>(
                            result, string.Format("{0} - {1}", result.Author, result.Name)));
                }
            }

            resultsLB.EndUpdate();
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            var selectedItems =
                resultsLB.SelectedItems.Cast<ToStringDecorator<RemotePlugin>>().ToList();
            var items = new ToStringDecorator<RemotePlugin>[selectedItems.Count];
            selectedItems.CopyTo(items, 0);

            resultsLB.BeginUpdate();
            depLB.BeginUpdate();

            foreach (var selectedItem in items)
            {
                resultsLB.Items.Remove(selectedItem);
                depLB.Items.Add(selectedItem);
            }

            resultsLB.EndUpdate();
            depLB.EndUpdate();
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            var selectedItems = depLB.SelectedItems.Cast<ToStringDecorator<RemotePlugin>>().ToList();
            var items = new ToStringDecorator<RemotePlugin>[selectedItems.Count];
            selectedItems.CopyTo(items);

            var txt = searchTB.Text;
            foreach (ToStringDecorator<RemotePlugin> selectedItem in items)
            {
                depLB.Items.Remove(selectedItem);
                if (txt.Length > 2 && selectedItem.ObjectValue.Name.ToUpper().Contains(txt.ToUpper()))
                {
                    resultsLB.Items.Add(selectedItem);
                }
            }
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            Dependencies.AddRange(
                depLB.Items.Cast<ToStringDecorator<RemotePlugin>>().Select(x => x.ObjectValue).ToList());

            Close();
        }
    }
}
