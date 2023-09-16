﻿// <auto-generated />
using System;
using AddictedProxy.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    [DbContext(typeof(EntityContext))]
    [Migration("20230916180341_CreateAtUpdatedAtForCredsAndOneTimeMig")]
    partial class CreateAtUpdatedAtForCredsAndOneTimeMig
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AddictedProxy.Database.Model.Credentials.AddictedUserCredentials", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Cookie")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DownloadExceededDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DownloadUsage")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastUsage")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Usage")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Cookie")
                        .IsUnique();

                    b.ToTable("AddictedUserCredentials");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Migration.OneTimeMigrationRelease", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MigrationType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RanAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("MigrationType", "State")
                        .IsUnique();

                    b.ToTable("OneTimeMigrationRelease");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Episode", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Discovered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("ExternalId")
                        .HasColumnType("bigint");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("Season")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("TvShowId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("TvShowId", "Season", "Number")
                        .IsUnique();

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Season", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastRefreshed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<long>("TvShowId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("TvShowId", "Number")
                        .IsUnique();

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Subtitle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("Completed")
                        .HasColumnType("boolean");

                    b.Property<double>("CompletionPct")
                        .HasColumnType("double precision");

                    b.Property<bool>("Corrected")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Discovered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("DownloadCount")
                        .HasColumnType("bigint");

                    b.Property<string>("DownloadUri")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("EpisodeId")
                        .HasColumnType("bigint");

                    b.Property<bool>("HD")
                        .HasColumnType("boolean");

                    b.Property<bool>("HearingImpaired")
                        .HasColumnType("boolean");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LanguageIsoCode")
                        .HasMaxLength(7)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("Scene")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StoragePath")
                        .HasColumnType("text");

                    b.Property<DateTime?>("StoredAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DownloadUri")
                        .IsUnique();

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.HasIndex("EpisodeId", "Language", "Version");

                    b.ToTable("Subtitles");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.TvShow", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Discovered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("ExternalId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastSeasonRefreshed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<int?>("TmdbId")
                        .HasColumnType("integer");

                    b.Property<int?>("TvdbId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.HasIndex("TvdbId");

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.ToTable("TvShows");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Episode", b =>
                {
                    b.HasOne("AddictedProxy.Database.Model.Shows.TvShow", "TvShow")
                        .WithMany("Episodes")
                        .HasForeignKey("TvShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TvShow");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Season", b =>
                {
                    b.HasOne("AddictedProxy.Database.Model.Shows.TvShow", "TvShow")
                        .WithMany("Seasons")
                        .HasForeignKey("TvShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TvShow");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Subtitle", b =>
                {
                    b.HasOne("AddictedProxy.Database.Model.Shows.Episode", "Episode")
                        .WithMany("Subtitles")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Episode");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Episode", b =>
                {
                    b.Navigation("Subtitles");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.TvShow", b =>
                {
                    b.Navigation("Episodes");

                    b.Navigation("Seasons");
                });
#pragma warning restore 612, 618
        }
    }
}
