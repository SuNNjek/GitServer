using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GitServer.Security
{
	public class GitServerUser : IdentityUser<Guid>
    {
		public DateTime CreatedAt { get; set; }
    }
}
