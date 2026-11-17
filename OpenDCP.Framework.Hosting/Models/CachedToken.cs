using System;

namespace OpenDCP.Framework.Hosting.Models
{
    public class CachedToken
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
