namespace NIHEI.SC4Buddy.Control.Remote
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Security.Authentication;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    using NIHEI.Common.TypeUtility;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;

    public class UserController
    {
        private readonly RemoteEntities entities;

        private readonly string passwordHashFilePath = Path.Combine(Application.LocalUserAppDataPath, "hash.login");

        public UserController(RemoteEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<User> Users
        {
            get
            {
                return entities.Users;
            }
        }

        public void Add(User user)
        {
            entities.Users.AddObject(user);
            entities.SaveChanges();
        }

        public void Update(User user)
        {
            entities.SaveChanges();
        }

        public User AutoLogin()
        {
            if (!File.Exists(passwordHashFilePath))
            {
                return null;
            }

            var email = Settings.Default.UserEmail;
            var hash = File.ReadAllBytes(passwordHashFilePath);

            try
            {
                return Login(email, hash);
            }
            catch (InvalidCredentialException)
            {
                return null;
            }
        }

        public User Login(string email, string password)
        {
            var possibleUser = GetPossibleUser(email);

            var saltedHash = GenerateSaltedHash(
                Encoding.Default.GetBytes(password), Encoding.Default.GetBytes(possibleUser.Salt));

            return CompareHashesAndUpdateSettings(saltedHash, possibleUser);
        }

        private User Login(string email, byte[] password)
        {
            var possibleUser = GetPossibleUser(email);

            return CompareHashesAndUpdateSettings(password, possibleUser);
        }

        public void CreateUser(string email, string password, string repeatPassword)
        {
            if (!password.Equals(repeatPassword, StringComparison.Ordinal))
            {
                throw new ValidationException(LocalizationStrings.PasswordsDoesNotMatch);
            }

            if (!EmailSeemsValid(email))
            {
                throw new ValidationException(LocalizationStrings.EmailIsNotValid);
            }

            var salt = StringUtility.GenerateRandomAlphaNumericString(25);

            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var saltBytes = Encoding.UTF8.GetBytes(salt);

            var saltedHashBytes = GenerateSaltedHash(passwordBytes, saltBytes);

            var user = new User { Email = email, Salt = salt, Passphrase = saltedHashBytes };

            Add(user);
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

        private User CompareHashesAndUpdateSettings(byte[] saltedHash, User possibleUser)
        {
            if (!saltedHash.SequenceEqual(possibleUser.Passphrase))
            {
                Settings.Default.UserEmail = string.Empty;
                if (File.Exists(passwordHashFilePath))
                {
                    File.Delete(passwordHashFilePath);
                }

                throw new InvalidCredentialException("Invalid username or password.");
            }

            File.WriteAllBytes(passwordHashFilePath, saltedHash);
            Settings.Default.Save();

            return possibleUser;
        }

        private User GetPossibleUser(string email)
        {
            var possibleUser =
                Users.FirstOrDefault(
                    x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));

            if (possibleUser == null)
            {
                throw new InvalidCredentialException("Invalid username or password.");
            }

            return possibleUser;
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
