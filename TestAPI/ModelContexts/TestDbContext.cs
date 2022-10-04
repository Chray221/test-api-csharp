//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Console;
//using System.Data.Entity;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using TestAPI.Data;

namespace TestAPI.ModelContexts
{
    public class TestDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ImageFile> Images { get; set; }
        //NOTE: when extending DbContext using Microsoft.EntityFrameworkCore;
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
            //NOTE: Update Databse or Migrate new Scheme when extending DbContext  using System.Data.Entity;
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<SchoolDBContext, EF6Console.Migrations.Configuration>());

            //NOTE:
            Database.EnsureCreated();

            //NOTE: Update Databse or Migrate new Scheme when extending DbContext using Microsoft.EntityFrameworkCore;
            //Logger.Log($"MIGRATIONS_START APPLIED={Database.GetAppliedMigrations().Count()} | PENDING={Database.GetPendingMigrations().Count()}");
            //if (Database.GetPendingMigrations().Any())
            //{
            //    Database.Migrate();
            //}
            //Logger.Log($"MIGRATIONS_END APPLIED={Database.GetAppliedMigrations().Count()} | PENDING={Database.GetPendingMigrations().Count()}");
        }

        //NOTE: when extending DbContext using System.Data.Entity;
        //public TestContext() : base("name=TestContext")
        //{
        //    Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        //}

        //NOTE: For Loggin using Console in Database query
        //public static readonly ILoggerFactory consoleLoggerFactory
        //    = new LoggerFactory(new[] {
        //          new ConsoleLoggerProvider((category, level) =>
        //            category == DbLoggerCategory.Database.Command.Name &&
        //            level == LogLevel.Information, true)
        //        });

        //NOTE: For Loggin using Console in Database query
        public static readonly LoggerFactory DbCommandDebugLoggerFactory
          = new LoggerFactory(new[] {
              new DebugLoggerProvider()
          });

        //NOTE: when extending DbContext using Microsoft.EntityFrameworkCore;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Filename=./video_games.sqlite");
            //optionsBuilder.UseSqlServer(@"Server=.\localdb;Database=TestingDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            optionsBuilder
                .UseLoggerFactory(DbCommandDebugLoggerFactory) // to set the logger for DB query
                .EnableSensitiveDataLogging(); // enable logging

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>()
            //    .Property(b => b.CreatedAt)
            //  //.HasDefaultValueSql("datetime('now')"); // using SQLite
            //  .HasDefaultValueSql("getdate()"); // using SQL

            //modelBuilder.Entity<User>()
            //   .Property(b => b.UpatedAt)
            //.HasComputedColumnSql("datetime('now')");

            //NOTE: for adding default value
            //modelBuilder.Entity<User>()
            //    .Property(b => b.ImageFileId)
            //    .HasDefaultValue(Guid.Empty);

            //NOTE: for AutoIncrement
            //modelBuilder.Entity<User>().Property(d => d.Id)
            //    .ValueGeneratedOnAdd();

            // how to add relationship
            //modelBuilder.Entity<User>()
            //    .HasOne<ImageFile>(d => d.ImageFile)
            //    .WithOne(imageFile => imageFile.User)
            //    .HasForeignKey(nameof(User.ImageId))
            //    .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);

        }
    }
}
