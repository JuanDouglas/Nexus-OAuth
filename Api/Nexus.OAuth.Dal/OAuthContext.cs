global using Microsoft.EntityFrameworkCore;
global using Nexus.OAuth.Dal.Models;
global using Nexus.OAuth.Dal.Models.Enums;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
using File = Nexus.OAuth.Dal.Models.File;
using FileAccess = Nexus.OAuth.Dal.Models.Enums.FileAccess;

namespace Nexus.OAuth.Dal;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Type_or_Member'
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
        ConnectionString = _lastConnection ??
            "Server=PC;Database=Nexus OAuth (Development);User id=OAuthDev;Password=D3vel0pm3nt;Trusted_Connection=true;";
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
            optionsBuilder.UseSqlServer(ConnectionString);
    }

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
            .IsClustered(false)
            .IsUnique();

        builder.Entity<Application>()
            .HasIndex(app => app.OwnerId)
            .IsClustered(false);

        builder.Entity<Application>()
            .HasIndex(app => app.Status)
            .IsClustered(false);

        #region Seeds
        builder.Entity<Account>()
            .HasData(new Account()
            {
                Id = 1,
                Culture = "pt-br",
                ConfirmationStatus = ConfirmationStatus.Support,
                DateOfBirth = new DateTime(2004, 8, 11),
                Email = "juandouglas2004@gmail.com",
                Created = DateTime.UtcNow,
                Name = "Juan Douglas Lima da Silva",
                Phone = "(61) 99260-6441",
                // Am0.B@t4ta
                Password = "$2a$10$dRVgoKzNY1ir9B8CGhUkPO4WYsZzXpcOyZriz6th1VzbuCK.DDMIS"
            });

        builder.Entity<File>()
            .HasData(new File()
            {
                Id = 1,
                Access = FileAccess.Public,
                DirectoryType = DirectoryType.Defaults,
                FileName = "defaultfile.png",
                Inserted = DateTime.UtcNow,
                Length = 2333,
                Type = FileType.Template,
                ResourceOwnerId = 1
            });

        builder.Entity<Application>()
            .HasData(new Application()
            {
                Id = 1,
                OwnerId = 1,
                Name = "Nexus Energy",
                Site = "https://energy.nexus-company.tech/",
                Status = ApplicationStatus.Active,
                Key = "u5108a260700563169i8686ea59m0850",
                Secret = "vazNEwy6EXi2oQ9X68J8Xx3R61KT0LJ6iJ055K29CFEZbCrvyf7a5r7UHs60hRtX49YczrPCXTmo5EnrxLwy3ELMbVA5gHEb",
                Description = "The Nexus Energy is a best energy store.",
                RedirectAuthorize = "https://energy.nexus-company.tech/oauth/authorize",
                RedirectLogin = "https://energy.nexus-company.tech/oauth/login"
            });

        builder.Entity<Application>()
          .HasData(new Application()
          {
              Id = 2,
              OwnerId = 1,
              Name = "Nexus Solutions",
              Site = "https://solutions.nexus-company.tech/",
              Status = ApplicationStatus.Active,
              Key = "a59m0850u510863169i8686ea2607005",
              Secret = "7a5r7UHs60hRMbVA5gHEbvazNEwy6EXi2oQ9X68J8Xx3R61KT0LJ6iJ055K29CFEZbCrvyftX49YczrPCXTmo5EnrxLwy3EL",
              Description = "Create your project with Nexus Company.",
              RedirectAuthorize = "https://localhost:44379/oauth/callback",
              RedirectLogin = "https://localhost:44379/oauth/login"
          });
        #endregion
    }

    public override void Dispose()
    {
        base.Dispose();

        GC.SuppressFinalize(this);
    }
}