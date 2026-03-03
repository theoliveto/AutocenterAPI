using FluentMigrator;

namespace AutocenterAPI.Data.Migrations {
    [Migration(2026022005)]
    public class _2026022005_CreatePasswordResetTokens : Migration {
        public override void Up() {
            if (!Schema.Table("PasswordResetTokens").Exists()) {

                Create.Table("PasswordResetTokens")
                    .WithColumn("id").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
                    .WithColumn("userId").AsInt32().NotNullable()
                    .WithColumn("tokenHash").AsBinary(32).NotNullable()
                    .WithColumn("expiresAtUtc").AsDateTime2().NotNullable()
                    .WithColumn("usedAtUtc").AsDateTime2().Nullable()
                    .WithColumn("createdAtUtc").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

                Create.ForeignKey("FK_PasswordResetTokens_Users")
                    .FromTable("PasswordResetTokens").ForeignColumn("userId")
                    .ToTable("Users").PrimaryColumn("id");

                Create.Index("IX_PasswordResetTokens_user_valid")
                    .OnTable("PasswordResetTokens")
                    .OnColumn("userId").Ascending()
                    .OnColumn("expiresAtUtc").Ascending()
                    .OnColumn("usedAtUtc").Ascending();
            }
        }

        public override void Down() {
            if (Schema.Table("PasswordResetTokens").Exists())
                Delete.Table("PasswordResetTokens");
        }
    }
}