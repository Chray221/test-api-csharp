using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestAPI.ModelContexts;
using Newtonsoft.Json.Serialization;

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

            //NOTE: using sql database
            services.AddDbContext<TestContext>();


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
