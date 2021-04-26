using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TestAPI.Migrations.CurbsideContextMigration
{
    public partial class AddImageUrlTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "ImageUrls",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        UpdatedAt = table.Column<DateTime>(nullable: false),
            //        Url = table.Column<string>(nullable: true),
            //        ThumbUrl = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ImageUrls", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Products",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        UpdatedAt = table.Column<DateTime>(nullable: false),
            //        ImageId = table.Column<int>(nullable: false),
            //        Name = table.Column<string>(nullable: true),
            //        Description = table.Column<string>(nullable: true),
            //        Price = table.Column<double>(nullable: false),
            //        Discount = table.Column<double>(nullable: false),
            //        IsDiscounted = table.Column<bool>(nullable: false),
            //        IsDiscountPercentage = table.Column<bool>(nullable: false),
            //        Quantity = table.Column<int>(nullable: false),
            //        Status = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Products", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Products_ImageUrls_ImageId",
            //            column: x => x.ImageId,
            //            principalTable: "ImageUrls",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Profiles",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        UpdatedAt = table.Column<DateTime>(nullable: false),
            //        FirstName = table.Column<string>(nullable: true),
            //        LastName = table.Column<string>(nullable: true),
            //        Email = table.Column<string>(nullable: true),
            //        Password = table.Column<string>(nullable: false),
            //        ImageId = table.Column<int>(nullable: false),
            //        ImageFileId = table.Column<int>(nullable: true),
            //        SocialMediaId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Profiles", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Profiles_ImageUrls_ImageFileId",
            //            column: x => x.ImageFileId,
            //            principalTable: "ImageUrls",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductImages",
            //    columns: table => new
            //    {
            //        ImageId = table.Column<int>(nullable: false),
            //        ProductId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductImages", x => x.ImageId);
            //        table.ForeignKey(
            //            name: "FK_ProductImages_ImageUrls_ImageId",
            //            column: x => x.ImageId,
            //            principalTable: "ImageUrls",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ProductImages_Products_ProductId",
            //            column: x => x.ProductId,
            //            principalTable: "Products",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductItemCategories",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        UpdatedAt = table.Column<DateTime>(nullable: false),
            //        ProductId = table.Column<int>(nullable: false),
            //        Category = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductItemCategories", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ProductItemCategories_Products_ProductId",
            //            column: x => x.ProductId,
            //            principalTable: "Products",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SocialMedias",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        UpdatedAt = table.Column<DateTime>(nullable: false),
            //        SocialId = table.Column<string>(nullable: true),
            //        ProfileId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SocialMedias", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_SocialMedias_Profiles_ProfileId",
            //            column: x => x.ProfileId,
            //            principalTable: "Profiles",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductImages_ProductId",
            //    table: "ProductImages",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductItemCategories_ProductId",
            //    table: "ProductItemCategories",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Products_ImageId",
            //    table: "Products",
            //    column: "ImageId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Profiles_ImageFileId",
            //    table: "Profiles",
            //    column: "ImageFileId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SocialMedias_ProfileId",
            //    table: "SocialMedias",
            //    column: "ProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "ProductItemCategories");

            migrationBuilder.DropTable(
                name: "SocialMedias");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "ImageUrls");
        }
    }
}
