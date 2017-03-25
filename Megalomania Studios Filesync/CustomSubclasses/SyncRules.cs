using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megalomania_Studios_Filesync.CustomSubclasses
{
    public class SyncRules
    {
        /*add any order-specific settings here as fields,
        add a static default value for them and use them in constructor*/

        public static SyncRules Default
        {
            get
            {
                return new SyncRules();
            }
        }
        
        public SyncRules()
        {
            /*for every setting, assign the given value to their field*/
        }
    }
}
