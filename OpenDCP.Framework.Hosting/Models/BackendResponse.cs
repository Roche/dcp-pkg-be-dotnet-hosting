namespace OpenDCP.Framework.Hosting.Models
{
    /// <summary>
    /// Represents a standardized wrapper for backend operation responses, combining data with execution metadata.
    /// </summary>
    /// <typeparam name="T">The type of the data payload returned by the operation. Must be a reference type (<see langword="class"/>).</typeparam>
    public class BackendResponse<T> where T : class
    {
        /// <summary>
        /// Gets or sets the primary data payload returned by the backend operation.
        /// </summary>
        /// <value>
        /// The strongly-typed data object of type <typeparamref name="T"/>.
        /// </value>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the metadata, status, or message details associated with the backend operation.
        /// </summary>
        /// <value>
        /// A <see cref="BackendResponseInfo"/> object containing the response type and descriptive details.
        /// </value>
        public BackendResponseInfo Info { get; set; }
    }
}