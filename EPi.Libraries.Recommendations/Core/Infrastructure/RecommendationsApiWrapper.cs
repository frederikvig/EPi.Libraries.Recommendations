namespace EPi.Libraries.Recommendations.Core.Infrastructure
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;
    using System.Threading;
    using System.Web.Configuration;

    using EPi.Libraries.Recommendations.Core.DataContracts;
    using EPi.Libraries.Recommendations.Core.Enums;
    using EPi.Libraries.Recommendations.Core.Models;

    using EPiServer.Logging;
    using EPiServer.ServiceLocation;

    using Newtonsoft.Json;

    /// <summary>
    /// A wrapper class to invoke Recommendations REST APIs
    /// </summary>
    [ServiceConfiguration(typeof(RecommendationsApiWrapper), Lifecycle = ServiceInstanceScope.Singleton)]
    public class RecommendationsApiWrapper
    {
        private readonly HttpClient httpClient;
        private readonly ILogger log;
        private readonly RecommendationSettingsRepository recommendationSettingsRepository;

        private readonly string baseUri = ConfigurationManager.AppSettings["recommendations:baseuri"];
        private readonly string accountKey = ConfigurationManager.AppSettings["recommendations:accountkey"];

        /// <summary>
        /// Gets the active build identifier.
        /// </summary>
        /// <value>The active build identifier.</value>
        public long ActiveBuildId
        {
            get
            {
                RecommendationSettings settings = this.recommendationSettingsRepository.GetSettingsByName(this.ActiveModel);
                return settings.ActiveBuildId;
            }
        }

        /// <summary>
        /// Gets or sets the active model.
        /// </summary>
        /// <value>The active model.</value>
        public string ActiveModel { get; set; }

        /// <summary>
        /// Constructor that initializes the Http Client.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">The appSetting 'recommendations:accountkey' is empty or not available.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public RecommendationsApiWrapper(ILogger log, RecommendationSettingsRepository recommendationSettingsRepository)
        {
            this.log = log;

            this.recommendationSettingsRepository = recommendationSettingsRepository;

            if (string.IsNullOrWhiteSpace(this.accountKey))
            {
                throw new ConfigurationErrorsException(
                    "The appSetting 'recommendations:accountkey' is empty or not available.");
            }

            if (string.IsNullOrWhiteSpace(this.baseUri))
            {
                this.baseUri = "https://westus.api.cognitive.microsoft.com/recommendations/v4.0";
            }

            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri(this.baseUri),
                Timeout = TimeSpan.FromMinutes(5),
                DefaultRequestHeaders =
                {
                    {"Ocp-Apim-Subscription-Key", this.accountKey}
                }
            };


        }

        /// <summary>
        /// Creates a new model.
        /// </summary>
        /// <param name="modelName">Name for the model</param>
        /// <param name="description">Description for the model</param>
        /// <returns>Model Information.</returns>
        /// <exception cref="HttpRequestException">Failed to create model.</exception>
        public ModelInfo CreateModel(string modelName, string description = null)
        {
            string uri = this.baseUri + "/models/";
            ModelRequestInfo modelRequestInfo = new ModelRequestInfo { ModelName = modelName, Description = description };
            HttpResponseMessage response = this.httpClient.PostAsJsonAsync(uri, modelRequestInfo).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(string.Format("Error {0}: Failed to create model {1}, \n reason {2}",
                    response.StatusCode, modelName, response.ExtractErrorInfo()));
            }

            string jsonString = response.ExtractReponse();
            ModelInfo modelInfo = JsonConvert.DeserializeObject<ModelInfo>(jsonString);
            this.ActiveModel = modelName;

            return modelInfo;
        }

        /// <summary>
        /// Upload catalog items to a model.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="content">The content.</param>
        /// <param name="catalogDisplayName">Name for the catalog</param>
        /// <returns>Statistics about the catalog import operation.</returns>
        /// <exception cref="HttpRequestException">Failed to import model items.</exception>
        public CatalogImportStats UploadCatalog(string modelId, Stream content, string catalogDisplayName)
        {
            return this.UploadCatalog(modelId, new StreamContent(content), catalogDisplayName);
        }

        /// <summary>
        /// Upload catalog items to a model.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="content">The content.</param>
        /// <param name="catalogDisplayName">Name for the catalog</param>
        /// <returns>Statistics about the catalog import operation.</returns>
        /// <exception cref="HttpRequestException">Failed to import model items.</exception>
        /// <exception cref="ArgumentNullException">Content is null. </exception>
        /// <exception cref="EncoderFallbackException">A fallback occurred (see Character Encoding in the .NET Framework for complete explanation)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to <see cref="T:System.Text.EncoderExceptionFallback" />.</exception>
        public CatalogImportStats UploadCatalog(string modelId, string content, string catalogDisplayName)
        {

            //MemoryStream stream = CreateMemoryStream(content);

            //return this.UploadCatalog(modelId, stream, catalogDisplayName);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                return this.UploadCatalog(modelId, stream, catalogDisplayName);
            }
        }

        /// <summary>
        /// Upload catalog items to a model.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="content">The content.</param>
        /// <param name="catalogDisplayName">Name for the catalog</param>
        /// <returns>Statistics about the catalog import operation.</returns>
        /// <exception cref="HttpRequestException">Failed to import model items.</exception>
        public CatalogImportStats UploadCatalog(string modelId, StreamContent content, string catalogDisplayName)
        {
            this.log.Information("[Recommendations] Uploading " + catalogDisplayName + " ...");
            string uri = this.baseUri + "/models/" + modelId + "/catalog?catalogDisplayName=" + catalogDisplayName;
            HttpResponseMessage response = this.httpClient.PostAsync(uri, content).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    string.Format("Error {0}: Failed to import catalog items for model {1} \n reason {2}",
                        response.StatusCode, modelId, response.ExtractErrorInfo()));
            }

            string jsonString = response.ExtractReponse();
            CatalogImportStats catalogImportStats = JsonConvert.DeserializeObject<CatalogImportStats>(jsonString);
            return catalogImportStats;
        }

        /// <summary>
        /// Upload usage data to a model.
        /// Usage files must be less than 200 MB.
        /// If you need to upload more than 200 MB, you may call this function multiple times.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="content">The content.</param>
        /// <param name="usageDisplayName">Name for the usage data being uploaded</param>
        /// <returns>Statistics about the usage upload operation.</returns>
        /// <exception cref="HttpRequestException">Failed to import usage data.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="content" /> is null. </exception>
        /// <exception cref="EncoderFallbackException">A fallback occurred (see Character Encoding in the .NET Framework for complete explanation)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to <see cref="T:System.Text.EncoderExceptionFallback" />.</exception>
        public UsageImportStats UploadUsage(string modelId, string content, string usageDisplayName)
        {
            //MemoryStream stream = CreateMemoryStream(content);

            //return this.UploadUsage(modelId, stream, usageDisplayName);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                return this.UploadUsage(modelId, stream, usageDisplayName);
            }
        }

        /// <summary>
        /// Upload usage data to a model.
        /// Usage files must be less than 200 MB.
        /// If you need to upload more than 200 MB, you may call this function multiple times.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="content">The content.</param>
        /// <param name="usageDisplayName">Name for the usage data being uploaded</param>
        /// <returns>Statistics about the usage upload operation.</returns>
        /// <exception cref="HttpRequestException">Failed to import usage data.</exception>
        /// <exception cref="ArgumentNullException">Uri was not set properly.</exception>
        public UsageImportStats UploadUsage(string modelId, Stream content, string usageDisplayName)
        {
            return this.UploadUsage(modelId, new StreamContent(content), usageDisplayName);
        }

        /// <summary>
        /// Upload usage data to a model.
        /// Usage files must be less than 200 MB.
        /// If you need to upload more than 200 MB, you may call this function multiple times.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="content">The content.</param>
        /// <param name="usageDisplayName">Name for the usage data being uploaded</param>
        /// <returns>Statistics about the usage upload operation.</returns>
        /// <exception cref="HttpRequestException">Failed to import usage data.</exception>
        /// <exception cref="ArgumentNullException">Uri was not set properly.</exception>
        public UsageImportStats UploadUsage(string modelId, StreamContent content, string usageDisplayName)
        {
            this.log.Information("[Recommendations] Uploading " + usageDisplayName + " ...");

            string uri = this.baseUri + "/models/" + modelId + "/usage?usageDisplayName=" + usageDisplayName;

            HttpResponseMessage response = this.httpClient.PostAsync(uri, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(string.Format("Error {0}: Failed to import usage data, for model {1} \n reason {2}", response.StatusCode, modelId, response.ExtractErrorInfo()));
            }

            string jsonString = response.ExtractReponse();
            UsageImportStats usageImportStats = JsonConvert.DeserializeObject<UsageImportStats>(jsonString);
            return usageImportStats;
        }

        /// <summary>
        /// Uploads the usage event.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="content">The content.</param>
        /// <returns>UsageImportStats.</returns>
        /// <exception cref="HttpRequestException">Failed to send usage event.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="content" /> is null. </exception>
        /// <exception cref="EncoderFallbackException">A fallback occurred (see Character Encoding in the .NET Framework for complete explanation)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to <see cref="T:System.Text.EncoderExceptionFallback" />.</exception>
        public void UploadUsageEvent(string modelId, UsageEvent content)
        {
            this.UploadUsageEvent(modelId, content.ToString());
        }

        /// <summary>
        /// Uploads the usage event.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="content">The content.</param>
        /// <returns>UsageImportStats.</returns>
        /// <exception cref="HttpRequestException">Failed to send usage event.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="content" /> is null. </exception>
        /// <exception cref="EncoderFallbackException">A fallback occurred (see Character Encoding in the .NET Framework for complete explanation)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to <see cref="T:System.Text.EncoderExceptionFallback" />.</exception>
        public void UploadUsageEvent(string modelId, string content)
        {
            //MemoryStream stream = CreateMemoryStream(content);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                this.UploadUsageEvent(modelId, stream);
            }
        }

        /// <summary>
        /// Uploads the usage event.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="content">The content.</param>
        /// <returns>UsageImportStats.</returns>
        /// <exception cref="HttpRequestException">Failed to send usage event.</exception>
        /// <exception cref="ArgumentNullException">The Uri was not set properly.</exception>
        public void UploadUsageEvent(string modelId, Stream content)
        {
            this.UploadUsageEvent(modelId, new StreamContent(content));
        }

        /// <summary>
        /// Uploads the usage event.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="content">The content.</param>
        /// <returns>UsageImportStats.</returns>
        /// <exception cref="HttpRequestException">Failed to send usage event.</exception>
        /// <exception cref="ArgumentNullException">The Uri was not set properly.</exception>
        public void UploadUsageEvent(string modelId, StreamContent content)
        {
            this.log.Information("Sending usage event...");

            string uri = this.baseUri + "/models/" + modelId + "/usage/events";
            HttpResponseMessage response = this.httpClient.PostAsync(uri, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(string.Format("Error {0}: Failed to send usage event, for model {1} \n reason {2}", response.StatusCode, modelId, response.ExtractErrorInfo()));
            }
        }


        /// <summary>
        /// Submit a model build, with passed build parameters.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="buildRequestInfo">Build parameters</param>
        /// <param name="operationLocationHeader">Build operation location</param>
        /// <returns>The build id.</returns>
        /// <exception cref="HttpRequestException">Failed to start build for model.</exception>
        public long BuildModel(string modelId, BuildRequestInfo buildRequestInfo, out string operationLocationHeader)
        {
            string uri = this.baseUri + "/models/" + modelId + "/builds";
            HttpResponseMessage response = this.httpClient.PostAsJsonAsync(uri, buildRequestInfo).Result;
            string jsonString = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(string.Format("Error {0}: Failed to start build for model {1}, \n reason {2}",
                    response.StatusCode, modelId, response.ExtractErrorInfo()));
            }

            operationLocationHeader = response.Headers.GetValues("Operation-Location").FirstOrDefault();
            BuildModelResponse buildModelResponse = JsonConvert.DeserializeObject<BuildModelResponse>(jsonString);

            return buildModelResponse.BuildId;
        }

        /// <summary>
        /// Create a business rule.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="content">The content.</param>
        /// <returns>The build id.</returns>
        /// <exception cref="HttpRequestException">Failed to start build for model.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="content" /> is null. </exception>
        /// <exception cref="EncoderFallbackException">A fallback occurred (see Character Encoding in the .NET Framework for complete explanation)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to <see cref="T:System.Text.EncoderExceptionFallback" />.</exception>
        public BusinessRuleInfo CreateBusinessRule(string modelId, string content)
        {
            //MemoryStream stream = CreateMemoryStream(content);
            //return this.CreateBusinessRule(modelId, stream);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                return this.CreateBusinessRule(modelId, stream);
            }
        }

        /// <summary>
        /// Create a business rule.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="content">The content.</param>
        /// <returns>The build id.</returns>
        /// <exception cref="HttpRequestException">Failed to start build for model.</exception>
        public BusinessRuleInfo CreateBusinessRule(string modelId, Stream content)
        {
            return this.CreateBusinessRule(modelId, new StreamContent(content));
        }

        /// <summary>
        /// Create a business rule.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="content">The content.</param>
        /// <returns>The build id.</returns>
        /// <exception cref="HttpRequestException">Failed to start build for model.</exception>
        public BusinessRuleInfo CreateBusinessRule(string modelId, StreamContent content)
        {
            string uri = this.baseUri + "/models/" + modelId + "/rules";
            HttpResponseMessage response = this.httpClient.PostAsync(uri, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(string.Format("Error {0}: Failed to create business rule, for model {1} \n reason {2}", response.StatusCode, modelId, response.ExtractErrorInfo()));
            }

            string jsonString = response.Content.ReadAsStringAsync().Result;
            BusinessRuleInfo businessRuleInfo = JsonConvert.DeserializeObject<BusinessRuleInfo>(jsonString);
            return businessRuleInfo;
        }

        /// <summary>
        /// Create a business rule.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <returns>The build id.</returns>
        /// <exception cref="HttpRequestException">Failed to start build for model.</exception>
        public BusinessRules GetAllBusinessRules(string modelId)
        {
            string uri = this.baseUri + "/models/" + modelId + "/rules";
            HttpResponseMessage response = this.httpClient.GetAsync(uri).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(string.Format("Error {0}: Failed to get business rules, for model {1} \n reason {2}", response.StatusCode, modelId, response.ExtractErrorInfo()));
            }

            string jsonString = response.Content.ReadAsStringAsync().Result;
            BusinessRules businessRules = JsonConvert.DeserializeObject<BusinessRules>(jsonString);
            return businessRules;
        }

        /// <summary>
        /// Delete all business rules.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <returns>The build id.</returns>
        /// <exception cref="HttpRequestException">Failed to start build for model.</exception>
        public void DeleteAllBusinessRules(string modelId)
        {
            string uri = this.baseUri + "/models/" + modelId + "/rules";
            HttpResponseMessage response = this.httpClient.DeleteAsync(uri).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(string.Format("Error {0}: Failed to send usage event, for model {1} \n reason {2}", response.StatusCode, modelId, response.ExtractErrorInfo()));
            }
        }

        /// <summary>
        /// Delete a business rule.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="ruleId">The rule identifier.</param>
        /// <returns>The build id.</returns>
        /// <exception cref="HttpRequestException">Failed to start build for model.</exception>
        public void DeleteBusinessRule(string modelId, string ruleId)
        {
            string uri = this.baseUri + "/models/" + modelId + "/rules/" + ruleId;
            HttpResponseMessage response = this.httpClient.DeleteAsync(uri).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(string.Format("Error {0}: Failed to send usage event, for model {1} \n reason {2}", response.StatusCode, modelId, response.ExtractErrorInfo()));
            }
        }


        /// <summary>
        /// Delete a certain build of a model.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <param name="buildId">Unique identifier of the build</param>
        /// <exception cref="HttpRequestException">Failed to delete build.</exception>
        public void DeleteBuild(string modelId, long buildId)
        {
            string uri = this.baseUri + "/models/" + modelId + "/builds/" + buildId;
            HttpResponseMessage response = this.httpClient.DeleteAsync(uri).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    string.Format("Error {0}: Failed to delete buildId {1} for modelId {2}, \n reason {3}",
                        response.StatusCode, buildId, modelId, response.ExtractErrorInfo()));
            }
        }

        /// <summary>
        /// Delete a model, also the associated catalog/usage data and any builds.
        /// </summary>
        /// <param name="modelId">Unique identifier of the model</param>
        /// <exception cref="HttpRequestException">Failed to delete model.</exception>
        public void DeleteModel(string modelId)
        {
            string uri = this.baseUri + "/models/" + modelId;
            HttpResponseMessage response = this.httpClient.DeleteAsync(uri).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    string.Format("Error {0}: Failed to delete modelId {1}, \n reason {2}",
                        response.StatusCode, modelId, response.ExtractErrorInfo()));
            }
        }

        /// <summary>
        /// Trigger a recommendation build for the given model.
        /// Note: unless configured otherwise the u2i (user to item/user based) recommendations are enabled too.
        /// </summary>
        /// <param name="modelId">the model id</param>
        /// <param name="buildDescription">a description for the build</param>
        /// <param name="enableModelInsights">if set to <c>true</c> [enables modeling insights, selects "LastEventSplitter" as the splitting strategy by default.</param>
        /// <param name="useFeaturesInModel">if set to <c>true</c> [features can be used to enhance the recommendation model].</param>
        /// <param name="allowColdItemPlacement">if set to <c>true</c> [the recommendation should also push cold items via feature similarity.].</param>
        /// <param name="modelingFeatureList">comma-separated list of feature names to be used in the recommendation build to enhance the recommendation.</param>
        /// <param name="operationLocationHeader">operation location header, can be used to cancel the build operation and to get status.</param>
        /// <returns>Unique indentifier of the build initiated.</returns>
        /// <exception cref="HttpRequestException">Failed to start build for model.</exception>
        public long CreateRecommendationsBuild(string modelId,
                                               string buildDescription,
                                               bool enableModelInsights,
                                               bool useFeaturesInModel,
                                               bool allowColdItemPlacement,
                                               string modelingFeatureList,
                                               out string operationLocationHeader)
        {
            // only used if splitter strategy is set to RandomSplitter
            RandomSplitterParameters randomSplitterParameters = new RandomSplitterParameters()
            {
                RandomSeed = 0,
                TestPercent = 10
            };

            RecommendationBuildParameters parameters = new RecommendationBuildParameters()
            {
                NumberOfModelIterations = 10,
                NumberOfModelDimensions = 20,
                ItemCutOffLowerBound = 1,
                EnableModelingInsights = enableModelInsights,
                SplitterStrategy = SplitterStrategy.LastEventSplitter,
                RandomSplitterParameters = randomSplitterParameters,
                EnableU2I = true,
                UseFeaturesInModel = useFeaturesInModel,
                AllowColdItemPlacement = allowColdItemPlacement,
                ModelingFeatureList = modelingFeatureList
            };

            BuildRequestInfo requestInfo = new BuildRequestInfo()
            {
                BuildType = BuildType.Recommendation,
                BuildParameters = new BuildParameters()
                {
                    Recommendation = parameters
                },
                Description = buildDescription
            };

            return this.BuildModel(modelId, requestInfo, out operationLocationHeader);
        }

        /// <summary>
        /// Trigger a recommendation build for the given model.
        /// Note: unless configured otherwise the u2i (user to item/user based) recommendations are enabled too.
        /// </summary>
        /// <param name="modelId">the model id</param>
        /// <param name="buildDescription">a description for the build.</param>
        /// <param name="enableModelInsights">if set to <c>true</c> [enable model insights].</param>
        /// <param name="operationLocationHeader">operation location header, can be used to cancel the build operation and to get status.</param>
        /// <returns>Unique indentifier of the build initiated.</returns>
        /// <exception cref="HttpRequestException">Failed to start build for model.</exception>
        public long CreateFbtBuild(string modelId, string buildDescription, bool enableModelInsights, out string operationLocationHeader)
        {

            // only used if splitter strategy is set to RandomSplitter
            RandomSplitterParameters randomSplitterParameters = new RandomSplitterParameters()
            {
                RandomSeed = 0,
                TestPercent = 10
            };

            FbtBuildParameters parameters = new FbtBuildParameters()
            {
                MinimalScore = 0,
                SimilarityFunction = FbtSimilarityFunction.Lift,
                SupportThreshold = 3,
                MaxItemSetSize = 2,
                EnableModelingInsights = enableModelInsights,
                SplitterStrategy = SplitterStrategy.LastEventSplitter,
                RandomSplitterParameters = randomSplitterParameters
            };

            BuildRequestInfo requestInfo = new BuildRequestInfo()
            {
                BuildType = BuildType.Fbt,
                BuildParameters = new BuildParameters()
                {
                    Fbt = parameters,
                },
                Description = buildDescription
            };

            return this.BuildModel(modelId, requestInfo, out operationLocationHeader);
        }

        /// <summary>
        /// Monitor operation status and wait for completion.
        /// </summary>
        /// <param name="operationId">The operation id</param>
        /// <returns>Build status</returns>
        public OperationInfo<T> WaitForOperationCompletion<T>(string operationId)
        {
            OperationInfo<T> operationInfo;

            string uri = this.baseUri + "/operations";

            while (true)
            {
                HttpResponseMessage response = this.httpClient.GetAsync(uri + "/" + operationId).Result;
                string jsonString = response.Content.ReadAsStringAsync().Result;
                operationInfo = JsonConvert.DeserializeObject<OperationInfo<T>>(jsonString);

                // Operation status {NotStarted, Running, Cancelling, Cancelled, Succeded, Failed}
                this.log.Information("[Recommendations] Operation Status: {0}. \t Will check again in 10 seconds.", operationInfo.Status);

                if (OperationStatus.Succeeded.ToString().Equals(operationInfo.Status) ||
                    OperationStatus.Failed.ToString().Equals(operationInfo.Status) ||
                    OperationStatus.Cancelled.ToString().Equals(operationInfo.Status))
                {
                    break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(10));
            }

            return operationInfo;
        }

        /// <summary>
        /// Extract the operation id from the operation header
        /// </summary>
        /// <param name="operationLocationHeader"></param>
        /// <returns></returns>
        public static string GetOperationId(string operationLocationHeader)
        {
            int index = operationLocationHeader.LastIndexOf('/');
            string operationId = operationLocationHeader.Substring(index + 1);
            return operationId;
        }

        /// <summary>
        /// Set an active build for the model.
        /// </summary>
        /// <param name="modelId">Unique idenfier of the model</param>
        /// <param name="updateActiveBuildInfo">The update active build information.</param>
        /// <exception cref="HttpRequestException">Failed to set active build.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void SetActiveBuild(string modelId, UpdateActiveBuildInfo updateActiveBuildInfo)
        {
            string uri = this.baseUri + "/models/" + modelId;
            using (
                ObjectContent<UpdateActiveBuildInfo> content =
                    new ObjectContent<UpdateActiveBuildInfo>(updateActiveBuildInfo, new JsonMediaTypeFormatter()))
            {

                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri) { Content = content };
                HttpResponseMessage response = this.httpClient.SendAsync(request).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        string.Format("Error {0}: Failed to set active build for modelId {1}, \n reason {2}",
                            response.StatusCode, modelId, response.ExtractErrorInfo()));
                }

                RecommendationSettings settings = this.recommendationSettingsRepository.GetSettingsByName(this.ActiveModel);
                settings.ActiveBuildId = updateActiveBuildInfo.ActiveBuildId;
                this.recommendationSettingsRepository.Save(settings);
            }
        }

        /// <summary>
        /// Get Item to Item (I2I) Recommendations or Frequently-Bought-Together (FBT) recommendations
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="itemIds">The item ids.</param>
        /// <param name="numberOfResults">The number of results.</param>
        /// <returns>The recommendation sets. Note that I2I builds will only return one item per set.
        /// FBT builds will return more than one item per set.</returns>
        /// <exception cref="HttpRequestException">Failed to get recommendations.</exception>
        public RecommendedItemSetInfoList GetRecommendations(string modelId, string itemIds, int numberOfResults)
        {
            return this.GetRecommendations(modelId, this.ActiveBuildId, itemIds, numberOfResults);
        }

        /// <summary>
        /// Get Item to Item (I2I) Recommendations or Frequently-Bought-Together (FBT) recommendations
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="buildId">The build identifier.</param>
        /// <param name="itemIds">The item ids.</param>
        /// <param name="numberOfResults">The number of results.</param>
        /// <returns>The recommendation sets. Note that I2I builds will only return one item per set.
        /// FBT builds will return more than one item per set.</returns>
        /// <exception cref="HttpRequestException">Failed to get recommendations.</exception>
        public RecommendedItemSetInfoList GetRecommendations(string modelId, long buildId, string itemIds, int numberOfResults)
        {
            string uri = this.baseUri + "/models/" + modelId + "/recommend/item?itemIds=" + itemIds + "&numberOfResults=" + numberOfResults + "&minimalScore=0";
            HttpResponseMessage response = this.httpClient.GetAsync(uri).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    string.Format("Error {0}: Failed to get recommendations for modelId {1}, buildId {2}, Reason: {3}",
                    response.StatusCode, modelId, buildId, response.ExtractErrorInfo()));
            }

            string jsonString = response.Content.ReadAsStringAsync().Result;
            RecommendedItemSetInfoList recommendedItemSetInfoList = JsonConvert.DeserializeObject<RecommendedItemSetInfoList>(jsonString);
            return recommendedItemSetInfoList;
        }

        /// <summary>
        /// Use historical transaction data to provide personalized recommendations for a user.
        /// The user history is extracted from the usage files used to train the model.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="numberOfResults">Desired number of recommendation results.</param>
        /// <returns>The recommendations for the user.</returns>
        /// <exception cref="HttpRequestException">Failed to get user recommendations.</exception>
        public RecommendedItemSetInfoList GetUserRecommendations(string modelId, string userId, int numberOfResults)
        {
            return this.GetUserRecommendations(modelId, this.ActiveBuildId, userId, numberOfResults);
        }

        /// <summary>
        /// Use historical transaction data to provide personalized recommendations for a user.
        /// The user history is extracted from the usage files used to train the model.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="buildId">The build identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="numberOfResults">Desired number of recommendation results.</param>
        /// <returns>The recommendations for the user.</returns>
        /// <exception cref="HttpRequestException">Failed to get user recommendations.</exception>
        public RecommendedItemSetInfoList GetUserRecommendations(string modelId, long buildId, string userId, int numberOfResults)
        {
            string uri = this.baseUri + "/models/" + modelId + "/recommend/user?userId=" + userId + "&numberOfResults=" + numberOfResults;
            HttpResponseMessage response = this.httpClient.GetAsync(uri).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    string.Format("Error {0}: Failed to get user recommendations for modelId {1}, buildId {2}, Reason: {3}",
                    response.StatusCode, modelId, buildId, response.ExtractErrorInfo()));
            }

            string jsonString = response.Content.ReadAsStringAsync().Result;
            RecommendedItemSetInfoList recommendedItemSetInfoList = JsonConvert.DeserializeObject<RecommendedItemSetInfoList>(jsonString);
            return recommendedItemSetInfoList;
        }

        /// <summary>
        /// Update model information
        /// </summary>
        /// <param name="modelId">the id of the model</param>
        /// <param name="activeBuildId">the id of the build to be active (optional)</param>
        /// <exception cref="HttpRequestException">Failed to set active build.</exception>
        public void SetActiveBuild(string modelId, long activeBuildId)
        {
            UpdateActiveBuildInfo info = new UpdateActiveBuildInfo()
            {
                ActiveBuildId = activeBuildId
            };

            this.SetActiveBuild(modelId, info);
        }



    }
}
