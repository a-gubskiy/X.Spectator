using Example.API.Probes;
using Example.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using X.Spectator.Base;

namespace Example.API
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
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddHostedService<CityHostedService>();

            services
                .AddSingleton<ILibraryService, LibraryService>()
                .AddSingleton<IPublishingHouseService, PublishingHouseService>()
                .AddSingleton<ILibraryServiceProbe, LibraryServiceProbe>()
                .AddSingleton<IStateEvaluator<SystemState>, SystemStateEvaluator>()
                .AddSingleton<SystemSpectator>(CreateSystemSpectator);
        }

        private SystemSpectator CreateSystemSpectator(IServiceProvider ctx)
        {
            var stateEvaluator = ctx.GetService<IStateEvaluator<SystemState>>();
            var retentionPeriod = TimeSpan.FromMinutes(5);

            return new SystemSpectator(stateEvaluator, retentionPeriod, SystemState.Normal);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
