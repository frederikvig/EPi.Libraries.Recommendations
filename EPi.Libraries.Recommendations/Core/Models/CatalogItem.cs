namespace EPi.Libraries.Recommendations.Core.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Class CatalogModel.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    public class CatalogItem
    {
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>The item identifier.</value>
        [StringLength(50)]
        [Required]
        [RegularExpression(@"^[0-9a-zA-Z_-]$")]
        public string ItemID { get; set; }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <value>The name of the item.</value>
        [StringLength(255)]
        [Required]
        [RegularExpression(@"^[0-9a-zA-Z\s]$")]
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the product category.
        /// </summary>
        /// <value>The product category.</value>
        [StringLength(255)]
        [Required]
        [RegularExpression(@"^[0-9a-zA-Z\s]$")]
        public string ProductCategory { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [StringLength(4000)]
        [RegularExpression(@"^[0-9a-zA-Z\s]$")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the features.
        /// </summary>
        /// <value>The features.</value>
        public Dictionary<string,string> Features { get; set; }

        /// <summary>
        /// Return the catalog item formatted for use in the recomendation api.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (this.Features.Count > 0)
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0},{1},{2},{3},{4}",
                    this.ItemID,
                    this.ItemName,
                    this.ProductCategory,
                    this.Description,
                    string.Join(",", this.Features.Select(f => string.Format(CultureInfo.InvariantCulture, "{0}={1}", f.Key, f.Value)).ToArray()));
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0},{1},{2},{3}",
                this.ItemID,
                this.ItemName,
                this.ProductCategory,
                this.Description);
        }
    }
}
