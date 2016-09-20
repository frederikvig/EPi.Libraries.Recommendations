using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPi.Libraries.Recommendations
{
    /// <summary>
    /// Interface IBuildProvider
    /// </summary>
    public interface IBuildProvider
    {
        /// <summary>
        /// Creates the build.
        /// </summary>
        /// <param name="modelId">The id of the model.</param>
        /// <param name="featureList">The list of features to use in the build.</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
        long? CreateBuild(string modelId, string featureList, out string message);
    }
}
