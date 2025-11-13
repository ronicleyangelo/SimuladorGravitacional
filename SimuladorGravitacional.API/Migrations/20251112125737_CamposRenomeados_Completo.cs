using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SimuladorGravitacional.API.Migrations
{
    /// <inheritdoc />
    public partial class CamposRenomeados_Completo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Corpos_Universos_UniversoId",
                table: "Corpos");

            migrationBuilder.DropForeignKey(
                name: "FK_Universos_Simulacoes_SimulacaoId",
                table: "Universos");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Universos",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Universos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SimulacaoId",
                table: "Universos",
                newName: "simulacao_id");

            migrationBuilder.RenameColumn(
                name: "FatorSimulacao",
                table: "Universos",
                newName: "fator_simulacao");

            migrationBuilder.RenameColumn(
                name: "CanvasWidth",
                table: "Universos",
                newName: "passo_tempo");

            migrationBuilder.RenameColumn(
                name: "CanvasHeight",
                table: "Universos",
                newName: "largura_canvas");

            migrationBuilder.RenameIndex(
                name: "IX_Universos_SimulacaoId",
                table: "Universos",
                newName: "IX_Universos_simulacao_id");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Simulacoes",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Simulacoes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "QuantidadeCorpos",
                table: "Simulacoes",
                newName: "quantidade_corpos");

            migrationBuilder.RenameColumn(
                name: "NumeroIteracoes",
                table: "Simulacoes",
                newName: "numero_iteracoes");

            migrationBuilder.RenameColumn(
                name: "NumeroColisoes",
                table: "Simulacoes",
                newName: "numero_colisoes");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "Simulacoes",
                newName: "data_criacao");

            migrationBuilder.RenameColumn(
                name: "Raio",
                table: "Corpos",
                newName: "raio");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Corpos",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Massa",
                table: "Corpos",
                newName: "massa");

            migrationBuilder.RenameColumn(
                name: "Densidade",
                table: "Corpos",
                newName: "densidade");

            migrationBuilder.RenameColumn(
                name: "Cor",
                table: "Corpos",
                newName: "cor");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Corpos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UniversoId",
                table: "Corpos",
                newName: "universo_id");

            migrationBuilder.RenameColumn(
                name: "VelY",
                table: "Corpos",
                newName: "velocidade_y");

            migrationBuilder.RenameColumn(
                name: "VelX",
                table: "Corpos",
                newName: "velocidade_x");

            migrationBuilder.RenameColumn(
                name: "PosY",
                table: "Corpos",
                newName: "posicao_y");

            migrationBuilder.RenameColumn(
                name: "PosX",
                table: "Corpos",
                newName: "posicao_x");

            migrationBuilder.RenameIndex(
                name: "IX_Corpos_UniversoId",
                table: "Corpos",
                newName: "IX_Corpos_universo_id");

            migrationBuilder.AddColumn<double>(
                name: "altura_canvas",
                table: "Universos",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "bordas_reflexivas",
                table: "Universos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "colisoes_habilitadas",
                table: "Universos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "configuracao",
                table: "Universos",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "constante_gravitacional",
                table: "Universos",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "cor_fundo",
                table: "Universos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_atualizacao",
                table: "Universos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_criacao",
                table: "Universos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "descricao",
                table: "Universos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "configuracao",
                table: "Simulacoes",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_atualizacao",
                table: "Simulacoes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "descricao",
                table: "Simulacoes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Simulacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "aceleracao_x",
                table: "Corpos",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "aceleracao_y",
                table: "Corpos",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "Corpos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_atualizacao",
                table: "Corpos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metadados",
                table: "Corpos",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tipo_corpo",
                table: "Corpos",
                type: "integer",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventosSimulacao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    simulacao_id = table.Column<int>(type: "integer", nullable: false),
                    data_hora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tipo_evento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    mensagem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    detalhes = table.Column<string>(type: "jsonb", nullable: true),
                    nivel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosSimulacao", x => x.id);
                    table.ForeignKey(
                        name: "FK_EventosSimulacao_Simulacoes_simulacao_id",
                        column: x => x.simulacao_id,
                        principalTable: "Simulacoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventosSimulacao_simulacao_id",
                table: "EventosSimulacao",
                column: "simulacao_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Corpos_Universos_universo_id",
                table: "Corpos",
                column: "universo_id",
                principalTable: "Universos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Universos_Simulacoes_simulacao_id",
                table: "Universos",
                column: "simulacao_id",
                principalTable: "Simulacoes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Corpos_Universos_universo_id",
                table: "Corpos");

            migrationBuilder.DropForeignKey(
                name: "FK_Universos_Simulacoes_simulacao_id",
                table: "Universos");

            migrationBuilder.DropTable(
                name: "EventosSimulacao");

            migrationBuilder.DropColumn(
                name: "altura_canvas",
                table: "Universos");

            migrationBuilder.DropColumn(
                name: "bordas_reflexivas",
                table: "Universos");

            migrationBuilder.DropColumn(
                name: "colisoes_habilitadas",
                table: "Universos");

            migrationBuilder.DropColumn(
                name: "configuracao",
                table: "Universos");

            migrationBuilder.DropColumn(
                name: "constante_gravitacional",
                table: "Universos");

            migrationBuilder.DropColumn(
                name: "cor_fundo",
                table: "Universos");

            migrationBuilder.DropColumn(
                name: "data_atualizacao",
                table: "Universos");

            migrationBuilder.DropColumn(
                name: "data_criacao",
                table: "Universos");

            migrationBuilder.DropColumn(
                name: "descricao",
                table: "Universos");

            migrationBuilder.DropColumn(
                name: "configuracao",
                table: "Simulacoes");

            migrationBuilder.DropColumn(
                name: "data_atualizacao",
                table: "Simulacoes");

            migrationBuilder.DropColumn(
                name: "descricao",
                table: "Simulacoes");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Simulacoes");

            migrationBuilder.DropColumn(
                name: "aceleracao_x",
                table: "Corpos");

            migrationBuilder.DropColumn(
                name: "aceleracao_y",
                table: "Corpos");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "Corpos");

            migrationBuilder.DropColumn(
                name: "data_atualizacao",
                table: "Corpos");

            migrationBuilder.DropColumn(
                name: "metadados",
                table: "Corpos");

            migrationBuilder.DropColumn(
                name: "tipo_corpo",
                table: "Corpos");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Universos",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Universos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "simulacao_id",
                table: "Universos",
                newName: "SimulacaoId");

            migrationBuilder.RenameColumn(
                name: "fator_simulacao",
                table: "Universos",
                newName: "FatorSimulacao");

            migrationBuilder.RenameColumn(
                name: "passo_tempo",
                table: "Universos",
                newName: "CanvasWidth");

            migrationBuilder.RenameColumn(
                name: "largura_canvas",
                table: "Universos",
                newName: "CanvasHeight");

            migrationBuilder.RenameIndex(
                name: "IX_Universos_simulacao_id",
                table: "Universos",
                newName: "IX_Universos_SimulacaoId");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Simulacoes",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Simulacoes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "quantidade_corpos",
                table: "Simulacoes",
                newName: "QuantidadeCorpos");

            migrationBuilder.RenameColumn(
                name: "numero_iteracoes",
                table: "Simulacoes",
                newName: "NumeroIteracoes");

            migrationBuilder.RenameColumn(
                name: "numero_colisoes",
                table: "Simulacoes",
                newName: "NumeroColisoes");

            migrationBuilder.RenameColumn(
                name: "data_criacao",
                table: "Simulacoes",
                newName: "DataCriacao");

            migrationBuilder.RenameColumn(
                name: "raio",
                table: "Corpos",
                newName: "Raio");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Corpos",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "massa",
                table: "Corpos",
                newName: "Massa");

            migrationBuilder.RenameColumn(
                name: "densidade",
                table: "Corpos",
                newName: "Densidade");

            migrationBuilder.RenameColumn(
                name: "cor",
                table: "Corpos",
                newName: "Cor");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Corpos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "universo_id",
                table: "Corpos",
                newName: "UniversoId");

            migrationBuilder.RenameColumn(
                name: "velocidade_y",
                table: "Corpos",
                newName: "VelY");

            migrationBuilder.RenameColumn(
                name: "velocidade_x",
                table: "Corpos",
                newName: "VelX");

            migrationBuilder.RenameColumn(
                name: "posicao_y",
                table: "Corpos",
                newName: "PosY");

            migrationBuilder.RenameColumn(
                name: "posicao_x",
                table: "Corpos",
                newName: "PosX");

            migrationBuilder.RenameIndex(
                name: "IX_Corpos_universo_id",
                table: "Corpos",
                newName: "IX_Corpos_UniversoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Corpos_Universos_UniversoId",
                table: "Corpos",
                column: "UniversoId",
                principalTable: "Universos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Universos_Simulacoes_SimulacaoId",
                table: "Universos",
                column: "SimulacaoId",
                principalTable: "Simulacoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
