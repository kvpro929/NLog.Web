﻿using System.Text;
#if !DNX
using System.Web;
using System.Collections.Specialized;
#else
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Primitives;
#endif
using NLog.LayoutRenderers;
using System.Collections.Generic;
using NLog.Config;
using System;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Referrer
    /// </summary>
    /// <example>
    /// <para>Example usage of ${aspnet-request-referrer}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-referrer} - Produces - Referrer URL String from the Request.
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-referrer")]
    public class AspNetRequestReferrerRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the Referrer URL from the HttpRequest <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.Request;

            if (httpRequest == null)
            {
                return;
            }

            string referrer = String.Empty;

#if !DNX
            referrer = httpRequest.UrlReferrer?.ToString();
#else
            referrer = httpRequest.Headers["Referer"].ToString();
#endif
            builder.Append(referrer);

        }
    }
}
