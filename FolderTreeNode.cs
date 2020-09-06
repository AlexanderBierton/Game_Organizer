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

        public GameFolderItemCollection SortFolders(string sortBy)
        {
            GameFolderItemCollection sortedCollection;

            if (sortBy.ToLower() == "size")
            {
                sortedCollection = new GameFolderItemCollection(GameFolders.Count);
                for (int i = 0; i < GameFolders.Count; i++)
                {
                    var item = GameFolders[i];
                    var currentIndex = i;

                    while (currentIndex > 0 && sortedCollection[currentIndex - 1].folderSize < item.folderSize)
                    {
                        currentIndex--;
                    }

                    sortedCollection.Insert(currentIndex, item);
                }
            }
            else
            {
                sortedCollection = GameFolders;
            }

            return sortedCollection;
        }
    }
}
