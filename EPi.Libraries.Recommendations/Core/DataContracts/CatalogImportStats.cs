namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class CatalogImportStats.
    /// </summary>
    [DataContract]
    public class CatalogImportStats
    {
        /// <summary>
        ///     Gets or sets the error line count.
        /// </summary>
        /// <value>The error line count.</value>
        [DataMember]
        [JsonProperty("errorLineCount")]
        public int ErrorLineCount { get; set; }

        /// <summary>
        ///     Gets or sets the error summary.
        /// </summary>
        /// <value>The error summary.</value>
        [DataMember]
        [JsonProperty("errorSummary")]
        public IEnumerable<ImportErrorStats> ErrorSummary { get; set; }

        /// <summary>
        ///     Gets or sets the imported line count.
        /// </summary>
        /// <value>The imported line count.</value>
        [DataMember]
        [JsonProperty("importedLineCount")]
        public int ImportedLineCount { get; set; }

        /// <summary>
        ///     Gets or sets the processed line count.
        /// </summary>
        /// <value>The processed line count.</value>
        [DataMember]
        [JsonProperty("processedLineCount")]
        public int ProcessedLineCount { get; set; }
    }
}