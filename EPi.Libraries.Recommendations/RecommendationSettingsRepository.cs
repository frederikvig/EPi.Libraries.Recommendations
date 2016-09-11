using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPi.Libraries.Recommendations
{
    using System.Configuration;
    using System.Web.Configuration;

    using EPi.Libraries.Recommendations.Core.Models;

    using EPiServer.Data;
    using EPiServer.Data.Dynamic;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Class RecommendationSettingsRepository.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    [ServiceConfiguration(typeof(RecommendationSettingsRepository), Lifecycle = ServiceInstanceScope.Singleton)]
    public class RecommendationSettingsRepository
    {
        /// <summary>
        /// Gets the recommendation settings store.
        /// </summary>
        /// <value>The recommendation settings store.</value>
        private static DynamicDataStore RecommendationSettingsStore
        {
            get
            {
                return typeof(RecommendationSettings).GetOrCreateStore();
            }
        }

        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetModelName()
        {
            string modelName = string.Empty;

            try
            {
                modelName = ConfigurationManager.AppSettings["recommendations:modelname"];
            }
            catch (NotSupportedException)
            {
            }

            if (string.IsNullOrWhiteSpace(modelName))
            {
                modelName = "EPiServerCommerce";
            }

            return modelName;
        }

        /// <summary>
        /// Gets the display name of the catalog.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetCatalogDisplayName()
        {
            string catalogDisplayName = string.Empty;

            try
            {
                catalogDisplayName = ConfigurationManager.AppSettings["recommendations:catalogdisplayname"];
            }
            catch (NotSupportedException)
            {
            }

            if (string.IsNullOrWhiteSpace(catalogDisplayName))
            {
                catalogDisplayName = "EPiServer Commerce catalog";
            }

            return catalogDisplayName;
        }

        /// <summary>
        /// Gets the display name of the usage.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetUsageDisplayName()
        {
            string usageDisplayName = string.Empty;

            try
            {
                usageDisplayName = ConfigurationManager.AppSettings["recommendations:usagedisplayname"];
            }
            catch (NotSupportedException)
            {
            }

            if (string.IsNullOrWhiteSpace(usageDisplayName))
            {
                usageDisplayName = "EPiServer Commerce catalog usages";
            }

            return usageDisplayName;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <returns>List&lt;RecommendationSettings&gt;.</returns>
        public virtual List<RecommendationSettings> GetSettings()
        {
            List<RecommendationSettings> settings = RecommendationSettingsStore.Items<RecommendationSettings>().ToList();
            return settings;
        }

        /// <summary>
        /// Gets the default settings.
        /// </summary>
        /// <returns>RecommendationSettings.</returns>
        public virtual RecommendationSettings GetDefaultSettings()
        {
            return this.GetSettingsByName(this.GetModelName());
        }

        /// <summary>
        /// Gets the settings by name.
        /// </summary>
        /// <param name="modelname">The modelname.</param>
        /// <returns>RecommendationSettings.</returns>
        public virtual RecommendationSettings GetSettingsByName(string modelname)
        {
            RecommendationSettings settings = this.GetSettings().FirstOrDefault( s => s.ModelName.Equals(modelname));

            if (settings != null)
            {
                return settings;
            }
            RecommendationSettings newSettings = new RecommendationSettings { ActiveBuildId = -1, ModelName = modelname, ModelId = string.Empty };
            Identity id = RecommendationSettingsStore.Save(newSettings);
            settings = GetSettingsById(id);
            return settings;
        }
        /// <summary>
        /// Gets the settings by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RecommendationSettings.</returns>
        private static RecommendationSettings GetSettingsById(Identity id)
        {
            return RecommendationSettingsStore.Load<RecommendationSettings>(id);
        }

        /// <summary>
        /// Saves the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void Save(RecommendationSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            RecommendationSettingsStore.Save(settings);
        }
    }
}
