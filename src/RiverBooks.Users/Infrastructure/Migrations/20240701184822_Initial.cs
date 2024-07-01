using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverBooks.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItem_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "Users",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserStreetAddress",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StreetAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress_State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress_Street1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress_Street2 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStreetAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStreetAddress_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "Users",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ApplicationUserId",
                schema: "Users",
                table: "CartItem",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStreetAddress_ApplicationUserId",
                schema: "Users",
                table: "UserStreetAddress",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItem",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "UserStreetAddress",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "ApplicationUsers",
                schema: "Users");
        }
    }
}
