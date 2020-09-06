using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Games_Organizer
{
    class FolderTreeNode : TreeNode
    {
        public string FolderPath = "";
        public GameFolderItemCollection GameFolders;

        public FolderTreeNode (string folderPath, string folderName = "")
            :base()
        {
            GameFolders = new GameFolderItemCollection();
            this.FolderPath = folderPath;
            if (String.IsNullOrEmpty(folderName))
                this.Name = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
            else
                this.Name = folderName;

            this.Text = folderName;
            
        }
    }
}
