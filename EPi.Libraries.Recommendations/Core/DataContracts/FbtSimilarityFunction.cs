namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     FBT similarity functions
    /// </summary>
    [DataContract]
    public enum FbtSimilarityFunction
    {
        /// <summary>
        ///     Count of co-occurrences, favors predictability.
        /// </summary>
        Cooccurrence, 

        /// <summary>
        ///     Lift favors serendipity
        /// </summary>
        Lift, 

        /// <summary>
        ///     Jaccard is a compromise between co-occurrences and lift.
        /// </summary>
        Jaccard
    }
}