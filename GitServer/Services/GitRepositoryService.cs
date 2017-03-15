using System;
using System.Collections.Generic;
using System.IO;
using GitServer.Settings;
using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace GitServer.Services
{
	public class GitRepositoryService : GitServiceBase
	{
		public string ServerCapabilities => Settings.ServerCapabilities;

		public IEnumerable<Repository> Repositories
		{
			get
			{
				DirectoryInfo basePath = new DirectoryInfo(Settings.BasePath);
				foreach(DirectoryInfo path in basePath.EnumerateDirectories())
				{
					string repPath = Repository.Discover(path.FullName);
					if (repPath != null)
						yield return new Repository(repPath);
				}
			}
		}

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

		public ReferenceCollection GetReferences(string repoName) => GetRepository(repoName).Refs;

		public void Test(string repoName)
		{
			Repository repo = GetRepository(repoName);

			foreach(TreeEntry entry in repo.Head.Tip.Tree)
			{
				
			}
		}
	}
}
