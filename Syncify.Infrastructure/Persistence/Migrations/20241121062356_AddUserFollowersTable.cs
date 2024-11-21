using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syncify.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFollowersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFollowers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowedId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FollowerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FollowedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowers", x => x.Id);
                    table.CheckConstraint("CHK_NoSelfFollow", "FollowedId <> FollowerId");
                    table.ForeignKey(
                        name: "FK_UserFollowers_AspNetUsers_FollowedId",
                        column: x => x.FollowedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserFollowers_AspNetUsers_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowers_Followed_Follower",
                table: "UserFollowers",
                columns: new[] { "FollowedId", "FollowerId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowers_FollowerId",
                table: "UserFollowers",
                column: "FollowerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollowers");
        }
    }
}
