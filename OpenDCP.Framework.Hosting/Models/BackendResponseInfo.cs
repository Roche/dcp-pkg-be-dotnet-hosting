using System.Collections.Generic;

namespace OpenDCP.Framework.Hosting.Models
{
    /// <summary>
    /// Represents a detailed message or status returned by a backend operation.
    /// </summary>
    public class BackendResponseInfo
    {
        /// <summary>
        /// Gets or sets the descriptive text or payload message from the backend.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> containing the message details, or <see langword="null"/> if no message is provided.
        /// </value>
        public string? Message { get; set; } = null;

        /// <summary>
        /// Gets or sets the severity or classification of the backend response.
        /// </summary>
        /// <value>
        /// One of the <see cref="BackendResponseInfoType"/> values. The default is <see cref="BackendResponseInfoType.Success"/>.
        /// </value>
        public BackendResponseInfoType Type { get; set; } = BackendResponseInfoType.Success;
        
        /// <summary>
        /// Gets or sets the descriptive text for placeholders used during message translations.
        /// </summary>
        public List<string> Params { get; set; } = new List<string>();
    }
}