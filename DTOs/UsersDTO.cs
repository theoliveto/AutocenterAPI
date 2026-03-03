namespace AutocenterAPI.DTOs {
    public class UsersDTO {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string login { get; set; } = string.Empty;
        public string? password { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public string? observations { get; set; }
        public bool active { get; set; }
        public DateTime register { get; set; }
        public DateTime? edit { get; set; }
        public byte[]? profile { get; set; }
        public string? profileBase64 => profile != null ? $"data:image/png;base64,{Convert.ToBase64String(profile)}" : null;
        public string? googleSub { get; set; }
    }
}