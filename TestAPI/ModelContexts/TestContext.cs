using System;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore;
using TestAPI.Models;

namespace TestAPI.ModelContexts
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
        }
        //public TestContext() : base("name=TestContext")
        //{
        //    this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        //}

        public DbSet<User> Users { get; set; }
    }
}
