using Newtonsoft.Json.Linq;
using System;

namespace OpenDCP.Framework.Hosting.Models
{
    public class SystemGeneratedSharedLinkDTO
    {
        public int ID { get; set; }
        public JToken JLink { get; set; }
        public Guid LinkGUID { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DateCreated { get; set; }
        public int SiteID { get; set; }
    }
}