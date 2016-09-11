namespace EPi.Libraries.Recommendations.Core.Models
{
    using EPiServer.Data;
    using EPiServer.Data.Dynamic;

    /// <summary>
    /// Class RecommendationSettings.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    [EPiServerDataStore(AutomaticallyRemapStore = true, AutomaticallyCreateStore = true)]
    public class RecommendationSettings
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Identity Id { get; set; }

        /// <summary>
        /// Gets or sets the active build identifier.
        /// </summary>
        /// <value>The active build identifier.</value>
        public long ActiveBuildId { get; set; }

        /// <summary>
        /// Gets or sets the model identifier.
        /// </summary>
        /// <value>The model identifier.</value>
        public string ModelId { get; set; }

        /// <summary>
        /// Gets or sets the name of the model.
        /// </summary>
        /// <value>The name of the model.</value>
        public string ModelName { get; set; }
    }
}
