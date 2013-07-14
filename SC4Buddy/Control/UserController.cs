namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Security.Authentication;
    using System.Security.Cryptography;
    using System.Text;

    using NIHEI.Common.TypeUtility;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Localization;

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

            var saltedHash = GenerateSaltedHash(
                Encoding.Default.GetBytes(password), Encoding.Default.GetBytes(possibleUser.Salt));

            if (!CompareByteArrays(saltedHash, possibleUser.Passphrase))
            {
                throw new InvalidCredentialException("Invalid username or password.");
            }

            return possibleUser;
        }

        public void CreateUser(string email, string password, string repeatPassword, string site, string username)
        {
            if (!password.Equals(repeatPassword, StringComparison.Ordinal))
            {
                throw new ValidationException(LocalizationStrings.PasswordsDoesNotMatch);
            }

            if (!EmailSeemsValid(email))
            {
                throw new ValidationException(LocalizationStrings.EmailIsNotValid);
            }

            var salt = StringUtility.GenerateRandomAlphaNumericString(50);

            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var saltBytes = Encoding.UTF8.GetBytes(salt);

            var saltedHashBytes = GenerateSaltedHash(passwordBytes, saltBytes);

            var user = new User { Email = email, Salt = salt, Passphrase = saltedHashBytes };

            userRegistry.Add(user);

            var author = new Author { Name = username, Site = site, User = user };

            authorRegistry.Add(author);

            user.Authors.Add(author);

            userRegistry.Update(user);
        }

        private static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            return !array1.Where((t, i) => t != array2[i]).Any();
        }

        private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            var plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (var i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            for (var i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        private bool EmailSeemsValid(string email)
        {
            if (!email.Contains("@"))
            {
                return false;
            }

            var mailbox = email.Substring(0, email.IndexOf("@", StringComparison.Ordinal));
            var domain = email.Substring(email.IndexOf("@", StringComparison.Ordinal) + 1);

            if (mailbox.Length < 1)
            {
                return false;
            }

            if (domain.Length < 4)
            {
                return false;
            }

            if (!domain.Contains("."))
            {
                return false;
            }

            return true;
        }
    }
}
