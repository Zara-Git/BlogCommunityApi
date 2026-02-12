using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogCommunityApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================
            // UP = Vad som ska skapas i databasen när migrationen körs
            // (dvs. skapa tabeller, relationer, seed-data, index)
            // =========================

            // 1) Skapar tabellen "Categories"
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    // Id = primärnyckel, auto-increment (Identity)
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    // Name = kategorinamn (obligatoriskt)
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    // Sätter primärnyckel för Categories
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            // 2) Skapar tabellen "Users"
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    // Id = primärnyckel, auto-increment
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    // Username = max 50 tecken, obligatoriskt
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),

                    // Email = max 100 tecken, obligatoriskt
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),

                    // PasswordHash = hashat lösenord (sparas inte i klartext)
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    // Sätter primärnyckel för Users
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            // 3) Skapar tabellen "Posts"
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    // Id = primärnyckel, auto-increment
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    // Title = max 120 tecken, obligatoriskt
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),

                    // Text = innehåll i inlägget, obligatoriskt
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),

                    // CreatedAt = när posten skapades
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),

                    // UserId = foreign key till Users (författaren)
                    UserId = table.Column<int>(type: "int", nullable: false),

                    // CategoryId = foreign key till Categories
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    // Primärnyckel för Posts
                    table.PrimaryKey("PK_Posts", x => x.Id);

                    // FK: Posts -> Categories (CategoryId)
                    // onDelete: Cascade betyder att om en kategori tas bort, tas dess posts bort automatiskt
                    table.ForeignKey(
                        name: "FK_Posts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    // FK: Posts -> Users (UserId)
                    // onDelete: Cascade betyder att om en user tas bort, tas userns posts bort automatiskt
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 4) Skapar tabellen "Comments"
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    // Id = primärnyckel, auto-increment
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    // Text = kommentarens text, obligatoriskt
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),

                    // CreatedAt = när kommentaren skapades
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),

                    // PostId = FK till Posts (vilket inlägg kommentaren hör till)
                    PostId = table.Column<int>(type: "int", nullable: false),

                    // UserId = FK till Users (vem som skrev kommentaren)
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    // Primärnyckel för Comments
                    table.PrimaryKey("PK_Comments", x => x.Id);

                    // FK: Comments -> Posts (PostId)
                    // onDelete: Cascade betyder att om en post tas bort, tas dess comments bort automatiskt
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    // FK: Comments -> Users (UserId)
                    // onDelete: Restrict betyder att man INTE kan ta bort en user om den har kommentarer
                    // (för att skydda referenser / historik). Du måste först ta bort kommentarer eller hantera det.
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // 5) Seed-data: lägger in några standard-kategorier från start
            // (Training, Fashion, Health) så du direkt kan skapa posts i Swagger/Postman
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Training" },
                    { 2, "Fashion" },
                    { 3, "Health" }
                });

            // 6) Index: förbättrar prestanda när man söker/joinar via FK-fälten

            // Index för att snabbt hitta kommentarer per post
            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            // Index för att snabbt hitta kommentarer per user
            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            // Index för att snabbt hitta posts per kategori
            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                column: "CategoryId");

            // Index för att snabbt hitta posts per user (författare)
            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =========================
            // DOWN = Hur man "rullar tillbaka" migrationen
            // (dvs. ta bort allt som Up skapade)
            // =========================

            // Tar bort Comments först (pga foreign keys)
            migrationBuilder.DropTable(
                name: "Comments");

            // Tar bort Posts
            migrationBuilder.DropTable(
                name: "Posts");

            // Tar bort Categories
            migrationBuilder.DropTable(
                name: "Categories");

            // Tar bort Users
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
