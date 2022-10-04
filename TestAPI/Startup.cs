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
using System.Linq;
using System;
using TestAPI.Services.Contracts;
using TestAPI.Services.Concretes;

namespace TestAPI
{
    public class Startup
    {
        private string _envName = string.Empty;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _envName = env.EnvironmentName;

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{_envName}.json",optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //NOTE: for mvc
            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                })
            //NOTE: change Newtonsoft Naming Policy to Snake Casing
                .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() })
            //NOTE: change System.Text.Json Naming Policy to Snake Casing
            //    .AddJsonOptions(options =>
            //       options.JsonSerializerOptions.PropertyNamingPolicy =
            //            new SnakeCasePropertyNamingPolicy())
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllers();

            


            // NOTE: add default versioning
            services.AddApiVersioning(cfg =>
            {
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
                cfg.AssumeDefaultVersionWhenUnspecified = true;
            });

            //NOTE: To use swagger
            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "TestC# API", Version = "v1.0" });
                    c.ResolveConflictingActions(apidescription => apidescription.First());
                })
            //NOTE: added Newtonsoft for Swagger
                .AddSwaggerGenNewtonsoftSupport()

                .AddLogging();

            //NOTE: using sql or sqlite database when sql addition is in the CONTEXT
            //services.AddDbContext<TestContext>();

            
            services.AddDbContext<TestDbContext>(options =>
            {
                #region from string connection
                //NOTE: Using in memory only 
                //options.UseInMemoryDatabase("UserList");

                //NOTE: using sqlite database the same as when sql addition is in the CONTEXT
                //options.UseSqlite("Filename=./test_context.db");

                //NOTE: for SQL Server using string connection
                //options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MvcMovieContext-2;Trusted_Connection=True;MultipleActiveResultSets=true");

                // using Postgresql database
                //options.UseNpgsql(Configuration.GetConnectionString("MyPostgresqlConn"));
                #endregion

                #region From appsetting.json

                #region NOTE: using sqlite database the same as when sql addition is in the CONTEXT using appsettings
                /* NOTE: using sqlite database the same as when sql addition is in the CONTEXT using appsettings
                 * NOTE: add testdb_source.db in the project
                 * NOTE: then add in appsettings.json or appsettings.Development.json
                 * =======================
                 * ,"AllowedHosts": "*",
                 * "ConnectionStrings": 
                 * {
                 *      "MySQLiteSourceConnection": "Data Source=./testdb_source.db"
                 * }
                 */
                options.UseSqlite(Configuration.GetConnectionString("MySQLiteSourceConnection"));
                #endregion

                #region NOTE: FOR SQL using appsettings
                /* NOTE: FOR SQL using appsettings
                 * NOTE: then add in appsettings.json or appsettings.Development.json
                 * =======================
                 * ,"AllowedHosts": "*",
                 * "ConnectionStrings": 
                 * {
                 *     "MvcMovieContext": "Server=(localdb)\*\mssqllocaldb;Database=MvcMovieContext-2;Trusted_Connection=True;MultipleActiveResultSets=true"
                 * }
                 */
                //options.UseSqlServer(Configuration.GetConnectionString("MyPostgresqlConnTemp1"));
                #endregion

                // using Postgresql database
                //options.UseNpgsql(Configuration.GetConnectionString("MyPostgresqlConn"));

                #endregion
            });

            //services
            //    .AddIdentity<User, IdentityRole<long>>()
            //    .AddDefaultTokenProviders();

            // adding services
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IImageFileRepository, ImageFileRepository>();
        }

        //NOTE: This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("Home/Error");
            }

            app.UseSwagger()
               .UseStaticFiles()
               .UseHttpsRedirection()
               .UseRouting()
               .UseAuthorization()
               .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
               .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Test Api");
                });

            app.UseMvc(routes =>
            {
                //default
                routes.MapRoute("default", "{version:apiversion}/api/{controller=Home}/{action=Index}/{id?}");
            });
            
        }

        //public static void ConfigureWebApi(IApplicationBuilder applicationBuilder)
        //{
        //    applicationBuilder.UseCors();
        //    applicationBuilder.UseWebSockets();
        //}

    }
}
