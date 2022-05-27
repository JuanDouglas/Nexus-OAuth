global using Microsoft.EntityFrameworkCore;
global using Nexus.OAuth.Dal.Models;
global using Nexus.OAuth.Dal.Models.Enums;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
using File = Nexus.OAuth.Dal.Models.File;

namespace Nexus.OAuth.Dal;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Type_or_Member'
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public partial class OAuthContext : DbContext
{
    #region DBSets
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<AccountConfirmation> AccountConfirmations { get; set; }
    public virtual DbSet<FirstStep> FirstSteps { get; set; }
    public virtual DbSet<Application> Applications { get; set; }
    public virtual DbSet<Authorization> Authorizations { get; set; }
    public virtual DbSet<Authentication> Authentications { get; set; }
    public virtual DbSet<QrCodeReference> QrCodes { get; set; }
    public virtual DbSet<QrCodeAuthorization> QrCodeAuthorizations { get; set; }
    public virtual DbSet<File> Files { get; set; }
    #endregion

    /// <summary>
    /// Database SqlServer Connection String
    /// </summary>
    private string ConnectionString { get; set; }
    private static string? _lastConnection;

    public OAuthContext()
    {
        ConnectionString = _lastConnection;
    }
    public OAuthContext(string conn)
    {
        ConnectionString = conn;
        _lastConnection = conn;
    }
    public OAuthContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(ConnectionString ??
                (_lastConnection ?? "Server=.\\SQLExpress;Database=Nexus OAuth;Trusted_Connection=true;"));
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var relationship in builder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
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

    public override void Dispose()
    {
        base.Dispose();
    }
}