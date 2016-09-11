namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using EPi.Libraries.Recommendations.Core.Enums;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///     Class FbtBuildParameters.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    [DataContract]
    public class FbtBuildParameters
    {
        /// <summary>
        ///     Gets or sets a value indicating whether [enable modeling insights].
        /// </summary>
        /// <value>
        ///     <c>null</c> if [enable modeling insights] contains no value, <c>true</c> if [enable modeling insights];
        ///     otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        [JsonProperty("enableModelingInsights")]
        public bool? EnableModelingInsights { get; set; }

        /// <summary>
        ///     Gets or sets the maximum size of the item set.
        /// </summary>
        /// <value>The maximum size of the item set.</value>
        [DataMember]
        [JsonProperty("maxItemSetSize")]
        public int? MaxItemSetSize { get; set; }

        /// <summary>
        ///     Gets or sets the minimal score.
        /// </summary>
        /// <value>The minimal score.</value>
        [DataMember]
        [JsonProperty("minimalScore")]
        public double? MinimalScore { get; set; }

        /// <summary>
        ///     Gets or sets the random splitter parameters.
        /// </summary>
        /// <value>The random splitter parameters.</value>
        [DataMember]
        [JsonProperty("randomSplitterParameters")]
        public RandomSplitterParameters RandomSplitterParameters { get; set; }

        /// <summary>
        ///     Gets or sets the similarity function.
        /// </summary>
        /// <value>The similarity function.</value>
        [DataMember]
        [JsonProperty("similarityFunction")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FbtSimilarityFunction SimilarityFunction { get; set; }

        /// <summary>
        ///     Gets or sets the splitter strategy.
        /// </summary>
        /// <value>The splitter strategy.</value>
        [DataMember]
        [JsonProperty("splitterStrategy")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SplitterStrategy SplitterStrategy { get; set; }

        /// <summary>
        ///     Gets or sets the support threshold.
        /// </summary>
        /// <value>The support threshold.</value>
        [DataMember]
        [JsonProperty("supportThreshold")]
        public int? SupportThreshold { get; set; }
    }
}