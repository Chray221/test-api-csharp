using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestAPI.ModelContexts;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Models;
using Microsoft.AspNetCore.Identity;
using TestAPI.Helpers;
using Newtonsoft.Json;

namespace TestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //NOTE: Using in memory only 
            // services.AddDbContext<TestContext>((opt) =>
            //opt.UseInMemoryDatabase("UserList"));

            //NOTE: using sql or sqlite database when sql addition is in the CONTEXT
            //services.AddDbContext<TestContext>();

            //NOTE: using sqlite database the same as when sql addition is in the CONTEXT
            //services.AddDbContext<TestContext>(options =>
            //{
            //    options.UseSqlite("Filename=./test_context.db");
            //});

            //NOTE: using sqlite database the same as when sql addition is in the CONTEXT using appsettings
            //services.AddDbContext<TestContext>(options =>
            //{
            //    options.UseSqlite(Configuration.GetConnectionString("MySQLiteSourceConnection"));
            //});

            //NOTE: FOR SQL
            //services.AddDbContext<TestContext>(options =>
            //{
            //    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MvcMovieContext-2;Trusted_Connection=True;MultipleActiveResultSets=true");
            //});

            //NOTE: FOR SQL using appsettings
            //services.AddDbContext<TestContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("MyPostgresqlConnTemp1")));
            //NOTE: then add in appsettings.json or appsettings.Development.json
            //=======================
            //,"AllowedHosts": "*",
            //"ConnectionStrings": {
            //    "MvcMovieContext": "Server=(localdb)\\mssqllocaldb;Database=MvcMovieContext-2;Trusted_Connection=True;MultipleActiveResultSets=true"
            //}

            // using Postgresql database
            services.AddDbContext<TestContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("MyPostgresqlConn")));
            services.AddDbContext<CurbsideContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("CurbsidePostgresqlConn")));

            //services.AddIdentity<User, IdentityRole<long>>()
            //    .AddDefaultTokenProviders();

            services.AddControllers()
            .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() });

            //NOTE: change Newtonsoft Naming Policy to Snake Casing
            services.AddMvc()
            .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() });

            services.AddMvcCore()
                .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() });
            SetupGlobalOptions();

            //NOTE: change System.Text.Json Naming Policy to Snake Casing
            //services.AddMvc()
            //    .AddJsonOptions(options =>
            //    options.JsonSerializerOptions.PropertyNamingPolicy =
            //        new SnakeCasePropertyNamingPolicy());
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var problems = new BadRequestMessageFormatter(context);
                        //return new BadRequestObjectResult(problems);
                        return new BadRequestObjectResult(problems.GetCustomBadRequestFormat());
                    };
                });

            //generic bad request message
            //services.Configure<ApiBehaviorOptions>(a =>
            //{
            //    a.InvalidModelStateResponseFactory = context =>
            //    {
            //        var problemDetails = new BadRequestMessageFormatter(context);
            //        //return MessageExtension.ShowRequiredMessage("asdf");
            //        return new BadRequestObjectResult(problemDetails)
            //        {
            //            Value = MessageExtension.ShowRequiredMessage("asdf"),
            //            ContentTypes = { "application / problem + json", "application / problem + xml" }
            //        };
            //    };
            //});
        }

        public void SetupGlobalOptions()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() }
            };
        }

        //NOTE: This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
