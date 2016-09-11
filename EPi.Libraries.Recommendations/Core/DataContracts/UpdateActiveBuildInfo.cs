namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class UpdateActiveBuildInfo.
    /// </summary>
    [DataContract]
    public class UpdateActiveBuildInfo
    {
        /// <summary>
        ///     Gets or sets the active build identifier.
        /// </summary>
        /// <value>The active build identifier.</value>
        [JsonProperty("activeBuildId")]
        public long ActiveBuildId { get; set; }
    }
}