using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using OpenDCP.Framework.Hosting.Exceptions;
using OpenDCP.Framework.Exceptions;
using System;

namespace OpenDCP.Framework.Hosting.CustomAttributes
{
    public class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public UnhandledExceptionFilterAttribute(IConfiguration configuration, ILoggerFactory factory)
        {
            _configuration = configuration;
            _logger = factory.CreateLogger<UnhandledExceptionFilterAttribute>();
        }

        public override void OnException(ExceptionContext context)
        {
            var errorInfo = new BackendResponseDTO();
            errorInfo.ErrorId = Guid.NewGuid().ToString();

            if (typeof(DCPBaseException).IsAssignableFrom(context.Exception.GetType()))
            {
                errorInfo.Message = context.Exception.Message;
            }
            else
            {
                errorInfo.Message = "There is an error - ErrorId : " + errorInfo.ErrorId;
            }

            if (bool.Parse(_configuration["DetailedErrorReturn"]))
            {
                errorInfo.StackTrace = context.Exception.StackTrace;
            }

            _logger.LogError("ErrorId : " + errorInfo.ErrorId + " - " + context.Exception.Message + ", InnerException ex:" + context.Exception.GetOriginalException()?.Message + ", stackTrace :" + context.Exception.StackTrace);

            var result = new ObjectResult(errorInfo);

            var type = context.Exception.GetType();

            if (type == typeof(DCPBadRequestException))
            {
                result.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (type == typeof(DCPInternalServerErrorException))
            {
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else if (type == typeof(DCPNotFoundException))
            {
                result.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else if (type == typeof(DCPUnauthorizedException))
            {
                result.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (type == typeof(DCPForbiddenException))
            {
                result.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            else
            {
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            context.Result = result;

        }
    }
}
