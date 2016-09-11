namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using EPi.Libraries.Recommendations.Core.Enums;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///     Class BuildRequestInfo.
    /// </summary>
    [DataContract]
    public class BuildRequestInfo
    {
        /// <summary>
        ///     Gets or sets the build parameters.
        /// </summary>
        /// <value>The build parameters.</value>
        [JsonProperty("buildParameters")]
        public BuildParameters BuildParameters { get; set; }

        /// <summary>
        ///     Gets or sets the type of the build.
        /// </summary>
        /// <value>The type of the build.</value>
        [JsonProperty("buildType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public BuildType BuildType { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}