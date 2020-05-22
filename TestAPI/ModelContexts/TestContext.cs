using System;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Console;
using TestAPI.Models;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

namespace TestAPI.ModelContexts
{
    public class TestContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        //NOTE: when extending DbContext using Microsoft.EntityFrameworkCore;
        public TestContext(DbContextOptions<TestContext> options) : base(options)
        {

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
                .EnableSensitiveDataLogging() // enable logging
                .UseSqlite("Filename=./test_context.db")
                ;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("datetime('now')");
            //modelBuilder.Entity<User>()
            //   .Property(b => b.UpatedAt)
               //.HasComputedColumnSql("datetime('now')");
            base.OnModelCreating(modelBuilder);
        }
    }
}
