using ComputationalLinguistics.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComputationalLinguistics.DAL
{
    public class ComputationalLinguisticsContext : DbContext
    {
        public DbSet<Word> Words { get; set; }
        public DbSet<TextFile> TextFiles { get; set; }
        public DbSet<WordInText> WordsInText { get; set; }
        public DbSet<TagInfo> TagsInfo { get; set; }

        public ComputationalLinguisticsContext(DbContextOptions<ComputationalLinguisticsContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WordInText>()
                .HasKey(o => new { o.TextFileId, Seek = o.OffSet });
        }
    }
}