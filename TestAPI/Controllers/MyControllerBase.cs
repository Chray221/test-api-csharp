﻿using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Helpers;
using TestAPI.ModelContexts;

namespace TestAPI.Controllers
{
    public class MyControllerBase : ControllerBase
    {

        public string RootPath { get { return Host.HostEnvironment.WebRootPath ?? Host.HostEnvironment.ContentRootPath; } }

        public MyControllerBase(IWebHostEnvironment environment)
        {
            Host.HostEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            Logger.Log($"PATH {RootPath}");
        }
    }
}
