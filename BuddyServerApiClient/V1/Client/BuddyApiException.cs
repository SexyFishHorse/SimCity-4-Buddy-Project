namespace Asser.Sc4Buddy.Server.Api.V1.Client
{
    using System;
    using Asser.Sc4Buddy.Server.Api.V1.Models;

    public class BuddyApiException : Exception
    {
        public BuddyApiException(ApiError apiError) : base(apiError.Error.Message)
        {
            ApiError = apiError;
        }

        public ApiError ApiError { get; private set; }
    }
}