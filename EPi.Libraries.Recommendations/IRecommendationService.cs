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

    using EPi.Libraries.Recommendations.Core.Enums;
    using EPi.Libraries.Recommendations.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;

    /// <summary>
    /// Interface IRecommendationService
    /// </summary>
    public interface IRecommendationService
    {
        /// <summary>
        /// Gets the catalog items.
        /// </summary>
        /// <param name="since">Catalog items since</param>
        /// <returns>List&lt;CatalogItem&gt;.</returns>
        List<CatalogItem> GetCatalogItems(DateTime since);

        /// <summary>
        /// Gets the item recommendations.
        /// </summary>
        /// <param name="itemIds">Comma-separated list of the items to recommend for. Max length: 1024.</param>
        /// <returns>List&lt;EntryContentBase&gt;.</returns>
        List<EntryContentBase> GetItemRecommendations(string itemIds);

        /// <summary>
        /// Gets the item recommendations.
        /// </summary>
        /// <param name="itemIds">Comma-separated list of the items to recommend for. Max length: 1024.</param>
        /// <param name="numberOfResults">Number of recommended items to return.</param>
        /// <returns>List&lt;EntryContentBase&gt;.</returns>
        List<EntryContentBase> GetItemRecommendations(string itemIds, int numberOfResults);

        /// <summary>
        /// Gets the usage items.
        /// </summary>
        /// <param name="since">Usage items since.</param>
        /// <returns>List&lt;UsageItem&gt;.</returns>
        List<UsageItem> GetUsageItems(DateTime since);

        /// <summary>
        /// Gets the user recommendations.
        /// </summary>
        /// <returns>List&lt;EntryContentBase&gt;.</returns>
        List<EntryContentBase> GetUserRecommendations();

        /// <summary>
        /// Gets the user recommendations.
        /// </summary>
        /// <param name="numberOfResults">Number of recommended items to return.</param>
        /// <returns>List&lt;EntryContentBase&gt;.</returns>
        List<EntryContentBase> GetUserRecommendations(int numberOfResults);

        /// <summary>
        /// Sends the usage event.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="code">The code.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="eventType">Type of the event.</param>
        void SendUsageEvent(int quantity, string code, decimal unitPrice, EventType eventType);

        /// <summary>
        /// Sends the usage event.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="code">The code.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="eventType">Type of the event.</param>
        void SendUsageEvent(decimal quantity, string code, decimal unitPrice, EventType eventType);
    }
}