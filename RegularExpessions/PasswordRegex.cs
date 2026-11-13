using System.Text.RegularExpressions;
namespace LoggingUsingMiddleware.RegularExpessions
{
    public static class PasswordRegex
    {
        private static readonly Regex _passwordRegexPattern = new(@"^(?=.*[A-Za-z])(?=.*\d).{8,64}$");

        public static bool IsMatchingPassword(string password)
        {
            if (_passwordRegexPattern.IsMatch(password))
            {
                return true;
            }
            return false;
        }
    }
}
