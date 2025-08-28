using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarInsurance.Api.Migrations
{
    /// <inheritdoc />
    public partial class Policies_EndDate_NotNull_Rebuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Policies SET EndDate = '2099-12-31' WHERE EndDate IS NULL;");

            migrationBuilder.CreateTable(
                name: "Policies_tmp",
                columns: table => new
                {
                    Id        = table.Column<int>(nullable: false)
                                .Annotation("Sqlite:Autoincrement", true),
                    CarId     = table.Column<int>(nullable: false),
                    Provider  = table.Column<string>(nullable: false),
                    StartDate = table.Column<string>(nullable: false),
                    EndDate   = table.Column<string>(nullable: false) 
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies_tmp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Policies_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO Policies_tmp (Id, CarId, Provider, StartDate, EndDate)
                SELECT Id, CarId, Provider, StartDate, EndDate FROM Policies;
            """);

            migrationBuilder.DropTable(name: "Policies");
            migrationBuilder.Sql(@"ALTER TABLE Policies_tmp RENAME TO Policies;");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_Policies_CarId ON Policies(CarId);");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
