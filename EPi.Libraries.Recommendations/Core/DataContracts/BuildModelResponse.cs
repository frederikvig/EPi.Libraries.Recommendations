namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class BuildModelResponse.
    /// </summary>
    [DataContract]
    public class BuildModelResponse
    {
        /// <summary>
        ///     Gets or sets the build identifier.
        /// </summary>
        /// <value>The build identifier.</value>
        [DataMember]
        [JsonProperty("buildId")]
        public long BuildId { get; set; }
    }
}