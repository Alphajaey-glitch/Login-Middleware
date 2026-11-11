using System.Text.RegularExpressions;

namespace LoggingUsingMiddleware.RegularExpessions
{
    public static class EmailRegex
    {
        static Regex _emailRegexPattern = new Regex(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(\.[A-Za-z0-9\-]+)*\.[A-Za-z]{2,}$");

        public static bool IsMatchingEmail(string email)
        {
            if(_emailRegexPattern.IsMatch(email))
            {
                return true;
            }
            return false;
        }
    }
}
