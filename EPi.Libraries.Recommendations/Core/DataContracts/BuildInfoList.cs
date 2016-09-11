namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class BuildInfoList.
    /// </summary>
    [DataContract]
    public class BuildInfoList
    {
        /// <summary>
        ///     Gets or sets the builds.
        /// </summary>
        /// <value>The builds.</value>
        [DataMember]
        [JsonProperty("builds")]
        public IEnumerable<BuildInfo> Builds { get; set; }
    }
}