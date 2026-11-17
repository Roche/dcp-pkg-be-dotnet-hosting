using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenDCP.Framework.Hosting.CustomAttributes
{
    /// <summary>
    /// Specifies that the responses of the decorated controller or action method should be cached in memory.
    /// Caching keys are constructed using the request path, query strings, and payload arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class MemoryCacheAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheAttribute"/> class.
        /// </summary>
        /// <param name="ttl">The Time-To-Live (TTL) expiration duration for the cached data, in seconds. Default is 86400 seconds (24 hours).</param>
        public MemoryCacheAttribute(int ttl = 86400) : base(typeof(MemoryCacheFilter))
        {
            Arguments = new object[] { ttl };
        }

        /// <summary>
        /// An asynchronous action filter that intercepts incoming requests to manage cache validation and response.
        /// </summary>
        public class MemoryCacheFilter : IAsyncActionFilter
        {
            private readonly IMemoryCache _cache;
            private readonly int _ttl;

            /// <summary>
            /// Initializes a new instance of the <see cref="MemoryCacheFilter"/> class.
            /// </summary>
            /// <param name="cache">The memory cache instance injected from the service container.</param>
            /// <param name="ttl">The absolute expiration lifetime context, in seconds.</param>
            public MemoryCacheFilter(IMemoryCache cache, int ttl)
            {
                _cache = cache;
                _ttl = ttl;
            }

            /// <summary>
            /// Executes before and after the action method execution to serve data from the cache or save a successful action result to it.
            /// </summary>
            /// <param name="context">The context concerning the action execution pipeline.</param>
            /// <param name="next">The delegate invoked to execute the next stage of the action pipeline.</param>
            /// <returns>A task that represents the asynchronous filter execution operation.</returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                //generate the unique cache key based on URL, Query, and Body payload
                string cacheKey = GenerateCacheKeyAsync(context);

                var cachedData = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_ttl);

                    //execute the actual controller action 
                    var executedContext = await next();

                    if (executedContext.Result is OkObjectResult okObjectResult)
                    {
                        return okObjectResult.Value;
                    }

                    return null;
                });

                if (cachedData != null)
                {
                    context.Result = new OkObjectResult(cachedData);
                }
            }

            /// <summary>
            /// Generates a unique string representing the context of the request route using its path, active query strings, and model state parameters.
            /// </summary>
            /// <param name="context">The context execution structure containing information on the current HTTP request.</param>
            /// <returns>A deterministic, unique string representing the request identity.</returns>
            private string GenerateCacheKeyAsync(ActionExecutingContext context)
            {
                var request = context.HttpContext.Request;
                var keyBuilder = new StringBuilder();

                keyBuilder.Append($"{request.Path}{request.QueryString}");

                //appending the body payload
                if (request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                    request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
                {
                    if (context.ActionArguments != null && context.ActionArguments.Any())
                    {
                        var serializedArguments = JsonSerializer.Serialize(context.ActionArguments);
                        keyBuilder.Append($"|Args:{serializedArguments}");
                    }
                }

                string rawKey = keyBuilder.ToString();
                return ComputeSha256Hash(rawKey);
            }

            /// <summary>
            /// Computes a deterministic SHA256 hash string from the provided raw data.
            /// </summary>
            /// <param name="rawData">The raw string data sequence to be processed.</param>
            /// <returns>A 64-character, lowercase hexadecimal hash string.</returns>
            private static string ComputeSha256Hash(string rawData)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                    return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
