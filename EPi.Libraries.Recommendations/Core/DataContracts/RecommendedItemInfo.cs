namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class RecommendedItemInfo.
    /// </summary>
    [DataContract]
    public class RecommendedItemInfo
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        [DataMember]
        [JsonProperty("metadata")]
        public string Metadata { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}