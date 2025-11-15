using Microsoft.EntityFrameworkCore;
using ProgramacaoAvancada.Models;

namespace ProgramacaoAvancada.Data
{
    public class SimulacaoDbContext : DbContext
    {
        public DbSet<Universo> Universos { get; set; }
        public DbSet<Corpo> Corpos { get; set; }

        public SimulacaoDbContext(DbContextOptions<SimulacaoDbContext> options) : base(options)
        {
        }

        // ✅ REMOVER o construtor padrão se existir
        // ✅ REMOVER a propriedade DbPath se existir

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // ✅ SUBSTITUA PELA SUA CONNECTION STRING DO NEON!
                var connectionString = "Host=ep-sparkling-wind-adq2d58k-pooler.c-2.us-east-1.aws.neon.tech;" +
                                      "Database=neondb;" +
                                      "Username=neondb_owner;" +
                                      "Password=npg_DIAyl9NZd0xS;" +
                                      "SSL Mode=Require;";

                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar relacionamento
            modelBuilder.Entity<Universo>()
                .HasMany(u => u.CorposNavigation)
                .WithOne(c => c.Universo)
                .HasForeignKey(c => c.UniversoId) // ✅ AGORA ESTÁ CORRETO
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}