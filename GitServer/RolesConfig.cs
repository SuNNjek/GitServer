using System;
using System.Linq;
using System.Threading.Tasks;
using GitServer.Data;
using GitServer.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GitServer
{
	public static class RolesConfig
    {
		private static readonly string[] _roles = new string[] { "admin", "user" };
		
		/// <summary>
		/// Creates the admin and user roles if they don't exist yet
		/// </summary>
		/// <param name="builder">The application builder</param>
		public static async Task SeedRoles(this IApplicationBuilder builder)
		{
			IServiceProvider serviceProvider = builder.ApplicationServices;
			using (IServiceScope scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				ApplicationDbContext dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

				if (dbContext.Database.GetPendingMigrations().Any())
				{
					//Wait for any migrations to finish
					await dbContext.Database.MigrateAsync();
				}

				RoleManager<GitServerRole> roleManager = serviceProvider.GetService<RoleManager<GitServerRole>>();
				foreach (string role in _roles)
				{
					//Create the role if it doesn't exist yet
					if (!await roleManager.RoleExistsAsync(role))
						await roleManager.CreateAsync(new GitServerRole { Name = role, CreatedAt = DateTime.Now });
				}
			}
		}
	}
}
