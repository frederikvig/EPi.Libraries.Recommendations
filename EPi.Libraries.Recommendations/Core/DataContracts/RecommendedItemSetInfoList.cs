namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class RecommendedItemSetInfoList.
    /// </summary>
    [DataContract]
    public class RecommendedItemSetInfoList
    {
        /// <summary>
        ///     Gets or sets the recommended item set information.
        /// </summary>
        /// <value>The recommended item set information.</value>
        [DataMember]
        [JsonProperty("recommendedItems")]
        public IEnumerable<RecommendedItemSetInfo> RecommendedItemSetInfo { get; set; }
    }
}