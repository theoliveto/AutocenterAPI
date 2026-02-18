namespace LibraryAPI.DTOs.Requests {
    public class UsersRequest {
        public string name { get; set; } = string.Empty;
        public string login { get; set; } = string.Empty;
        public string? password { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public string? observations { get; set; } = string.Empty;
        public bool active { get; set; }
        public IFormFile? profile { get; set; }
    }
}