namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Holds a recommendation result, which is a set of recommended items with reasoning and rating/score.
    /// </summary>
    [DataContract]
    public class RecommendedItemSetInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecommendedItemSetInfo" /> class.
        /// </summary>
        public RecommendedItemSetInfo()
        {
            this.Items = new List<RecommendedItemInfo>();
        }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [DataMember]
        [JsonProperty("items")]
        public IEnumerable<RecommendedItemInfo> Items { get; set; }

        /// <summary>
        ///     Gets or sets the rating.
        /// </summary>
        /// <value>The rating.</value>
        [DataMember]
        [JsonProperty("rating")]
        public double Rating { get; set; }

        /// <summary>
        ///     Gets or sets the reasoning.
        /// </summary>
        /// <value>The reasoning.</value>
        [DataMember]
        [JsonProperty("reasoning")]
        public IEnumerable<string> Reasoning { get; set; }
    }
}