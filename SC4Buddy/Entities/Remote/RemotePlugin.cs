namespace NIHEI.SC4Buddy.Entities.Remote
{
    using System.Collections.Generic;

    public class RemotePlugin
    {
        public int Id { get; set; }

        public Author Author { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public IEnumerable<RemotePluginFile> PluginFiles { get; set; }

        public string Name { get; set; }

        public int AuthorId
        {
            get
            {
                return Author.Id;
            }
        }

        public IEnumerable<RemotePlugin> Dependencies { get; set; }

        public ICollection<PluginReport> Reports { get; set; }
    }
}
