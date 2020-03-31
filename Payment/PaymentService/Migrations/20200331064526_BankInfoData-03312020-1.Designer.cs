﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PaymentService.Data;

namespace PaymentService.Migrations
{
    [DbContext(typeof(PaymentDbContext))]
    [Migration("20200331064526_BankInfoData-03312020-1")]
    partial class BankInfoData033120201
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("PaymentService.Model.BankInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("BankCode")
                        .HasColumnName("bank_code")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Url")
                        .HasColumnName("url")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("BankCode")
                        .IsUnique();

                    b.ToTable("bank_info");
                });
#pragma warning restore 612, 618
        }
    }
}
