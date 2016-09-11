namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class ModelRequestInfo.
    /// </summary>
    [DataContract]
    public class ModelRequestInfo
    {
        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the name of the model.
        /// </summary>
        /// <value>The name of the model.</value>
        [DataMember]
        [JsonProperty("modelName")]
        public string ModelName { get; set; }
    }
}