namespace NIHEI.SC4Buddy.View.Developer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.Common.IO;
    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Properties;

    public partial class DeveloperForm : Form
    {
        private readonly PluginController pluginController;

        private readonly PluginGroupController pluginGroupController;

        private readonly UserFolderController userFolderController;

        private readonly ICollection<string> selectedFiles;

        private readonly RemotePluginRegistry remotePluginRegistry;

        private readonly RemotePluginFileController remotePluginFileController;

        private readonly AuthorController authorController;

        private readonly List<RemotePlugin> dependencies;

        public DeveloperForm(
            PluginController pluginController,
            PluginGroupController pluginGroupController,
            UserFolderController userFolderController,
            AuthorController authorController,
            RemotePluginFileController remotePluginFileController)
        {
            InitializeComponent();

            this.pluginController = pluginController;
            this.pluginGroupController = pluginGroupController;
            this.userFolderController = userFolderController;

            remotePluginRegistry = RemoteRegistryFactory.RemotePluginRegistry;
            this.remotePluginFileController = remotePluginFileController;
            this.authorController = authorController;

            selectedFiles = new List<string>();
            dependencies = new List<RemotePlugin>();
        }

        private void Button1Click(object sender, EventArgs e)
        {
            foreach (var plugin in pluginController.Plugins)
            {
                pluginController.Delete(plugin);
            }

            foreach (var pluginGroup in pluginGroupController.Groups)
            {
                pluginGroupController.Delete(pluginGroup);
            }

            foreach (var folder in userFolderController.UserFolders.Where(x => x.Id != 1))
            {
                userFolderController.Delete(folder);
            }

            var mainFolder = userFolderController.UserFolders.First(x => x.Id == 1);
            {
                mainFolder.Alias = "Main plugin folder";
                mainFolder.Path = " ";
                userFolderController.Update(mainFolder);
            }

            MessageBox.Show(this, @"Database reset.");
        }

        private void Button2Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var file = openFileDialog1.FileName;

                var md5 = Md5ChecksumUtility.CalculateChecksum(file).ToHex();

                md5Textbox.Text = md5;
            }
        }

        private void Button3Click(object sender, EventArgs e)
        {
            var txt = md5Textbox.Text;

            Clipboard.SetText(txt);
        }

        private void Md5TextboxTextChanged(object sender, EventArgs e)
        {
            button3.Enabled = !string.IsNullOrWhiteSpace(md5Textbox.Text);
        }

        private void FilesButtonClick(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog(this) == DialogResult.OK)
            {
                var files = openFileDialog2.FileNames;
                foreach (var file in files)
                {
                    selectedFiles.Add(file);
                    filesLB.Items.Add(new FileInfo(file).Name);
                }
            }
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            var remotePlugin = new RemotePlugin
                                   {
                                       Name = nameTB.Text.Trim(),
                                       Author = authorController.GetAuthorByName(authorTB.Text.Trim()),
                                       Link = linkTB.Text.Trim(),
                                       Description = descTB.Text.Trim()
                                   };

            foreach (var dependency in dependencies)
            {
                remotePlugin.Dependencies.Add(dependency);
                dependency.DependencyFor.Add(remotePlugin);
                remotePluginRegistry.Update(dependency);
            }

            remotePluginRegistry.Add(remotePlugin);

            foreach (var file in selectedFiles)
            {
                var fileInfo = new FileInfo(file);
                var remoteFile = new RemotePluginFile
                                     {
                                         Checksum =
                                             Md5ChecksumUtility.CalculateChecksum(fileInfo).ToHex(),
                                         Name = fileInfo.Name,
                                         PluginId = remotePlugin.Id,
                                         Plugin = remotePlugin
                                     };
                remotePlugin.PluginFiles.Add(remoteFile);
                remotePluginFileController.Add(remoteFile);
            }

            ClearRemotePluginForm();
        }

        private void ClearRemotePluginForm()
        {
            nameTB.Text = string.Empty;
            authorTB.Text = string.Empty;
            linkTB.Text = string.Empty;
            descTB.Text = string.Empty;
            selectedFiles.Clear();
            filesLB.Items.Clear();
            dependencies.Clear();
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            ClearRemotePluginForm();
        }

        private void DependenciesButtonClick(object sender, EventArgs e)
        {
            var dialog = new DeveloperDependenciesForm();
            dialog.ShowDialog(this);

            dependencies.AddRange(dialog.Dependencies);
        }

        private void DescTbKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != (Keys.Control | Keys.Enter))
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
            SaveButtonClick(sender, EventArgs.Empty);
        }

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
