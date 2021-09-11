﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using AzureProjectTemplate.Mais.Marketplace.Infra.Context;

namespace AzureProjectTemplate.Mais.Marketplace.Infra.Migrations
{
    [DbContext(typeof(EntityContext))]
    [Migration("20190320191123_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AzureProjectTemplate.Mais.Marketplace.Domain.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CEP")
                        .IsRequired()
                        .HasColumnType("VARCHAR(8)")
                        .HasMaxLength(8);

                    b.HasKey("Id");

                    b.ToTable("Address","dbo");
                });

            modelBuilder.Entity("AzureProjectTemplate.Mais.Marketplace.Domain.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AddressId");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("DATETIME2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("VARCHAR(30)")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Customer","dbo");
                });

            modelBuilder.Entity("AzureProjectTemplate.Mais.Marketplace.Domain.Models.Customer", b =>
                {
                    b.HasOne("AzureProjectTemplate.Mais.Marketplace.Domain.Models.Address", "Address")
                        .WithMany("Customers")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
