using FluentMigrator;

namespace LibraryAPI.Data.Migrations {

    [Migration(2026021302)]
    public class _2026021302_AlterPasswordLength : Migration {
        public override void Up() {
            Alter.Column("password")
                .OnTable("Users")
                .AsString(100)
                .NotNullable();
        }

        public override void Down() {
            Alter.Column("password")
                .OnTable("Users")
                .AsString(50)
                .NotNullable();
        }
    }
}