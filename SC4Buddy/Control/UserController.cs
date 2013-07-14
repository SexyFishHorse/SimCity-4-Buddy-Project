namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Linq;
    using System.Security.Authentication;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;

    using NIHEI.Common.TypeUtility;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class UserController
    {
        private readonly UserRegistry userRegistry;

        private readonly AuthorRegistry authorRegistry;

        public UserController(UserRegistry userRegistry, AuthorRegistry authorRegistry)
        {
            this.userRegistry = userRegistry;
            this.authorRegistry = authorRegistry;
        }

        public User Login(string email, string password)
        {
            var possibleUser =
                userRegistry.Users.FirstOrDefault(
                    x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));

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

        public void CreateUser(string email, string password, string site, string username)
        {
            var emailRegex = new Regex(@"/.+@.+\..+/i");
            if (!emailRegex.IsMatch(email))
            {
                throw new ArgumentException("E-mail is not valid.");
            }

            var salt = StringUtility.GenerateRandomAlphaNumericString(256);

            var passphrase = string.Concat(salt, password);

            var algorithm = new SHA256Managed();

            var hash = algorithm.ComputeHash(passphrase.ToByteArray()).ToStringValue();

            var user = new User { Email = email, Salt = salt, Passphrase = hash };

            userRegistry.Add(user);

            var author = new Author() { Name = username, Site = site, User = user };

            authorRegistry.Add(author);
        }
    }
}
