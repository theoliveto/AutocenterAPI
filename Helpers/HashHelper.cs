using System.Text;
using System.Security.Cryptography;

namespace AutocenterAPI.Helpers {
    public static class HashHelper {
        public static string? CreateSha256(string? text) {
            if (text is null) { 
                return null;
            }

            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}