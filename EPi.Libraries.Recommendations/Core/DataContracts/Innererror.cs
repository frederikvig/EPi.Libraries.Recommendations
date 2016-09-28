namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    /// Class Innererror.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    [DataContract]
    public class Innererror
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [DataMember]
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        [DataMember]
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}