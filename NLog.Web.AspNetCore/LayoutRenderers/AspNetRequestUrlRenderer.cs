﻿using System.Text;
#if !NETSTANDARD_1plus
using System.Web;
using System.Collections.Specialized;
#endif
using NLog.LayoutRenderers;
using System.Collections.Generic;
using NLog.Config;
using System;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request URL
    /// </summary>
    /// <para>Example usage of ${aspnet-request-url}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-url:IncludeQueryString=true} - produces http://www.exmaple.com/?t=1
    /// ${aspnet-request-url:IncludeQueryString=false} - produces http://www.exmaple.com/
    /// ${aspnet-request-url:IncludePort=true} - produces http://www.exmaple.com:80/
    /// ${aspnet-request-url:IncludePort=false} - produces http://www.exmaple.com/
    /// ${aspnet-request-url:IncludePort=true:IncludeQueryString=true} - produces http://www.exmaple.com:80/?t=1    
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-url")]
    public class AspNetRequestUrlRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// To specify whether to exclude / include the Query string.
        /// </summary>
        public bool IncludeQueryString { get; set; } = false;

        /// <summary>
        /// To specify whether to exclude / include the Port.
        /// </summary>
        public bool IncludePort { get; set; } = false;

        /// <summary>
        /// Renders the Request URL from the HttpRequest
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();

            if (httpRequest == null)
                return;

            string url = String.Empty;
            string pathAndQuery = String.Empty;
            string port = String.Empty;
#if !NETSTANDARD_1plus
            if (httpRequest.Url != null)
            {
                if (IncludePort && httpRequest.Url?.Port > 0)
                {
                    port = ":" + httpRequest.Url.Port.ToString();
                }

                if (IncludeQueryString && httpRequest.Url?.PathAndQuery != null)
                {
                    pathAndQuery = httpRequest.Url.PathAndQuery;
                }
                else
                {
                    pathAndQuery = httpRequest.Url.AbsolutePath;
                }

                url = $"{httpRequest.Url.Scheme}://{httpRequest.Url.Host}{port}{pathAndQuery}";
            }

#else
            if (IncludeQueryString)
                pathAndQuery = httpRequest.QueryString.ToUriComponent();

            if (IncludePort && httpRequest.Host.Port > 0)
            {
                port = ":" + httpRequest.Host.Port.ToString();
            }

            url = $"{httpRequest.Scheme}://{httpRequest.Host.ToUriComponent()}{port}{httpRequest.PathBase.ToUriComponent()}{httpRequest.Path.ToUriComponent()}{pathAndQuery}";
#endif
            builder.Append(url);

        }
    }
}
