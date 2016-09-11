namespace EPi.Libraries.Recommendations.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using EPi.Libraries.Recommendations.Core.Enums;

    /// <summary>
    /// Class UsageItem.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    public class UsageItem
    {
        //<User Id>,<Item Id>,<Time>,[<Event>]
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        [StringLength(255)]
        [Required]
        [RegularExpression(@"^[0-9a-zA-Z_-]$")]
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>The item identifier.</value>
        [StringLength(50)]
        [Required]
        [RegularExpression(@"^[0-9a-zA-Z_-]$")]
        public string ItemID { get; set; }

        /// <summary>
        /// Gets or sets the event date.
        /// </summary>
        /// <value>The event date.</value>
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        /// <value>The type of the event.</value>
        public EventType EventType { get; set; }

        /// <summary>
        /// Return the usage item formatted for use in the recomendation api.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            const string DateTimeFormat = "yyyy/MM/ddTHH:mm:ss";
            string forWebservice = this.EventDate.ToString(DateTimeFormat, CultureInfo.InvariantCulture);

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0},{1},{2},{3}",
                this.UserID,
                this.ItemID,
                forWebservice,
                this.EventType.ToString());
        }
    }
}
