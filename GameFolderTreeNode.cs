using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Games_Organizer
{
    class GameFolderTreeNode : TreeNode
    {
        GameFolderItemCollection folders;

        public GameFolderTreeNode(string FolderName):
            base (FolderName)
        {

        }
    }
}
