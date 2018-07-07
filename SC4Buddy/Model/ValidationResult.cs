namespace Nihei.SC4Buddy.Model
{
    public class ValidationResult
    {
        public ValidationResult(bool valid, string[] errorMessages)
        {
            Valid = valid;
            ErrorMessages = errorMessages;
        }

        public ValidationResult(bool valid, string errorMessage)
        {
            Valid = valid;
            ErrorMessages = new[] { errorMessage };
        }

        public bool Valid { get; private set; }

        public string[] ErrorMessages { get; private set; }
    }
}
