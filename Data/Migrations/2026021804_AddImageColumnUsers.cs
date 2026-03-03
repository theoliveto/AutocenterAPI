using FluentMigrator;

namespace AutocenterAPI.Data.Migrations {

    [Migration(2026021804)]
    public class _2026021804_AddImageColumnUsers : Migration {
        public override void Up() {
            Alter.Table("Users")
                .AddColumn("profile")
                .AsCustom("VARBINARY(MAX)")
                .Nullable();
        }

        public override void Down() {
            Delete.Column("profile")
                .FromTable("Users");
        }
    }
}