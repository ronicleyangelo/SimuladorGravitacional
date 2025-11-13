using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimuladorGravitacional.API.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarEventoSimulacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "passo_tempo",
                table: "Universos",
                type: "double precision",
                nullable: false,
                defaultValue: 1.0,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_criacao",
                table: "Universos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "cor_fundo",
                table: "Universos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "rgb(0,0,0)",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<double>(
                name: "constante_gravitacional",
                table: "Universos",
                type: "double precision",
                nullable: false,
                defaultValue: 6.6742999999999994E-11,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<bool>(
                name: "colisoes_habilitadas",
                table: "Universos",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "bordas_reflexivas",
                table: "Universos",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "Simulacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "nivel",
                table: "EventosSimulacao",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "mensagem",
                table: "EventosSimulacao",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_hora",
                table: "EventosSimulacao",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "tipo_corpo",
                table: "Corpos",
                type: "integer",
                maxLength: 50,
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "cor",
                table: "Corpos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "rgb(255,255,255)",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "ativo",
                table: "Corpos",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<double>(
                name: "aceleracao_y",
                table: "Corpos",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "aceleracao_x",
                table: "Corpos",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "passo_tempo",
                table: "Universos",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 1.0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_criacao",
                table: "Universos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<string>(
                name: "cor_fundo",
                table: "Universos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "rgb(0,0,0)");

            migrationBuilder.AlterColumn<double>(
                name: "constante_gravitacional",
                table: "Universos",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 6.6742999999999994E-11);

            migrationBuilder.AlterColumn<bool>(
                name: "colisoes_habilitadas",
                table: "Universos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "bordas_reflexivas",
                table: "Universos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "Simulacoes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "nivel",
                table: "EventosSimulacao",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "mensagem",
                table: "EventosSimulacao",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_hora",
                table: "EventosSimulacao",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<int>(
                name: "tipo_corpo",
                table: "Corpos",
                type: "integer",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 50,
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "cor",
                table: "Corpos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "rgb(255,255,255)");

            migrationBuilder.AlterColumn<bool>(
                name: "ativo",
                table: "Corpos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<double>(
                name: "aceleracao_y",
                table: "Corpos",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "aceleracao_x",
                table: "Corpos",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 0.0);
        }
    }
}
