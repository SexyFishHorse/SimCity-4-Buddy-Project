﻿// <auto-generated/>
namespace Nihei.SC4Buddy.Plugins.View
{
    public partial class PluginsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginsForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.installedPluginsListView = new System.Windows.Forms.ListView();
            this.pluginColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pluginInfoSplitContainer = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.nameLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.authorLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.descriptionRichTextBox = new System.Windows.Forms.RichTextBox();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.errorPanel = new System.Windows.Forms.Panel();
            this.errorTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.disableFilesButton = new System.Windows.Forms.Button();
            this.moveOrCopyButton = new System.Windows.Forms.Button();
            this.updateInfoButton = new System.Windows.Forms.Button();
            this.uninstallButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.selectFilesToInstallDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanForNewPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanForNonpluginFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.identifyNewPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateInfoForKnownPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForMissingDependenciesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInFileExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.updateInfoBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.identifyPluginsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.dependencyCheckerBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pluginInfoSplitContainer)).BeginInit();
            this.pluginInfoSplitContainer.Panel1.SuspendLayout();
            this.pluginInfoSplitContainer.Panel2.SuspendLayout();
            this.pluginInfoSplitContainer.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.errorPanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.installedPluginsListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2Collapsed = true;
            // 
            // installedPluginsListView
            // 
            this.installedPluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginColumn});
            resources.ApplyResources(this.installedPluginsListView, "installedPluginsListView");
            this.installedPluginsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.installedPluginsListView.MultiSelect = false;
            this.installedPluginsListView.Name = "installedPluginsListView";
            this.installedPluginsListView.UseCompatibleStateImageBehavior = false;
            this.installedPluginsListView.View = System.Windows.Forms.View.Details;
            this.installedPluginsListView.SelectedIndexChanged += new System.EventHandler(this.InstalledPluginsListViewSelectedIndexChanged);
            // 
            // pluginColumn
            // 
            resources.ApplyResources(this.pluginColumn, "pluginColumn");
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.pluginInfoSplitContainer);
            this.groupBox1.Controls.Add(this.disableFilesButton);
            this.groupBox1.Controls.Add(this.moveOrCopyButton);
            this.groupBox1.Controls.Add(this.updateInfoButton);
            this.groupBox1.Controls.Add(this.uninstallButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // pluginInfoSplitContainer
            // 
            resources.ApplyResources(this.pluginInfoSplitContainer, "pluginInfoSplitContainer");
            this.pluginInfoSplitContainer.Name = "pluginInfoSplitContainer";
            // 
            // pluginInfoSplitContainer.Panel1
            // 
            this.pluginInfoSplitContainer.Panel1.Controls.Add(this.panel2);
            // 
            // pluginInfoSplitContainer.Panel2
            // 
            this.pluginInfoSplitContainer.Panel2.Controls.Add(this.errorPanel);
            this.pluginInfoSplitContainer.Panel2Collapsed = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.nameLabel);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.authorLabel);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.linkLabel);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.Name = "nameLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // authorLabel
            // 
            resources.ApplyResources(this.authorLabel, "authorLabel");
            this.authorLabel.Name = "authorLabel";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.descriptionRichTextBox);
            this.panel1.Name = "panel1";
            // 
            // descriptionRichTextBox
            // 
            this.descriptionRichTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.descriptionRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.descriptionRichTextBox, "descriptionRichTextBox");
            this.descriptionRichTextBox.Name = "descriptionRichTextBox";
            // 
            // linkLabel
            // 
            resources.ApplyResources(this.linkLabel, "linkLabel");
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.TabStop = true;
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelLinkClicked);
            // 
            // errorPanel
            // 
            this.errorPanel.BackColor = System.Drawing.SystemColors.Info;
            this.errorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.errorPanel.Controls.Add(this.errorTextBox);
            this.errorPanel.Controls.Add(this.label3);
            resources.ApplyResources(this.errorPanel, "errorPanel");
            this.errorPanel.Name = "errorPanel";
            // 
            // errorTextBox
            // 
            resources.ApplyResources(this.errorTextBox, "errorTextBox");
            this.errorTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.errorTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.errorTextBox.Name = "errorTextBox";
            this.errorTextBox.ReadOnly = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Image = global::Nihei.SC4Buddy.Properties.Resources.IconError;
            this.label3.Name = "label3";
            // 
            // disableFilesButton
            // 
            resources.ApplyResources(this.disableFilesButton, "disableFilesButton");
            this.disableFilesButton.Name = "disableFilesButton";
            this.disableFilesButton.UseVisualStyleBackColor = true;
            this.disableFilesButton.Click += new System.EventHandler(this.DisableFilesButtonClick);
            // 
            // moveOrCopyButton
            // 
            resources.ApplyResources(this.moveOrCopyButton, "moveOrCopyButton");
            this.moveOrCopyButton.Name = "moveOrCopyButton";
            this.moveOrCopyButton.UseVisualStyleBackColor = true;
            this.moveOrCopyButton.Click += new System.EventHandler(this.MoveOrCopyButtonClick);
            // 
            // updateInfoButton
            // 
            resources.ApplyResources(this.updateInfoButton, "updateInfoButton");
            this.updateInfoButton.Name = "updateInfoButton";
            this.updateInfoButton.UseVisualStyleBackColor = true;
            this.updateInfoButton.Click += new System.EventHandler(this.UpdateInfoButtonClick);
            // 
            // uninstallButton
            // 
            resources.ApplyResources(this.uninstallButton, "uninstallButton");
            this.uninstallButton.Name = "uninstallButton";
            this.uninstallButton.UseVisualStyleBackColor = true;
            this.uninstallButton.Click += new System.EventHandler(this.UninstallButtonClick);
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // selectFilesToInstallDialog
            // 
            this.selectFilesToInstallDialog.Multiselect = true;
            resources.ApplyResources(this.selectFilesToInstallDialog, "selectFilesToInstallDialog");
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pluginsToolStripMenuItem,
            this.toolsToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installToolStripMenuItem,
            this.scanForNewPluginsToolStripMenuItem});
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            resources.ApplyResources(this.pluginsToolStripMenuItem, "pluginsToolStripMenuItem");
            // 
            // installToolStripMenuItem
            // 
            this.installToolStripMenuItem.Name = "installToolStripMenuItem";
            resources.ApplyResources(this.installToolStripMenuItem, "installToolStripMenuItem");
            this.installToolStripMenuItem.Click += new System.EventHandler(this.InstallToolStripMenuItemClick);
            // 
            // scanForNewPluginsToolStripMenuItem
            // 
            this.scanForNewPluginsToolStripMenuItem.Name = "scanForNewPluginsToolStripMenuItem";
            resources.ApplyResources(this.scanForNewPluginsToolStripMenuItem, "scanForNewPluginsToolStripMenuItem");
            this.scanForNewPluginsToolStripMenuItem.Click += new System.EventHandler(this.ScanForNewPluginsToolStripMenuItemClick);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scanForNonpluginFilesToolStripMenuItem,
            this.identifyNewPluginsToolStripMenuItem,
            this.updateInfoForKnownPluginsToolStripMenuItem,
            this.checkForMissingDependenciesToolStripMenuItem,
            this.openInFileExplorerToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // scanForNonpluginFilesToolStripMenuItem
            // 
            this.scanForNonpluginFilesToolStripMenuItem.Name = "scanForNonpluginFilesToolStripMenuItem";
            resources.ApplyResources(this.scanForNonpluginFilesToolStripMenuItem, "scanForNonpluginFilesToolStripMenuItem");
            this.scanForNonpluginFilesToolStripMenuItem.Click += new System.EventHandler(this.ScanForNonpluginFilesToolStripMenuItemClick);
            // 
            // identifyNewPluginsToolStripMenuItem
            // 
            this.identifyNewPluginsToolStripMenuItem.Name = "identifyNewPluginsToolStripMenuItem";
            resources.ApplyResources(this.identifyNewPluginsToolStripMenuItem, "identifyNewPluginsToolStripMenuItem");
            this.identifyNewPluginsToolStripMenuItem.Click += new System.EventHandler(this.IdentifyNewPluginsToolStripMenuItemClick);
            // 
            // updateInfoForKnownPluginsToolStripMenuItem
            // 
            this.updateInfoForKnownPluginsToolStripMenuItem.Name = "updateInfoForKnownPluginsToolStripMenuItem";
            resources.ApplyResources(this.updateInfoForKnownPluginsToolStripMenuItem, "updateInfoForKnownPluginsToolStripMenuItem");
            this.updateInfoForKnownPluginsToolStripMenuItem.Click += new System.EventHandler(this.UpdateInfoForKnownPluginsToolStripMenuItemClick);
            // 
            // checkForMissingDependenciesToolStripMenuItem
            // 
            this.checkForMissingDependenciesToolStripMenuItem.Name = "checkForMissingDependenciesToolStripMenuItem";
            resources.ApplyResources(this.checkForMissingDependenciesToolStripMenuItem, "checkForMissingDependenciesToolStripMenuItem");
            this.checkForMissingDependenciesToolStripMenuItem.Click += new System.EventHandler(this.CheckForMissingDependenciesToolStripMenuItemClick);
            // 
            // openInFileExplorerToolStripMenuItem
            // 
            this.openInFileExplorerToolStripMenuItem.Name = "openInFileExplorerToolStripMenuItem";
            resources.ApplyResources(this.openInFileExplorerToolStripMenuItem, "openInFileExplorerToolStripMenuItem");
            this.openInFileExplorerToolStripMenuItem.Click += new System.EventHandler(this.OpenInFileExplorerToolStripMenuItemClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            resources.ApplyResources(this.toolStripProgressBar, "toolStripProgressBar");
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            resources.ApplyResources(this.toolStripStatusLabel, "toolStripStatusLabel");
            // 
            // updateInfoBackgroundWorker
            // 
            this.updateInfoBackgroundWorker.WorkerReportsProgress = true;
            this.updateInfoBackgroundWorker.WorkerSupportsCancellation = true;
            this.updateInfoBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.UpdateInfoBackgroundWorkerDoWork);
            this.updateInfoBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerProgressChanged);
            this.updateInfoBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UpdateInfoBackgroundWorkerRunWorkerCompleted);
            // 
            // identifyPluginsBackgroundWorker
            // 
            this.identifyPluginsBackgroundWorker.WorkerReportsProgress = true;
            this.identifyPluginsBackgroundWorker.WorkerSupportsCancellation = true;
            this.identifyPluginsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.IdentifyPluginsBackgroundWorkerDoWork);
            this.identifyPluginsBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerProgressChanged);
            this.identifyPluginsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.IdentifyPluginsBackgroundWorkerRunWorkerCompleted);
            // 
            // dependencyCheckerBackgroundWorker
            // 
            this.dependencyCheckerBackgroundWorker.WorkerReportsProgress = true;
            this.dependencyCheckerBackgroundWorker.WorkerSupportsCancellation = true;
            this.dependencyCheckerBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DependencyCheckerBackgroundWorkerDoWork);
            this.dependencyCheckerBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerProgressChanged);
            this.dependencyCheckerBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DependencyCheckerBackgroundWorkerRunWorkerCompleted);
            // 
            // PluginsForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PluginsForm";
            this.Activated += new System.EventHandler(this.UserFolderFormActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PluginsFormFormClosing);
            this.Load += new System.EventHandler(this.UserFolderFormLoad);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.PluginsFormDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.PluginsFormDragEnter);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.pluginInfoSplitContainer.Panel1.ResumeLayout(false);
            this.pluginInfoSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pluginInfoSplitContainer)).EndInit();
            this.pluginInfoSplitContainer.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.errorPanel.ResumeLayout(false);
            this.errorPanel.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label authorLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button uninstallButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox descriptionRichTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.ListView installedPluginsListView;
        private System.Windows.Forms.ColumnHeader pluginColumn;
        private System.Windows.Forms.OpenFileDialog selectFilesToInstallDialog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.Button updateInfoButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanForNewPluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanForNonpluginFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem identifyNewPluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForMissingDependenciesToolStripMenuItem;
        private System.Windows.Forms.Button moveOrCopyButton;
        private System.Windows.Forms.Button disableFilesButton;
        private System.Windows.Forms.Panel errorPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox errorTextBox;
        private System.Windows.Forms.SplitContainer pluginInfoSplitContainer;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem openInFileExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateInfoForKnownPluginsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.ComponentModel.BackgroundWorker updateInfoBackgroundWorker;
        private System.ComponentModel.BackgroundWorker identifyPluginsBackgroundWorker;
        private System.ComponentModel.BackgroundWorker dependencyCheckerBackgroundWorker;
    }
}