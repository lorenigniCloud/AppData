using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppData.Infrastructures.Models
{
    public partial class AppDataSqlServerContext : IdentityDbContext<IdentityUser>
    {
        public AppDataSqlServerContext(DbContextOptions<AppDataSqlServerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Word> Words { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Importante per la configurazione di Identity

            modelBuilder.Entity<Word>(entity =>
            {
                entity.ToTable("Word");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.BookTitle)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Content)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.FirstLetter)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
