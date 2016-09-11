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
namespace EPi.Libraries.Recommendations.Core.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    ///     Class Upsale.
    /// </summary>
    /// <remarks>Upsale enables you to enforce items to return in the recommendation results.</remarks>
    /// <author>Jeroen Stemerdink</author>
    [DataContract]
    public class Upsale
    {
        /// <summary>
        ///     Gets or sets the item ids.
        /// </summary>
        /// <value>The item ids.</value>
        [DataMember]
        [JsonProperty("itemIds")]
        public string[] ItemIds { get; set; }

        /// <summary>
        ///     Gets or sets the number of items to upsale.
        /// </summary>
        /// <value>The number of items to upsale.</value>
        [DataMember]
        [JsonProperty("numberOfItemsToUpsale")]
        public int NumberOfItemsToUpsale { get; set; }
    }
}