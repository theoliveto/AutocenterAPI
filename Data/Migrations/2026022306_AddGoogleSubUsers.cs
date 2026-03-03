using FluentMigrator;

namespace AutocenterAPI.Data.Migrations {
    [Migration(2026022306)]
    public class _2026022306_AddGoogleSubUsers : Migration {
        public override void Up() {
            Alter.Table("Users")
                .AddColumn("googleSub")
                .AsString(100)
                .Nullable();

            Execute.Sql(@"
                CREATE UNIQUE INDEX UX_Users_googleSub
                ON Users(googleSub)
                WHERE googleSub IS NOT NULL;
            ");
        }

        public override void Down() {
            Execute.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = 'UX_Users_googleSub'
                    AND object_id = OBJECT_ID('Users')
                )
                DROP INDEX UX_Users_googleSub ON Users;
            ");

            Delete.Column("googleSub")
                .FromTable("Users");
        }
    }
}