using AutocenterAPI.DTOs;
using AutocenterAPI.Helpers;
using Microsoft.Data.SqlClient;

namespace AutocenterAPI.Interfaces {
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
        UsersDTO? GetByGoogleSub(string googleSub);
        void SetGoogleSub(int userId, string googleSub);
        UsersDTO CreateGoogleUser(UsersDTO dto);
    }
}