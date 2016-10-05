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
namespace EPi.Libraries.Recommendations.Core.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    using EPi.Libraries.Recommendations.Core.DataContracts;
    using EPi.Libraries.Recommendations.Core.Enums;
    using EPi.Libraries.Recommendations.Core.Models;

    using EPiServer.Logging;
    using EPiServer.Security;
    using EPiServer.ServiceLocation;

    using Mediachase.Commerce.Orders;
    using Mediachase.Commerce.Security;

    using Newtonsoft.Json;

    /// <summary>
    /// Class Helpers.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    public static class Helpers
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILogger Log = LogManager.GetLogger();

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
        /// Creates the content of the catalog.
        /// </summary>
        /// <param name="catalogItems">The catalog items.</param>
        /// <returns>System.String.</returns>
        public static string CreateCatalogContent(this List<CatalogItem> catalogItems)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (CatalogItem catalogItem in catalogItems)
            {
                try
                {
                    stringBuilder.AppendLine(catalogItem.ToString());
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }

            string catalogContent = stringBuilder.ToString();

            return catalogContent;
        }

        /// <summary>
        /// Creates the content of the usage.
        /// </summary>
        /// <param name="usageItems">The usage items.</param>
        /// <returns>System.String.</returns>
        public static string CreateUsageContent(this List<UsageItem> usageItems)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (UsageItem usageItem in usageItems)
            {
                try
                {
                    stringBuilder.AppendLine(usageItem.ToString());
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }

            string usageContent = stringBuilder.ToString();

            return usageContent;
        }

        /// <summary>
        /// Creates the content of the usage.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>System.String.</returns>
        public static string CreateUsageContent(this LineItem lineItem, EventType eventType)
        {
            return lineItem.CreateUsageContent(PrincipalInfo.CurrentPrincipal.GetContactId().ToString(), eventType);
        }

        /// <summary>
        /// Creates the content of the usage.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>System.String.</returns>
        public static string CreateUsageContent(this LineItem lineItem, string userId, EventType eventType)
        {
            UsageItem usageItem = new UsageItem
                                      {
                                          EventDate = DateTime.Now,
                                          ItemID = lineItem.Code,
                                          UserID = userId,
                                          EventType = eventType
                                      };

            return usageItem.ToString();
        }

        /// <summary>
        /// Creates the content of the usage event.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="code">The code.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>UsageEvent.</returns>
        public static UsageEvent CreateUsageEventContent(
            int quantity,
            string code,
            decimal unitPrice,
            EventType eventType)
        {

            return CreateUsageEventContent(
                PrincipalInfo.CurrentPrincipal.GetContactId().ToString(),
                quantity,
                code,
                unitPrice,
                eventType);
        }

        /// <summary>
        /// Creates the content of the usage event.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="code">The code.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>UsageEvent.</returns>
        public static UsageEvent CreateUsageEventContent(
            decimal quantity,
            string code,
            decimal unitPrice,
            EventType eventType)
        {
            try
            {
                return CreateUsageEventContent(
                    PrincipalInfo.CurrentPrincipal.GetContactId().ToString(),
                    decimal.ToInt32(quantity),
                    code,
                    unitPrice,
                    eventType);
            }
            catch (OverflowException)
            {
                return CreateUsageEventContent(1, code, unitPrice, eventType);
            }
        }

        /// <summary>
        /// Creates the content of the usage event.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="code">The code.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>UsageEvent.</returns>
        public static UsageEvent CreateUsageEventContent(
            string userId,
            int quantity,
            string code,
            decimal unitPrice,
            EventType eventType)
        {
            const string DateTimeFormat = "yyyy/MM/ddTHH:mm:ss";
            string forWebservice = DateTime.Now.ToString(DateTimeFormat, CultureInfo.InvariantCulture);

            UsageEvent usageEvent = new UsageEvent
                                        {
                                            BuildId = -1,
                                            UserId = userId,
                                            Events =
                                                new[]
                                                    {
                                                        new Event
                                                            {
                                                                Count = quantity,
                                                                EventType = eventType,
                                                                ItemId = code,
                                                                Timestamp = forWebservice,
                                                                UnitPrice = decimal.ToSingle(unitPrice)
                                                            }
                                                    }
                                        };

            return usageEvent;
        }



        /// <summary>
        /// Gets the feature list.
        /// </summary>
        /// <param name="catalogItems">The catalog items.</param>
        /// <returns>System.String.</returns>
        public static string GetFeatureList(this List<CatalogItem> catalogItems)
        {
            Dictionary<string, string> mergedDictionary = new Dictionary<string, string>();

            try
            {
                mergedDictionary = catalogItems.Aggregate(
                    mergedDictionary,
                    (current, catalogItem) =>
                    current.Concat(catalogItem.Features)
                        .GroupBy(d => d.Key)
                        .ToDictionary(d => d.Key, d => d.First().Value));

                return string.Join(",", mergedDictionary.Keys);
            }
            catch (ArgumentNullException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Extract error info
        /// </summary>
        /// <returns>System.String.</returns>
        public static ErrorInfo ExtractErrorInfo(this HttpResponseMessage response)
        {
            ErrorInfo errorInfo = null;

            string detailedReason = null;
            if (response.Content != null)
            {
                detailedReason = response.Content.ReadAsStringAsync().Result;
            }

            errorInfo = detailedReason != null
                            ? JsonConvert.DeserializeObject<ErrorInfo>(detailedReason)
                            : new ErrorInfo
                                  {
                                      Error =
                                          new Error
                                              {
                                                  Code = string.Empty,
                                                  InnerError = null,
                                                  Message = response.ReasonPhrase
                                              }
                                  };

            return errorInfo;
        }

        /// <summary>
        /// Extract error information from HTTP response message.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="HttpRequestException">Error extracting response.</exception>
        public static string ExtractReponse(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }

            string detailedReason = null;
            if (response.Content != null)
            {
                detailedReason = response.Content.ReadAsStringAsync().Result;
            }

            string errorMsg = detailedReason == null
                                  ? response.ReasonPhrase
                                  : response.ReasonPhrase + "->" + detailedReason;

            string error = string.Format(
                "Status code: {0}\nDetail information: {1}",
                (int)response.StatusCode,
                errorMsg);
            throw new HttpRequestException("Response: " + error);
        }

        /// <summary>
        /// Gets the recommendation settings.
        /// </summary>
        /// <returns>RecommendationSettings.</returns>
        /// <exception cref="HttpRequestException">Failed to get all models.</exception>
        /// <exception cref="ArgumentNullException">No model found.</exception>
        /// <exception cref="ArgumentException">No model found.</exception>
        public static RecommendationSettings GetRecommendationSettings()
        {
            string modelName = RecommendationSettingsRepository.GetModelName();
            string catalogDisplayName = RecommendationSettingsRepository.GetCatalogDisplayName();

            RecommendationSettings settings = RecommendationSettingsRepository.GetSettingsByName(modelName);

            if (!string.IsNullOrEmpty(settings.ModelId))
            {
                return settings;
            }

            try
            {
                Log.Information("[Recommendations] Creating a new model {0}...", modelName);

                ModelInfo modelInfo = Recommender.CreateModel(modelName, catalogDisplayName);

                Log.Information("[Recommendations] Model '{0}' created with ID: {1}", modelName, modelInfo.Id);

                settings.ModelId = modelInfo.Id;
            }
            catch (HttpRequestException)
            {
                // In case the settings are not correct, an error will be thrown as the model already exists. In that case, find the model, and update the settings
                ModelInfo modelInfo = Recommender.FindModel(modelName);

                if (modelInfo != null)
                {
                    settings.ModelId = modelInfo.Id;
                    settings.ActiveBuildId = modelInfo.ActiveBuildId;
                }
            }

            settings.ModelName = modelName;
            settings.CatalogDisplayName = catalogDisplayName;

            RecommendationSettingsRepository.Save(settings);

            return settings;
        }
    }
}