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
        public long folderSize = -1;
        public Image folderImage = null;

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
            if (this.folderImage != null)
            {
                this.imgFolderIcon.Image = this.folderImage;
                return;
            }

            MethodInvoker m = delegate
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(this.folderPath);
                    FileInfo[] files = info.GetFiles("*.exe");

                    if (files.Length > 0)
                    {
                        this.folderImage = Icon.ExtractAssociatedIcon(files[0].FullName).ToBitmap();
                        this.imgFolderIcon.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                    else
                    {
                        this.folderImage = Games_Organizer.Properties.Resources.GameFolder;
                        this.imgFolderIcon.SizeMode = PictureBoxSizeMode.CenterImage;
                    }

                    this.imgFolderIcon.Image = this.folderImage;
                }
                catch(Exception e)
                {

                }
                
            };

            if (this.InvokeRequired)
                this.Invoke(m);
            else
                m.Invoke();
        }

        public void SetFolderSize()
        {
            if (this.folderSize == -1)
            {
                MethodInvoker m = delegate
                {
                    DirectoryInfo info = new DirectoryInfo(this.folderPath);

                    this.folderSize = getFolderSize(info);
                    this.lblFolderSize.Text = (this.folderSize == -1 ? "N/A" : Helper.FormatBytes(this.folderSize));
                };

                if (this.InvokeRequired)
                    this.Invoke(m);
                else
                    m.Invoke();
            }
            else
            {
                this.lblFolderSize.Text = (this.folderSize == -1 ? "N/A" : Helper.FormatBytes(this.folderSize));
            }
        }

        private long getFolderSize(DirectoryInfo folderPath)
        {
            try
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
            catch (Exception e)
            {
                return -1;
            }
            
        }
    }
}
