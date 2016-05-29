#if NETSTANDARD_1plus
using NLog.LayoutRenderers;
using System.Text;
using Microsoft.AspNetCore.Routing;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET CORE host.
    /// </summary>
    /// <remarks>
    /// Use this layout renderer host.
    /// </remarks>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-host}    
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-host")]
    public class AspNetCoreHostRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var host = HttpContextAccessor?.HttpContext?.TryGetRequest()?.Host;

            if (host != null)
            {
                var hostString = host.ToString();

                if (!string.IsNullOrEmpty(hostString))
                    builder.Append(hostString);
            }
        }
    }
}
#endif