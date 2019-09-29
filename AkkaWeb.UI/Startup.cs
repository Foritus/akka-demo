using System;
using Akka.Actor;
using AkkaWeb.UI.Actors;
using AkkaWeb.UI.Actors.API;
using AkkaWeb.UI.Actors.Messages;
using AkkaWeb.UI.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaWeb.UI
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var system = ActorSystem.Create("AkkaWeb");
            services.AddSingleton(system);
            services.AddSingleton(prov =>
            {
                var actorSystem = prov.GetService<ActorSystem>();
                var balanceHub = prov.GetService<IHubContext<BalanceHub>>();
                return new TypedActorRef<BankActor>(actorSystem.ActorOf(Props.Create(() => new BankActor(balanceHub))));
            });

            var allCustomers = system.ActorSelection("akka://AkkaWeb/user/Customer*");
            system.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromMilliseconds(100), allCustomers, new SpendRandom(), ActorRefs.NoSender);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvcWithDefaultRoute();
            app.UseSignalR(config =>
            {
                config.MapHub<BalanceHub>("/BalanceHub");
            });
        }
    }
}
