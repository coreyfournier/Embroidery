using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Embroidery.Client.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 75, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 2058, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 4, nullable: true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 388, nullable: false),
                    SizeInKb = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ImageThumbnail = table.Column<byte[]>(type: "BLOB", nullable: true),
                    FileHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    CleanName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Length = table.Column<byte>(type: "INTEGER", nullable: true),
                    Width = table.Column<byte>(type: "INTEGER", nullable: true),
                    FontSize = table.Column<float>(type: "REAL", nullable: false),
                    Letter = table.Column<char>(type: "TEXT", nullable: false),
                    FolderId = table.Column<int>(type: "INTEGER", nullable: false),
                    HasError = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FolderId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileTagRelationship",
                columns: table => new
                {
                    FileId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTagRelationship", x => new { x.TagId, x.FileId });
                    table.ForeignKey(
                        name: "FK_FileTagRelationship_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileTagRelationship_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_CleanName",
                table: "Files",
                column: "CleanName");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileHash",
                table: "Files",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FolderId_Name",
                table: "Files",
                columns: new[] { "FolderId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_HasError",
                table: "Files",
                column: "HasError");

            migrationBuilder.CreateIndex(
                name: "IX_FileTagRelationship_FileId",
                table: "FileTagRelationship",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_Path",
                table: "Folders",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Key",
                table: "Settings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_FolderId",
                table: "Tags",
                column: "FolderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileTagRelationship");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Folders");
        }
    }
}
