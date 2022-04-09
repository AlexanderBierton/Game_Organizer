using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Ookii.Dialogs;
using System.Threading;

namespace Games_Organizer
{
    public partial class MainForm : Form
    {
        List<FolderTreeNode> folderTreeNodes;
        const string STATIC_SAVE = "envs/saved_end.dat";
        DriveInfo[] Drives;

        public MainForm()
        {
            InitializeComponent();
            Drives = DriveInfo.GetDrives();
            folderTreeNodes = new List<FolderTreeNode>();
            ImageList imgList = new ImageList();
            imgList.Images.Add(Properties.Resources.Folder);
            imgList.Images.Add(Image.FromFile("res/FolderOpen_16x.png"));
            imgList.Images.Add(Image.FromFile("res/HardDrive_16x.png"));
            imgList.Images.Add(Properties.Resources.GameFolder);

            List<string> comboList = new List<string>();
            comboList.Add("Name");
            comboList.Add("Size");
            
            comboBox1.DataSource = comboList;
            comboBox1.DisplayMember = "Name";

            folderTreeView.ImageList = imgList;

            SetUpEnvironment();
        }

        private void SetUpEnvironment()
        {
            Directory.CreateDirectory("envs");

            if (File.Exists(STATIC_SAVE))
            {
                string lines = File.ReadAllText(STATIC_SAVE);

                string[] splitLines = lines.Split('|');

                foreach(string line in splitLines)
                {
                    string[] splitContent = line.Split('¬');

                    switch(splitContent[0])
                    {
                        case "folders":
                            string[] paths = splitContent[1].Split(',');
                            foreach(string path in paths)
                            {
                                if (path == "")
                                    continue;

                                FolderTreeNode newNode = BuildNodeTree(path);
                                folderTreeNodes.Add(newNode);
                            }
                            break;

                        default:
                            // Do Nothing
                            break;
                    }
                }
            }
        }

        private void onAddFolder(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.System
            };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath;

                FolderTreeNode newNode = BuildNodeTree(folderPath);
                folderTreeView.SelectedNode = newNode;
                folderTreeNodes.Add(newNode);
            }
        }

        private FolderTreeNode BuildNodeTree(string folderPath)
        {
            // If node is null then its a root node
            string[] splitPath = folderPath.Split('\\');
            TreeNode prevNode = null;
            FolderTreeNode folderNode = null;

            while (splitPath.Length > 0)
            {
                if (prevNode == null)
                {
                    TreeNode node = null;
                    foreach (TreeNode rootNode in folderTreeView.Nodes)
                    {
                        if (rootNode.Tag.Equals(splitPath[0]))
                        {
                            node = rootNode;
                            break;
                        }
                    }
                    
                    if (node == null)
                    {
                        node = new TreeNode(splitPath[0]);
                        DriveInfo currentDrive = new DriveInfo("c:\\");
                        foreach (DriveInfo info in Drives)
                        {
                            if (info.Name == splitPath[0] + "\\")
                            {
                                currentDrive = info;
                                break;
                            }
                        }
                        //long usedSpace = currentDrive.TotalSize - currentDrive.AvailableFreeSpace;
                        node.Name = splitPath[0];
                        node.Text = splitPath[0] + String.Format(" ({0} / {1})", Helper.FormatBytes(currentDrive.AvailableFreeSpace), Helper.FormatBytes(currentDrive.TotalSize));
                        node.Tag = splitPath[0];
                        node.ImageIndex = 2;
                        node.SelectedImageIndex = 2;
                        folderTreeView.Nodes.Add(node);
                    }

                    splitPath = splitPath.Skip(1).ToArray();
                    prevNode = node;
                    continue;
                }

                if (splitPath.Length > 1)
                {
                    TreeNode[] existingNode = prevNode.Nodes.Find(splitPath[0], false);
                    TreeNode node = null;

                    if (existingNode.Length > 0)
                    {
                        node = existingNode[0];
                    }
                    else
                    {
                        node = new TreeNode(splitPath[0]);
                        node.ImageIndex = 0;
                        node.Name = splitPath[0];
                        node.SelectedImageIndex = 0;
                        prevNode.Nodes.Add(node);
                    }
                   
                    splitPath = splitPath.Skip(1).ToArray();
                    prevNode = node;
                }
                else
                {
                    FolderTreeNode node = new FolderTreeNode(folderPath, splitPath[0]);
                    node.ImageIndex = 3;
                    node.SelectedImageIndex = 3;
                    node.Name = splitPath[0];
                    prevNode.Nodes.Add(node);
                    folderNode = node;
                    splitPath = splitPath.Skip(1).ToArray();
                }
            }

            return folderNode;
        }

        private void afterNodeSelect(object sender, TreeViewEventArgs e)
        {
            comboBox1.SelectedIndex = 0;

            flowLayoutPanel1.Controls.Clear();
            if (!(e.Node is FolderTreeNode))
            {
                return;
            }

            FolderTreeNode folderNode = (FolderTreeNode)e.Node;

            if (Directory.Exists(folderNode.FolderPath))
            {
                DirectoryInfo folderDir = new DirectoryInfo(folderNode.FolderPath);

                if (folderNode.GameFolders.Count > 0)
                {
                    foreach (GameFolderItem gameFolder in folderNode.GameFolders)
                    {
                        flowLayoutPanel1.Controls.Add(gameFolder);
                    }
                    return;
                }

                foreach (DirectoryInfo dir in folderDir.GetDirectories())
                {
                    GameFolderItem item = new GameFolderItem(dir.FullName);
                    item.Parent = flowLayoutPanel1;
                    item.Width = item.Parent.Width - 30;
                    item.Anchor = (AnchorStyles.Left | AnchorStyles.Right);

                    flowLayoutPanel1.Controls.Add(item);
                    folderNode.GameFolders.Add(item);
                }
            }
        }

        private void ReadFolder(object sender, EventArgs e)
        {
            folderReaderWorker.RunWorkerAsync();
        }

        private void updateToolStripState()
        {
            bool enabled = folderTreeView.SelectedNode != null && folderTreeView.SelectedNode is FolderTreeNode && folderTreeView.Focused;

            removeFolderTool.Enabled = enabled;
        }

        private void testClick(object sender, EventArgs e)
        {
            
        }

        private void OnSortIndexChanged(object sender, EventArgs e)
        {
            if (folderTreeView.SelectedNode != null)
            {
                flowLayoutPanel1.Controls.Clear();

                if (!(folderTreeView.SelectedNode is FolderTreeNode))
                    return;

                FolderTreeNode node = (FolderTreeNode)folderTreeView.SelectedNode;

                GameFolderItemCollection test = node.SortFolders(comboBox1.SelectedItem.ToString());

                foreach (GameFolderItem testItem in test)
                {
                    flowLayoutPanel1.Controls.Add(testItem);
                }
            }
            
        }

        private void OnSave(object sender, EventArgs e)
        {
            string data = "folders¬";

            foreach(FolderTreeNode node in folderTreeNodes)
                data += node.FullPath + ',';
            
            data.TrimEnd(',');
            data += '|';

            File.WriteAllText(STATIC_SAVE, data);
        }

        private void expandAllNodes(object sender, EventArgs e)
        {
            foreach (TreeNode node in this.folderTreeView.Nodes)
            {
                node.ExpandAll();
            }
        }

        private void collapseAll(object sender, EventArgs e)
        {
            foreach (TreeNode node in this.folderTreeView.Nodes)
            {
                node.Collapse();
            }
        }
    }
}
