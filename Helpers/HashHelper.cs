using System.Text;
using System.Security.Cryptography;

namespace LibraryAPI.Helpers {
    public static class HashHelper {
        public static string GerarSha256(string text) {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}