using System;
using System.IO;
using System.Text;
using GitServer.Security;
using LibGit2Sharp;
using Microsoft.AspNetCore.Identity;

namespace GitServer.Controllers
{
	public class GitCommandOptions
    {
		public bool AdvertiseRefs { get; set; }
		public bool EndStreamWithNull { get; set; }
		public string Service { get; set; }
		public Repository Repository { get; set; }
		public UserManager<GitServerUser> UserManager { get; set; }
		public RoleManager<GitServerRole> RoleManager { get; set; }

		public GitCommandOptions(
			Repository repo,
			string service,
			bool advertiseRefs,
			UserManager<GitServerUser> userManager,
			RoleManager<GitServerRole> roleManager,
			bool endStreamWithNull = true
		)
		{
			Repository = repo;
			Service = service;
			AdvertiseRefs = advertiseRefs;
			UserManager = userManager;
			RoleManager = roleManager;
			EndStreamWithNull = endStreamWithNull;
		}

		public override string ToString()
		{
			if (!Service.StartsWith("git-"))
				throw new InvalidOperationException();

			StringBuilder builder = new StringBuilder();

			builder.Append(Service.Substring(4));
			builder.Append(" --stateless-rpc");

			if (AdvertiseRefs)
				builder.Append(" --advertise-refs");

			builder.Append($@" ""{Repository.Info.Path.TrimEnd(Path.DirectorySeparatorChar)}""");

			return builder.ToString();
		}
	}
}
