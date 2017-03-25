using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megalomania_Studios_Filesync.CustomSubclasses
{
    public class SyncOrder
    {
        public string pathFrom { get; set; }
        public string pathTo { get; set; }
        public SyncRules syncSettings { get; set; }
        public SyncOrder(string from, string to, SyncRules settings)
        {
            pathFrom = from;
            pathTo = to;
        }
    }
}
