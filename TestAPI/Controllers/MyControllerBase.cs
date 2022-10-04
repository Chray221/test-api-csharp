using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Helpers;
using TestAPI.ModelContexts;

namespace TestAPI.Controllers
{
    public class MyControllerBase : ControllerBase
    {
        // initializing DB connected by Context
        public TestDbContext testContext;

        public string RootPath { get { return Host.HostEnvironment.WebRootPath ?? Host.HostEnvironment.ContentRootPath; } }

        public MyControllerBase(TestDbContext context, IWebHostEnvironment environment)
        {
            Host.HostEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            testContext = context;
            Logger.Log($"PATH {RootPath}");
        }
    }
}
