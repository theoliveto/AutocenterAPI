using FluentMigrator;

namespace AutocenterAPI.Data.Migrations {

    [Migration(2026021303)]
    public class _2026021303_CreateAdminUser : Migration {
        public override void Up() {
            Execute.Sql(@"
                IF NOT EXISTS 
                    (SELECT 1 FROM Users WHERE login = 'admin')
                BEGIN
                    INSERT INTO Users (name, login, password, email, role, observations, active, register, edit)
                    VALUES (
                        'Administrator', 
                        'admin', 
                        'fcb9138ed7f6215c90804783ccaece440a5658f878fc4fd28adb76ecebbdc5f3', 
                        'oliveto.vinicius@uceff.edu.br', 
                        'Administrator', 
                        'Default administrator user',
                        1,
                        GETDATE(),
                        GETDATE()
                    )
                END
            ");
        }

        public override void Down() {
            Execute.Sql(@"DELETE FROM Users WHERE login = 'admin'");
        }
    }
}