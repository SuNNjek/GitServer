using System.IO;
using GitServer.Settings;
using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace GitServer.Services
{
	public class GitFileService : GitServiceBase
	{
		public GitFileService(IOptions<GitSettings> settings) : base(settings)
		{
		}

		public Tree GetFileTree(string repoName)
		{
			Repository repo = GetRepository(repoName);
			return repo.Head.Tip.Tree;
		}

		public ReferenceCollection GetInfoRefs(string repoName)
		{
			return GetRepository(repoName).Refs;
		}
	}
}
