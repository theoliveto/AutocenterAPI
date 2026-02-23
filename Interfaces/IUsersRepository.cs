using LibraryAPI.DTOs;
using LibraryAPI.Helpers;
using Microsoft.Data.SqlClient;

namespace LibraryAPI.Interfaces {
    public interface IUsersRepository {
        IEnumerable<UsersDTO> GetAll();
        UsersDTO? GetById(int id);
        void Insert(UsersDTO dto);
        void Update(UsersDTO dto);
        void Delete(int id);
        public UsersDTO? Login(string loginOrEmail, string password);
        bool LoginExists(string login, int? ignoreId = null);
        bool EmailExists(string email, int? ignoreId = null);
        UsersDTO? GetByEmail(string email);
        void CreatePasswordResetToken(int userId, byte[] tokenHash, DateTime expiresAtUtc);
        bool TryUsePasswordResetToken(int userId, byte[] tokenHash, DateTime nowUtc);
        void UpdatePassword(int userId, string passwordHashSha256);
    }
}