using System.Data.Entity;
using PerformanceEvaluator.DAL.Entities;

namespace PerformanceEvaluator.DAL.Context
{
    public class PerformanceEvaluatorContext : DbContext
    {
        public PerformanceEvaluatorContext() : base("PerformanceEvaluatorConnection")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<PerformanceEvaluatorContext>());
        }

        public virtual DbSet<Website> Websites { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<ResponseTime> RequestTimes { get; set; }
    }
}
