using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CareerConnect.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSampleSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.Sql("""
                DELETE FROM [Jobs]
                WHERE ([Id] = 1 AND [CompanyId] = 1 AND [RecruiterId] = 2 AND [Title] = N'Software Engineer' AND [Description] = N'C# Developer needed')
                   OR ([Id] = 2 AND [CompanyId] = 2 AND [RecruiterId] = 3 AND [Title] = N'Frontend Developer' AND [Description] = N'React expert')
                   OR ([Id] = 3 AND [CompanyId] = 3 AND [RecruiterId] = 4 AND [Title] = N'Intern' AND [Description] = N'Learn .NET');

                DELETE FROM [Users]
                WHERE [PasswordHash] = N'dummy_hash_for_testing'
                  AND (([Id] = 1 AND [Email] = N'admin@test.com')
                    OR ([Id] = 2 AND [Email] = N'recruiter1@test.com')
                    OR ([Id] = 3 AND [Email] = N'recruiter2@test.com')
                    OR ([Id] = 4 AND [Email] = N'recruiter3@test.com')
                    OR ([Id] = 5 AND [Email] = N'jobseeker1@test.com')
                    OR ([Id] = 6 AND [Email] = N'jobseeker2@test.com')
                    OR ([Id] = 7 AND [Email] = N'jobseeker3@test.com')
                    OR ([Id] = 8 AND [Email] = N'jobseeker4@test.com')
                    OR ([Id] = 9 AND [Email] = N'jobseeker5@test.com'));

                DELETE FROM [Companies]
                WHERE ([Id] = 1 AND [Name] = N'Tech Corp'
                       OR [Id] = 2 AND [Name] = N'Startup Inc'
                       OR [Id] = 3 AND [Name] = N'Global Solutions')
                  AND NOT EXISTS (SELECT 1 FROM [Recruiters] WHERE [CompanyId] = [Companies].[Id])
                  AND NOT EXISTS (SELECT 1 FROM [Jobs] WHERE [CompanyId] = [Companies].[Id]);

                DELETE FROM [Skills]
                WHERE ([Id] = 1 AND [Name] = N'C#'
                       OR [Id] = 2 AND [Name] = N'React'
                       OR [Id] = 3 AND [Name] = N'SQL Server'
                       OR [Id] = 4 AND [Name] = N'TypeScript'
                       OR [Id] = 5 AND [Name] = N'ASP.NET Core')
                  AND NOT EXISTS (SELECT 1 FROM [JobSkills] WHERE [SkillId] = [Skills].[Id])
                  AND NOT EXISTS (SELECT 1 FROM [JobSeekerSkills] WHERE [SkillId] = [Skills].[Id]);
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Jobs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Jobs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Name",
                table: "Skills",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Location",
                table: "Jobs",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Title",
                table: "Jobs",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Skills_Name",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_Location",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_Title",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name",
                table: "Companies");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
