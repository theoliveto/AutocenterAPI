namespace LibraryAPI.DTOs {
    public class UsersDTO {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string login { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public string? observations { get; set; }
        public bool active { get; set; }
        public DateTime register { get; set; }
        public DateTime? edit { get; set; }
    }
}