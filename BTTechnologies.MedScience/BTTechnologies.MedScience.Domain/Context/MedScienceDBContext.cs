using System.Data.Entity;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Context
{
    /// <summary>
    /// Context
    /// </summary>
    public class MedScienceDBContext : DbContext
    {
        /// <summary>
        /// Creating model keys
        /// </summary>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().
                HasMany(c => c.Roles).
                WithMany(p => p.Accounts).
                Map(
                    m =>
                        {
                            m.ToTable("AccountRoles");
                            m.MapLeftKey("IdAccount");
                            m.MapRightKey("IdRole");
                        });

            modelBuilder.Entity<Article>().
                HasMany(a => a.Authors).
                WithMany(au => au.Articles).
                Map(
                    m =>
                        {
                            m.ToTable("ArticlesAuthors");
                            m.MapLeftKey("IdArticle");
                            m.MapRightKey("IdAuthor");
                        });

            modelBuilder.Entity<Article>().
                HasMany(a => a.Attachments).
                WithMany(da => da.Articles).
                Map(
                    m =>
                        {
                            m.ToTable("ArticlesAttachments");
                            m.MapLeftKey("IdArticle");
                            m.MapRightKey("IdDocAttachment");
                        });
            
            modelBuilder.Entity<Article>().
                HasMany(c => c.Categories).
                WithMany(p => p.Articles).
                Map(
                    m =>
                        {
                            m.ToTable("ArticlesCategoriesRelations");
                            m.MapLeftKey("IdArticle");
                            m.MapRightKey("IdCategory");
                        });
            
            modelBuilder.Entity<Author>().HasRequired(a => a.Account).WithMany(ac => ac.Authors).HasForeignKey(
                a => a.AccountId);

            modelBuilder.Entity<AccountActivationCode>().HasRequired(a => a.Account).WithOptional().Map(m => m.MapKey("IdAccount"));
        }

        /// <summary>
        /// Accounts
        /// </summary>
        public DbSet<Account> Accounts { get; set; }
        
        /// <summary>
        /// Roles
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Authors
        /// </summary>
        public DbSet<Author> Authors { get; set; }
        
        /// <summary>
        /// Authors data view
        /// </summary>
        public DbSet<AuthorsFullDataRecord> AuthorsStatisticsRecords { get; set; }

        /// <summary>
        /// Articles
        /// </summary>
        public DbSet<Article> Articles { get; set; }

        /// <summary>
        /// Articles data view
        /// </summary>
        public DbSet<ArticlesFullDataRecord> ArticlesFullDataRecords { get; set; }

        /// <summary>
        /// Attachments of documents
        /// </summary>
        public DbSet<DocAttachment> DocAttachments { get; set; }

        /// <summary>
        /// Categories of documents
        /// </summary>
        public DbSet<ArticleCategory> ArticleCategories { get; set; }

        /// <summary>
        /// Articles changes logs
        /// </summary>
        public DbSet<ArticleChangesLog> ArticleChangesLogs { get; set; }

        /// <summary>
        /// Accounts activation codes
        /// </summary>
        public DbSet<AccountActivationCode> AccountActivationCodes { get; set; }

        /// <summary>
        /// Files uploaded to site
        /// </summary>
        public DbSet<SiteFile> SiteFiles { get; set; }

        /// <summary>
        /// Tiles which are shown on the pages
        /// </summary>
        public DbSet<PageTile> PageTiles { get; set; }
    }
}
