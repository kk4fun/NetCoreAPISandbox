﻿namespace NetCoreApiSandbox.Migrations
{
    #region

    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using NetCoreApiSandbox.Infrastructure;

    #endregion

    [DbContext(typeof(NetCoreSandboxApiContext))]
    internal class NetCoreSandboxApiContextModelSnapshot: ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("NetCoreApiSandbox.Domain.Article",
                                b =>
                                {
                                    b.Property<int>("Id")
                                     .ValueGeneratedOnAdd()
                                     .HasColumnName("id")
                                     .HasColumnType("INTEGER");

                                    b.Property<int?>("AuthorId").HasColumnType("INTEGER");

                                    b.Property<string>("Body").HasColumnType("TEXT");

                                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");

                                    b.Property<string>("Description").HasColumnType("TEXT");

                                    b.Property<string>("Slug").HasColumnType("TEXT");

                                    b.Property<string>("Title").HasColumnType("TEXT");

                                    b.Property<DateTime>("UpdatedAt").HasColumnType("TEXT");

                                    b.HasKey("Id");

                                    b.HasIndex("AuthorId");

                                    b.ToTable("Articles");
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.ArticleFavorite",
                                b =>
                                {
                                    b.Property<int>("ArticleId").HasColumnType("INTEGER");

                                    b.Property<int>("PersonId").HasColumnType("INTEGER");

                                    b.HasKey("ArticleId", "PersonId");

                                    b.HasIndex("PersonId");

                                    b.ToTable("ArticleFavorites");
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.ArticleTag",
                                b =>
                                {
                                    b.Property<int>("ArticleId").HasColumnType("INTEGER");

                                    b.Property<string>("TagId").HasColumnType("TEXT");

                                    b.HasKey("ArticleId", "TagId");

                                    b.HasIndex("TagId");

                                    b.ToTable("ArticleTags");
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.Comment",
                                b =>
                                {
                                    b.Property<int>("Id")
                                     .ValueGeneratedOnAdd()
                                     .HasColumnName("id")
                                     .HasColumnType("INTEGER");

                                    b.Property<int>("ArticleId").HasColumnType("INTEGER");

                                    b.Property<int>("AuthorId").HasColumnType("INTEGER");

                                    b.Property<string>("Body").HasColumnType("TEXT");

                                    b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");

                                    b.Property<DateTime>("UpdatedAt").HasColumnType("TEXT");

                                    b.HasKey("Id");

                                    b.HasIndex("ArticleId");

                                    b.HasIndex("AuthorId");

                                    b.ToTable("Comments");
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.FollowedPeople",
                                b =>
                                {
                                    b.Property<int>("ObserverId").HasColumnType("INTEGER");

                                    b.Property<int>("TargetId").HasColumnType("INTEGER");

                                    b.HasKey("ObserverId", "TargetId");

                                    b.HasIndex("TargetId");

                                    b.ToTable("FollowedPeople");
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.Person",
                                b =>
                                {
                                    b.Property<int>("Id")
                                     .ValueGeneratedOnAdd()
                                     .HasColumnName("id")
                                     .HasColumnType("INTEGER");

                                    b.Property<string>("Bio").HasColumnType("TEXT");

                                    b.Property<string>("Email").HasColumnType("TEXT");

                                    b.Property<byte[]>("Hash").HasColumnType("BLOB");

                                    b.Property<string>("Image").HasColumnType("TEXT");

                                    b.Property<byte[]>("Salt").HasColumnType("BLOB");

                                    b.Property<string>("Username").HasColumnType("TEXT");

                                    b.HasKey("Id");

                                    b.ToTable("Persons");
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.Tag",
                                b =>
                                {
                                    b.Property<string>("Id").HasColumnName("id").HasColumnType("TEXT");

                                    b.HasKey("Id");

                                    b.ToTable("Tags");
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.Article",
                                b =>
                                {
                                    b.HasOne("NetCoreApiSandbox.Domain.Person", "Author")
                                     .WithMany()
                                     .HasForeignKey("AuthorId");
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.ArticleFavorite",
                                b =>
                                {
                                    b.HasOne("NetCoreApiSandbox.Domain.Article", "Article")
                                     .WithMany("ArticleFavorites")
                                     .HasForeignKey("ArticleId")
                                     .OnDelete(DeleteBehavior.Cascade)
                                     .IsRequired();

                                    b.HasOne("NetCoreApiSandbox.Domain.Person", "Person")
                                     .WithMany("ArticleFavorites")
                                     .HasForeignKey("PersonId")
                                     .OnDelete(DeleteBehavior.Cascade)
                                     .IsRequired();
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.ArticleTag",
                                b =>
                                {
                                    b.HasOne("NetCoreApiSandbox.Domain.Article", "Article")
                                     .WithMany("ArticleTags")
                                     .HasForeignKey("ArticleId")
                                     .OnDelete(DeleteBehavior.Cascade)
                                     .IsRequired();

                                    b.HasOne("NetCoreApiSandbox.Domain.Tag", "Tag")
                                     .WithMany("ArticleTags")
                                     .HasForeignKey("TagId")
                                     .OnDelete(DeleteBehavior.Cascade)
                                     .IsRequired();
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.Comment",
                                b =>
                                {
                                    b.HasOne("NetCoreApiSandbox.Domain.Article", "Article")
                                     .WithMany("Comments")
                                     .HasForeignKey("ArticleId")
                                     .OnDelete(DeleteBehavior.Cascade)
                                     .IsRequired();

                                    b.HasOne("NetCoreApiSandbox.Domain.Person", "Author")
                                     .WithMany()
                                     .HasForeignKey("AuthorId")
                                     .OnDelete(DeleteBehavior.Cascade)
                                     .IsRequired();
                                });

            modelBuilder.Entity("NetCoreApiSandbox.Domain.FollowedPeople",
                                b =>
                                {
                                    b.HasOne("NetCoreApiSandbox.Domain.Person", "Observer")
                                     .WithMany("Followers")
                                     .HasForeignKey("ObserverId")
                                     .OnDelete(DeleteBehavior.Restrict)
                                     .IsRequired();

                                    b.HasOne("NetCoreApiSandbox.Domain.Person", "Target")
                                     .WithMany("Following")
                                     .HasForeignKey("TargetId")
                                     .OnDelete(DeleteBehavior.Restrict)
                                     .IsRequired();
                                });
#pragma warning restore 612, 618
        }
    }
}
