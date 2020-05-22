using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestAPI.ModelContexts;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;

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

            //NOTE: using sql or sqlite database the same as when sql addition is in the CONTEXT
            services.AddDbContext<TestContext>(options =>
            {
                options.UseSqlite("Filename=./test_context.db");
            });

            //NOTE: FOR SQL
            //services.AddDbContext<TestContext>(options => {
            //    options.UseSqlServer("server=.;database=myDb;trusted_connection=true;"));
            //});

            //NOTE: FOR SQL using appsettings
            //services.AddDbContext<ConfigurationContext>(options => {
            //    options.UseSqlServer(Configuration.GetConnectionString("MyConnection"));
            //});
            //NOTE: then add in appsettings.json or appsettings.Development.json
            //=======================
            //,"AllowedHosts": "*",
            //"ConnectionStrings": {
            //    "MyConnection": "server=.;database=myDb;trusted_connection=true;"
            //  }

            services.AddControllers()
            .AddNewtonsoftJson();

            //NOTE: change Newtonsoft Naming Policy to Snake Casing
            services.AddMvc()
            .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() });

            //NOTE: change System.Text.Json Naming Policy to Snake Casing
            //services.AddMvc()
            //    .AddJsonOptions(options =>
            //    options.JsonSerializerOptions.PropertyNamingPolicy =
            //        new SnakeCasePropertyNamingPolicy());

            services.AddControllers();
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
