using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GitServer.Security
{
	public class GitServerRole : IdentityRole<Guid>
    {
		public DateTime CreatedAt { get; set; }
		public string Description { get; set; }
    }
}
