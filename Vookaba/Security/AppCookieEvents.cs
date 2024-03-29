﻿#nullable enable
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Vookaba.Common;
using Vookaba.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vookaba.Security
{
    public class AppCookieEvents : CookieAuthenticationEvents
    {
        private const string LastCheck = "checked";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> principalFactory;
        private readonly ISystemClock clock;
        private readonly ApplicationOptions appOptions;
        private readonly IdentityOptions identityOptions;

        public AppCookieEvents(UserManager<ApplicationUser> userManager,
                               IUserClaimsPrincipalFactory<ApplicationUser> principalFactory,
                               ISystemClock clock,
                               IOptions<ApplicationOptions> chanOptions,
                               IOptions<IdentityOptions> identityOptions
            )
        {
            this.userManager = userManager;
            this.principalFactory = principalFactory;
            this.clock = clock;
            this.appOptions = chanOptions.Value;
            this.identityOptions = identityOptions.Value;
        }
        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (context.Principal == null)
            {
                return;
            }
            if (IsDoubleCheckRequred(context) || IsValidationExpired(context))
            {
                var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);
                if (user.SecurityStamp != context.Principal.FindFirstValue(identityOptions.ClaimsIdentity.SecurityStampClaimType))
                {
                    var userPrincipal = await principalFactory.CreateAsync(user);
                    var userIdentity = userPrincipal.Identities.First(x => x.AuthenticationType == IdentityConstants.ApplicationScheme);
                    var principal = GetUpdatedPrincipal(context.Principal, userIdentity);
                    context.ReplacePrincipal(principal);
                }
                SetValidationTime(context);
                context.ShouldRenew = true;
            }
        }

        public override Task SigningIn(CookieSigningInContext context)
        {
            SetValidationTime(context);
            return base.SigningIn(context);
        }

        private bool IsDoubleCheckRequred(CookieValidatePrincipalContext context)
        {
            return appOptions.DoubleCheckPermissions
                && context.Principal!.HasClaim(x => x.Type == ClaimTypes.Role);
        }

        private bool IsValidationExpired(CookieValidatePrincipalContext context)
        {
            if (appOptions.PermissionsCheckInterval <= TimeSpan.Zero)
            {
                return false;
            }
            if (TryGetValidationTime(context, out var lastCheck))
            {
                return clock.UtcNow.Subtract(lastCheck) >= appOptions.PermissionsCheckInterval;
            }

            return true;
        }

        private ClaimsPrincipal GetUpdatedPrincipal(ClaimsPrincipal oldPrincipal, ClaimsIdentity identity)
        {
            var resultPrincipal = new ClaimsPrincipal();
            foreach (var oldIdentity in oldPrincipal.Identities)
            {
                if (oldIdentity.AuthenticationType == identity.AuthenticationType)
                {
                    var newIdentity = oldIdentity.Clone();
                    var claimsToDelete = GetClaimsToReplace(newIdentity.Claims).ToList();
                    foreach (var item in claimsToDelete)
                    {
                        newIdentity.TryRemoveClaim(item);
                    }
                    var claimsToAdd = GetClaimsToReplace(identity.Claims).ToList();
                    foreach (var item in claimsToAdd)
                    {
                        newIdentity.AddClaim(item);
                    }
                    resultPrincipal.AddIdentity(newIdentity);
                }
                else
                {
                    resultPrincipal.AddIdentity(oldIdentity);
                }
            }
            return resultPrincipal;
        }

        private IEnumerable<Claim> GetClaimsToReplace(IEnumerable<Claim> claims)
        {
            return claims.Where(c => c.Type == ApplicationConstants.ClaimTypes.BoardPermission
                || c.Type == identityOptions.ClaimsIdentity.RoleClaimType
                || c.Type == identityOptions.ClaimsIdentity.SecurityStampClaimType);
        }

        private void SetValidationTime(PropertiesContext<CookieAuthenticationOptions> context)
        {
            context.Properties.Items[LastCheck] = clock.UtcNow.ToString();
        }

        private bool TryGetValidationTime(PropertiesContext<CookieAuthenticationOptions> context, out DateTimeOffset date)
        {
            var hasItem = context.Properties.Items.TryGetValue(LastCheck, out var dateString);
            if (hasItem)
            {
                return DateTimeOffset.TryParse(dateString, out date);
            }
            date = new();
            return false;
        }
    }
}
