using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BigDataBoost.Data;
using BigDataBoost.Data.Repositories;
using BigDataBoost.Data.Abstract;
using BigDataBoost.API.ViewModels.Mappings;
using System.Net;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using BigDataBoost.API.Core;
using BigDataBoost.Data.HostedServices;
using System.Threading.Tasks;
using System.Threading;

namespace BigDataBoost.API
{
    public class Startup
    {
        private static string _applicationPath = string.Empty;
        string sqlConnectionString = string.Empty;
        bool useInMemoryProvider = false;
        int useRealTimeValuesFrequency = 10;
        public IConfigurationRoot Configuration { get; }
        public Startup(IHostingEnvironment env)
        {
            _applicationPath = env.WebRootPath;
            // Setup configuration sources.

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            try
            {
                useInMemoryProvider = bool.Parse(Configuration["AppSettings:InMemoryProvider"]);
                useRealTimeValuesFrequency = int.Parse(Configuration["AppSettings:RealTimeValuesFrequency"]);
            }
            catch { }

            services.AddDbContext<BigDataBoostContext>(options => {
                switch (useInMemoryProvider)
                {
                    case true:
                        options.UseInMemoryDatabase();
                        break;
                    default:
                        options.UseSqlServer(sqlConnectionString,
                    b => b.MigrationsAssembly("BigDataBoost.API"));
                    break;
                }
            });
            

            // Repositories
            services.AddScoped<ITagDefRepository, TagDefRepository>();
            services.AddScoped<ITagHistRepository, TagHistRepository>();


            // Automapper Configuration
            AutoMapperConfiguration.Configure();

            // Enable Cors
            services.AddCors();

            // Add MVC services to the services container.
            services.AddMvc()
                .AddJsonOptions(opts =>
                {
                    // Force Camel Case to JSON
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            // Hosted services
            //services.AddSingleton<RealTimeUpdaterTask>();
            //services.AddSingleton<IHostedService, RealTimeUpdaterService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            // Add MVC to the request pipeline.
            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseExceptionHandler(
              builder =>
              {
                  builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
              });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                // Uncomment the following line to add a route for porting Web API 2 controllers.
                //routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            });

            BigDataBoostDbInitializer.Initialize(app.ApplicationServices);

            var wtoken = new CancellationTokenSource();

            var task = Task.Factory.StartNew(() =>
            {
                int modValue = 5;
                int counter = 0;
                while (true)
                {
                    wtoken.Token.ThrowIfCancellationRequested();
                    BigDataBoostDbInitializer.GenerateRunTimeData((counter % modValue == 0? Model.TagStatus.Error : Model.TagStatus.Good));
                    Thread.Sleep(useRealTimeValuesFrequency * 1000);
                    counter++;
                    if (counter > 100)
                        counter = 0;
                }
            });
        }
    }
}
