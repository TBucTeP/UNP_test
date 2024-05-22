using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UNP.Migrations
{
    /// <inheritdoc />
    public partial class _3_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnpDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vunp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vnaimp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vnaimk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dreg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nmns = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vmns = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ckodsost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dlikv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vlikv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastChecked = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnpDatas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnpDatas");
        }
    }
}
