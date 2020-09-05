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

            folderTreeView.ImageList = imgList;
        }

        private void onAddFolder(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();

            folderBrowserDialog.RootFolder = Environment.SpecialFolder.System;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath;
                string folderName = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);

                FolderTreeNode newNode = new FolderTreeNode(folderPath, folderName);
                newNode.ImageIndex = 0;

                folderTreeView.Focus();
                folderTreeView.Nodes.Add(newNode);
                folderTreeView.SelectedNode = newNode;
            }
        }

        private void afterNodeSelect(object sender, TreeViewEventArgs e)
        {
            FolderTreeNode folderNode = (FolderTreeNode) e.Node;

            DirectoryInfo folderDir = new DirectoryInfo(folderNode.FolderPath);

            listView1.SuspendLayout();
            listView1.Items.Clear();

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
    }
}
