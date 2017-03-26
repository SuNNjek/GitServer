using System;
using GitServer.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GitServer.Data
{
	public class ApplicationDbContext : IdentityDbContext<GitServerUser, GitServerRole, Guid>
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
