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
namespace EPi.Libraries.Recommendations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;

    using EPi.Libraries.Recommendations.Core.DataContracts;
    using EPi.Libraries.Recommendations.Core.Enums;
    using EPi.Libraries.Recommendations.Core.Infrastructure;
    using EPi.Libraries.Recommendations.Core.Models;

    using EPiServer;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Commerce.Order;
    using EPiServer.Core;
    using EPiServer.Logging;
    using EPiServer.Security;
    using EPiServer.ServiceLocation;

    using Mediachase.Commerce.Catalog;
    using Mediachase.Commerce.Orders;
    using Mediachase.Commerce.Security;

    /// <summary>
    /// Class RecommendationService.
    /// </summary>
    /// <seealso cref="EPi.Libraries.Recommendations.IRecommendationService" />
    /// <seealso cref="IRecommendationService" />
    /// <author>Jeroen Stemerdink</author>
    public abstract class RecommendationService : IRecommendationService
    {
        /// <summary>
        /// The content loader
        /// </summary>
        protected readonly IContentLoader ContentLoader;

        /// <summary>
        /// The order repository
        /// </summary>
        protected readonly IOrderRepository OrderRepository;

        /// <summary>
        /// The log
        /// </summary>
        protected readonly ILogger Log;

        /// <summary>
        /// The reference converter
        /// </summary>
        protected readonly ReferenceConverter ReferenceConverter;
        /// <summary>
        /// The recommendations API wrapper
        /// </summary>
        protected readonly RecommendationsApiWrapper RecommendationsApiWrapper;

        /// <summary>
        /// The recommendation settings repository
        /// </summary>
        protected readonly RecommendationSettingsRepository RecommendationSettingsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="recommendationsApiWrapper">The wrapper for the recomendations api.</param>
        /// <param name="recommendationSettingsRepository">The recomendation settings.</param>
        /// <param name="contentLoader">The content loader.</param>
        /// <param name="referenceConverter">The reference converter.</param>
        /// <param name="orderRepository">The order repository.</param>
        /// <param name="log">The log.</param>
        protected RecommendationService(
            RecommendationsApiWrapper recommendationsApiWrapper,
            RecommendationSettingsRepository recommendationSettingsRepository,
            IContentLoader contentLoader,
            ReferenceConverter referenceConverter,
            IOrderRepository orderRepository,
            ILogger log)
        {
            this.RecommendationsApiWrapper = recommendationsApiWrapper;
            this.RecommendationSettingsRepository = recommendationSettingsRepository;
            this.ContentLoader = contentLoader;
            this.ReferenceConverter = referenceConverter;
            this.OrderRepository = orderRepository;
            this.Log = log;
        }

        /// <summary>
        /// Gets the usage items.
        /// </summary>
        /// <param name="since">Usage items since.</param>
        /// <returns>List&lt;UsageItem&gt;.</returns>
        public virtual List<UsageItem> GetUsageItems(DateTime since)
        {
            List<PurchaseOrder> orders = OrderContext.Current.FindPurchaseOrdersByStatus(OrderStatus.InProgress, OrderStatus.Completed, OrderStatus.OnHold, OrderStatus.AwaitingExchange, OrderStatus.PartiallyShipped).Where(po => po.Created > since).ToList();

            IEnumerable<UsageItem> usageItems =
                from purchaseOrder in orders
                from lineItem in purchaseOrder.GetAllLineItems()
                select new UsageItem { UserID = purchaseOrder.CustomerId.ToString(), ItemID = lineItem.Code, EventDate = purchaseOrder.Created, EventType = EventType.Purchase };

            return usageItems.ToList();
        }

        /// <summary>
        /// Gets the catalog items.
        /// </summary>
        /// <param name="since">Catalog items since</param>
        /// <returns>List&lt;CatalogItem&gt;.</returns>
        public virtual List<CatalogItem> GetCatalogItems(DateTime since)
        {
            List<CatalogItem> catalogItems = new List<CatalogItem>();

            IEnumerable<ContentReference> descendents =
                this.ContentLoader.GetDescendents(this.ReferenceConverter.GetRootLink());

            foreach (ContentReference contentReference in descendents)
            {
                VariationContent variation;

                if (!this.ContentLoader.TryGet(contentReference, out variation))
                {
                    continue;
                }

                if (variation.Created < since)
                {
                    continue;
                }

                NodeContent node =
                    this.ContentLoader.GetAncestors(contentReference).OfType<NodeContent>().FirstOrDefault();

                CatalogItem catalogItem = new CatalogItem
                                              {
                                                  ItemID = variation.Code,
                                                  ItemName = variation.Name,
                                                  ProductCategory = node == null ? "undefined" : node.Name,
                                                  Description = string.Empty,
                                                  Features = new Dictionary<string, string>()
                                              };

                catalogItems.Add(catalogItem);
            }

            return catalogItems;
        }

        /// <summary>
        /// Sends the usage event.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="code">The code.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="eventType">Type of the event.</param>
        public virtual void SendUsageEvent(int quantity, string code, decimal unitPrice, EventType eventType)
        {
            UsageEvent usageEvent = Helpers.CreateUsageEventContent(quantity, code, unitPrice, eventType);
            this.SendUsageEvent(usageEvent);
        }

        /// <summary>
        /// Sends the usage event.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="code">The code.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="eventType">Type of the event.</param>
        public virtual void SendUsageEvent(decimal quantity, string code, decimal unitPrice, EventType eventType)
        {
            UsageEvent usageEvent = Helpers.CreateUsageEventContent(quantity, code, unitPrice, eventType);
            this.SendUsageEvent(usageEvent);
        }

        /// <summary>
        /// Gets the item to item recommendations. Returns 6.
        /// </summary>
        /// <param name="itemIds">Comma-separated list of the items to recommend for. Max length: 1024</param>
        /// <returns>List&lt;EntryContentBase&gt;.</returns>
        public virtual List<EntryContentBase> GetItemRecommendations(string itemIds)
        {
            return this.GetItemRecommendations(itemIds, 6);
        }

        /// <summary>
        /// Gets the item to item recommendations.
        /// </summary>
        /// <param name="itemIds">Comma-separated list of the items to recommend for. Max length: 1024</param>
        /// <param name="numberOfResults">The number of results.</param>
        /// <returns>List&lt;EntryContentBase&gt;.</returns>
        public virtual List<EntryContentBase> GetItemRecommendations(string itemIds, int numberOfResults)
        {
            List<EntryContentBase> recommendations = new List<EntryContentBase>();
            RecommendationSettings settings = this.RecommendationSettingsRepository.GetDefaultSettings();

            this.Log.Information("[Recommendations] Getting item to item recommendations for ids: {0}", itemIds);

            try
            {
                RecommendedItemSetInfoList itemSets = this.RecommendationsApiWrapper.GetRecommendations(settings.ModelId, -1, itemIds, numberOfResults);

                if (itemSets.RecommendedItemSetInfo != null)
                {
                    foreach (RecommendedItemSetInfo recoSet in itemSets.RecommendedItemSetInfo.OrderByDescending( s => s.Rating))
                    {
                        recommendations.AddRange(recoSet.Items.Select(item => this.GetEntryByCode(item.Id)).Where(entry => entry != null));
                    }
                }
                else
                {
                    this.Log.Information("[Recommendations] No item to item recommendations found for ids: {0}", itemIds);
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                this.Log.Error(string.Format(CultureInfo.InvariantCulture, "[Recommendations] Error getting item to item recommendations for ids: {0}", itemIds), httpRequestException);
            }
            catch (ArgumentNullException argumentNullException)
            {
                this.Log.Error(string.Format(CultureInfo.InvariantCulture, "[Recommendations] Error getting item to item recommendations for ids: {0}", itemIds), argumentNullException);
            }

            return recommendations;
        }

        /// <summary>
        /// Gets the user recommendations. Returns 6
        /// </summary>
        /// <returns>List&lt;EntryContentBase&gt;.</returns>
        public virtual List<EntryContentBase> GetUserRecommendations()
        {
            return this.GetUserRecommendations(6);
        }

        /// <summary>
        /// Gets the user recommendations.
        /// </summary>
        /// <param name="numberOfResults">Number of recommended items to return.</param>
        /// <returns>List&lt;EntryContentBase&gt;.</returns>
        public virtual List<EntryContentBase> GetUserRecommendations(int numberOfResults)
        {
            List<EntryContentBase> recommendations = new List<EntryContentBase>();

            RecommendationSettings settings = this.RecommendationSettingsRepository.GetDefaultSettings();
            string userId = PrincipalInfo.CurrentPrincipal.GetContactId().ToString();

            this.Log.Information("[Recommendations] Getting user recommendations for user: {0}", userId);

            try
            {
                RecommendedItemSetInfoList itemSets = this.RecommendationsApiWrapper.GetUserRecommendations(settings.ModelId, -1, userId, numberOfResults);

                if (itemSets.RecommendedItemSetInfo != null)
                {
                    foreach (RecommendedItemSetInfo recoSet in itemSets.RecommendedItemSetInfo.OrderByDescending(s => s.Rating))
                    {
                        recommendations.AddRange(recoSet.Items.Select(item => this.GetEntryByCode(item.Id)).Where(entry => entry != null));
                    }
                }
                else
                {
                    this.Log.Information("[Recommendations] No recommendations found for user: {0}", userId);
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                this.Log.Error(string.Format(CultureInfo.InvariantCulture, "[Recommendations] Error getting user recommendations for user: {0}", userId), httpRequestException);
            }
            catch (ArgumentNullException argumentNullException)
            {
                this.Log.Error(string.Format(CultureInfo.InvariantCulture, "[Recommendations] Error getting user recommendations for user: {0}", userId), argumentNullException);
            }

            return recommendations;
        }

        /// <summary>
        /// Gets the entry by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>EntryContentBase.</returns>
        protected EntryContentBase GetEntryByCode(string code)
        {
            // Get the ContentReference for the recommendation
            ContentReference contentReference = this.ReferenceConverter.GetContentLink(code);

            // Try to get the entry
            EntryContentBase entry;
            return this.ContentLoader.TryGet(contentReference, out entry) ? entry : null;
        }

        /// <summary>
        /// Sends the usage event.
        /// </summary>
        /// <param name="usageEvent">The usage event.</param>
        protected void SendUsageEvent(UsageEvent usageEvent)
        {
            RecommendationSettings settings = this.RecommendationSettingsRepository.GetDefaultSettings();

            try
            {
                this.RecommendationsApiWrapper.UploadUsageEvent(settings.ModelId, usageEvent);
            }
            catch (HttpRequestException httpRequestException)
            {
                this.Log.Error("[Recommendations] Error sending usage event.", httpRequestException);
            }
        }
    }
}