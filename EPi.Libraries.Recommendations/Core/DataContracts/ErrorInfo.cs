namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Globalization;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    /// Class ErrorInfo.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    [DataContract]
    public class ErrorInfo
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        [DataMember]
        [JsonProperty("error")]
        public Error Error { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string messageFormat = "{0}: {1} [{2}]";
            string message = string.Empty;

            if (this.Error != null && this.Error.InnerError != null)
            {
                message = string.Format(CultureInfo.InvariantCulture, messageFormat, this.Error.Code, this.Error.InnerError.Message, this.Error.InnerError.Code);
            }
            else if (this.Error != null)
            {
                message = string.Format(CultureInfo.InvariantCulture, messageFormat, this.Error.Code, this.Error.Message, string.Empty);
            }

            return message;
        }
    }
}
