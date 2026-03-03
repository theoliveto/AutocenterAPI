namespace AutocenterAPI.DTOs.Auth {
    public class LoginResponseDTO {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
        public required object User { get; set; }
    }
}