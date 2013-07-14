namespace NIHEI.SC4Buddy.Model
{
    using System;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class SessionEventArgs : EventArgs
    {
        public SessionEventArgs(User user)
        {
            User = user;
        }

        public User User { get; set; }
    }
}
