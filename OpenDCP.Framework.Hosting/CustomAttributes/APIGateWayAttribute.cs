using System;

namespace OpenDCP.Framework.Hosting.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method |
                       AttributeTargets.Class)]
    public class ApiGatewayAttribute : Attribute
    {
        public bool IsExposed { get; set; }
    }
}
