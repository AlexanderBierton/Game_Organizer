using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games_Organizer
{
    class GameFolderItemCollection : List<GameFolderItem>
    {
        public GameFolderItemCollection() :
            base()
        {

        }

        public GameFolderItemCollection(int Capacity) :
            base(Capacity)
        {

        }
    }
}
