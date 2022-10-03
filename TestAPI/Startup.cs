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
            //NOTE: for mvc
            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                })
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
                .AddLogging();

            //NOTE: using sql or sqlite database when sql addition is in the CONTEXT
            //services.AddDbContext<TestContext>();

            
            services.AddDbContext<TestContext>(options =>
            {
                #region from string connection
                //NOTE: Using in memory only 
                options.UseInMemoryDatabase("UserList");

                //NOTE: using sqlite database the same as when sql addition is in the CONTEXT
                //options.UseSqlite("Filename=./test_context.db");

                //NOTE: for SQL Server using string connection
                //options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MvcMovieContext-2;Trusted_Connection=True;MultipleActiveResultSets=true");

                // using Postgresql database
                //options.UseNpgsql(Configuration.GetConnectionString("MyPostgresqlConn"));
                #endregion

                #region From appsetting.json
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

                // using Postgresql database
                //options.UseNpgsql(Configuration.GetConnectionString("MyPostgresqlConn"));
                #endregion
            });

            //services
            //    .AddIdentity<User, IdentityRole<long>>()
            //    .AddDefaultTokenProviders();

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() });

            //NOTE: change Newtonsoft Naming Policy to Snake Casing
            services
                .AddMvc()
                .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() });

            //NOTE: change System.Text.Json Naming Policy to Snake Casing
            //services.AddMvc()
            //    .AddJsonOptions(options =>
            //    options.JsonSerializerOptions.PropertyNamingPolicy =
            //        new SnakeCasePropertyNamingPolicy());

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
        }

        public static void ConfigureWebApi(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseCors();
            applicationBuilder.UseWebSockets();
        }
    }
}
