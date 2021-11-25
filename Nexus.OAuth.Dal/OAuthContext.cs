using Microsoft.EntityFrameworkCore;
using Nexus.OAuth.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Dal
{
    public partial class OAuthContext : DbContext
    {

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Authorization> Authorizations { get; set; }
        /// <summary>
        /// Database SqlServer Connection String
        /// </summary>
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
        public static string ConnectionString { get; set; }
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
        public OAuthContext()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                ConnectionString = "Server=.\\SQLEXPRESS;Database=OAuth;Trusted_Connection=true;";
            }
        }

        public OAuthContext(DbContextOptions<OAuthContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString);
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
