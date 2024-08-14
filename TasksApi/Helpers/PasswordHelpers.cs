using System.Text.RegularExpressions;

namespace TasksApi.Helpers
{
    public static class PasswordHelpers
    {
        public static bool ValidatePassword(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length > 5 && Regex.Count(password, @"\W") > 0 && Regex.Count(password, "[0-9]") > 0;
        }
    }
}
