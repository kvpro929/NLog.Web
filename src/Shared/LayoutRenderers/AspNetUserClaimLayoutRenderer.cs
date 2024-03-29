﻿#if ASP_NET_CORE || NET46_OR_GREATER

using System;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET User ClaimType Value Lookup.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-user-claim}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-User-Claim-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-user-claim")]
    public class AspNetUserClaimLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Key to lookup using <see cref="ClaimsIdentity.FindFirst(string)"/> with fallback to <see cref="ClaimsPrincipal.FindFirst(string)"/>
        /// </summary>
        /// <remarks>
        /// When value is prefixed with "ClaimTypes." (Remember dot) then ít will lookup in well-known claim types from <see cref="ClaimTypes"/>. Ex. ClaimsTypes.Name
        /// </remarks>
        [RequiredParameter]
        [DefaultParameter]
        public string ClaimType { get; set; }

        /// <inheritdoc />
        protected override void InitializeLayoutRenderer()
        {
            if (ClaimType?.Trim().StartsWith("ClaimTypes.", StringComparison.OrdinalIgnoreCase) == true || ClaimType?.Trim().StartsWith("ClaimType.", StringComparison.OrdinalIgnoreCase) == true)
            {
                var fieldName = ClaimType.Substring(ClaimType.IndexOf('.') + 1).Trim();
                var claimTypesField = typeof(ClaimTypes).GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
                if (claimTypesField != null)
                {
                    ClaimType = claimTypesField.GetValue(null) as string ?? ClaimType.Trim();
                }
            }

            base.InitializeLayoutRenderer();
        }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            try
            {
                var claimsPrincipel = HttpContextAccessor.HttpContext.User;
                if (claimsPrincipel == null)
                {
                    InternalLogger.Debug("aspnet-user-claim - HttpContext User is null");
                    return;
                }

                var claimsIdentity = claimsPrincipel.Identity as ClaimsIdentity;    // Prioritize primary identity
                var claim = claimsIdentity?.FindFirst(ClaimType)
#if ASP_NET_CORE
                    ?? claimsPrincipel.FindFirst(ClaimType)
#endif
                    ;
                if (claim != null)
                {
                    builder.Append(claim?.Value);
                }
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "aspnet-user-claim - HttpContext has been disposed");
            }
        }
    }
}

#endif