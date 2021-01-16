using Microsoft.AspNetCore.Mvc;
using System;

namespace Microsoft.Diagnostics.Monitoring.RestServer
{
    internal static class ExceptionExtensions
    {
        public static ProblemDetails ToProblemDetails(this Exception ex, int statusCode)
        {
            return new ProblemDetails
            {
                Detail = ex.Message,
                Status = statusCode
            };
        }
    }
}