using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSFileSyncer.Classes
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SyncRules
    {
        /*add any order-specific settings here as fields,
        add a static const default value for them and use them in constructor*/

        [JsonProperty(PropertyName = "sync_type", Required = Required.Always)]
        public SyncType SyncType { get; set; }
        [JsonProperty(PropertyName = "sync_time", Required = Required.Default)]
        public int SyncTime { get; set; }

        public static SyncRules Default
        {
            get
            {
                return new SyncRules(SyncType.Always);
            }
        }

        public SyncRules(SyncType syncType)
        {
            /*for every setting, assign the given value to their field*/
            SyncType = syncType;
        }
    }

    public enum SyncType
    {
        Always = 0,
        Never = -1,
        TimeSpan = 1
    }
}
