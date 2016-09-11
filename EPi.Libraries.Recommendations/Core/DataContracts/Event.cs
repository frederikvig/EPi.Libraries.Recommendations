namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using EPi.Libraries.Recommendations.Core.Enums;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///     Class Event.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    [DataContract]
    public class Event
    {
        /// <summary>
        ///     Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        [DataMember]
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        ///     Gets or sets the type of the event.
        /// </summary>
        /// <value>The type of the event.</value>
        [DataMember]
        [JsonProperty("eventType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EventType EventType { get; set; }

        /// <summary>
        ///     Gets or sets the item identifier.
        /// </summary>
        /// <value>The item identifier.</value>
        [DataMember]
        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        ///     Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        [DataMember]
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        ///     Gets or sets the unit price.
        /// </summary>
        /// <value>The unit price.</value>
        [DataMember]
        [JsonProperty("unitPrice")]
        public float UnitPrice { get; set; }
    }
}