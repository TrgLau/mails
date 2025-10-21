using System.Security.Cryptography;
using System.Text;

namespace TrgHelpers.PasswordHelper
{
    public static class PasswordHelper
    {
        private const string Alphanum = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!=@/*-+$%'_\\";

        public static string RandomAlphaNumeric(int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length), "Le nombre de caractères doit être > 0.");

            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int idx = RandomNumberGenerator.GetInt32(Alphanum.Length);
                sb.Append(Alphanum[idx]);
            }

            return sb.ToString();
        }
    }
}