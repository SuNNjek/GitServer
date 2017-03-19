using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using GitServer.Settings;
using System.IO;
using GitServer.Extensions;
using GitServer.Services;
using LibGit2Sharp;
using GitServer.Attributes;
using GitServer.Models;

namespace GitServer.Controllers
{
    public class GitController : GitControllerBase
    {
		private GitFileService _fileService;

		public GitController(
			GitFileService fileService
			, GitRepositoryService repoService
			, IOptions<GitSettings> gitOptions
			, IOptions<LogSettings> logOptions
		) : base(gitOptions, logOptions, repoService)
		{
			_fileService = fileService;
		}

		[HttpGet()]
		public IActionResult GetRepositories() => Json(RepositoryService.Repositories.Select(d => new DirectoryInfo(d.Info.Path).Name));

		public IActionResult ExecuteUploadPack(string repoName)
			=> TryGetResult(repoName, () => GitCommand(repoName, "git-upload-pack", false));

		public IActionResult ExecuteReceivePack(string repoName)
			=> TryGetResult(repoName, () => GitCommand(repoName, "git-receive-pack", false, false));

		public IActionResult GetInfoRefs(string repoName, string service)
			=> TryGetResult(repoName, () => GitCommand(repoName, service, true));

		public IActionResult GetFileView(string repoName, string filePath)
			=> TryGetResult(repoName, () =>
				{
					FileTreeEntry model;
					if (filePath != null)
					{
						TreeEntry entry = _fileService.GetFileTreeEntry(repoName, filePath);
						model = new FileTreeEntry(repoName, entry.Path, entry.Target);
					}
					else
					{
						model = new FileTreeEntry(repoName, Path.DirectorySeparatorChar.ToString(), _fileService.GetFileTree(repoName, filePath));
					}

					return View("Files", model);
				}
		);
    }
}