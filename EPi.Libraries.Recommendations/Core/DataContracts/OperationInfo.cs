namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class OperationInfo.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OperationInfo<T>
    {
        /// <summary>
        ///     Gets or sets the created date time.
        /// </summary>
        /// <value>The created date time.</value>
        [DataMember]
        [JsonProperty("createdDateTime")]
        public string CreatedDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the last action date time.
        /// </summary>
        /// <value>The last action date time.</value>
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty("lastActionDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public string LastActionDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the percent complete.
        /// </summary>
        /// <value>The percent complete.</value>
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty("percentComplete", NullValueHandling = NullValueHandling.Ignore)]
        public int PercentComplete { get; set; }

        /// <summary>
        ///     Gets or sets the resource location.
        /// </summary>
        /// <value>The resource location.</value>
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty("resourceLocation", NullValueHandling = NullValueHandling.Ignore)]
        public string ResourceLocation { get; set; }

        /// <summary>
        ///     Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        public T Result { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [DataMember]
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DataMember]
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}