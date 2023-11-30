using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class TriggerForUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 CREATE OR REPLACE FUNCTION updated_set_now()
                                 RETURNS TRIGGER AS $$
                                 BEGIN
                                   NEW.UpdatedAt := now() at time zone 'utc';
                                   RETURN NEW;
                                 END;
                                 $$ LANGUAGE plpgsql;
                                 """);

            foreach (var table in new[]{"AddictedUserCredentials", "Episodes", "OneTimeMigrationRelease", "Seasons", "Subtitles", "TvShows"})
            {
                migrationBuilder.Sql($"""
                                      CREATE OR REPLACE TRIGGER updated_at_trigger_{table}
                                      BEFORE UPDATE ON "{table}"
                                      FOR EACH ROW EXECUTE PROCEDURE updated_set_now();
                                      """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var table in new[]{"AddictedUserCredentials", "Episodes", "OneTimeMigrationRelease", "Seasons", "Subtitles", "TvShows"})
            {
                migrationBuilder.Sql($"""
                                      DROP TRIGGER updateAtTrigger{table};
                                      """);
            }
            
            migrationBuilder.Sql("""
                                 DROP FUNCTION updatedAtAsNow();
                                 """);
        }
    }
}