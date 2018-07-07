namespace Nihei.SC4Buddy.Remote.Models
{
    using System;
    using Nihei.SC4Buddy.Remote.Utils;

    public class BuddyServerException : Exception
    {
        public BuddyServerException(string message, ApiConnectCodes code) : base(message)
        {
            Code = code;
        }

        public ApiConnectCodes Code { get; set; }
    }
}