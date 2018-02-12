using EFC.SamuraiApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace EFC.SamuraiApp.Data
{
    public class SamuraiContext : DbContext
    {
        private static readonly LoggerFactory ConsoleLoggerFactory = new LoggerFactory(
            new [] { new ConsoleLoggerProvider(
                (category, level) => category == DbLoggerCategory.Database.Command.Name && 
                                     level    == LogLevel.Information, true
            )}    
        );


        #region Ctor

        public SamuraiContext()
        { }

        public SamuraiContext(DbContextOptions<SamuraiContext> options)
            : base(options)
        { }
        
        #endregion Ctor


        #region DbSets

        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }

        #endregion DbSets


        #region Override Methods

        protected override void OnConfiguring(DbContextOptionsBuilder opts)
        {
            //optionsBuilder.UseSqlServer(connStr);
            // TODO: review if IsConfigured when it's instanced by SamuraiContext(DbContextOptions<SamuraiContext> options)
            if (!opts.IsConfigured)
            {
                var connStr = "Server=(localdb)\\mssqllocaldb; Database=SamuraiAppCore; Trusted_Connection=True;";
                opts.UseSqlServer(connStr, o => o.MaxBatchSize(500));
            }

            opts.UseLoggerFactory(ConsoleLoggerFactory)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>()
                .HasKey(s => new { s.SamuraiId, s.BattleId });
        }

        #endregion Override Methods
    }
}
