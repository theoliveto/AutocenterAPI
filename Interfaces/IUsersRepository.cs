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
    }
}