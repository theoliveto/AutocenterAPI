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
            var hasPassword = !string.IsNullOrWhiteSpace(dto.password);
            var hasProfile = dto.profile != null && dto.profile.Length > 0;

            if (hasPassword && hasProfile) {
                _conn.Execute(@"
                    UPDATE Users 
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
            } else if (hasPassword && !hasProfile) {
                _conn.Execute(@"
                    UPDATE Users 
                    SET
                        name = @name,
                        login = @login,
                        password = @password,
                        email = @email,
                        role = @role,
                        observations = @observations,
                        active = @active,
                        edit = GETDATE()
                    WHERE 
                        id = @id", dto
                );
            } else if (!hasPassword && hasProfile) {
                _conn.Execute(@"
                    UPDATE Users 
                    SET
                        name = @name,
                        login = @login,
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
                    UPDATE Users 
                    SET
                        name = @name,
                        login = @login,
                        email = @email,
                        role = @role,
                        observations = @observations,
                        active = @active,
                        edit = GETDATE()
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

        public bool LoginExists(string login, int? ignoreId = null) {
            return _conn.ExecuteScalar<int>(
                @" SELECT COUNT(1) FROM Users WHERE login = @login AND (@ignoreId IS NULL OR id <> @ignoreId)", 
                new { login, ignoreId }
            ) > 0;
        }

        public bool EmailExists(string email, int? ignoreId = null) {
            return _conn.ExecuteScalar<int>(
                @"SELECT COUNT(1) FROM Users WHERE email = @email AND (@ignoreId IS NULL OR id <> @ignoreId)", 
                new { email, ignoreId }
            ) > 0;
        }

        public UsersDTO? GetByEmail(string email) {
            const string sql = @"
                SELECT TOP 1 id, name, login, password, email
                FROM Users
                WHERE LOWER(email) = LOWER(@email) AND active = 1;
            ";

            return _conn.QueryFirstOrDefault<UsersDTO>(sql, new { email });
        }

        public void CreatePasswordResetToken(int userId, byte[] tokenHash, DateTime expiresAtUtc) {
            const string sql = @"
                INSERT INTO PasswordResetTokens (id, userId, tokenHash, expiresAtUtc, usedAtUtc, createdAtUtc)
                VALUES (NEWID(), @userId, @tokenHash, @expiresAtUtc, NULL, SYSUTCDATETIME());
            ";

            _conn.Execute(sql, new { userId, tokenHash, expiresAtUtc });
        }

        public bool TryUsePasswordResetToken(int userId, byte[] tokenHash, DateTime nowUtc) {
            const string sql = @"
                UPDATE 
                    PasswordResetTokens
                SET 
                    usedAtUtc = @nowUtc
                WHERE 
                    userId = @userId
                    AND tokenHash = @tokenHash
                    AND usedAtUtc IS NULL
                    AND expiresAtUtc > @nowUtc;
            ";

            var rows = _conn.Execute(sql, new { userId, tokenHash, nowUtc });
            return rows > 0;
        }

        public void UpdatePassword(int userId, string passwordHashSha256) {
            const string sql = @"
                UPDATE Users
                SET 
                    password = @passwordHashSha256,
                    edit = GETDATE()
                WHERE id = @userId;
            ";

            _conn.Execute(sql, new { userId, passwordHashSha256 });
        }
    }
}