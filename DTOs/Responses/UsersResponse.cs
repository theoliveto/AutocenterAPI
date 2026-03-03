namespace AutocenterAPI.DTOs.Responses {
    public class UsersResponse {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string login { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? profile { get; set; }
    }
}