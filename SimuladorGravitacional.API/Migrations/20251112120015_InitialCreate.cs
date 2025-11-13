using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SimuladorGravitacional.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Simulacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    NumeroIteracoes = table.Column<int>(type: "integer", nullable: false),
                    NumeroColisoes = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeCorpos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Universos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CanvasWidth = table.Column<double>(type: "double precision", nullable: false),
                    CanvasHeight = table.Column<double>(type: "double precision", nullable: false),
                    FatorSimulacao = table.Column<double>(type: "double precision", nullable: false, defaultValue: 100000.0),
                    SimulacaoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Universos_Simulacoes_SimulacaoId",
                        column: x => x.SimulacaoId,
                        principalTable: "Simulacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Corpos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Massa = table.Column<double>(type: "double precision", nullable: false),
                    Densidade = table.Column<double>(type: "double precision", nullable: false),
                    Raio = table.Column<double>(type: "double precision", nullable: false),
                    Cor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PosX = table.Column<double>(type: "double precision", nullable: false),
                    PosY = table.Column<double>(type: "double precision", nullable: false),
                    VelX = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    VelY = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    UniversoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corpos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Corpos_Universos_UniversoId",
                        column: x => x.UniversoId,
                        principalTable: "Universos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Corpos_UniversoId",
                table: "Corpos",
                column: "UniversoId");

            migrationBuilder.CreateIndex(
                name: "IX_Universos_SimulacaoId",
                table: "Universos",
                column: "SimulacaoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Corpos");

            migrationBuilder.DropTable(
                name: "Universos");

            migrationBuilder.DropTable(
                name: "Simulacoes");
        }
    }
}
