namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class RankingBuildParameters.
    /// </summary>
    [DataContract]
    public class RankingBuildParameters
    {
        /// <summary>
        ///     Gets or sets the item cut off lower bound.
        /// </summary>
        /// <value>The item cut off lower bound.</value>
        [DataMember]
        [JsonProperty("itemCutOffLowerBound")]
        public int? ItemCutOffLowerBound { get; set; }

        /// <summary>
        ///     Gets or sets the item cut off upper bound.
        /// </summary>
        /// <value>The item cut off upper bound.</value>
        [DataMember]
        [JsonProperty("itemCutOffUpperBound")]
        public int? ItemCutOffUpperBound { get; set; }

        /// <summary>
        ///     Gets or sets the number of model dimensions.
        /// </summary>
        /// <value>The number of model dimensions.</value>
        [DataMember]
        [JsonProperty("numberOfModelDimensions")]
        public int? NumberOfModelDimensions { get; set; }

        /// <summary>
        ///     Gets or sets the number of model iterations.
        /// </summary>
        /// <value>The number of model iterations.</value>
        [DataMember]
        [JsonProperty("numberOfModelIterations")]
        public int? NumberOfModelIterations { get; set; }

        /// <summary>
        ///     Gets or sets the user cut off lower bound.
        /// </summary>
        /// <value>The user cut off lower bound.</value>
        [DataMember]
        [JsonProperty("userCutOffLowerBound")]
        public int? UserCutOffLowerBound { get; set; }

        /// <summary>
        ///     Gets or sets the user cut off upper bound.
        /// </summary>
        /// <value>The user cut off upper bound.</value>
        [DataMember]
        [JsonProperty("userCutOffUpperBound")]
        public int? UserCutOffUpperBound { get; set; }

        /// <summary>
        ///     Class RecommendationBuildParameters.
        /// </summary>
        /// <author>Jeroen Stemerdink</author>
        [DataContract]
        public class RecommendationBuildParameters
        {
            /// <summary>
            ///     Gets or sets a value indicating whether [allow cold item placement].
            /// </summary>
            /// <value>
            ///     <c>null</c> if [allow cold item placement] contains no value, <c>true</c> if [allow cold item placement];
            ///     otherwise, <c>false</c>.
            /// </value>
            [DataMember]
            [JsonProperty("allowColdItemPlacement")]
            public bool? AllowColdItemPlacement { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether [enable feature correlation].
            /// </summary>
            /// <value>
            ///     <c>null</c> if [enable feature correlation] contains no value, <c>true</c> if [enable feature correlation];
            ///     otherwise, <c>false</c>.
            /// </value>
            [DataMember]
            [JsonProperty("enableFeatureCorrelation")]
            public bool? EnableFeatureCorrelation { get; set; }

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
            ///     Gets or sets a value indicating whether [enable u2 i].
            /// </summary>
            /// <value><c>null</c> if [enable u2 i] contains no value, <c>true</c> if [enable u2 i]; otherwise, <c>false</c>.</value>
            [DataMember]
            [JsonProperty("enableU2I")]
            public bool? EnableU2I { get; set; }

            /// <summary>
            ///     Gets or sets the item cut off lower bound.
            /// </summary>
            /// <value>The item cut off lower bound.</value>
            [DataMember]
            [JsonProperty("itemCutOffLowerBound")]
            public int? ItemCutOffLowerBound { get; set; }

            /// <summary>
            ///     Gets or sets the item cut off upper bound.
            /// </summary>
            /// <value>The item cut off upper bound.</value>
            [DataMember]
            [JsonProperty("itemCutOffUpperBound")]
            public int? ItemCutOffUpperBound { get; set; }

            /// <summary>
            ///     Gets or sets the modeling feature list.
            /// </summary>
            /// <value>The modeling feature list.</value>
            [DataMember]
            [JsonProperty("modelingFeatureList")]
            public string ModelingFeatureList { get; set; }

            /// <summary>
            ///     Gets or sets the number of model dimensions.
            /// </summary>
            /// <value>The number of model dimensions.</value>
            [DataMember]
            [JsonProperty("numberOfModelDimensions")]
            public int? NumberOfModelDimensions { get; set; }

            /// <summary>
            ///     Gets or sets the number of model iterations.
            /// </summary>
            /// <value>The number of model iterations.</value>
            [DataMember]
            [JsonProperty("numberOfModelIterations")]
            public int? NumberOfModelIterations { get; set; }

            /// <summary>
            ///     Gets or sets the reasoning feature list.
            /// </summary>
            /// <value>The reasoning feature list.</value>
            [DataMember]
            [JsonProperty("reasoningFeatureList")]
            public string ReasoningFeatureList { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether [use features in model].
            /// </summary>
            /// <value>
            ///     <c>null</c> if [use features in model] contains no value, <c>true</c> if [use features in model]; otherwise,
            ///     <c>false</c>.
            /// </value>
            [DataMember]
            [JsonProperty("useFeaturesInModel")]
            public bool? UseFeaturesInModel { get; set; }

            /// <summary>
            ///     Gets or sets the user cut off lower bound.
            /// </summary>
            /// <value>The user cut off lower bound.</value>
            [DataMember]
            [JsonProperty("userCutOffLowerBound")]
            public int? UserCutOffLowerBound { get; set; }

            /// <summary>
            ///     Gets or sets the user cut off upper bound.
            /// </summary>
            /// <value>The user cut off upper bound.</value>
            [DataMember]
            [JsonProperty("userCutOffUpperBound")]
            public int? UserCutOffUpperBound { get; set; }
        }

        /// <summary>
        ///     Class UpdateModelRequestInfo.
        /// </summary>
        /// <author>Jeroen Stemerdink</author>
        [DataContract]
        public class UpdateModelRequestInfo
        {
            /// <summary>
            ///     Gets or sets the active build identifier.
            /// </summary>
            /// <value>The active build identifier.</value>
            [DataMember]
            [JsonProperty("activeBuildId")]
            public long? ActiveBuildId { get; set; }

            /// <summary>
            ///     Gets or sets the description.
            /// </summary>
            /// <value>The description.</value>
            [DataMember]
            [JsonProperty("description")]
            public string Description { get; set; }
        }
    }
}