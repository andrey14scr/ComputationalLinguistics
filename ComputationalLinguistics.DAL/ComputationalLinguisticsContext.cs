using ComputationalLinguistics.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComputationalLinguistics.DAL
{
    public class ComputationalLinguisticsContext : DbContext
    {
        public DbSet<Word> Words { get; set; }

        public ComputationalLinguisticsContext(DbContextOptions<ComputationalLinguisticsContext> options) : base(options) { }
    }
}