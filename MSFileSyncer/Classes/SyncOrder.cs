using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSFileSyncer.Classes
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SyncOrder
    {
        [JsonProperty(PropertyName = "origin_folder", Required = Required.Always)]
        public string OriginFolder { get; set; }
        [JsonProperty(PropertyName = "destination_folder", Required = Required.Always)]
        public string DestinationFolder { get; set; }
        [JsonProperty(PropertyName = "sync_settings", Required = Required.Always)]
        public SyncRules Settings { get; set; }
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
