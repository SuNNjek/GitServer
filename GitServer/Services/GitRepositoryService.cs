﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitServer.Settings;
using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace GitServer.Services
{
	public class GitRepositoryService : GitServiceBase
	{
		private static List<string> _repos = null;

		public IEnumerable<Repository> Repositories => RepositoryDirectories.Select(d => new Repository(d.FullName));

		public IEnumerable<DirectoryInfo> RepositoryDirectories
		{
			get
			{
				if (_repos == null)
				{
					_repos = new List<string>();
					DirectoryInfo basePath = new DirectoryInfo(Settings.BasePath);
					foreach (DirectoryInfo path in basePath.EnumerateDirectories())
					{
						string repPath = Repository.Discover(path.FullName);
						if (repPath != null)
							_repos.Add(repPath);
					}
				}

				return _repos.Select(d => new DirectoryInfo(d));
			}
		}

		public GitRepositoryService(IOptions<GitSettings> settings) : base(settings)
		{
		}

		public Repository CreateRepository(string name)
		{
			string path = Path.Combine(Settings.BasePath, name);
			Repository repo = new Repository(Repository.Init(path, true));

			_repos.Add(path);
			return repo;
		}

		public void DeleteRepository(string name)
		{
			Exception e = null;
			for(int i = 0; i < 3; i++)
			{
				try
				{
					string path = Path.Combine(Settings.BasePath, name);
					Directory.Delete(path, true);

					_repos.Remove(path);
				}
				catch(Exception ex) { e = ex; }
			}

			if (e != null)
				throw new GitException("Failed to delete repository", e);
		}

		public ReferenceCollection GetReferences(string repoName) => GetRepository(repoName).Refs;
		
		public void Test()
		{
			Repository test = GetRepository("asdf");

			ObjectDatabase db = test.ObjectDatabase;
			
		}
	}
}
