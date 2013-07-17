namespace NIHEI.SC4Buddy.Control
{
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Model;

    public class SessionController
    {
        private static SessionController instance = new SessionController();

        private readonly UserController controller;

        private SessionController()
        {
            controller = new UserController(RemoteRegistryFactory.UserRegistry);
        }

        public delegate void SessionEventHandler(SessionController sender, SessionEventArgs eventArgs);

        public event SessionEventHandler UserLoggedIn;

        public event SessionEventHandler UserLoggedOut;

        public static SessionController Instance
        {
            get
            {
                return instance;
            }

            set
            {
                instance = value;
            }
        }

        public User User { get; private set; }

        public bool IsLoggedIn
        {
            get
            {
                return User != null;
            }
        }

        public void Login(string username, string password)
        {
            User = controller.Login(username, password);
            RaiseUserLoggedInEvent();
        }

        public void Logout()
        {
            User = null;
            RaiseUserLoggedOutEvent();
        }

        public void AttemptAutoLogin()
        {
            User = controller.AutoLogin();
            if (User != null)
            {
                RaiseUserLoggedInEvent();
            }
        }

        protected virtual void RaiseUserLoggedInEvent()
        {
            if (UserLoggedIn != null)
            {
                UserLoggedIn(this, new SessionEventArgs(User));
            }
        }

        protected virtual void RaiseUserLoggedOutEvent()
        {
            if (UserLoggedOut != null)
            {
                UserLoggedOut(this, new SessionEventArgs(User));
            }
        }
    }
}