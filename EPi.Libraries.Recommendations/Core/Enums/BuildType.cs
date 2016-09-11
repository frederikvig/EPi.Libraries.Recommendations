namespace EPi.Libraries.Recommendations.Core.Enums
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     Types of build
    /// </summary>
    [DataContract]
    public enum BuildType
    {
        /// <summary>
        ///     Build that will create a model for recommendation
        /// </summary>
        Recommendation = 1, 

        /// <summary>
        ///     A build that creates a model to score features.
        /// </summary>
        Ranking = 2, 

        /// <summary>
        ///     A build that creates a model for fbt
        /// </summary>
        Fbt = 3
    }
}