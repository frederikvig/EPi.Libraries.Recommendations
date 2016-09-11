namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    /// <summary>
    ///     Utility class holding the result of import operation
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    internal class ImportReport
    {
        /// <summary>
        ///     Gets or sets the error count.
        /// </summary>
        /// <value>The error count.</value>
        public int ErrorCount { get; set; }

        /// <summary>
        ///     Gets or sets the information.
        /// </summary>
        /// <value>The information.</value>
        public string Info { get; set; }

        /// <summary>
        ///     Gets or sets the line count.
        /// </summary>
        /// <value>The line count.</value>
        public int LineCount { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(
                "successfully imported {0}/{1} lines for {2}", 
                this.LineCount - this.ErrorCount, 
                this.LineCount, 
                this.Info);
        }
    }
}