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
    ///     Class Parameters.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    [DataContract]
    public class Parameters
    {
        /// <summary>
        ///     Gets or sets the block list.
        /// </summary>
        /// <remarks>BlockList enables you to provide a list of items that you do not want to return in the recommendation results. </remarks>
        /// <value>The block list.</value>
        [DataMember]
        [JsonProperty("blockList")]
        public Blocklist BlockList { get; set; }

        /// <summary>
        ///     Gets or sets the feature block list.
        /// </summary>
        /// <remarks>
        ///     Feature BlockList enables you to block items based on the values of its features. Do not send more than 1000
        ///     items in a single blocklist rule or your call may timeout. If you need to block more than 1000 items, you can make
        ///     several blocklist calls.
        /// </remarks>
        /// <value>The feature block list.</value>
        [DataMember]
        [JsonProperty("featureBlockList")]
        public Featureblocklist FeatureBlockList { get; set; }

        /// <summary>
        ///     Gets or sets the feature white list.
        /// </summary>
        /// <remarks>Feature White List enables you to only recommend items that have specific feature values.</remarks>
        /// <value>The feature white list.</value>
        [DataMember]
        [JsonProperty("featureWhiteList")]
        public Featurewhitelist FeatureWhiteList { get; set; }

        /// <summary>
        ///     Gets or sets the per seed block list.
        /// </summary>
        /// <remarks>
        ///     Per Seed Block List enables you to provide per item a list of items that cannot be returned as recommendation
        ///     results.
        /// </remarks>
        /// <value>The per seed block list.</value>
        [DataMember]
        [JsonProperty("perSeedBlockList")]
        public Perseedblocklist PerSeedBlockList { get; set; }

        /// <summary>
        ///     Gets or sets the upsale.
        /// </summary>
        /// <remarks>Upsale enables you to enforce items to return in the recommendation results.</remarks>
        /// <value>The upsale.</value>
        [DataMember]
        [JsonProperty("upsale")]
        public Upsale Upsale { get; set; }

        /// <summary>
        ///     Gets or sets the white list.
        /// </summary>
        /// <remarks>White List enables you to only suggest recommendations from a list of items.</remarks>
        /// <value>The white list.</value>
        [DataMember]
        [JsonProperty("whiteList")]
        public Whitelist WhiteList { get; set; }
    }
}