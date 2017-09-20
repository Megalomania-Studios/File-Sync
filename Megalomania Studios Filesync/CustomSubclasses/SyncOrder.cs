using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megalomania_Studios_Filesync.CustomSubclasses
{
    public class SyncOrder
    {
        public string PathFrom { get; set; }
        public string PathTo { get; set; }
        public SyncRules SyncSettings { get; set; }
        public SyncOrder(string from, string to, SyncRules settings = null)
        {
            if (settings == null)
            {
                settings = SyncRules.Default;
            }
            PathFrom = from;
            PathTo = to;
        }
    }
}
