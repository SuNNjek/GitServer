using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitServer.Data;
using GitServer.Helpers;
using GitServer.Services;
using GitServer.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;
using GitServer.Security;
using GitServer.Security.BasicAuthentication;

namespace GitServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
			services.AddOptions();

			// Add settings
			services.Configure<GitSettings>(Configuration.GetSection(nameof(GitSettings)));
			services.Configure<LogSettings>(Configuration.GetSection(nameof(LogSettings)));
			services.Configure<EmailSettings>(Configuration.GetSection(nameof(EmailSettings)));

			// Add database context
			services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

			// Add identity
			services.AddIdentity<GitServerUser, GitServerRole>(d =>
				{
					// Cookie settings
					d.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(30);
					d.Cookies.ApplicationCookie.LoginPath = "/users/login";
					d.Cookies.ApplicationCookie.LogoutPath = "/users/logout";

					// Lockout settings
					d.Lockout.MaxFailedAccessAttempts = 10;
					d.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);

					// Password settings
					d.Password.RequiredLength = 8;

					// Sign-in settings
					d.SignIn.RequireConfirmedEmail = true;

					// User settings
					d.User.RequireUniqueEmail = true;
				})
				.AddEntityFrameworkStores<ApplicationDbContext, Guid>()
				.AddDefaultTokenProviders();

			// Add git services
			services.AddTransient<GitRepositoryService>();
			services.AddTransient<GitFileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/error");
			}

			app.UseBasicAuthentication("GitServer");
			app.UseIdentity();
            app.UseMvc(d => RouteConfig.RegisterRoutes(d));

			//Create roles
			app.SeedRoles().Wait();

			//Use static css stylesheets
			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new MinCssFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Styles")),
				RequestPath = new PathString("/css")
			});

			//Use static css stylesheets
			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
				RequestPath = new PathString("/img")
			});
		}
	}
}
