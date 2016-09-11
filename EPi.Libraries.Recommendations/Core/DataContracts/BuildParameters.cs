namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class BuildParameters.
    /// </summary>
    [DataContract]
    public class BuildParameters
    {
        /// <summary>
        ///     Gets or sets the FBT.
        /// </summary>
        /// <value>The FBT.</value>
        [DataMember]
        [JsonProperty("fbt")]
        public FbtBuildParameters Fbt { get; set; }

        /// <summary>
        ///     Gets or sets the ranking.
        /// </summary>
        /// <value>The ranking.</value>
        [DataMember]
        [JsonProperty("ranking")]
        public RankingBuildParameters Ranking { get; set; }

        /// <summary>
        ///     Gets or sets the recommendation.
        /// </summary>
        /// <value>The recommendation.</value>
        [DataMember]
        [JsonProperty("recommendation")]
        public RecommendationBuildParameters Recommendation { get; set; }
    }
}