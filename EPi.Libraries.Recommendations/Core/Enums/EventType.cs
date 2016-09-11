namespace EPi.Libraries.Recommendations.Core.Enums
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enum EventType
    /// </summary>
    [DataContract]
    public enum EventType
    {
        /// <summary>
        /// Clicked a product link
        /// </summary>
        Click = 1,

        /// <summary>
        /// Clicked recommendation
        /// </summary>
        RecommendationClick = 2,

        /// <summary>
        /// Added item to shopping cart
        /// </summary>
        AddShopCart = 3,

        /// <summary>
        /// Item removed from shopping cart
        /// </summary>
        RemoveShopCart = 4,

        /// <summary>
        /// Purchase made
        /// </summary>
        Purchase = 5
    }
}