using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverBooks.EmailSending.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "EmailSending");

            migrationBuilder.CreateTable(
                name: "EmailOutboxItems",
                schema: "EmailSending",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTimeUtcProcessed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOutboxItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutboxItems_DateTimeUtcProcessed",
                schema: "EmailSending",
                table: "EmailOutboxItems",
                column: "DateTimeUtcProcessed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailOutboxItems",
                schema: "EmailSending");
        }
    }
}
