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
using TestAPI.Helpers;
using TestAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            #region For MVC Controller Settings
            //NOTE: for mvc
            services.AddMvc(options =>
                {
                    options.Filters.Add<HttpResponseExceptionFilter>();
                    //options.Filters.Add<AutoAuthenticationFilter>();
                    options.EnableEndpointRouting = false;
                })
                //NOTE: change Newtonsoft Naming Policy to Snake Casing
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver =
                         new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() };
                    // ignore reference loop
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                //NOTE: change System.Text.Json Naming Policy to Snake Casing
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy =
                         new SnakeCasePropertyNamingPolicy();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //NOTE: for controller
            services.AddControllers();
            services.AddControllersWithViews();
            #endregion

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

            //NOTE: to set controller name to lowecase
            services.AddRouting(options => options.LowercaseUrls = true);

            ConfigureDbContext(services);
            if (Configuration.GetValue<bool>("JWT:IsEnabled"))
            {
                ConfigureJWTAuthentication(services);
                //ConfigureJWTAuthenticationV2(services);
            }
            else
            {
                ConfigureDefaultAuthentication(services);
            }
            ConfigureRepositories(services);
        }

        //NOTE: This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error/development");
                app.UseHsts();
            }
            else
            {
                app.UseExceptionHandler("Home/Error");
                //app.UseExceptionHandler("/error");
            }
            
            app.UseSwagger()
               .UseStaticFiles()
               .UseHttpsRedirection()
               .UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                })
               .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Test Api");
                });

            // how to map api route 
            app.UseMvc(routes =>
            {
                //default
                routes.MapRoute(
                    name:"default",
                    template: "v{version:apiversion}/api/{controller}/{action}");
            });

            //NOTE: if using filters
            app.UseHostFiltering();
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            //NOTE: using sql or sqlite database when sql addition is in the CONTEXT
            //services.AddDbContext<TestContext>();

            //NOTE: add dbcontext
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
        }

        private void ConfigureJWTAuthentication(IServiceCollection services)
        {
            //adding indentity to DbContext
            services.AddIdentity<ApplicationUser, IdentityRole>()
                //adding indentity to DbContext
                .AddEntityFrameworkStores<TestDbContext>()
                //.AddSignInManager()
                .AddDefaultTokenProviders();

            #region Adding Authentication
            // Adding Authentication  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                // Adding Jwt Bearer  
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["JWT:ValidAudience"],
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                    };
                });
            services.AddAuthorization();
            #endregion
        }

        private void ConfigureJWTAuthenticationV2(IServiceCollection services)
        {
            //adding indentity to DbContext
            services.AddIdentity<ApplicationUser, IdentityRole>()
                //adding indentity to DbContext
                .AddEntityFrameworkStores<TestDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            #region Adding Authentication
            // Adding Authentication  
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                // Adding Jwt Bearer  
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = Configuration["JWT:ValidAudience"],
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                    };
                });
            services.AddAuthorization();
            #endregion
        }

        private static void ConfigureDefaultAuthentication(IServiceCollection services)
        {
            services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TestDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(o => { });
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            // adding services
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IImageFileRepository, ImageFileRepository>();
            services.AddTransient<IJwtSerivceManager, JwtSerivceManager>();
        }

    }
}
