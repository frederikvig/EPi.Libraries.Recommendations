namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class UsageEvent.
    /// </summary>
    [DataContract]
    public class UsageEvent
    {
        /// <summary>
        ///     Gets or sets the build identifier.
        /// </summary>
        /// <value>The build identifier.</value>
        [DataMember]
        [JsonProperty("buildId")]
        public int BuildId { get; set; }

        /// <summary>
        ///     Gets or sets the events.
        /// </summary>
        /// <value>The events.</value>
        [DataMember]
        [JsonProperty("events")]
        public Event[] Events { get; set; }

        /// <summary>
        ///     Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        [DataMember]
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>The serialized object.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}