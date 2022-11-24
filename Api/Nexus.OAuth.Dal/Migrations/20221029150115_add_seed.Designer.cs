﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nexus.OAuth.Dal;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    [DbContext(typeof(OAuthContext))]
    [Migration("20221029150115_add_seed")]
    partial class add_seed
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<short>("ConfirmationStatus")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(96)
                        .HasColumnType("nvarchar(96)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int?>("ProfileImageID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("ProfileImageID");

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConfirmationStatus = (short)3,
                            Created = new DateTime(2022, 10, 29, 15, 1, 14, 884, DateTimeKind.Utc).AddTicks(9505),
                            Culture = "pt-br",
                            DateOfBirth = new DateTime(2004, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "juandouglas2004@gmail.com",
                            Name = "Juan Douglas Lima da Silva",
                            Password = "$2a$12$GPuArXDC.No0A4gqIADCsOcugLWr8Ij31PubiwS/s.Cj2w/K0KadG",
                            Phone = "(61) 99260-6441"
                        });
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.AccountConfirmation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(96)
                        .HasColumnType("nvarchar(96)");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.Property<bool>("Valid")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountConfirmations");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.Application", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2500)
                        .HasColumnType("nvarchar(2500)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("LogoId")
                        .HasColumnType("int");

                    b.Property<short?>("MinConfirmationStatus")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<string>("RedirectAuthorize")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("RedirectLogin")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Site")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("Key"), false);

                    b.HasIndex("LogoId");

                    b.HasIndex("OwnerId");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("OwnerId"), false);

                    b.HasIndex("Status");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("Status"), false);

                    b.ToTable("Applications");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "The Nexus Energy is a best energy store.",
                            Key = "u5108a260700563169i8686ea59m0850",
                            Name = "Nexus Energy",
                            OwnerId = 1,
                            RedirectAuthorize = "",
                            RedirectLogin = "",
                            Secret = "vazNEwy6EXi2oQ9X68J8Xx3R61KT0LJ6iJ055K29CFEZbCrvyf7a5r7UHs60hRtX49YczrPCXTmo5EnrxLwy3ELMbVA5gHEb",
                            Site = "https://energy.nexus-company.tech/",
                            Status = (short)127
                        });
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.Authentication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AuthorizationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<double?>("ExpiresIn")
                        .HasColumnType("float");

                    b.Property<int?>("FirstStepId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Ip")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("varbinary(16)")
                        .HasColumnName("IpAdress");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<int?>("QrCodeAuthorizationId")
                        .HasColumnType("int");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<short>("TokenType")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("AuthorizationId");

                    b.HasIndex("FirstStepId");

                    b.HasIndex("QrCodeAuthorizationId");

                    b.ToTable("Authentications");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.Authorization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<int>("ApplicationId")
                        .HasColumnType("int");

                    b.Property<string>("ClientKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<double?>("ExpiresIn")
                        .HasColumnType("float");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<byte[]>("ScopesBytes")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("State")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("Used")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ApplicationId");

                    b.ToTable("Authorizations");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<short>("Access")
                        .HasColumnType("smallint");

                    b.Property<short>("DirectoryType")
                        .HasColumnType("smallint");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(1200)
                        .HasColumnType("nvarchar(1200)");

                    b.Property<DateTime>("Inserted")
                        .HasColumnType("datetime2");

                    b.Property<long>("Length")
                        .HasColumnType("bigint");

                    b.Property<int?>("ResourceOwnerId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("ResourceOwnerId");

                    b.ToTable("Files");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Access = (short)1,
                            DirectoryType = (short)2,
                            FileName = "defaultfile.png",
                            Inserted = new DateTime(2022, 10, 29, 15, 1, 14, 884, DateTimeKind.Utc).AddTicks(9608),
                            Length = 2333L,
                            ResourceOwnerId = 1,
                            Type = (short)2
                        });
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.FirstStep", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("ClientKey")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("Ip")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("varbinary(16)")
                        .HasColumnName("IpAdress");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("Redirect")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("UserAgent")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("FirstSteps");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.QrCodeAuthorization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("AuthorizeDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<int>("QrCodeReferenceId")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("QrCodeReferenceId");

                    b.ToTable("QrCodeAuthorizations");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.QrCodeReference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClientKey")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<DateTime>("Create")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("Ip")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("varbinary(6)")
                        .HasColumnName("IpAdress");

                    b.Property<DateTime?>("Use")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Used")
                        .HasColumnType("bit");

                    b.Property<string>("UserAgent")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<bool>("Valid")
                        .HasColumnType("bit");

                    b.Property<string>("ValidationToken")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("QrCodes");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.Account", b =>
                {
                    b.HasOne("Nexus.OAuth.Dal.Models.File", "ProfileImage")
                        .WithMany()
                        .HasForeignKey("ProfileImageID")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("ProfileImage");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.AccountConfirmation", b =>
                {
                    b.HasOne("Nexus.OAuth.Dal.Models.Account", "AccountNavigation")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("AccountNavigation");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.Application", b =>
                {
                    b.HasOne("Nexus.OAuth.Dal.Models.File", "Logo")
                        .WithMany()
                        .HasForeignKey("LogoId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Nexus.OAuth.Dal.Models.Account", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Logo");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.Authentication", b =>
                {
                    b.HasOne("Nexus.OAuth.Dal.Models.Authorization", "AuthorizationNavigation")
                        .WithMany()
                        .HasForeignKey("AuthorizationId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Nexus.OAuth.Dal.Models.FirstStep", "FirstStepNavigation")
                        .WithMany()
                        .HasForeignKey("FirstStepId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Nexus.OAuth.Dal.Models.QrCodeAuthorization", "QrCodeAuthorizationNavigation")
                        .WithMany()
                        .HasForeignKey("QrCodeAuthorizationId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("AuthorizationNavigation");

                    b.Navigation("FirstStepNavigation");

                    b.Navigation("QrCodeAuthorizationNavigation");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.Authorization", b =>
                {
                    b.HasOne("Nexus.OAuth.Dal.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Nexus.OAuth.Dal.Models.Application", "Application")
                        .WithMany()
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Application");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.File", b =>
                {
                    b.HasOne("Nexus.OAuth.Dal.Models.Account", "ResourceOwner")
                        .WithMany()
                        .HasForeignKey("ResourceOwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("ResourceOwner");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.FirstStep", b =>
                {
                    b.HasOne("Nexus.OAuth.Dal.Models.Account", "AccountNavigation")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("AccountNavigation");
                });

            modelBuilder.Entity("Nexus.OAuth.Dal.Models.QrCodeAuthorization", b =>
                {
                    b.HasOne("Nexus.OAuth.Dal.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Nexus.OAuth.Dal.Models.QrCodeReference", "QrCodeReference")
                        .WithMany()
                        .HasForeignKey("QrCodeReferenceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("QrCodeReference");
                });
#pragma warning restore 612, 618
        }
    }
}
