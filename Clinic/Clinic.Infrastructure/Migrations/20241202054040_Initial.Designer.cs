﻿// <auto-generated />
using System;
using Clinic.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clinic.Infrastructure.Migrations
{
    [DbContext(typeof(ClinicDbContext))]
    [Migration("20241202054040_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Clinic.Domain.Entities.Patient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Active")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("Clinic.Domain.Entities.PersonName", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Family")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string[]>("Given")
                        .HasColumnType("text[]");

                    b.Property<string>("Use")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PersonNames");
                });

            modelBuilder.Entity("Clinic.Domain.Entities.PersonName", b =>
                {
                    b.HasOne("Clinic.Domain.Entities.Patient", "Patient")
                        .WithOne("Name")
                        .HasForeignKey("Clinic.Domain.Entities.PersonName", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("Clinic.Domain.Entities.Patient", b =>
                {
                    b.Navigation("Name")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
