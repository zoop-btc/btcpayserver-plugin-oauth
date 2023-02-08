﻿// <auto-generated />
using BTCPayServer.Plugins.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BTCPayServer.Plugins.OAuth.Migrations
{
    [DbContext(typeof(OAuthPluginDbContext))]
    [Migration("20230207175944_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("BTCPayServer.Plugins.OAuth")
                .HasAnnotation("ProductVersion", "6.0.9");

            modelBuilder.Entity("BTCPayServer.Plugins.OAuth.Data.Models.OAuthSession", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("Audience")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<long>("ExpiresAt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<long>("IssuedAt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Issuer")
                        .HasColumnType("TEXT");

                    b.Property<long>("NotBefore")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Scope")
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .HasColumnType("TEXT");

                    b.Property<string>("TokenType")
                        .HasColumnType("TEXT");

                    b.Property<string>("TokenUse")
                        .HasColumnType("TEXT");

                    b.HasKey("Token");

                    b.ToTable("OAuthSessions", "BTCPayServer.Plugins.OAuth");
                });
#pragma warning restore 612, 618
        }
    }
}