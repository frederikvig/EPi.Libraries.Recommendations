namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class UsageImportStats.
    /// </summary>
    /// <seealso cref="EPi.Libraries.Recommendations.Core.DataContracts.CatalogImportStats" />
    [DataContract]
    public class UsageImportStats : CatalogImportStats
    {
        /// <summary>
        ///     Gets or sets the file identifier.
        /// </summary>
        /// <value>The file identifier.</value>
        [DataMember]
        [JsonProperty("fileId")]
        public string FileId { get; set; }
    }
}