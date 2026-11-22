using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Code123.Migrations;

/// <inheritdoc />
public partial class _20261122151549_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LearningPaths",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LearningPaths", x => x.Id);
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
            name: "Units",
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
                table.PrimaryKey("PK_Units", x => x.Id);
                table.ForeignKey(
                    name: "FK_Units_LearningPaths_LearningPathsModelId",
                    column: x => x.LearningPathsModelId,
                    principalTable: "LearningPaths",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Lessons",
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
                table.PrimaryKey("PK_Lessons", x => x.Id);
                table.CheckConstraint("CK_Lessons_Content", "DATALENGTH([Content]) <= 100000");
                table.ForeignKey(
                    name: "FK_Lessons_Units_UnitsModelId",
                    column: x => x.UnitsModelId,
                    principalTable: "Units",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "ExerciseQuestions",
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
                table.PrimaryKey("PK_ExerciseQuestions", x => x.Id);
                table.ForeignKey(
                    name: "FK_ExerciseQuestions_Lessons_LessonsModelId",
                    column: x => x.LessonsModelId,
                    principalTable: "Lessons",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ExerciseQuestions_LessonsModelId",
            table: "ExerciseQuestions",
            column: "LessonsModelId");

        migrationBuilder.CreateIndex(
            name: "IX_Lessons_UnitsModelId",
            table: "Lessons",
            column: "UnitsModelId");

        migrationBuilder.CreateIndex(
            name: "IX_Units_LearningPathsModelId",
            table: "Units",
            column: "LearningPathsModelId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ExerciseQuestions");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Lessons");

        migrationBuilder.DropTable(
            name: "Units");

        migrationBuilder.DropTable(
            name: "LearningPaths");
    }
}
