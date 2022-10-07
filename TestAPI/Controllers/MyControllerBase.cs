using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Helpers;

namespace TestAPI.Controllers
{
    public class MyControllerBase : ControllerBase
    {

        public string RootPath { get { return Host.HostEnvironment.WebRootPath ?? Host.HostEnvironment.ContentRootPath; } }
        public string RootUrl = "";

        public MyControllerBase(IWebHostEnvironment environment)
        {
            Host.HostEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            Logger.Log($"PATH {RootPath}");
        }
    }
}
