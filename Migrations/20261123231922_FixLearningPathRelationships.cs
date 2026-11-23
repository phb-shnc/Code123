using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Code123.Migrations;

/// <inheritdoc />
public partial class _20261123231922_FixLearningPathRelationships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ExerciseQuestions_Lessons_LessonsModelId",
            table: "ExerciseQuestions");

        migrationBuilder.DropForeignKey(
            name: "FK_Lessons_Units_UnitsModelId",
            table: "Lessons");

        migrationBuilder.DropForeignKey(
            name: "FK_Units_LearningPaths_LearningPathsModelId",
            table: "Units");

        migrationBuilder.DropIndex(
            name: "IX_Units_LearningPathsModelId",
            table: "Units");

        migrationBuilder.DropIndex(
            name: "IX_Lessons_UnitsModelId",
            table: "Lessons");

        migrationBuilder.DropIndex(
            name: "IX_ExerciseQuestions_LessonsModelId",
            table: "ExerciseQuestions");

        migrationBuilder.DropColumn(
            name: "LearningPathsModelId",
            table: "Units");

        migrationBuilder.DropColumn(
            name: "UnitsModelId",
            table: "Lessons");

        migrationBuilder.DropColumn(
            name: "LessonsModelId",
            table: "ExerciseQuestions");

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "LearningPaths",
            type: "nvarchar(120)",
            maxLength: 120,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(100)",
            oldMaxLength: 100);

        migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            table: "LearningPaths",
            type: "int",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.AddColumn<string>(
            name: "Level",
            table: "LearningPaths",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Status",
            table: "LearningPaths",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateIndex(
            name: "IX_Units_LearningPathId",
            table: "Units",
            column: "LearningPathId");

        migrationBuilder.CreateIndex(
            name: "IX_Lessons_UnitId",
            table: "Lessons",
            column: "UnitId");

        migrationBuilder.CreateIndex(
            name: "IX_ExerciseQuestions_LessonId",
            table: "ExerciseQuestions",
            column: "LessonId");

        migrationBuilder.AddForeignKey(
            name: "FK_ExerciseQuestions_Lessons_LessonId",
            table: "ExerciseQuestions",
            column: "LessonId",
            principalTable: "Lessons",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Lessons_Units_UnitId",
            table: "Lessons",
            column: "UnitId",
            principalTable: "Units",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Units_LearningPaths_LearningPathId",
            table: "Units",
            column: "LearningPathId",
            principalTable: "LearningPaths",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ExerciseQuestions_Lessons_LessonId",
            table: "ExerciseQuestions");

        migrationBuilder.DropForeignKey(
            name: "FK_Lessons_Units_UnitId",
            table: "Lessons");

        migrationBuilder.DropForeignKey(
            name: "FK_Units_LearningPaths_LearningPathId",
            table: "Units");

        migrationBuilder.DropIndex(
            name: "IX_Units_LearningPathId",
            table: "Units");

        migrationBuilder.DropIndex(
            name: "IX_Lessons_UnitId",
            table: "Lessons");

        migrationBuilder.DropIndex(
            name: "IX_ExerciseQuestions_LessonId",
            table: "ExerciseQuestions");

        migrationBuilder.DropColumn(
            name: "DisplayOrder",
            table: "LearningPaths");

        migrationBuilder.DropColumn(
            name: "Level",
            table: "LearningPaths");

        migrationBuilder.DropColumn(
            name: "Status",
            table: "LearningPaths");

        migrationBuilder.AddColumn<int>(
            name: "LearningPathsModelId",
            table: "Units",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UnitsModelId",
            table: "Lessons",
            type: "int",
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "LearningPaths",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(120)",
            oldMaxLength: 120);

        migrationBuilder.AddColumn<int>(
            name: "LessonsModelId",
            table: "ExerciseQuestions",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Units_LearningPathsModelId",
            table: "Units",
            column: "LearningPathsModelId");

        migrationBuilder.CreateIndex(
            name: "IX_Lessons_UnitsModelId",
            table: "Lessons",
            column: "UnitsModelId");

        migrationBuilder.CreateIndex(
            name: "IX_ExerciseQuestions_LessonsModelId",
            table: "ExerciseQuestions",
            column: "LessonsModelId");

        migrationBuilder.AddForeignKey(
            name: "FK_ExerciseQuestions_Lessons_LessonsModelId",
            table: "ExerciseQuestions",
            column: "LessonsModelId",
            principalTable: "Lessons",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Lessons_Units_UnitsModelId",
            table: "Lessons",
            column: "UnitsModelId",
            principalTable: "Units",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Units_LearningPaths_LearningPathsModelId",
            table: "Units",
            column: "LearningPathsModelId",
            principalTable: "LearningPaths",
            principalColumn: "Id");
    }
}
