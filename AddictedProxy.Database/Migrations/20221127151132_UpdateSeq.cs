using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SELECT SETVAL('public.\"Episodes_Id_seq\"', COALESCE(MAX(\"Id\"), 1) ) FROM public.\"Episodes\";");
            migrationBuilder.Sql("SELECT SETVAL('public.\"Seasons_Id_seq\"', COALESCE(MAX(\"Id\"), 1) ) FROM public.\"Seasons\";");
            migrationBuilder.Sql("SELECT SETVAL('public.\"Subtitles_Id_seq\"', COALESCE(MAX(\"Id\"), 1) ) FROM public.\"Subtitles\";");
            migrationBuilder.Sql("SELECT SETVAL('public.\"TvShows_Id_seq\"', COALESCE(MAX(\"Id\"), 1) ) FROM public.\"TvShows\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
