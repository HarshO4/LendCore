using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingPlatform.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoanAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    AssetValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreditScore = table.Column<int>(type: "INTEGER", nullable: false),
                    Ltv = table.Column<decimal>(type: "TEXT", nullable: false),
                    Decision = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DecisionReason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    RuleApplied = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanApplications");
        }
    }
}
