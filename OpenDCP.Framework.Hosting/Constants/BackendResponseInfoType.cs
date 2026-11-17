/// <summary>
/// Specifies the classification type of a backend response message.
/// </summary>
public enum BackendResponseInfoType : byte
{
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The response contains informational data that does not affect the application flow.
    /// </summary>
    Info = 2,

    /// <summary>
    /// The operation completed, but with potential issues or non-critical anomalies.
    /// </summary>
    Warning = 3,

    /// <summary>
    /// The operation failed due to a critical error or unhandled exception.
    /// </summary>
    Error = 4
}