using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Embroidery.Client.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 388, nullable: false),
                    SizeInKb = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ImageThumbnail = table.Column<byte[]>(type: "BLOB", nullable: true),
                    FileHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    LikeFileId = table.Column<int>(type: "INTEGER", nullable: true),
                    Length = table.Column<byte>(type: "INTEGER", nullable: true),
                    Width = table.Column<byte>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Files_LikeFileId",
                        column: x => x.LikeFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    FileId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileHash",
                table: "Files",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_Files_LikeFileId",
                table: "Files",
                column: "LikeFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_Path_Name",
                table: "Files",
                columns: new[] { "Path", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_FileId",
                table: "Tags",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
