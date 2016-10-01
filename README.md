# EPi.Libraries.Recommendations

By Jeroen Stemerdink


[![GitHub version](https://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Recommendations.svg)](http://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Recommendations)
[![Semver](http://img.shields.io/SemVer/2.0.0.png)](http://semver.org/spec/v2.0.0.html)
[![Platform](https://img.shields.io/badge/platform-.NET 4.5.2-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/EPiServer-%209.12.0-orange.svg?style=flat)](http://world.episerver.com/cms/)
[![Issue Count](https://codeclimate.com/github/jstemerdink/EPi.Libraries.Recommendations/badges/issue_count.svg)](https://codeclimate.com/github/jstemerdink/EPi.Libraries.Recommendations)
[![Stories in Backlog](https://badge.waffle.io/jstemerdink/EPi.Libraries.Recommendations.svg?label=enhancement&title=Backlog)](http://waffle.io/jstemerdink/EPi.Libraries.Recommendations)

## About
Use the Microsoft Cognitive Services Recommendations API with EPiServer Commerce

## Parts

[Core](EPi.Libraries.Recommendations/README.md)

[Export catalog](EPi.Libraries.Recommendations.CatalogExportJob/README.md)

[Export usage stats](EPi.Libraries.Recommendations.UsageExportJob/README.md)

## Setings
Add an account key to your appsettings: ```<add key="recommendations:accountkey" value="YourKey" />```

If you want to use the 'Frequently Bought Together build' change the key in the appSettings ```<add key="recommendations:useftbbuild" value="true" />```

If the baseuri for the API changes, update the key in the appSettings ```<add key="recommendations:baseuri" value="https://westus.api.cognitive.microsoft.com/recommendations/v4.0" />```

If you want to use a different model name, update the key in the appSettings ```<add key="recommendations:modelname" value="EPiServerCommerce" />```

If you want to use a different display name for the catalog, update the key in the appSettings ```<add key="recommendations:catalogdisplayname" value="EPiServer Commerce catalog" />```

If you want to use a different display name for the usages, update the key in the appSettings ```<add key="recommendations:usagedisplayname" value="EPiServer Commerce catalog usages" />```

## Implementation

As all commerce setups differ you will need to implement your own version of the recommendation service. Probably you will only need to override the GetCatalogItems.
Below you find an example for QuickSilver.

```csharp

    [ServiceConfiguration(typeof(IRecommendationService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class QuicksilverRecommendationService : RecommendationService
    {
        public QuicksilverRecommendationService(RecommendationsApiWrapper recommendationsApiWrapper, RecommendationSettingsRepository recommendationSettingsRepository, IContentLoader contentLoader, ReferenceConverter referenceConverter, IOrderRepository orderRepository, ILogger log )
            : base(recommendationsApiWrapper, recommendationSettingsRepository, contentLoader, referenceConverter, orderRepository, log)
        {
        }

        public override List<UsageItem> GetUsageItems(DateTime since)
        {
            List<PurchaseOrder> orders = OrderContext.Current.FindPurchaseOrdersByStatus(OrderStatus.InProgress, OrderStatus.Completed, OrderStatus.OnHold, OrderStatus.AwaitingExchange, OrderStatus.PartiallyShipped).Where(po => po.Created > since).ToList();
            
            IEnumerable<UsageItem> usageItems =
                from purchaseOrder in orders
                from lineItem in purchaseOrder.GetAllLineItems()
                select new UsageItem { UserID = purchaseOrder.CustomerId.ToString(), ItemID = lineItem.Code, EventDate = purchaseOrder.Created, EventType = EventType.Purchase };

            return usageItems.ToList();
        }

        public override List<CatalogItem> GetCatalogItems(DateTime since)
        {
            List<CatalogItem> catalogItems = new List<CatalogItem>();

            IEnumerable<ContentReference> descendents =
                this.ContentLoader.GetDescendents(this.ReferenceConverter.GetRootLink());

            foreach (ContentReference contentReference in descendents)
            {
                FashionVariant variation;

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
                                                  Features = new Dictionary<string, string>() { { "color", variation.Color }, { "size", variation.Size } }
                                              };

                catalogItems.Add(catalogItem);
            }

            return catalogItems;
        }

        public override void SendUsageEvent(int quantity, string code, decimal unitPrice, EventType eventType)
        {
            UsageEvent usageEvent = Helpers.CreateUsageEventContent(quantity, code, unitPrice, eventType);
            this.SendUsageEvent(usageEvent);
        }
    }
}
```

You can override some more methods if you need to, this is the full list:
```csharp
        List<CatalogItem> GetCatalogItems(DateTime since);
        List<EntryContentBase> GetItemRecommendations(string itemIds);
        List<T> GetItemRecommendations<T>(string itemIds);
        List<EntryContentBase> GetItemRecommendations(string itemIds, int numberOfResults);
        List<T> GetItemRecommendations<T>(string itemIds, int numberOfResults);
        List<UsageItem> GetUsageItems(DateTime since);
        List<EntryContentBase> GetUserRecommendations();
        List<T> GetUserRecommendations<T>();
        List<EntryContentBase> GetUserRecommendations(int numberOfResults);
        List<T> GetUserRecommendations<T>(int numberOfResults);
        void SendUsageEvent(int quantity, string code, decimal unitPrice, EventType eventType);
        void SendUsageEvent(decimal quantity, string code, decimal unitPrice, EventType eventType);
```

> *Powered by ReSharper*

> [![image](http://resources.jetbrains.com/assets/media/open-graph/jetbrains_250x250.png)](http://jetbrains.com)

