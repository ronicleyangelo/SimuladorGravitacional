using Microsoft.EntityFrameworkCore;
using ProgramacaoAvancada.Models;

namespace ProgramacaoAvancada.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Simulacao> Simulacoes { get; set; }
        public DbSet<Universo> Universos { get; set; }
        public DbSet<Corpo> Corpos { get; set; }
        public DbSet<EventoSimulacao> EventosSimulacao { get; set; } // ADICIONAR ESTA LINHA

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da Simulacao
            modelBuilder.Entity<Simulacao>(entity =>
            {
                entity.ToTable("Simulacoes");
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Nome)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(s => s.DataCriacao)
                    .HasDefaultValueSql("NOW()");

                entity.Property(s => s.DataAtualizacao)
                    .IsRequired(false);

                entity.Property(s => s.NumeroIteracoes)
                    .IsRequired();

                entity.Property(s => s.NumeroColisoes)
                    .IsRequired();

                entity.Property(s => s.QuantidadeCorpos)
                    .IsRequired();

                entity.Property(s => s.Status)
                    .IsRequired()
                    .HasDefaultValue(StatusSimulacao.Rascunho);

                entity.Property(s => s.Descricao)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(s => s.Configuracao)
                    .HasColumnType("jsonb")
                    .IsRequired(false);

                // Relação 1:1 com Universo
                entity.HasOne(s => s.Universo)
                    .WithOne(u => u.Simulacao)
                    .HasForeignKey<Universo>(u => u.SimulacaoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relação 1:N com EventosSimulacao
                entity.HasMany(s => s.Eventos)
                    .WithOne(e => e.Simulacao)
                    .HasForeignKey(e => e.SimulacaoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração do Universo
            modelBuilder.Entity<Universo>(entity =>
            {
                entity.ToTable("Universos");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nome)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(u => u.Descricao)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(u => u.LarguraCanvas)
                    .IsRequired();

                entity.Property(u => u.AlturaCanvas)
                    .IsRequired();

                entity.Property(u => u.FatorSimulacao)
                    .HasDefaultValue(1e5);

                entity.Property(u => u.ConstanteGravitacional)
                    .HasDefaultValue(6.67430e-11);

                entity.Property(u => u.PassoTempo)
                    .HasDefaultValue(1.0);

                entity.Property(u => u.ColisoesHabilitadas)
                    .HasDefaultValue(true);

                entity.Property(u => u.BordasReflexivas)
                    .HasDefaultValue(false);

                entity.Property(u => u.CorFundo)
                    .HasMaxLength(50)
                    .HasDefaultValue("rgb(0,0,0)");

                entity.Property(u => u.DataCriacao)
                    .HasDefaultValueSql("NOW()");

                entity.Property(u => u.DataAtualizacao)
                    .IsRequired(false);

                entity.Property(u => u.Configuracao)
                    .HasColumnType("jsonb")
                    .IsRequired(false);

                // Relação 1:N com Corpos
                entity.HasMany(u => u.Corpos)
                    .WithOne(c => c.Universo)
                    .HasForeignKey(c => c.UniversoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração do Corpo
            modelBuilder.Entity<Corpo>(entity =>
            {
                entity.ToTable("Corpos");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Nome)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.Massa)
                    .IsRequired();

                entity.Property(c => c.Densidade)
                    .IsRequired();

                entity.Property(c => c.Raio)
                    .IsRequired();

                entity.Property(c => c.Cor)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("rgb(255,255,255)");

                entity.Property(c => c.PosicaoX)
                    .IsRequired();

                entity.Property(c => c.PosicaoY)
                    .IsRequired();

                entity.Property(c => c.VelocidadeX)
                    .HasDefaultValue(0);

                entity.Property(c => c.VelocidadeY)
                    .HasDefaultValue(0);

                entity.Property(c => c.AceleracaoX)
                    .HasDefaultValue(0);

                entity.Property(c => c.AceleracaoY)
                    .HasDefaultValue(0);

                entity.Property(c => c.TipoCorpo)
                    .IsRequired()
                    .HasDefaultValue(TipoCorpoCeleste.Planeta);

                entity.Property(c => c.Ativo)
                    .HasDefaultValue(true);

                entity.Property(c => c.DataAtualizacao)
                    .IsRequired(false);

                entity.Property(c => c.Metadados)
                    .HasColumnType("jsonb")
                    .IsRequired(false);
            });

            // Configuração do EventoSimulacao (NOVO)
            modelBuilder.Entity<EventoSimulacao>(entity =>
            {
                entity.ToTable("EventosSimulacao");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DataHora)
                    .HasDefaultValueSql("NOW()");

                entity.Property(e => e.TipoEvento)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Mensagem)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(e => e.Detalhes)
                    .HasColumnType("jsonb")
                    .IsRequired(false);

                entity.Property(e => e.Nivel)
                    .IsRequired()
                    .HasDefaultValue(NivelEvento.Informacao);

                // Relação com Simulacao
                entity.HasOne(e => e.Simulacao)
                    .WithMany(s => s.Eventos)
                    .HasForeignKey(e => e.SimulacaoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}