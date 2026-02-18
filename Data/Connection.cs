using System.Data;
using Microsoft.Data.SqlClient;

namespace LibraryAPI.Data {
    public class Connection {
        private readonly IConfiguration _configuration;

        public Connection(IConfiguration configuration) {
            _configuration = configuration;
        }

        public IDbConnection GetConnection() {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}