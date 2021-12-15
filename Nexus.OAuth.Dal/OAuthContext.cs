global using Nexus.OAuth.Dal.Models;
global using Nexus.OAuth.Dal.Models.Enums;
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;

namespace Nexus.OAuth.Dal
{
    public partial class OAuthContext : DbContext
    {

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<FirstStep> FirstSteps { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Authorization> Authorizations { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Database SqlServer Connection String
        /// </summary>
        public static string? ConnectionString { get; set; }
        public OAuthContext()
        {
        }
        public OAuthContext(DbContextOptions<OAuthContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString ?? "Server=.\\SQLEXPRESS;Database=OAuth;Trusted_Connection=true;");
            }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            builder.Entity<Account>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<Application>()
                .HasIndex(app => app.Key)
                .IsUnique();

            OnModelCreatingPartial(builder);
        }

    }
}
