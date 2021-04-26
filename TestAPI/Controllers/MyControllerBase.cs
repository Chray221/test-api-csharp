using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI.Helpers;
using TestAPI.ModelContexts;

namespace TestAPI.Controllers
{
    public class MyControllerBase<T> : ControllerBase where T : DbContext
    {
        // initializing DB connected by Context
        public T dbContext;

        public string RootPath { get { return Host.HostEnvironment.WebRootPath ?? Host.HostEnvironment.ContentRootPath; } }

        public MyControllerBase(T context, IWebHostEnvironment environment)
        {
            Host.HostEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            dbContext = context;
            Logger.Log($"PATH {RootPath}");
        }
    }
}
