using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class KindOfProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KindOfProducts",
                columns: table => new
                {
                    KindOfProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Detail = table.Column<string>(type: "text", nullable: false),
                    Create = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Update = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KindOfProducts", x => x.KindOfProductId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Detail = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Create = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Update = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    KindOfProductId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_KindOfProducts_KindOfProductId",
                        column: x => x.KindOfProductId,
                        principalTable: "KindOfProducts",
                        principalColumn: "KindOfProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "KindOfProducts",
                columns: new[] { "KindOfProductId", "Create", "Detail", "Name", "Update" },
                values: new object[] { 1, new DateTime(2023, 11, 16, 18, 11, 4, 817, DateTimeKind.Local).AddTicks(4802), "Laptops", "Laptop", new DateTime(2023, 11, 16, 18, 11, 4, 817, DateTimeKind.Local).AddTicks(4848) });

            migrationBuilder.CreateIndex(
                name: "IX_Products_KindOfProductId",
                table: "Products",
                column: "KindOfProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "KindOfProducts");
        }
    }
}
