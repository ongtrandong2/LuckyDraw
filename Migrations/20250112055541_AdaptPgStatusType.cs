using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AdaptPgStatusType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""PG""
                ALTER COLUMN ""Status"" TYPE INTEGER
                USING CASE
                    WHEN ""Status"" = '1' THEN 1
                    WHEN ""Status"" = '0' THEN 0
                    ELSE NULL
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""PG""
                ALTER COLUMN ""Status"" TYPE TEXT
                USING CASE
                    WHEN ""Status"" = 1 THEN '1'
                    WHEN ""Status"" = 0 THEN '0'
                    ELSE 'Unknown'
                END;
            ");
        }
    }
}
