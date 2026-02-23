using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace LibraryAPI.Helpers {
    public class ResetTokenHelper {
        public static string GenerateToken() {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return WebEncoders.Base64UrlEncode(bytes);
        }

        public static byte[] Sha256(string token) {
            return SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        }
    }
}