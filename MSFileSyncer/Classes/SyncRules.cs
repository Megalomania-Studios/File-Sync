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

        //The type of the sync order
        [JsonProperty(PropertyName = "sync_type", Required = Required.Always)]
        public SyncType SyncType { get; set; }
        private const SyncType defaultSyncType = SyncType.Always;
        //The interval to sync it, required when SyncType is TimeSpan
        [JsonProperty(PropertyName = "sync_time", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TimeSpan SyncTime { get; set; }
        //When the target files should be overwritten (Always, only newer, never)
        [JsonProperty(PropertyName = "override", Required = Required.Always)]
        public DoIf Override { get; set; }
        private const DoIf defaultOverride = DoIf.OnlyNewer;
        //When files missing in origin should be deletet in destination (Always, last changed was after last sync AKA only newer, never)
        [JsonProperty(PropertyName = "delete", Required = Required.Always)]
        public DoIf Delete { get; set; }
        private const DoIf defaultDelete = DoIf.OnlyNewer;
        
        public static SyncRules Default
        {
            get
            {
                return new SyncRules(defaultSyncType, defaultOverride, defaultDelete);
            }
        }

        public SyncRules(SyncType syncType, DoIf _override, DoIf delete)
        {
            /*for every setting, assign the given value to their field*/
            SyncType = syncType;
            Override = _override;
            Delete = delete;
        }
    }

    public enum SyncType
    {
        Always = 0,
        Never = -1,
        TimeSpan = 1
    }

    public enum DoIf
    {
        Always = 0,
        OnlyNewer = 1,
        Never = -1
    }
}
