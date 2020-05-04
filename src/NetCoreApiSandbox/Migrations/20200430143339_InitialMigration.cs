namespace NetCoreApiSandbox.Migrations
{
    #region

    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    #endregion

    public partial class InitialMigration: Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("Persons",
                                         table => new
                                         {
                                             id = table.Column<int>(nullable: false)
                                                       .Annotation("Sqlite:Autoincrement", true),
                                             Username = table.Column<string>(nullable: true),
                                             Email = table.Column<string>(nullable: true),
                                             Bio = table.Column<string>(nullable: true),
                                             Image = table.Column<string>(nullable: true),
                                             Hash = table.Column<byte[]>(nullable: true),
                                             Salt = table.Column<byte[]>(nullable: true)
                                         },
                                         constraints: table => { table.PrimaryKey("PK_Persons", x => x.id); });

            migrationBuilder.CreateTable("Tags",
                                         table => new { id = table.Column<string>(nullable: false) },
                                         constraints: table => { table.PrimaryKey("PK_Tags", x => x.id); });

            migrationBuilder.CreateTable("Articles",
                                         table => new
                                         {
                                             id = table.Column<int>(nullable: false)
                                                       .Annotation("Sqlite:Autoincrement", true),
                                             Slug = table.Column<string>(nullable: true),
                                             Title = table.Column<string>(nullable: true),
                                             Description = table.Column<string>(nullable: true),
                                             Body = table.Column<string>(nullable: true),
                                             AuthorId = table.Column<int>(nullable: true),
                                             CreatedAt = table.Column<DateTime>(nullable: false),
                                             UpdatedAt = table.Column<DateTime>(nullable: false)
                                         },
                                         constraints: table =>
                                         {
                                             table.PrimaryKey("PK_Articles", x => x.id);

                                             table.ForeignKey("FK_Articles_Persons_AuthorId",
                                                              x => x.AuthorId,
                                                              "Persons",
                                                              "id",
                                                              onDelete: ReferentialAction.Restrict);
                                         });

            migrationBuilder.CreateTable("FollowedPeople",
                                         table => new
                                         {
                                             ObserverId = table.Column<int>(nullable: false),
                                             TargetId = table.Column<int>(nullable: false)
                                         },
                                         constraints: table =>
                                         {
                                             table.PrimaryKey("PK_FollowedPeople",
                                                              x => new { x.ObserverId, x.TargetId });

                                             table.ForeignKey("FK_FollowedPeople_Persons_ObserverId",
                                                              x => x.ObserverId,
                                                              "Persons",
                                                              "id",
                                                              onDelete: ReferentialAction.Restrict);

                                             table.ForeignKey("FK_FollowedPeople_Persons_TargetId",
                                                              x => x.TargetId,
                                                              "Persons",
                                                              "id",
                                                              onDelete: ReferentialAction.Restrict);
                                         });

            migrationBuilder.CreateTable("ArticleFavorites",
                                         table => new
                                         {
                                             ArticleId = table.Column<int>(nullable: false),
                                             PersonId = table.Column<int>(nullable: false)
                                         },
                                         constraints: table =>
                                         {
                                             table.PrimaryKey("PK_ArticleFavorites",
                                                              x => new { x.ArticleId, x.PersonId });

                                             table.ForeignKey("FK_ArticleFavorites_Articles_ArticleId",
                                                              x => x.ArticleId,
                                                              "Articles",
                                                              "id",
                                                              onDelete: ReferentialAction.Cascade);

                                             table.ForeignKey("FK_ArticleFavorites_Persons_PersonId",
                                                              x => x.PersonId,
                                                              "Persons",
                                                              "id",
                                                              onDelete: ReferentialAction.Cascade);
                                         });

            migrationBuilder.CreateTable("ArticleTags",
                                         table => new
                                         {
                                             ArticleId = table.Column<int>(nullable: false),
                                             TagId = table.Column<string>(nullable: false)
                                         },
                                         constraints: table =>
                                         {
                                             table.PrimaryKey("PK_ArticleTags", x => new { x.ArticleId, x.TagId });

                                             table.ForeignKey("FK_ArticleTags_Articles_ArticleId",
                                                              x => x.ArticleId,
                                                              "Articles",
                                                              "id",
                                                              onDelete: ReferentialAction.Cascade);

                                             table.ForeignKey("FK_ArticleTags_Tags_TagId",
                                                              x => x.TagId,
                                                              "Tags",
                                                              "id",
                                                              onDelete: ReferentialAction.Cascade);
                                         });

            migrationBuilder.CreateTable("Comments",
                                         table => new
                                         {
                                             id = table.Column<int>(nullable: false)
                                                       .Annotation("Sqlite:Autoincrement", true),
                                             Body = table.Column<string>(nullable: true),
                                             AuthorId = table.Column<int>(nullable: false),
                                             ArticleId = table.Column<int>(nullable: false),
                                             CreatedAt = table.Column<DateTime>(nullable: false),
                                             UpdatedAt = table.Column<DateTime>(nullable: false)
                                         },
                                         constraints: table =>
                                         {
                                             table.PrimaryKey("PK_Comments", x => x.id);

                                             table.ForeignKey("FK_Comments_Articles_ArticleId",
                                                              x => x.ArticleId,
                                                              "Articles",
                                                              "id",
                                                              onDelete: ReferentialAction.Cascade);

                                             table.ForeignKey("FK_Comments_Persons_AuthorId",
                                                              x => x.AuthorId,
                                                              "Persons",
                                                              "id",
                                                              onDelete: ReferentialAction.Cascade);
                                         });

            migrationBuilder.CreateIndex("IX_ArticleFavorites_PersonId", "ArticleFavorites", "PersonId");

            migrationBuilder.CreateIndex("IX_Articles_AuthorId", "Articles", "AuthorId");

            migrationBuilder.CreateIndex("IX_ArticleTags_TagId", "ArticleTags", "TagId");

            migrationBuilder.CreateIndex("IX_Comments_ArticleId", "Comments", "ArticleId");

            migrationBuilder.CreateIndex("IX_Comments_AuthorId", "Comments", "AuthorId");

            migrationBuilder.CreateIndex("IX_FollowedPeople_TargetId", "FollowedPeople", "TargetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("ArticleFavorites");

            migrationBuilder.DropTable("ArticleTags");

            migrationBuilder.DropTable("Comments");

            migrationBuilder.DropTable("FollowedPeople");

            migrationBuilder.DropTable("Tags");

            migrationBuilder.DropTable("Articles");

            migrationBuilder.DropTable("Persons");
        }
    }
}
