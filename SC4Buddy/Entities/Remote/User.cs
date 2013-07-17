namespace NIHEI.SC4Buddy.Entities.Remote
{
    using System;

    public partial class User
    {
        public bool IsDeveloper
        {
            get
            {
                return Rights.Equals("Developer", StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool IsAuthor
        {
            get
            {
                return Rights.Equals("Author", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
