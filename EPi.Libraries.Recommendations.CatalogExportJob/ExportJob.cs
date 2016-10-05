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
namespace EPi.Libraries.Recommendations.CatalogExportJob
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
    [ScheduledPlugIn(DisplayName = "Export catalog items", Description = "Export catalog items to Recommendations API")]
    public class ExportJob : ScheduledJobBase
    {
        /// <summary>
        /// The log
        /// </summary>
        private readonly ILogger log = LogManager.GetLogger();

        /// <summary>
        /// The catalog display name
        /// </summary>
        private string catalogDisplayName;

        /// <summary>
        /// The catalog items
        /// </summary>
        private List<CatalogItem> catalogItems;

        /// <summary>
        /// The last execution date
        /// </summary>
        private DateTime lastExecution;

        /// <summary>
        /// The model identifier
        /// </summary>
        private string modelId;

        /// <summary>
        /// Stop signaled
        /// </summary>
        private bool stopSignaled;

        /// <summary>
        /// Gets the recommendation service.
        /// </summary>
        /// <value>The recommendation service.</value>
        /// <exception cref="ActivationException">if there is are errors resolving
        ///             the service instance.</exception>
        private static IRecommendationService RecommendationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRecommendationService>();
            }
        }

        /// <summary>
        /// Gets the scheduled job repository.
        /// </summary>
        /// <value>The scheduled job repository.</value>
        /// <exception cref="ActivationException">if there is are errors resolving
        ///             the service instance.</exception>
        private static IScheduledJobRepository ScheduledJobRepository
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IScheduledJobRepository>();
            }
        }

        /// <summary>
        /// Gets the recommender.
        /// </summary>
        /// <value>The recommender.</value>
        /// <exception cref="ActivationException">if there is are errors resolving
        ///             the service instance.</exception>
        private static RecommendationsApiWrapper Recommender
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RecommendationsApiWrapper>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportJob"/> class.
        /// </summary>
        public ExportJob()
        {
            this.IsStoppable = true;
        }

        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        public override string Execute()
        {
            this.OnStatusChanged(string.Format("Starting execution of {0}", this.GetType()));

            try
            {
                this.InitSettings();
            }
            catch (HttpRequestException httpRequestException)
            {
                return httpRequestException.Message;
            }
            catch (ArgumentNullException argumentNullException)
            {
                return argumentNullException.Message;
            }
            catch (ArgumentException argumentException)
            {
                return argumentException.Message;
            }
            catch (ActivationException activationException)
            {
                return activationException.Message;
            }

            string uploadCatalogMessage;

            if (!this.UploadCatalog(out uploadCatalogMessage))
            {
                return uploadCatalogMessage;
            }

            return this.stopSignaled ? "Stop of job was called" : uploadCatalogMessage;
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
        /// <exception cref="HttpRequestException">Failed to get or create model.</exception>
        /// <exception cref="ArgumentNullException">No model found.</exception>
        /// <exception cref="ArgumentException">No model found.</exception>
        /// <exception cref="ActivationException">if there is are errors resolving
        ///             the service instance.</exception>
        private void InitSettings()
        {
            ScheduledJob thisJob = ScheduledJobRepository.Get(this.ScheduledJobId);
            this.lastExecution = thisJob.LastExecution;

            RecommendationSettings settings = Helpers.GetRecommendationSettings();

            this.modelId = settings.ModelId;
            this.catalogDisplayName = settings.CatalogDisplayName;
        }

        /// <summary>
        /// Uploads the catalog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool UploadCatalog(out string message)
        {
            // Import data to the model.
            this.log.Information("[Recommendations] Importing catalog files...");

            try
            {
                this.catalogItems = RecommendationService.GetCatalogItems(this.lastExecution);
            }
            catch (ActivationException activationException)
            {
                message = string.Format(CultureInfo.InvariantCulture, "[Recommendations] Error sending catalog: {0}", activationException.Message);
                this.log.Error(message, activationException);
                return false;
            }


            if (this.catalogItems.Count == 0)
            {
                message = string.Format(
                CultureInfo.InvariantCulture,
                "[Recommendations] Imported {0} catalog items.",
                0);

                return true;
            }

            string catalogContent = this.catalogItems.CreateCatalogContent();

            if (string.IsNullOrWhiteSpace(catalogContent))
            {
                message = string.Format(
                CultureInfo.InvariantCulture,
                "[Recommendations] Imported {0} catalog items.",
                0);

                return true;
            }

            CatalogImportStats catalogImportStats;

            try
            {
                catalogImportStats = Recommender.UploadCatalog(this.modelId, catalogContent, this.catalogDisplayName);
            }
            catch (HttpRequestException httpRequestException)
            {
                message = string.Format(
                CultureInfo.InvariantCulture,
                "[Recommendations] Error sending catalog: {0}", httpRequestException.Message);

                this.log.Error(message, httpRequestException);
                this.log.Information("[Recommendations] Catalog content trying to send:\\r\n{0}", catalogContent);
                return false;
            }
            catch (ArgumentNullException argumentNullException)
            {
                message = string.Format(CultureInfo.InvariantCulture, "[Recommendations] Error sending catalog: {0}", argumentNullException.Message);

                this.log.Error(message, argumentNullException);
                return false;
            }
            catch (EncoderFallbackException encoderFallbackException)
            {
                message = string.Format(CultureInfo.InvariantCulture, "[Recommendations] Error sending catalog: {0}", encoderFallbackException.Message);
                this.log.Error(message, encoderFallbackException);
                return false;
            }
            catch (ActivationException activationException)
            {
                message = string.Format(CultureInfo.InvariantCulture, "[Recommendations] Error sending catalog: {0}", activationException.Message);
                this.log.Error(message, activationException);
                return false;
            }

            message = string.Format(
                CultureInfo.InvariantCulture,
                "[Recommendations] Imported {0} catalog items.",
                catalogImportStats.ImportedLineCount);

            this.log.Information(message);

            return true;
        }
    }
}