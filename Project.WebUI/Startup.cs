using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.AppCode.Providers;
using Project.WebUI.AppCode.Extensions;
using Project.WebUI.AppCode.Providers;
using Project.WebUI.AppCode.Services;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities.Membership;
using System;
using System.Linq;

namespace Project.WebUI
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRouting(cfg =>
            {
                cfg.LowercaseUrls = true;
            });
            services.AddDbContext<ProjectDbContext>(cfg =>
            {
                cfg.UseSqlServer(configuration.GetConnectionString("projectCString"));
            });
            services.AddIdentity<ProjectUser, ProjectRole>()
                .AddEntityFrameworkStores<ProjectDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<ProjectIdentityErrorDescriber>();

            services.AddScoped<UserManager<ProjectUser>>();
            services.AddScoped<SignInManager<ProjectUser>>();
            services.AddScoped<RoleManager<ProjectRole>>();
            services.Configure<AntiforgeryOptions>(cfg =>
            {
                cfg.Cookie.Name = "project-ant";
            });
            services.Configure<CryptoServiceOptions>(cfg =>
            {
                configuration.GetSection("cryptography").Bind(cfg);
            });
            services.AddSingleton<ICryptoService, CryptoService>();
            services.Configure<EmailServiceOptions>(cfg =>
            {
                configuration.GetSection("emailAccount").Bind(cfg);
            });
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IClaimsTransformation, AppClaimProvider>();
            var assemblies = AppDomain.CurrentDomain
               .GetAssemblies()
               .Where(a => a.FullName.StartsWith("Project."))
               .ToArray();
            services.AddMediatR(assemblies);

            services.AddAuthentication(cfg =>
            {
                cfg.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                cfg.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                cfg.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                cfg.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
               .AddCookie(cfg =>
               {
                   cfg.Cookie.Name = "project";
                   cfg.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                   cfg.LoginPath = "/signin.html";
                   cfg.AccessDeniedPath = "/accessdenied.html";
               });

            services.AddAuthorization(cfg =>
            {

                foreach (string principal in AppClaimProvider.principals)
                {
                    cfg.AddPolicy(principal, p =>
                    {
                        p.RequireAssertion(handler =>
                        {
                            return handler.User.HasAccess(principal);

                        });
                    });
                }
            });

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.SeedMembership();
            app.UseStaticFiles();   

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(name: "defaultAdmin",
                  areaName: "Admin",
                  pattern: "admin/{controller=users}/{action=index}/{id?}");
                endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
