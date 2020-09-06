using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Games_Organizer
{
    public partial class GameFolderItem : UserControl
    {
        public string folderPath = "";
        public string folderName = "";
        public int folderSize = 0;
        public Image folderImage;

        public GameFolderItem()
        {
            InitializeComponent();
        }

        public GameFolderItem(string folderPath)
        {
            InitializeComponent();
            this.folderPath = folderPath;
            this.folderName = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
        }

        private void testWOrker()
        {

        }

        public GameFolderItem(string folderName, Image folderImage)
        {
            InitializeComponent();
            this.folderName = folderName;
            this.folderImage = folderImage;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.lblFolderName.Text = this.folderName;
            this.lblFolderPath.Text = this.folderPath;
            
            this.lblFolderSize.Text = "Processing...";

            SetFolderIcon();
            SetFolderSize();
        }

        public void SetFolderIcon()
        {
            MethodInvoker m = delegate
            {
                DirectoryInfo info = new DirectoryInfo(this.folderPath);
                FileInfo[] files = info.GetFiles("*.exe");

                if (files.Length > 0)
                    this.imgFolderIcon.Image = Icon.ExtractAssociatedIcon(files[0].FullName).ToBitmap();
                else
                    this.imgFolderIcon.Image = Image.FromFile("res/Folder_16x.png");
            };

            if (this.InvokeRequired)
                this.Invoke(m);
            else
                m.Invoke();
        }

        public void SetFolderSize()
        {
            MethodInvoker m = delegate
            {
                DirectoryInfo info = new DirectoryInfo(this.folderPath);

                long size = getFolderSize(info);

                this.lblFolderSize.Text = Helper.FormatBytes(size);
            };

            if (this.InvokeRequired)
                this.Invoke(m);
            else
                m.Invoke();
        }

        private long getFolderSize(DirectoryInfo folderPath)
        {
            long size = 0;

            FileInfo[] files = folderPath.GetFiles();
            DirectoryInfo[] directories = folderPath.GetDirectories();

            foreach (FileInfo file in files)
            {
                size += file.Length;
            }

            foreach (DirectoryInfo dir in directories)
            {
                size += getFolderSize(dir);
            }

            return size;
        }
    }
}
