using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_api_redis_test.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using StackExchange.Redis;

namespace dotnet_api_redis_test
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
            services.AddControllers();
            InitRedis(services);
            services.AddScoped<RedisService>();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
            .Enrich.With()
            .CreateLogger();
        }

        private void InitRedis(IServiceCollection services)
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(Configuration["RedisConnectionString"]);
            options.SyncTimeout = 10 * 10000;
            var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(options);
            services.AddSingleton(redisConnectionMultiplexer);
            Console.WriteLine("InitRedis success.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
