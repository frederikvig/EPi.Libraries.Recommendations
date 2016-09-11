namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class RandomSplitterParameters.
    /// </summary>
    [DataContract]
    public class RandomSplitterParameters
    {
        /// <summary>
        ///     Gets or sets the random seed.
        /// </summary>
        /// <value>The random seed.</value>
        [DataMember]
        [JsonProperty("randomSeed")]
        public int? RandomSeed { get; set; }

        /// <summary>
        ///     Gets or sets the test percent.
        /// </summary>
        /// <value>The test percent.</value>
        [DataMember]
        [JsonProperty("testPercent")]
        public int? TestPercent { get; set; }
    }
}