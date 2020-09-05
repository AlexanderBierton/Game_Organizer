using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Games_Organizer
{
    class GameFolder
    {
        public int ID { get; protected set; }
        public int ParentID { get; set; }
        public int size { get; set; }
        public ListViewItem ListViewItem { get; set; }
    }
}
