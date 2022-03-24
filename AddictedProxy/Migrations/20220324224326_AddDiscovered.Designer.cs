﻿// <auto-generated />
using System;
using AddictedProxy.Database;
using AddictedProxy.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AddictedProxy.Migrations
{
    [DbContext(typeof(EntityContext))]
    [Migration("20220324224326_AddDiscovered")]
    partial class AddDiscovered
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("AddictedProxy.Model.Shows.Episode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Season")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TvShowId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TvShowId", "Season", "Number")
                        .IsUnique();

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("AddictedProxy.Model.Shows.Season", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TvShowId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TvShowId", "Number")
                        .IsUnique();

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("AddictedProxy.Model.Shows.Subtitle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Completed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Corrected")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DownloadUri")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("EpisodeId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HD")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HearingImpaired")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DownloadUri")
                        .IsUnique();

                    b.HasIndex("EpisodeId");

                    b.ToTable("Subtitles");
                });

            modelBuilder.Entity("AddictedProxy.Model.Shows.TvShow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Discovered")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastEpisodeRefreshed")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastSeasonRefreshed")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TvShows");
                });

            modelBuilder.Entity("AddictedProxy.Model.Shows.Episode", b =>
                {
                    b.HasOne("AddictedProxy.Model.Shows.TvShow", "TvShow")
                        .WithMany("Episodes")
                        .HasForeignKey("TvShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TvShow");
                });

            modelBuilder.Entity("AddictedProxy.Model.Shows.Season", b =>
                {
                    b.HasOne("AddictedProxy.Model.Shows.TvShow", "TvShow")
                        .WithMany("Seasons")
                        .HasForeignKey("TvShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TvShow");
                });

            modelBuilder.Entity("AddictedProxy.Model.Shows.Subtitle", b =>
                {
                    b.HasOne("AddictedProxy.Model.Shows.Episode", "Episode")
                        .WithMany("Subtitles")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Episode");
                });

            modelBuilder.Entity("AddictedProxy.Model.Shows.Episode", b =>
                {
                    b.Navigation("Subtitles");
                });

            modelBuilder.Entity("AddictedProxy.Model.Shows.TvShow", b =>
                {
                    b.Navigation("Episodes");

                    b.Navigation("Seasons");
                });
#pragma warning restore 612, 618
        }
    }
}
