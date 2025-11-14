using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schematics.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLineCategoriesAndStationLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LineCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineCategories", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "LineCategoryId",
                table: "Lines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Lines_LineCategoryId",
                table: "Lines",
                column: "LineCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lines_LineCategories_LineCategoryId",
                table: "Lines",
                column: "LineCategoryId",
                principalTable: "LineCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateTable(
                name: "StationLines",
                columns: table => new
                {
                    StationId = table.Column<int>(type: "int", nullable: false),
                    LineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationLines", x => new { x.StationId, x.LineId });
                    table.ForeignKey(
                        name: "FK_StationLines_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StationLines_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StationLines_LineId",
                table: "StationLines",
                column: "LineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StationLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Lines_LineCategories_LineCategoryId",
                table: "Lines");

            migrationBuilder.DropIndex(
                name: "IX_Lines_LineCategoryId",
                table: "Lines");

            migrationBuilder.DropColumn(
                name: "LineCategoryId",
                table: "Lines");

            migrationBuilder.DropTable(
                name: "LineCategories");
        }
    }
}
