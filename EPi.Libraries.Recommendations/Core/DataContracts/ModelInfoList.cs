namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class ModelInfoList.
    /// </summary>
    [DataContract]
    public class ModelInfoList
    {
        /// <summary>
        ///     Gets or sets the models.
        /// </summary>
        /// <value>The models.</value>
        [DataMember]
        [JsonProperty("models")]
        public IEnumerable<ModelInfo> Models { get; set; }
    }
}