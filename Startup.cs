// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer.Models;
using IdentityServer.Services;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace IdentityServer
{
    public class Startup
    { 
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            //    services.AddControllersWithViews();


            services.AddControllersWithViews();
            services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                  //  options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                });





            //var builder = services.AddIdentityServer(options =>
            //{
            //    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
            //    options.EmitStaticAudienceClaim = true;
            //})
            //    .AddInMemoryIdentityResources(Config.IdentityResources)
            //    .AddInMemoryApiScopes(Config.ApiScopes)
            //    .AddInMemoryClients(Config.Clients);

            // not recommended for production - you need to store your key material somewhere secure
            //  builder.AddDeveloperSigningCredential();

            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;




            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly))
            );

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();







            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.UserInteraction.LoginUrl = "/Account/Login";
                options.UserInteraction.LogoutUrl = "/Account/Logout";
                options.Authentication = new AuthenticationOptions()
                {
                    CookieLifetime = TimeSpan.FromHours(10), // ID server cookie timeout set to 10 hours
                    CookieSlidingExpiration = true
                };
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                options.EnableTokenCleanup = true;
            }) 
            .AddAspNetIdentity<ApplicationUser>();




            services.AddScoped<IProfileService, ProfileService>();

            services.AddDbContext<Data.ConfigurationDbContext>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));







        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapDefaultControllerRoute();
            //});


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("parameters",
                             "parameters/{level}/{type}/{id}",
                             defaults: new { controller = "Home", action = "Parameters" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });


 




        }
    }
}
