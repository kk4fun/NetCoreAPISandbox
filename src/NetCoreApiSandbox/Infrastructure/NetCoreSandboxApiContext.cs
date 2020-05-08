namespace NetCoreApiSandbox.Infrastructure
{
    #region

    using System.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using NetCoreApiSandbox.Domain;

    #endregion

    public class NetCoreSandboxApiContext: DbContext
    {
        private IDbContextTransaction _currentTransaction;

        public NetCoreSandboxApiContext(DbContextOptions options): base(options) { }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<ArticleTag> ArticleTags { get; set; }

        public DbSet<ArticleFavorite> ArticleFavorites { get; set; }

        public DbSet<FollowedPeople> FollowedPeople { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleTag>(b =>
            {
                b.HasKey(t => new { t.ArticleId, t.TagId });

                b.HasOne(pt => pt.Article).WithMany(p => p.ArticleTags).HasForeignKey(pt => pt.ArticleId);

                b.HasOne(pt => pt.Tag).WithMany(t => t.ArticleTags).HasForeignKey(pt => pt.TagId);
            });

            modelBuilder.Entity<ArticleFavorite>(b =>
            {
                b.HasKey(t => new { t.ArticleId, PersonId = t.UserId });

                b.HasOne(pt => pt.Article).WithMany(p => p.ArticleFavorites).HasForeignKey(pt => pt.ArticleId);

                b.HasOne(pt => pt.User).WithMany(t => t.ArticleFavorites).HasForeignKey(pt => pt.UserId);
            });

            modelBuilder.Entity<FollowedPeople>(b =>
            {
                b.HasKey(t => new { t.ObserverId, t.TargetId });

                // we need to add OnDelete RESTRICT otherwise for the SqlServer database provider, 
                // app.ApplicationServices.GetRequiredService<NetCoreSandboxApiContext>().Database.EnsureCreated(); throws the following error:
                // System.Data.SqlClient.SqlException
                // HResult = 0x80131904
                // Message = Introducing FOREIGN KEY constraint 'FK_FollowedPeople_Persons_TargetId' on table 'FollowedPeople' may cause cycles or multiple cascade paths.Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
                // Could not create constraint or index. See previous errors.
                b.HasOne(pt => pt.Observer)
                 .WithMany(p => p.Followers)
                 .HasForeignKey(pt => pt.ObserverId)
                 .OnDelete(DeleteBehavior.Restrict);

                // we need to add OnDelete RESTRICT otherwise for the SqlServer database provider, 
                // app.ApplicationServices.GetRequiredService<NetCoreSandboxApiContext>().Database.EnsureCreated(); throws the following error:
                // System.Data.SqlClient.SqlException
                // HResult = 0x80131904
                // Message = Introducing FOREIGN KEY constraint 'FK_FollowingPeople_Persons_TargetId' on table 'FollowedPeople' may cause cycles or multiple cascade paths.Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
                // Could not create constraint or index. See previous errors.
                b.HasOne(pt => pt.Target)
                 .WithMany(t => t.Following)
                 .HasForeignKey(pt => pt.TargetId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(user =>
            {
                user.HasOne(user => user.Person)
                    .WithOne(person => person.User)
                    .HasForeignKey<Person>(user => user.Id);
            });
        }

        #region Transaction Handling

        public void BeginTransaction()
        {
            if (this._currentTransaction != null)
            {
                return;
            }

            if (!this.Database.IsInMemory())
            {
                this._currentTransaction = this.Database.BeginTransaction(IsolationLevel.ReadCommitted);
            }
        }

        public void CommitTransaction()
        {
            try
            {
                this._currentTransaction?.Commit();
            }
            catch
            {
                this.RollbackTransaction();

                throw;
            }
            finally
            {
                if (this._currentTransaction != null)
                {
                    this._currentTransaction.Dispose();
                    this._currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                this._currentTransaction?.Rollback();
            }
            finally
            {
                if (this._currentTransaction != null)
                {
                    this._currentTransaction.Dispose();
                    this._currentTransaction = null;
                }
            }
        }

        #endregion
    }
}
