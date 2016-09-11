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
namespace EPi.Libraries.Recommendations.Core.Enums
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     Enum RuleType
    /// </summary>
    [DataContract]
    public enum RuleType
    {
        /// <summary>
        ///     BlockList enables you to provide a list of items that you do not want to return in the recommendation results.
        /// </summary>
        BlockList,

        /// <summary>
        ///     Feature BlockList enables you to block items based on the values of its features. Do not send more than 1000 items
        ///     in a single blocklist rule or your call may timeout. If you need to block more than 1000 items, you can make
        ///     several blocklist calls.
        /// </summary>
        FeatureBlockList,

        /// <summary>
        ///     Upsale enables you to enforce items to return in the recommendation results.
        /// </summary>
        Upsale,

        /// <summary>
        ///     White List enables you to only suggest recommendations from a list of items.
        /// </summary>
        WhiteList,

        /// <summary>
        ///     Feature White List enables you to only recommend items that have specific feature values.
        /// </summary>
        FeatureWhiteList,

        /// <summary>
        ///     Per Seed Block List enables you to provide per item a list of items that cannot be returned as recommendation
        ///     results.
        /// </summary>
        PerSeedBlockList
    }
}