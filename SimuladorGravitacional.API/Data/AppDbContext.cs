using Microsoft.EntityFrameworkCore;
using ProgramacaoAvancada.Models;

namespace ProgramacaoAvancada.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<SimulacaoSnapshot> SimulacaoSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SimulacaoSnapshot>(entity =>
            {
                entity.ToTable("SimulacaoSnapshots");
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Nome)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(s => s.DataCriacao)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(s => s.NumeroIteracoes)
                    .IsRequired();

                entity.Property(s => s.NumeroColisoes)
                    .IsRequired();

                entity.Property(s => s.QuantidadeCorpos)
                    .IsRequired();

                entity.Property(s => s.ConteudoJson)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");
            });
        }
    }
}