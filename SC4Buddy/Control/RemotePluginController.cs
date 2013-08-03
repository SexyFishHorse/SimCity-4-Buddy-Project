namespace NIHEI.SC4Buddy.Control
{
    using System;

    public class RemotePluginController
    {
        public static bool ValidateLinkAndAuthor(string link, Author author)
        {
            var siteUri = new UriBuilder(author.Site);
            var linkUri = new UriBuilder(link);

            return linkUri.Host.EndsWith(siteUri.Host, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ValidateLinkAndAuthor(string link, Entities.Remote.Author author)
        {
            var siteUri = new UriBuilder(author.Site);
            var linkUri = new UriBuilder(link);

            return linkUri.Host.EndsWith(siteUri.Host, StringComparison.OrdinalIgnoreCase);
        }
    }
}
