namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class ModelInfo.
    /// </summary>
    [DataContract]
    public class ModelInfo
    {
        /// <summary>
        ///     Gets or sets the active build identifier.
        /// </summary>
        /// <value>The active build identifier.</value>
        [DataMember]
        [JsonProperty("activeBuildId")]
        public long ActiveBuildId { get; set; }

        /// <summary>
        ///     Gets or sets the created date time.
        /// </summary>
        /// <value>The created date time.</value>
        [DataMember]
        [JsonProperty("createdDateTime")]
        public string CreatedDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}