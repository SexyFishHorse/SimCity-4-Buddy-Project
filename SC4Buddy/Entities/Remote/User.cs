namespace NIHEI.SC4Buddy.Entities.Remote
{
    using System;

    public class User
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

        public string Email { get; set; }

        public string Salt { get; set; }

        public byte[] Passphrase { get; set; }

        public string Rights { get; set; }
    }
}
