﻿// <auto-generated />
using System;
using AddictedProxy.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AddictedProxy.Database.Migrations
{
    [DbContext(typeof(EntityContext))]
    partial class EntityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.9");

            modelBuilder.Entity("AddictedProxy.Database.Model.Credentials.AddictedUserCredentials", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Cookie")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DownloadExceededDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("DownloadUsage")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastUsage")
                        .HasColumnType("TEXT");

                    b.Property<int>("Usage")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Cookie")
                        .IsUnique();

                    b.ToTable("AddictedUserCredentials");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Episode", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Discovered")
                        .HasColumnType("TEXT");

                    b.Property<long>("ExternalId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Season")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("TvShowId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TvShowId", "Season", "Number")
                        .IsUnique();

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Season", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastRefreshed")
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TvShowId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TvShowId", "Number")
                        .IsUnique();

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Shows.Subtitle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Completed")
                        .HasColumnType("INTEGER");

                    b.Property<double>("CompletionPct")
                        .HasColumnType("REAL");

                    b.Property<bool>("Corrected")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Discovered")
                        .HasColumnType("TEXT");

                    b.Property<long>("DownloadCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DownloadUri")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("EpisodeId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HD")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HearingImpaired")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Scene")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StoragePath")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("StoredAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

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
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Discovered")
                        .HasColumnType("TEXT");

                    b.Property<long>("ExternalId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastSeasonRefreshed")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .UseCollation("NOCASE");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TmdbId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("UniqueId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.HasIndex("UniqueId")
                        .IsUnique();

                    b.ToTable("TvShows");
                });

            modelBuilder.Entity("AddictedProxy.Database.Model.Stats.ShowPopularity", b =>
                {
                    b.Property<long>("TvShowId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastRequestedDate")
                        .HasColumnType("TEXT");

                    b.Property<long>("RequestedCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("TvShowId", "Language");

                    b.ToTable("ShowPopularity");
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

            modelBuilder.Entity("AddictedProxy.Database.Model.Stats.ShowPopularity", b =>
                {
                    b.HasOne("AddictedProxy.Database.Model.Shows.TvShow", "TvShow")
                        .WithMany()
                        .HasForeignKey("TvShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TvShow");
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
