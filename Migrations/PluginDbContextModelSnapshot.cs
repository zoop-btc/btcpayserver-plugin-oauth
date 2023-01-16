﻿// <auto-generated />
using System;
using BTCPayServer.Plugins.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BTCPayServer.Plugins.Template.Migrations
{
    [DbContext(typeof(PluginDbContext))]
    partial class PluginDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("BTCPayServer.Plugins.Template")
                .HasAnnotation("ProductVersion", "3.1.10");

            modelBuilder.Entity("BTCPayServer.Plugins.Template.Data.PluginData", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PluginRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
