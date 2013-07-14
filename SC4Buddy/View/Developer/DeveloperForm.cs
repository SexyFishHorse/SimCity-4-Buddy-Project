namespace NIHEI.SC4Buddy.View.Developer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.Common.IO;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Properties;

    public partial class DeveloperForm : Form
    {
        private readonly PluginRegistry pluginRegistry;

        private readonly UserFolderRegistry userFolderRegistry;

        private readonly ICollection<string> selectedFiles;

        private readonly RemotePluginRegistry remotePluginRegistry;

        private readonly List<RemotePlugin> dependencies;

        public DeveloperForm()
        {
            InitializeComponent();

            pluginRegistry = RegistryFactory.PluginRegistry;

            userFolderRegistry = RegistryFactory.UserFolderRegistry;

            selectedFiles = new List<string>();

            remotePluginRegistry = RemoteRegistryFactory.RemotePluginRegistry;

            dependencies = new List<RemotePlugin>();
        }

        private void Button1Click(object sender, EventArgs e)
        {
            foreach (var plugin in pluginRegistry.Plugins)
            {
                pluginRegistry.Delete(plugin);
            }

            var pluginGroupRegistry = RegistryFactory.PluginGroupRegistry;
            foreach (var pluginGroup in pluginGroupRegistry.PluginGroups)
            {
                pluginGroupRegistry.Delete(pluginGroup);
            }

            foreach (var folder in userFolderRegistry.UserFolders.Where(x => x.Id != 1))
            {
                userFolderRegistry.Delete(folder);
            }

            var mainFolder = userFolderRegistry.UserFolders.First(x => x.Id == 1);
            {
                mainFolder.Alias = "Main plugin folder";
                mainFolder.Path = " ";
                userFolderRegistry.Update(mainFolder);
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

        private void Button4Click(object sender, EventArgs e)
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

        private void Button5Click(object sender, EventArgs e)
        {
            var remotePlugin = new RemotePlugin
                                   {
                                       Name = nameTB.Text.Trim(),
                                       Author = authorTB.Text.Trim(),
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
                remotePlugin.Files.Add(remoteFile);
                RemoteRegistryFactory.RemotePluginFileRegistry.Add(remoteFile);
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

        private void Button6Click(object sender, EventArgs e)
        {
            ClearRemotePluginForm();
        }

        private void Button7Click(object sender, EventArgs e)
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
            Button5Click(sender, EventArgs.Empty);
        }

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
