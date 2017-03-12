using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitServer.Settings;
using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace GitServer.Services
{
	public class GitRepositoryService : GitServiceBase
	{
		public GitRepositoryService(IOptions<GitSettings> settings) : base(settings)
		{
		}

		public Repository CreateRepository(string name)
			=> new Repository(Repository.Init(Path.Combine(Settings.BasePath, name), true));

		public void DeleteRepository(string name)
		{
			Exception e = null;
			for(int i = 0; i < 3; i++)
			{
				try
				{
					Directory.Delete(Path.Combine(Settings.BasePath, name), true);
				}
				catch(Exception ex) { e = ex; }
			}

			if (e != null)
				throw new GitException("Failed to delete repository", e);
		}
	}
}
