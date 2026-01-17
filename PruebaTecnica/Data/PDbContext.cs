using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Models;
using System.Security.Principal;

namespace PruebaTecnica.Data
{
    public class PDbContext : DbContext
    {
        public PDbContext(DbContextOptions<PDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar DNI como único
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.DNI)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}

