using System.Collections.Generic;

namespace OpenDCP.Framework.Hosting.Models
{
    [System.Obsolete]
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public Dictionary<string, string[]> Headers { get; set; } = new Dictionary<string, string[]>();
    }
}
