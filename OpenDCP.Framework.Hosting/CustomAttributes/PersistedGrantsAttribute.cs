using OpenDCP.Framework.Connections.Core;
using OpenDCP.Framework.Exceptions;
using OpenDCP.Framework.Hosting.Constants;
using OpenDCP.Framework.Hosting.Exceptions;
using OpenDCP.Framework.Security.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDCP.Framework.Hosting.CustomAttributes
{
    public class PersistedGrantsAttribute : ActionFilterAttribute
    {
        private readonly IConfiguration _configuration;
        private readonly IDCPCoreConnection _connection;
        public PersistedGrantsAttribute(IConfiguration configuration, IDCPCoreConnection connection)
        {
            _configuration = configuration;
            _connection = connection;
        }
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
            {
                await next();
                return;
            }

            var bearerToken = context.HttpContext.Request.Headers[HeaderNames.Authorization].FirstOrDefault()?.Replace("Bearer", "").Trim();

            if (string.IsNullOrEmpty(bearerToken))
            {
                var queryString = context.HttpContext.Request.Query["access_token"];
                bearerToken = queryString.ToString();

                if (string.IsNullOrWhiteSpace(bearerToken))
                {
                    throw new DCPUnauthorizedException(ErrorMessages.SessiontTokenExpriedOrNotValid);
                }

            }

            string persistedGrantsType = context.HttpContext.User.Claims.Where(x => x.Type == "persistedGrantsType").Select(x => x.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(persistedGrantsType))
            {
                throw new DCPUnauthorizedException(ErrorMessages.PersistedGrantsTypeRequired);
            }

            byte.TryParse(persistedGrantsType, out byte persistedGrantsTypeResult);

            if (!Enum.IsDefined(typeof(PersistedGrantsType), persistedGrantsTypeResult))
            {
                throw new DCPUnauthorizedException(ErrorMessages.PersistedGrantsTypeNotSupportedExtension);
            }

            if (persistedGrantsTypeResult == (byte)PersistedGrantsType.APIGateWay)
            {
                var apiGatewayHeaderName = _configuration["Headers:Gateway:Name"];
                var microserviceHeaderName = _configuration["Headers:Microservice:Name"];

                var apiGatewayHeader = context.HttpContext.Request.Headers[apiGatewayHeaderName].FirstOrDefault();
                var microserviceHeader = context.HttpContext.Request.Headers[microserviceHeaderName].FirstOrDefault();

                if (!string.Equals(apiGatewayHeader, _configuration["Headers:Gateway:Value"], StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(microserviceHeader, _configuration["Headers:Microservice:Name"], StringComparison.OrdinalIgnoreCase))
                {
                    throw new DCPUnauthorizedException(ErrorMessages.UserClientCredentialsCanBeUsedOnlyFromAPIGateway);
                }
            }

            var response = await _connection.GetAsync(DCPCoreEndpoints.DCPCorePersistedGrantsEndPoint);

            if (!response.Response)
            {
                throw new DCPUnauthorizedException(ErrorMessages.SessiontTokenExpriedOrNotValid);
            }
            else
            {
                await next();
            }
        }
    }
}
