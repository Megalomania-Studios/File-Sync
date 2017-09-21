using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSyncLibrary
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SyncOrder
    {
        //folder to copy from
        [JsonProperty(PropertyName = "origin_folder", Required = Required.Always)]
        public string OriginFolder { get; set; }
        //folder to copy to
        [JsonProperty(PropertyName = "destination_folder", Required = Required.Always)]
        public string DestinationFolder { get; set; }
        //settings for this sync order
        [JsonProperty(PropertyName = "sync_settings", Required = Required.Always)]
        public SyncRules Settings { get; set; }
        //DateTime when this was last synced
        [JsonProperty(PropertyName = "last_synced", Required = Required.Default)]
        public DateTime LastSynced { get; set; }

        public SyncOrder(string from, string to, SyncRules settings = null)
        {
            if (settings == null)
            {
                settings = SyncRules.Default;
            }
            Settings = settings;
            OriginFolder = from;
            DestinationFolder = to;
        }
    }
}
