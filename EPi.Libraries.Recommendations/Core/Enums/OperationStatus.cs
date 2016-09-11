namespace EPi.Libraries.Recommendations.Core.Enums
{
    /// <summary>
    /// Enum OperationStatus
    /// </summary>
    public enum OperationStatus
    {
        /// <summary>
        /// Build is in queue waiting for execution
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// Build is in progress
        /// </summary>
        Running = 1,

        /// <summary>
        /// Build is in the process of cancellation
        /// </summary>
        Cancelling = 2,

        /// <summary>
        /// Build was cancelled
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Build ended with success
        /// </summary>
        Succeeded = 4,

        /// <summary>
        /// Build ended with error
        /// </summary>
        Failed = 5
    }
}