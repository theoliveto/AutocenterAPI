using Dapper;
using LibraryAPI.Data;
using LibraryAPI.DTOs;
using LibraryAPI.Helpers;
using LibraryAPI.Interfaces;
using System.Data;

namespace LibraryAPI.Repositories {
    public class UsersRepository : IUsersRepository {
        private readonly IDbConnection _conn;

        public UsersRepository(Connection connection) {
            _conn = connection.GetConnection();
        }

        public IEnumerable<UsersDTO> GetAll() {
            return _conn.Query<UsersDTO>("SELECT * FROM Users");
        }

        public UsersDTO? GetById(int id) {
            return _conn.QueryFirstOrDefault<UsersDTO>(
                $"SELECT * FROM Users WHERE id = @id",
                new { id }
            );
        }

        public void Insert(UsersDTO dto) {
            _conn.Execute(@"
                INSERT INTO Users (name, login, password, email, role, observations, active, register, profile)
                VALUES (@name, @login, @password, @email, @role, @observations, @active, GETDATE(), @profile)", dto
            );
        }

        public void Update(UsersDTO dto) {
            if (!string.IsNullOrWhiteSpace(dto.password) && !(dto.profile != null)) {
                _conn.Execute(@"
                    UPDATE 
                        Users
                    SET 
                        name = @name, 
                        login = @login, 
                        password = @password, 
                        email = @email, 
                        role = @role, 
                        observations = @observations, 
                        active = @active,
                        edit = GETDATE(),
                        profile = @profile
                    WHERE 
                        id = @id", dto
                );
            } else {
                _conn.Execute(@"
                UPDATE 
                    Users
                SET 
                    name = @name, 
                    login = @login,
                    email = @email, 
                    role = @role, 
                    observations = @observations, 
                    active = @active,
                    edit = GETDATE(),
                WHERE 
                    id = @id", dto
                );
            }
        }

        public void Delete(int id) {
            _conn.Execute("DELETE FROM Users WHERE id = @id", new { id });
        }

        public UsersDTO? Login(string login, string password) {
            var passwordHash = HashHelper.CreateSha256(password);

            return _conn.QueryFirstOrDefault<UsersDTO>(
                @"SELECT * FROM Users WHERE (login = @login OR email = @login) AND password = @passwordHash",
                new { login, passwordHash }
            );
        }
    }
}