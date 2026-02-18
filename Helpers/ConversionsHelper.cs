namespace LibraryAPI.Helpers {
    public class ConversionsHelper {
        public static byte[]? ConvertToBytes(IFormFile? file) {
            if (file == null || file.Length == 0) return null;

            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
}