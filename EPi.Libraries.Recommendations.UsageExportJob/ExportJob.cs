﻿// Copyright © 2016 Jeroen Stemerdink.
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
    using System.Configuration;
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
        /// The catalog display name
        /// </summary>
        private string catalogDisplayName;

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
        public override string Execute()
        {
            this.OnStatusChanged(string.Format("Starting execution of {0}", this.GetType()));

            this.InitSettings();

            string uploadUsageMessage;
            string buildMessage;

            if (!this.UploadUsage(out uploadUsageMessage))
            {
                return uploadUsageMessage;
            }

            bool useFtbBuild;
            bool.TryParse(ConfigurationManager.AppSettings["recommendations:useftbbuild"], out useFtbBuild);

            if (useFtbBuild)
            {
                if (!this.CreateFtbBuild(out buildMessage))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", uploadUsageMessage, buildMessage);
                }
            }
            else
            {
                if (!this.CreateRecommendationsBuild(out buildMessage))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", uploadUsageMessage, buildMessage);
                }
            }

            try
            {
                Recommender.SetActiveBuild(this.modelId, this.buildId);
            }
            catch (HttpRequestException httpRequestException)
            {
                buildMessage = httpRequestException.Message;
            }

            this.catalogItems.Clear();

            return this.stopSignaled ? "Stop of job was called" : string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", uploadUsageMessage, buildMessage);
        }

        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            this.stopSignaled = true;
        }

        /// <summary>
        /// Creates the FTB build.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CreateFtbBuild(out string message)
        {
            // Trigger a FTB build.
            string operationLocationHeader;
            this.log.Information(
                "[Recommendations] Triggering FTB build for model '{0}'. \nThis will take a few minutes...",
                this.modelId);

            this.buildId = Recommender.CreateFbtBuild(
                this.modelId,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Frequenty-Bought-Together Build {0}",
                    DateTime.UtcNow.ToString("yyyyMMddHHmmss")),
                false,
                out operationLocationHeader);

            // Monitor the build and wait for completion.
            this.log.Information("[Recommendations] Monitoring FTB build {0}", this.buildId);

            OperationInfo<BuildInfo> buildInfo =
                Recommender.WaitForOperationCompletion<BuildInfo>(
                    RecommendationsApiWrapper.GetOperationId(operationLocationHeader));

            message = string.Format(
                CultureInfo.InvariantCulture,
                "[Recommendations] Build {0} ended with status {1}.\n",
                this.buildId,
                buildInfo.Status);

            this.log.Information(message);

            if (string.Compare(buildInfo.Status, "Succeeded", StringComparison.OrdinalIgnoreCase) != 0)
            {
                this.log.Information("[Recommendations] FBT build {0} did not end successfully.", this.buildId);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a model.
        /// Returns the model ID for the model.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        private string CreateModel(string name)
        {
            this.log.Information("[Recommendations] Creating a new model {0}...", name);
            ModelInfo modelInfo = Recommender.CreateModel(name, this.catalogDisplayName);
            this.log.Information("[Recommendations] Model '{0}' created with ID: {1}", name, modelInfo.Id);

            return modelInfo.Id;
        }

        /// <summary>
        /// Creates the recommendations build.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CreateRecommendationsBuild(out string message)
        {
            // Trigger a recommendation build.
            string operationLocationHeader;
            this.log.Information(
                "[Recommendations] Triggering recommendation build for model '{0}'. \nThis will take a few minutes...",
                this.modelId);

            string featureList = this.catalogItems.GetFeatureList();
            bool useFeaturesInModel = !string.IsNullOrWhiteSpace(featureList);
            bool allowColdItemPlacement = !string.IsNullOrWhiteSpace(featureList);

            this.buildId = Recommender.CreateRecommendationsBuild(
                this.modelId,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "[Recommendations] Build {0}",
                    DateTime.UtcNow.ToString("yyyyMMddHHmmss")),
                false,
                useFeaturesInModel,
                allowColdItemPlacement,
                featureList,
                out operationLocationHeader);

            // Monitor the build and wait for completion.
            this.log.Information("[Recommendations] Monitoring recommendation build {0}", this.buildId);

            OperationInfo<BuildInfo> buildInfo =
                Recommender.WaitForOperationCompletion<BuildInfo>(
                    RecommendationsApiWrapper.GetOperationId(operationLocationHeader));

            message = string.Format(
                CultureInfo.InvariantCulture,
                "[Recommendations] Build {0} ended with status {1}.\n",
                this.buildId,
                buildInfo.Status);

            this.log.Information(message);

            if (string.Compare(buildInfo.Status, "Succeeded", StringComparison.OrdinalIgnoreCase) != 0)
            {
                this.log.Information(
                    "[Recommendations] Recommendation build {0} did not end successfully.",
                    this.buildId);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes the settings.
        /// </summary>
        private void InitSettings()
        {
            this.modelName = RecommendationSettingsRepository.GetModelName();

            this.catalogDisplayName = RecommendationSettingsRepository.GetCatalogDisplayName();

            this.usageDisplayName = RecommendationSettingsRepository.GetUsageDisplayName();

            ScheduledJob thisJob = ScheduledJobRepository.Get(this.ScheduledJobId);
            this.lastExecution = thisJob.LastExecution;

            RecommendationSettings settings = RecommendationSettingsRepository.GetSettingsByName(this.modelName);

            this.buildId = settings.ActiveBuildId;

            if (string.IsNullOrEmpty(settings.ModelId))
            {
                this.modelId = this.CreateModel(this.modelName);
                settings.ModelId = this.modelId;
                settings.ModelName = this.modelName;
                RecommendationSettingsRepository.Save(settings);
            }
            else
            {
                this.modelId = settings.ModelId;
            }

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