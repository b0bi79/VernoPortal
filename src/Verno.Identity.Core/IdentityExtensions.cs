using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Abp.Collections.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Text;
using Abp.UI;
using Microsoft.AspNetCore.Identity;

namespace Verno.Identity
{
    public static class IdentityExtensions
    {
        private static readonly Dictionary<string, string> IdentityLocalizations
            = new Dictionary<string, string>
              {
                  {"User already in role.", "Identity.UserAlreadyInRole"},
                  {"User is not in role.", "Identity.UserNotInRole"},
                  {"Role {0} does not exist.", "Identity.RoleNotFound"},
                  {"Incorrect password.", "Identity.PasswordMismatch"},
                  {"User name {0} is invalid, can only contain letters or digits.", "Identity.InvalidUserName"},
                  {"Passwords must be at least {0} characters.", "Identity.PasswordTooShort"},
                  {"{0} cannot be null or empty.", "Identity.PropertyTooShort"},
                  {"Name {0} is already taken.", "Identity.DuplicateName"},
                  {"User already has a password set.", "Identity.UserAlreadyHasPassword"},
                  {"Passwords must have at least one non letter or digit character.", "Identity.PasswordRequireNonLetterOrDigit"},
                  {"UserId not found.", "Identity.UserIdNotFound"},
                  {"Invalid token.", "Identity.InvalidToken"},
                  {"Email '{0}' is invalid.", "Identity.InvalidEmail"},
                  {"User {0} does not exist.", "Identity.UserNameNotFound"},
                  {"Lockout is not enabled for this user.", "Identity.LockoutNotEnabled"},
                  {"Passwords must have at least one uppercase ('A'-'Z').", "Identity.PasswordRequireUpper"},
                  {"Passwords must have at least one digit ('0'-'9').", "Identity.PasswordRequireDigit"},
                  {"Passwords must have at least one lowercase ('a'-'z').", "Identity.PasswordRequireLower"},
                  {"Email '{0}' is already taken.", "Identity.DuplicateEmail"},
                  {"A user with that external login already exists.", "Identity.ExternalLoginExists"},
                  {"An unknown failure has occured.", "Identity.DefaultError"}
              };

        /// <summary>
        /// Checks errors of given <see cref="IdentityResult"/> and throws <see cref="UserFriendlyException"/> if it's not succeeded.
        /// </summary>
        /// <param name="identityResult">Identity result to check</param>
        public static void CheckErrors(this IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
            {
                return;
            }

            throw new UserFriendlyException(identityResult.Errors.JoinAsString(" "));
        }

        /// <summary>
        /// Checks errors of given <see cref="IdentityResult"/> and throws <see cref="UserFriendlyException"/> if it's not succeeded.
        /// </summary>
        /// <param name="identityResult">Identity result to check</param>
        /// <param name="localizationManager">Localization manager to localize error messages</param>
        public static void CheckErrors(this IdentityResult identityResult, ILocalizationManager localizationManager)
        {
            if (identityResult.Succeeded)
            {
                return;
            }

            throw new UserFriendlyException(identityResult.LocalizeErrors(localizationManager));
        }

        /// <summary>
        /// Checks errors of given <see cref="IdentityResult"/> and throws <see cref="UserFriendlyException"/> if it's not succeeded.
        /// </summary>
        /// <param name="identityResults">Identity results to check</param>
        /// <param name="localizationManager">Localization manager to localize error messages</param>
        public static void CheckErrors(this IEnumerable<IdentityResult> identityResults, ILocalizationManager localizationManager)
        {
            var errors = (from identityResult in identityResults
                          where !identityResult.Succeeded
                          select identityResult.LocalizeErrors(localizationManager)).ToList();

            if (errors.Count > 0)
                throw new UserFriendlyException(string.Join(Environment.NewLine, errors));
        }

        public static string LocalizeErrors(this IdentityResult identityResult, ILocalizationManager localizationManager)
        {
            if (identityResult.Succeeded)
            {
                throw new ArgumentException("identityResult.Succeeded should be false in order to localize errors.");
            }

            if (identityResult.Errors == null)
            {
                throw new ArgumentException("identityResult.Errors should not be null.");
            }

            return identityResult.Errors.Select(err => LocalizeErrorMessage(err, localizationManager)).JoinAsString(" ");
        }

        private static string LocalizeErrorMessage(IdentityError identityErrorMessage, ILocalizationManager localizationManager)
        {
            var localizationSource = localizationManager.GetSource(IdentityConsts.LocalizationSourceName);

            foreach (var identityLocalization in IdentityLocalizations)
            {
                string[] values;
                if (FormattedStringValueExtracter.IsMatch(identityErrorMessage.Description, identityLocalization.Key, out values))
                {
                    return localizationSource.GetString(identityLocalization.Value, values.Cast<object>().ToArray());
                }
            }

            return localizationSource.GetString(identityErrorMessage.Description);
        }

        /// <summary>Return the user name using the UserNameClaimType</summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetUserName(this IIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            ClaimsIdentity identity1 = identity as ClaimsIdentity;
            return identity1?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        }

        /// <summary>Return the user id using the UserIdClaimType</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            ClaimsIdentity identity1 = identity as ClaimsIdentity;

            string firstValue = identity1?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (firstValue != null)
                return (T)Convert.ChangeType(firstValue, typeof(T), CultureInfo.InvariantCulture);
            return default(T);
        }

        /// <summary>Return the user id using the UserIdClaimType</summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetUserId(this IIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            ClaimsIdentity identity1 = identity as ClaimsIdentity;
            return identity1?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        }

        /// <summary>
        ///     Return the claim value for the first claim with the specified type if it exists, null otherwise
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            Claim first = identity.FindFirst(claimType);
            return first?.Value;
        }
    }
}
