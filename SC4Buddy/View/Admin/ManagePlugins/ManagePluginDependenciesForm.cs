namespace NIHEI.SC4Buddy.View.Admin.ManagePlugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ManagePluginDependenciesForm : Form
    {
        private readonly RemotePluginController remotePluginController;

        private readonly AuthorController authorController;

        private RemotePlugin selectedDependency;

        private RemotePlugin selectedSearchResult;

        private RemotePlugin newPlugin;

        public ManagePluginDependenciesForm(
            ICollection<RemotePlugin> pluginDependencies,
            RemotePluginController remotePluginController,
            AuthorController authorController)
        {
            Dependencies = pluginDependencies;
            this.remotePluginController = remotePluginController;
            this.authorController = authorController;
            InitializeComponent();

            UpdateListView(dependenciesListView, Dependencies);
        }

        public ICollection<RemotePlugin> Dependencies { get; private set; }

        private void UpdateListView(ListView listView, IEnumerable<RemotePlugin> dependencies)
        {
            listView.BeginUpdate();
            listView.Items.Clear();

            foreach (var dependency in dependencies)
            {
                var item = new ListViewItemWithObjectValue<RemotePlugin>(dependency.Name, dependency);
                item.SubItems.Add(dependency.Author.Name);
                item.SubItems.Add(dependency.Link);

                listView.Items.Add(item);
            }

            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView.EndUpdate();
        }

        private void DependenciesListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDependency = dependenciesListView.SelectedItems.Count > 0
                                     ? ((ListViewItemWithObjectValue<RemotePlugin>)dependenciesListView.SelectedItems[0])
                                           .Value
                                     : null;

            removeDependencyButton.Enabled = selectedDependency != null;
        }

        private void RemoveDependencyButtonClick(object sender, EventArgs e)
        {
            Dependencies.Remove(selectedDependency);

            selectedDependency = null;

            UpdateListView(dependenciesListView, Dependencies);

            removeDependencyButton.Enabled = false;
        }

        private void SearchTextBoxTextChanged(object sender, EventArgs e)
        {
            var text = searchTextBox.Text.Trim();

            if (text.Length < 3)
            {
                return;
            }

            var results = remotePluginController.SearchForPlugin(text);

            UpdateListView(resultsListView, results);
        }

        private void ResultsListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSearchResult = resultsListView.SelectedItems.Count > 0
                                       ? ((ListViewItemWithObjectValue<RemotePlugin>)resultsListView.SelectedItems[0])
                                             .Value
                                       : null;

            selectButton.Enabled = selectedSearchResult != null;
        }

        private void SelectButtonClick(object sender, EventArgs e)
        {
            if (!Dependencies.Contains(selectedSearchResult))
            {
                Dependencies.Add(selectedSearchResult);
            }

            selectedSearchResult = null;

            selectButton.Enabled = false;

            UpdateListView(dependenciesListView, Dependencies);
        }

        private void AddDependencyButtonClick(object sender, EventArgs e)
        {
            nameTextBox.Enabled = true;
            nameTextBox.Text = string.Empty;
            authorComboBox.Enabled = true;
            authorComboBox.Text = string.Empty;
            linkTextBox.Enabled = true;
            linkTextBox.Text = string.Empty;

            nameTextBox.Focus();

            addDependencyButton.Enabled = false;
            cancelAddButton.Enabled = true;

            newPlugin = new RemotePlugin();

            dependenciesListView.Items.Add(new ListViewItemWithObjectValue<RemotePlugin>("(new)", newPlugin));
        }

        private void CancelAddButtonClick(object sender, EventArgs e)
        {
            nameTextBox.Enabled = false;
            nameTextBox.Text = string.Empty;
            authorComboBox.Enabled = false;
            authorComboBox.Text = string.Empty;
            linkTextBox.Enabled = false;
            linkTextBox.Text = string.Empty;

            cancelAddButton.Enabled = false;
            saveAndAddButton.Enabled = false;

            addDependencyButton.Enabled = true;

            foreach (var item in
                dependenciesListView.Items.Cast<ListViewItemWithObjectValue<RemotePlugin>>()
                    .Where(item => item.Value.Equals(newPlugin)))
            {
                dependenciesListView.Items.Remove(item);
                break;
            }

            newPlugin = null;
        }

        private void SaveAndAddButtonClick(object sender, EventArgs e)
        {
            try
            {
                newPlugin.Name = nameTextBox.Text.Trim();
                newPlugin.Link = linkTextBox.Text.Trim();
                var authorText = authorComboBox.Text;

                var author = authorController.GetAuthorByName(authorText)
                             ?? new Author { Name = authorText, Site = new Uri(newPlugin.Link).Host };

                newPlugin.Author = author;

                remotePluginController.Add(newPlugin);

                Dependencies.Add(newPlugin);

                UpdateListView(dependenciesListView, Dependencies);

                nameTextBox.Text = string.Empty;
                nameTextBox.Enabled = false;
                authorComboBox.Text = string.Empty;
                authorComboBox.Enabled = false;
                linkTextBox.Text = string.Empty;
                linkTextBox.Enabled = false;

                saveAndAddButton.Enabled = false;
                cancelAddButton.Enabled = false;

                addDependencyButton.Enabled = true;

                newPlugin = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error during save operation");
            }
        }

        private void UnknownDependencyAddFieldChanged(object sender, EventArgs e)
        {
            saveAndAddButton.Enabled = !string.IsNullOrWhiteSpace(nameTextBox.Text.Trim())
                                       && !string.IsNullOrWhiteSpace(authorComboBox.Text.Trim())
                                       && !string.IsNullOrWhiteSpace(linkTextBox.Text.Trim());
        }
    }
}
