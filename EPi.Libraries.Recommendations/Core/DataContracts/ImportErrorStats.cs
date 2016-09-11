namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class ImportErrorStats.
    /// </summary>
    [DataContract]
    public class ImportErrorStats
    {
        /// <summary>
        ///     Gets or sets the error code.
        /// </summary>
        /// <value>The error code.</value>
        [DataMember]
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        ///     Gets or sets the error code count.
        /// </summary>
        /// <value>The error code count.</value>
        [DataMember]
        [JsonProperty("errorCodeCount")]
        public int ErrorCodeCount { get; set; }
    }
}