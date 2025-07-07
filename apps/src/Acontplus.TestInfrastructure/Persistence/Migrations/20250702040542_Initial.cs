using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acontplus.TestHostApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Config");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<int>(type: "int", nullable: false),
                    FromMobile = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WhatsAppUsage",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Used = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Limit = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Unlimited = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<int>(type: "int", nullable: false),
                    FromMobile = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppUsage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CreatedAt",
                table: "Usuarios",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CreatedByUserId",
                table: "Usuarios",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IsActive",
                table: "Usuarios",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IsDeleted",
                table: "Usuarios",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppUsage_CreatedAt",
                schema: "Config",
                table: "WhatsAppUsage",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppUsage_CreatedByUserId",
                schema: "Config",
                table: "WhatsAppUsage",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppUsage_IsActive",
                schema: "Config",
                table: "WhatsAppUsage",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppUsage_IsDeleted",
                schema: "Config",
                table: "WhatsAppUsage",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "UX_WhatsAppUsage_CompanyId",
                schema: "Config",
                table: "WhatsAppUsage",
                column: "CompanyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "WhatsAppUsage",
                schema: "Config");
        }
    }
}
