using System;

namespace OpenDCP.Framework.Hosting.Models
{
    /// <summary>
    /// Represents a standardized error response returned by the API.
    /// Designed for legacy clients that expect specific fields.
    /// </summary>
    public sealed class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the error message describing what went wrong.
        /// This field is required for all error responses.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace of the exception, if available.
        /// Will be <c>null</c> in production environments if error details are sanitized.
        /// </summary>
        public string? StackTrace { get; set; } = null;

        /// <summary>
        /// Gets or sets the unique identifier for this specific error occurrence.
        /// Useful for correlating logs and debugging issues.
        /// </summary>
        public Guid ErrorId { get; set; } = Guid.NewGuid();
    }
}
