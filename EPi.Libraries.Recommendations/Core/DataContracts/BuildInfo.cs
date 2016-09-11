/***************************************************************************************
 *
 * This file contains several class that simplify the serialization / deserialization
 * of the requests/responses to the RESTful API.
 *
 ***************************************************************************************/
namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using EPi.Libraries.Recommendations.Core.Enums;

    using Newtonsoft.Json;

    #region response classes

    /// <summary>
    ///     Class BuildInfo.
    /// </summary>
    [DataContract]
    public class BuildInfo
    {
        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the end date time.
        /// </summary>
        /// <value>The end date time.</value>
        [DataMember]
        [JsonProperty("endDateTime")]
        public string EndDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        ///     Gets or sets the model identifier.
        /// </summary>
        /// <value>The model identifier.</value>
        [DataMember]
        [JsonProperty("modelId")]
        public string ModelId { get; set; }

        /// <summary>
        ///     Gets or sets the name of the model.
        /// </summary>
        /// <value>The name of the model.</value>
        [DataMember]
        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        /// <summary>
        ///     Gets or sets the modified date time.
        /// </summary>
        /// <value>The modified date time.</value>
        [DataMember]
        [JsonProperty("modifiedDateTime")]
        public string ModifiedDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the start date time.
        /// </summary>
        /// <value>The start date time.</value>
        [DataMember]
        [JsonProperty("startDateTime")]
        public string StartDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [DataMember]
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        ///     Gets or sets the status message.
        /// </summary>
        /// <value>The status message.</value>
        [DataMember]
        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DataMember]
        [JsonProperty("type")]
        public BuildType Type { get; set; }
    }

    #endregion
}