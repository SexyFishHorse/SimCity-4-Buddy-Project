namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Linq;
    using System.Security.Authentication;
    using System.Security.Cryptography;

    using NIHEI.Common.TypeUtility;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class UserController
    {
        private readonly UserRegistry registry;

        public UserController(UserRegistry registry)
        {
            this.registry = registry;
        }

        public User Login(string username, string password)
        {
            var possibleUser =
                registry.Users.FirstOrDefault(
                    x => x.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));

            if (possibleUser == null)
            {
                throw new InvalidCredentialException("Invalid username or password.");
            }

            if (!possibleUser.Activated)
            {
                throw new AuthenticationException("User is not activated yet.");
            }

            var algorithm = new SHA256Managed();

            var passwordWithSalt = string.Concat(password, possibleUser.Salt);

            var bytes = passwordWithSalt.ToByteArray();
            var hash = algorithm.ComputeHash(bytes);
            var hashedPassphrase = hash.ToStringValue();

            if (!hashedPassphrase.Equals(possibleUser.Passphrase))
            {
                throw new InvalidCredentialException("Invalid username or password.");
            }

            return possibleUser;
        }
    }
}
