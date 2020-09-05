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
        public MainForm()
        {
            InitializeComponent();

            ImageList imgList = new ImageList();
            imgList.Images.Add(Image.FromFile("res/Folder_16x.png"));
            imgList.Images.Add(Image.FromFile("res/FolderOpen_16x.png"));
            imgList.Images.Add(Image.FromFile("res/HardDrive_16x.png"));

            folderTreeView.ImageList = imgList;
        }

        private void onAddFolder(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();

            folderBrowserDialog.RootFolder = Environment.SpecialFolder.System;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath;

                FolderTreeNode newNode = BuildNodeTree(folderPath);
                folderTreeView.SelectedNode = newNode;
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
                        if (rootNode.Name == splitPath[0])
                        {
                            node = rootNode;
                            break;
                        }
                    }
                    
                    if (node == null)
                    {
                        node = new TreeNode(splitPath[0]);
                        node.Name = splitPath[0];
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
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
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
            listView1.SuspendLayout();
            listView1.Items.Clear();
            updateToolStripState();

            if (!(e.Node is FolderTreeNode))
            {
                listView1.ResumeLayout();
                return;
            }
                

            FolderTreeNode folderNode = (FolderTreeNode) e.Node;

            DirectoryInfo folderDir = new DirectoryInfo(folderNode.FolderPath);

            foreach (DirectoryInfo dir in folderDir.GetDirectories())
            {
                FolderListItem item = new FolderListItem();

                item.FolderPath = dir.FullName;
                item.SubItems.Add(dir.Name);
                item.SubItems[0].Text = dir.Name;
                item.SubItems[1].Text = "";

                listView1.Items.Add(item);
            }

            listView1.ResumeLayout();
            

            folderReaderWorker.RunWorkerAsync();
        }

        private void getSubItemInfo()
        {

            foreach (FolderListItem folderItem in listView1.Items)
            {
                MethodInvoker m = delegate
                {
                    long size = getFolderSize(folderItem.FolderPath);

                    folderItem.SubItems[1].Text = Helper.FormatBytes(size);
                };

                if (listView1.InvokeRequired)
                    listView1.Invoke(m);
                else
                    m.Invoke();
            }
            
        }

        private long getFolderSize(string folderPath)
        {
            return getFolderSize(new DirectoryInfo(folderPath));
        }

        private long getFolderSize(DirectoryInfo folderPath)
        {
            long size = 0;

            FileInfo[] files = folderPath.GetFiles();
            DirectoryInfo[] directories = folderPath.GetDirectories();

            foreach(FileInfo file in files)
            {
                size += file.Length;
            }

            foreach(DirectoryInfo dir in directories)
            {
                size += getFolderSize(dir);
            }

            return size;
        }

        private void onFolderReader(object sender, DoWorkEventArgs e)
        {
            if (listView1.InvokeRequired)
            {
                listView1.Invoke(new MethodInvoker(delegate
                {
                    foreach(FolderListItem folderItem in listView1.Items)
                    {
                        long size = getFolderSize(folderItem.FolderPath);

                        folderItem.SubItems[1].Text = Helper.FormatBytes(size);
                    }
                }));
            }
            else
            {
                foreach (FolderListItem folderItem in listView1.Items)
                {
                    long size = getFolderSize(folderItem.FolderPath);

                    folderItem.SubItems[1].Text = Helper.FormatBytes(size);
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
    }
}
