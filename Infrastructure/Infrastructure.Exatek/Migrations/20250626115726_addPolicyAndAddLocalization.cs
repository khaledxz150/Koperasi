using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addPolicyAndAddLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryLocalizations_Dictionaries_ID",
                schema: "User",
                table: "DictionaryLocalizations");

            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryLocalizations_Languages_LanguageID",
                schema: "User",
                table: "DictionaryLocalizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Languages",
                schema: "User",
                table: "Languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DictionaryLocalizations",
                schema: "User",
                table: "DictionaryLocalizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dictionaries",
                schema: "User",
                table: "Dictionaries");

            migrationBuilder.EnsureSchema(
                name: "DictionaryLocalization");

            migrationBuilder.EnsureSchema(
                name: "Languages");

            migrationBuilder.EnsureSchema(
                name: "Localization");

            migrationBuilder.EnsureSchema(
                name: "PolicyLocalization");

            migrationBuilder.RenameTable(
                name: "Languages",
                schema: "User",
                newName: "Dictionary",
                newSchema: "Languages");

            migrationBuilder.RenameTable(
                name: "DictionaryLocalizations",
                schema: "User",
                newName: "Dictionary",
                newSchema: "DictionaryLocalization");

            migrationBuilder.RenameTable(
                name: "Dictionaries",
                schema: "User",
                newName: "Dictionary",
                newSchema: "Localization");

            migrationBuilder.RenameIndex(
                name: "IX_DictionaryLocalizations_LanguageID",
                schema: "DictionaryLocalization",
                table: "Dictionary",
                newName: "IX_Dictionary_LanguageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dictionary",
                schema: "Languages",
                table: "Dictionary",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dictionary",
                schema: "DictionaryLocalization",
                table: "Dictionary",
                columns: new[] { "ID", "LanguageID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dictionary",
                schema: "Localization",
                table: "Dictionary",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "System",
                schema: "PolicyLocalization",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System", x => new { x.ID, x.LanguageID });
                    table.ForeignKey(
                        name: "FK_System_Dictionary_LanguageID",
                        column: x => x.LanguageID,
                        principalSchema: "Languages",
                        principalTable: "Dictionary",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "DictionaryLocalization",
                table: "Dictionary",
                columns: new[] { "ID", "LanguageID", "Description" },
                values: new object[,]
                {
                    { 186, 1, "Mobile OTP accepted, proceed" },
                    { 186, 2, "تم قبول رمز التحقق، تابع الخطوات التالية" }
                });

            migrationBuilder.InsertData(
                schema: "PolicyLocalization",
                table: "System",
                columns: new[] { "ID", "LanguageID", "Content" },
                values: new object[,]
                {
                    { 1, 1, "Lorem ipsum policy text in English." },
                    { 1, 2, "لوريم إيبسوم نص السياسات باللغة العربية." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_System_LanguageID",
                schema: "PolicyLocalization",
                table: "System",
                column: "LanguageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dictionary_Dictionary_ID",
                schema: "DictionaryLocalization",
                table: "Dictionary",
                column: "ID",
                principalSchema: "Localization",
                principalTable: "Dictionary",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dictionary_Dictionary_LanguageID",
                schema: "DictionaryLocalization",
                table: "Dictionary",
                column: "LanguageID",
                principalSchema: "Languages",
                principalTable: "Dictionary",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictionary_Dictionary_ID",
                schema: "DictionaryLocalization",
                table: "Dictionary");

            migrationBuilder.DropForeignKey(
                name: "FK_Dictionary_Dictionary_LanguageID",
                schema: "DictionaryLocalization",
                table: "Dictionary");

            migrationBuilder.DropTable(
                name: "System",
                schema: "PolicyLocalization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dictionary",
                schema: "Localization",
                table: "Dictionary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dictionary",
                schema: "Languages",
                table: "Dictionary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dictionary",
                schema: "DictionaryLocalization",
                table: "Dictionary");

            migrationBuilder.DeleteData(
                schema: "DictionaryLocalization",
                table: "Dictionary",
                keyColumns: new[] { "ID", "LanguageID" },
                keyValues: new object[] { 186, 1 });

            migrationBuilder.DeleteData(
                schema: "DictionaryLocalization",
                table: "Dictionary",
                keyColumns: new[] { "ID", "LanguageID" },
                keyValues: new object[] { 186, 2 });

            migrationBuilder.RenameTable(
                name: "Dictionary",
                schema: "Localization",
                newName: "Dictionaries",
                newSchema: "User");

            migrationBuilder.RenameTable(
                name: "Dictionary",
                schema: "Languages",
                newName: "Languages",
                newSchema: "User");

            migrationBuilder.RenameTable(
                name: "Dictionary",
                schema: "DictionaryLocalization",
                newName: "DictionaryLocalizations",
                newSchema: "User");

            migrationBuilder.RenameIndex(
                name: "IX_Dictionary_LanguageID",
                schema: "User",
                table: "DictionaryLocalizations",
                newName: "IX_DictionaryLocalizations_LanguageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dictionaries",
                schema: "User",
                table: "Dictionaries",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Languages",
                schema: "User",
                table: "Languages",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DictionaryLocalizations",
                schema: "User",
                table: "DictionaryLocalizations",
                columns: new[] { "ID", "LanguageID" });

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryLocalizations_Dictionaries_ID",
                schema: "User",
                table: "DictionaryLocalizations",
                column: "ID",
                principalSchema: "User",
                principalTable: "Dictionaries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryLocalizations_Languages_LanguageID",
                schema: "User",
                table: "DictionaryLocalizations",
                column: "LanguageID",
                principalSchema: "User",
                principalTable: "Languages",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
