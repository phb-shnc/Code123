using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Code123.Migrations;

/// <inheritdoc />
public partial class _20261122145710_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LearningPathsModel",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LearningPathsModel", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UnitsModel",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LearningPathId = table.Column<int>(type: "int", nullable: false),
                Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                OrderIndex = table.Column<int>(type: "int", nullable: false),
                LearningPathsModelId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UnitsModel", x => x.Id);
                table.ForeignKey(
                    name: "FK_UnitsModel_LearningPathsModel_LearningPathsModelId",
                    column: x => x.LearningPathsModelId,
                    principalTable: "LearningPathsModel",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "LessonsModel",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UnitId = table.Column<int>(type: "int", nullable: false),
                Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false),
                UnitsModelId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LessonsModel", x => x.Id);
                table.ForeignKey(
                    name: "FK_LessonsModel_UnitsModel_UnitsModelId",
                    column: x => x.UnitsModelId,
                    principalTable: "UnitsModel",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "ExerciseQuestionsModel",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LessonId = table.Column<int>(type: "int", nullable: false),
                QuestionText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                Options = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                CorrectAnswer = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                LessonsModelId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExerciseQuestionsModel", x => x.Id);
                table.ForeignKey(
                    name: "FK_ExerciseQuestionsModel_LessonsModel_LessonsModelId",
                    column: x => x.LessonsModelId,
                    principalTable: "LessonsModel",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ExerciseQuestionsModel_LessonsModelId",
            table: "ExerciseQuestionsModel",
            column: "LessonsModelId");

        migrationBuilder.CreateIndex(
            name: "IX_LessonsModel_UnitsModelId",
            table: "LessonsModel",
            column: "UnitsModelId");

        migrationBuilder.CreateIndex(
            name: "IX_UnitsModel_LearningPathsModelId",
            table: "UnitsModel",
            column: "LearningPathsModelId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ExerciseQuestionsModel");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "LessonsModel");

        migrationBuilder.DropTable(
            name: "UnitsModel");

        migrationBuilder.DropTable(
            name: "LearningPathsModel");
    }
}
