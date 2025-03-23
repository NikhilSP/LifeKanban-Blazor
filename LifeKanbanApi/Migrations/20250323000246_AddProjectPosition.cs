﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeKanbanApi.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Projects");
        }
    }
}
