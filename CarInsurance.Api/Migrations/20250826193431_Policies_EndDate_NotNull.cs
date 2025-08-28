using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarInsurance.Api.Migrations
{
    /// <inheritdoc />
    public partial class Policies_EndDate_NotNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Policies SET EndDate = '2099-12-31' WHERE EndDate IS NULL;");
            migrationBuilder.Sql("""
            PRAGMA foreign_keys=off;
            CREATE TABLE IF NOT EXISTS Policies_tmp (
            Id        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            CarId     INTEGER NOT NULL,
            Provider  TEXT    NOT NULL,
            StartDate TEXT    NOT NULL,
            EndDate   TEXT    NOT NULL,
            CONSTRAINT FK_Policies_Cars_CarId FOREIGN KEY (CarId) REFERENCES Cars (Id) ON DELETE CASCADE
            );
            INSERT INTO Policies_tmp (Id,CarId,Provider,StartDate,EndDate)
            SELECT Id,CarId,Provider,StartDate,EndDate FROM Policies;
            DROP TABLE Policies;
            ALTER TABLE Policies_tmp RENAME TO Policies;
            PRAGMA foreign_keys=on;
            """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
