using FluentMigrator;

namespace LibraryAPI.Data.Migrations {

    [Migration(2026021301)]
    public class _2026021301_CreateUsers : Migration {
        public override void Up() {
            if (!Schema.Table("Users").Exists()) {
                Create.Table("Users")
                    .WithColumn("id").AsInt32().Identity().NotNullable()
                    .WithColumn("name").AsString(150).NotNullable()
                    .WithColumn("login").AsString(50).NotNullable()
                    .WithColumn("password").AsString(50).NotNullable()
                    .WithColumn("email").AsString(100).NotNullable()
                    .WithColumn("role").AsString(25).NotNullable()
                    .WithColumn("observations").AsString(500).Nullable()
                    .WithColumn("active").AsBoolean().NotNullable()
                    .WithColumn("register").AsDateTime().NotNullable()
                    .WithColumn("edit").AsDateTime().Nullable();

                Create.PrimaryKey("PK_Users")
                    .OnTable("Users")
                    .Column("id");

                Create.Index("UK_Users_login")
                    .OnTable("Users")
                    .OnColumn("login").Ascending()
                    .WithOptions().Unique();

                Create.Index("UK_Users_email")
                    .OnTable("Users")
                    .OnColumn("email").Ascending()
                    .WithOptions().Unique();
            }
        }

        public override void Down() {
            if (Schema.Table("Users").Exists())
                Delete.Table("Users");
        }
    }
}