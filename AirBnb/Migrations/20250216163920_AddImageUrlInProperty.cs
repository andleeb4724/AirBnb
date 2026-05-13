using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirBnb.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlInProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b0e4ce79-8c8c-4071-81c4-add072ea9493", "47a882b6-6613-4ece-8286-c704f363f466" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0d69c607-8b09-4516-8c85-485a4f3d8f26", "4a1e91fd-0704-4930-a204-25ac53ae5498" });

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0d69c607-8b09-4516-8c85-485a4f3d8f26");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b0e4ce79-8c8c-4071-81c4-add072ea9493");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "47a882b6-6613-4ece-8286-c704f363f466");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4a1e91fd-0704-4930-a204-25ac53ae5498");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Properties");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0d69c607-8b09-4516-8c85-485a4f3d8f26", null, "Host", "HOST" },
                    { "b0e4ce79-8c8c-4071-81c4-add072ea9493", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "47a882b6-6613-4ece-8286-c704f363f466", 0, "9b3bc700-fe41-4248-ae79-74ec0e53586b", "admin@airbnb.com", true, "Admin User", false, null, "ADMIN@AIRBNB.COM", "ADMIN@AIRBNB.COM", "AQAAAAIAAYagAAAAEPVfnNPrbx4N7LSt8sfGQUcCCw0UNTsYkLCOB61lLiR2p+Q913FKTrUC1qbqzxzDEQ==", null, false, "5794b447-c02c-40e9-a3c3-7814ca22b70e", false, "admin@airbnb.com" },
                    { "4a1e91fd-0704-4930-a204-25ac53ae5498", 0, "3d067aea-51cb-42e9-88aa-be14e66932a8", "host@airbnb.com", true, "John Doe", false, null, "HOST@AIRBNB.COM", "HOST@AIRBNB.COM", "AQAAAAIAAYagAAAAEJEMCczqcQ16snAynrILdXzN7HReKqgQSu8smWpjgFqEz5n9phcz6ltPOQ7vtcK/Og==", null, false, "4dcf989a-b33d-4ea3-aab5-3d3dc63e5814", false, "host@airbnb.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "b0e4ce79-8c8c-4071-81c4-add072ea9493", "47a882b6-6613-4ece-8286-c704f363f466" },
                    { "0d69c607-8b09-4516-8c85-485a4f3d8f26", "4a1e91fd-0704-4930-a204-25ac53ae5498" }
                });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Description", "HostId", "PricePerNight", "Title" },
                values: new object[,]
                {
                    { 1, "A beautiful luxury apartment in the heart of NYC.", "4a1e91fd-0704-4930-a204-25ac53ae5498", 250m, "Luxury Apartment in New York" },
                    { 2, "A peaceful retreat in the mountains.", "4a1e91fd-0704-4930-a204-25ac53ae5498", 150m, "Cozy Cabin in the Woods" }
                });
        }
    }
}
