using OpenDCP.Framework.Security.Models;
using System.Collections.Generic;


namespace OpenDCP.Framework.Hosting.Models
{
    public class UserContextDTO
    {
        public string UnixId { get; set; }
        public int? UserId { get; set; } = null;
        public List<PermissionDTO> Permissions { get; set; }
        public byte PersistedGrantType { get; set; }
        public string BearerToken { get; set; } = null;
        public string UserDisplayName { get; set; }
    }
}
