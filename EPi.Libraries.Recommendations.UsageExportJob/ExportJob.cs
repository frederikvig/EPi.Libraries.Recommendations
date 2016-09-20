// Copyright © 2016 Jeroen Stemerdink.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
namespace EPi.Libraries.Recommendations.UsageExportJob
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Text;

    using EPi.Libraries.Recommendations.Core.DataContracts;
    using EPi.Libraries.Recommendations.Core.Infrastructure;
    using EPi.Libraries.Recommendations.Core.Models;

    using EPiServer.DataAbstraction;
    using EPiServer.Logging;
    using EPiServer.PlugIn;
    using EPiServer.Scheduler;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Class ExportJob.
    /// </summary>
    /// <seealso cref="EPiServer.Scheduler.ScheduledJobBase" />
    /// <author>Jeroen Stemerdink</author>
    [ScheduledPlugIn(DisplayName = "Export usage stats", Description = "Export usage stats to Recommendations API")]
    public class ExportJob : ScheduledJobBase
    {
        /// <summary>
        /// The log
        /// </summary>
        private readonly ILogger log = LogManager.GetLogger();

        /// <summary>
        /// The build identifier
        /// </summary>
        private long buildId;

        /// <summary>
        /// The catalog items
        /// </summary>
        private List<CatalogItem> catalogItems;

        /// <summary>
        /// The last execution
        /// </summary>
        private DateTime lastExecution;

        /// <summary>
        /// The model identifier
        /// </summary>
        private string modelId;

        /// <summary>
        /// The model name
        /// </summary>
        private string modelName;

        /// <summary>
        /// Stop was signaled
        /// </summary>
        private bool stopSignaled;

        /// <summary>
        /// The usage display name
        /// </summary>
        private string usageDisplayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportJob"/> class.
        /// </summary>
        public ExportJob()
        {
            this.IsStoppable = true;
        }

        /// <summary>
        /// Gets the build provider.
        /// </summary>
        /// <value>The build provider.</value>
        private static IBuildProvider BuildProvider
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IBuildProvider>();
            }
        }

        /// <summary>
        /// Gets the recommendation service.
        /// </summary>
        /// <value>The recommendation service.</value>
        private static IRecommendationService RecommendationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRecommendationService>();
            }
        }

        /// <summary>
        /// Gets the recommendation settings repository.
        /// </summary>
        /// <value>The recommendation settings repository.</value>
        private static RecommendationSettingsRepository RecommendationSettingsRepository
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RecommendationSettingsRepository>();
            }
        }

        /// <summary>
        /// Gets the recommender.
        /// </summary>
        /// <value>The recommender.</value>
        private static RecommendationsApiWrapper Recommender
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RecommendationsApiWrapper>();
            }
        }

        /// <summary>
        /// Gets the scheduled job repository.
        /// </summary>
        /// <value>The scheduled job repository.</value>
        private static IScheduledJobRepository ScheduledJobRepository
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IScheduledJobRepository>();
            }
        }

        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        /// <exception cref="HttpRequestException">Failed to set active build.</exception>
        public override string Execute()
        {
            this.OnStatusChanged(string.Format("Starting execution of {0}", this.GetType()));

            try
            {
                this.InitSettings();
            }
            catch (ApplicationException applicationException)
            {
                this.log.Error("[Recommendations] Error sending usage stats", applicationException);
                return applicationException.Message;
            }

            string uploadUsageMessage;
            string buildMessage;

            if (!this.UploadUsage(out uploadUsageMessage))
            {
                return uploadUsageMessage;
            }

            string featureList = this.catalogItems.GetFeatureList();

            long? generatedBuild = BuildProvider.CreateBuild(this.modelId, featureList, out buildMessage);

            if (!generatedBuild.HasValue)
            {
                return buildMessage;
            }

            Recommender.SetActiveBuild(this.modelId, this.buildId);

            this.catalogItems.Clear();

            if (this.stopSignaled)
            {
                return "Stop of job was called";
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", uploadUsageMessage, buildMessage);
        }

        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            this.stopSignaled = true;
        }

        /// <summary>
        /// Initializes the settings.
        /// </summary>
        /// <exception cref="ApplicationException">[Recommendations] Catalog not exported, run this job first.</exception>
        private void InitSettings()
        {
            this.modelName = RecommendationSettingsRepository.GetModelName();

            this.usageDisplayName = RecommendationSettingsRepository.GetUsageDisplayName();

            ScheduledJob thisJob = ScheduledJobRepository.Get(this.ScheduledJobId);
            this.lastExecution = thisJob.LastExecution;

            RecommendationSettings settings = RecommendationSettingsRepository.GetSettingsByName(this.modelName);

            this.buildId = settings.ActiveBuildId;

            if (string.IsNullOrEmpty(settings.ModelId))
            {
                throw new ApplicationException("[Recommendations] Catalog not exported, run this job first.");
            }

            this.modelId = settings.ModelId;

            this.catalogItems = RecommendationService.GetCatalogItems(DateTime.MinValue);
        }

        /// <summary>
        /// Uploads the usage.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool UploadUsage(out string message)
        {
            this.log.Information("[Recommendations] Importing usage files...");

            List<UsageItem> usageItems = RecommendationService.GetUsageItems(this.lastExecution);

            if (usageItems.Count == 0)
            {
                message = string.Format(CultureInfo.InvariantCulture, "[Recommendations] Imported {0} usage stats.", 0);

                return true;
            }

            string usageContent = usageItems.CreateUsageContent();

            if (string.IsNullOrWhiteSpace(usageContent))
            {
                message = string.Format(CultureInfo.InvariantCulture, "[Recommendations] Imported {0} usage stats.", 0);

                return true;
            }

            UsageImportStats usageImportStats;

            try
            {
                usageImportStats = Recommender.UploadUsage(this.modelId, usageContent, this.usageDisplayName);
            }
            catch (HttpRequestException httpRequestException)
            {
                message = string.Format(
                    CultureInfo.InvariantCulture,
                    "[Recommendations] Error sending usage stats: {0}",
                    httpRequestException.Message);

                this.log.Error("[Recommendations] Error sending usage stats", httpRequestException);
                this.log.Information("[Recommendations] Usage stats content trying to send:\\r\n{0}", usageContent);
                return false;
            }
            catch (ArgumentNullException argumentNullException)
            {
                message = string.Format(
                    CultureInfo.InvariantCulture,
                    "[Recommendations] Error sending usage stats: {0}",
                    argumentNullException.Message);

                this.log.Error("[Recommendations] Error sending usage stats", argumentNullException);
                return false;
            }
            catch (EncoderFallbackException encoderFallbackException)
            {
                message = string.Format(
                    CultureInfo.InvariantCulture,
                    "[Recommendations] Error sending usage stats: {0}",
                    encoderFallbackException.Message);
                this.log.Error("[Recommendations] Error sending usage stats", encoderFallbackException);
                return false;
            }

            message = string.Format(
                CultureInfo.InvariantCulture,
                "[Recommendations] Imported {0} catalog items.",
                usageImportStats.ImportedLineCount);

            this.log.Information(message);

            return true;
        }
    }
}