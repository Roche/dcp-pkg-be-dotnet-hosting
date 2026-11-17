using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenDCP.Framework.Exceptions;
using OpenDCP.Framework.Hosting.Exceptions;
using OpenDCP.Framework.Security.Constants;
using OpenDCP.Framework.Security.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace OpenDCP.Framework.Hosting.Controllers
{
    public class DCPFrameworkBaseController : ControllerBase
    {
        protected readonly IConfiguration _configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="claimReceiver">
        /// The base helper.
        /// </param>
        public DCPFrameworkBaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// The get user display name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetUserDisplayName()
        {
            return
                ((ClaimsIdentity)User.Identity).Claims.Where(x => x.Type == "displayName")
                .Select(x => x.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// The get user permissions.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        protected List<PermissionDTO> GetUserPermissions()
        {
            var availableUserPermissions = ((ClaimsIdentity)User.Identity).Claims.Where(x => x.Type == "permissions")
                .Select(x => x.Value)
                .ToList();

            List<PermissionDTO> availableUserPermissionsList = new List<PermissionDTO>();

            foreach (string item in availableUserPermissions)
            {
                PermissionDTO model = JsonConvert.DeserializeObject<PermissionDTO>(item);
                availableUserPermissionsList.Add(model);
            }

            return availableUserPermissionsList;
        }
        protected bool IsGlobalAdmin()
        {
            bool.TryParse(((ClaimsIdentity)User.Identity).Claims.Where(x => x.Type == "isGlobalAdmin").Select(x => x.Value).FirstOrDefault(), out bool result);

            return result;
        }
        /// <summary>
        /// The get user id.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected int GetCurrentUserId()
        {
            string claimTimeDifference =
                ((ClaimsIdentity)User.Identity).Claims.Where(x => x.Type == "currentLoggedUserId")
                .Select(x => x.Value).FirstOrDefault();

            int.TryParse(claimTimeDifference, out int result);

            return result;
        }
        protected byte GetPersistedGrantsType()
        {
            var skipAuthorization = _configuration["DevelopmentMode"] != null
                                 && bool.TryParse(
                                     _configuration["DevelopmentMode"],
                                     out bool developmentMode) && developmentMode;
            if (skipAuthorization)
            {
                return (byte)PersistedGrantsType.InternalMicroServiceClient;
            }

            string persistedGrantsType =
          ((ClaimsIdentity)User.Identity).Claims.Where(x => x.Type == "persistedGrantsType")
          .Select(x => x.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(persistedGrantsType))
            {
                throw new DCPUnauthorizedException(ErrorMessages.PersistedGrantsTypeRequired);
            }


            byte.TryParse(persistedGrantsType, out byte persistedGrantsTypeResult);

            if (!Enum.IsDefined(typeof(PersistedGrantsType), persistedGrantsTypeResult))
            {
                throw new DCPUnauthorizedException(ErrorMessages.PersistedGrantsTypeNotSupportedExtension);
            }

            return persistedGrantsTypeResult;
        }
        /// <summary>
        /// Get current user name
        /// </summary>
        /// <returns></returns>
        protected string GetUsername()
        {
            return User.Identity.Name;
        }
    }
}
