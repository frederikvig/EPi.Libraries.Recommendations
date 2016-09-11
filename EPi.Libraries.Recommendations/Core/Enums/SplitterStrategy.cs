namespace EPi.Libraries.Recommendations.Core.Enums
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     FBT similarity functions
    /// </summary>
    [DataContract]
    public enum SplitterStrategy
    {
        /// <summary>
        ///     Takes last transaction of users as the test set to use for evaluation.
        /// </summary>
        LastEventSplitter,

        /// <summary>
        ///     Takes a random set of transactions as the test set to use for evaluation.
        /// </summary>
        RandomSplitter
    }
}