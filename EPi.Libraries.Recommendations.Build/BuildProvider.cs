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
namespace EPi.Libraries.Recommendations.Build
{
    using System;
    using System.Globalization;
    using System.Net.Http;

    using EPi.Libraries.Recommendations.Core.DataContracts;
    using EPi.Libraries.Recommendations.Core.Infrastructure;

    using EPiServer.Logging;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Class BuildProvider.
    /// </summary>
    /// <seealso cref="EPi.Libraries.Recommendations.IBuildProvider" />
    /// <author>Jeroen Stemerdink</author>
    [ServiceConfiguration(typeof(IBuildProvider), Lifecycle = ServiceInstanceScope.Singleton)]
    public class BuildProvider : IBuildProvider
    {
        /// <summary>
        /// The log
        /// </summary>
        private readonly ILogger log = LogManager.GetLogger();

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
        /// Creates the build.
        /// </summary>
        /// <param name="modelId">The id of the model.</param>
        /// <param name="featureList">The list of features to use in the build.</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
        public long? CreateBuild(string modelId, string featureList, out string message)
        {
            // Trigger a recommendation build.
            string operationLocationHeader = string.Empty;
            this.log.Information(
                "[Recommendations] Triggering recommendation build for model '{0}'. \nThis will take a few minutes...",
                modelId);

            bool useFeaturesInModel = !string.IsNullOrWhiteSpace(featureList);
            bool allowColdItemPlacement = !string.IsNullOrWhiteSpace(featureList);

            long? buildId = null;

            try
            {
                buildId = Recommender.CreateRecommendationsBuild(
                    modelId,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "[Recommendations] Build {0}",
                        DateTime.UtcNow.ToString("yyyyMMddHHmmss")),
                    false,
                    useFeaturesInModel,
                    allowColdItemPlacement,
                    featureList,
                    out operationLocationHeader);
            }
            catch (HttpRequestException httpRequestException)
            {
                this.log.Error("[Recommendations] Error executing build", httpRequestException);
            }

            // Monitor the build and wait for completion.
            this.log.Information("[Recommendations] Monitoring recommendation build {0}", buildId);

            OperationInfo<BuildInfo> buildInfo =
                Recommender.WaitForOperationCompletion<BuildInfo>(
                    RecommendationsApiWrapper.GetOperationId(operationLocationHeader));

            message = string.Format(
                CultureInfo.InvariantCulture,
                "[Recommendations] Build {0} ended with status {1}.\n",
                buildId,
                buildInfo.Status);

            this.log.Information(message);

            if (string.Compare(buildInfo.Status, "Succeeded", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return buildId;
            }

            this.log.Information("[Recommendations] Recommendation build {0} did not end successfully.", buildId);
            return null;
        }
    }
}