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

namespace GitServer.Controllers
{
    [Route("git")]
    public class GitController : GitControllerBase
    {
		private GitFileService _fileService;
		private GitRepositoryService _repoService;

		public GitController(
			GitFileService fileService
			, GitRepositoryService repoService
			, IOptions<LogSettings> logOptions
		) : base(logOptions)
		{
			_fileService = fileService;
			_repoService = repoService;
		}

		[HttpGet()]
		public IActionResult GetRepositories()
			=> Json(_repoService.Repositories.Select(d => new DirectoryInfo(d.Info.Path).Name));

		/// <summary>
		/// Action for reference discovery
		/// </summary>
		/// <param name="repoName">The name of the repository</param>
		/// <param name="service">The name of the service</param>
		/// <returns></returns>
		[HttpGet("{repoName}/info/refs")]
		[ResponseHeader("Cache-Control", "no-cache")]
		[Produces("text/plain")]
		public IActionResult GetInfoRefs(string repoName, string service)
		{
			try
			{
				switch (service)
				{
					case "git-receive-pack":
					case "git-upload-pack":
						{
							List<string> lines = new List<string>();
							lines.Add($"# service={service}");

							int lineNumber = 0;
							foreach (Reference reference in _repoService.GetReferences(repoName))
							{
								string line = $"{reference.TargetIdentifier} {reference.CanonicalName}";

								if (lineNumber++ == 0)
									line += $"\0{_repoService.ServerCapabilities}";

								lines.Add(line);
							}

							return PackageList($"application/x-{service}-advertisement", lines);
						}

					default:
						Response.ContentType = "text/plain";
						return StatusCode(403, "Requested service is not supported");
				}
			}
			catch (RepositoryNotFoundException)
			{
				return MakeError("Repository not found", repoName);
			}
			catch (Exception e)
			{
				return MakeError(e, repoName);
			}
		}

		[HttpPost("{repoName}/info/refs")]
		[ResponseHeader("Cache-Control", "no-cache")]
		[GitBasicAuthentication()]
		public IActionResult PostInfoRefs(string repoName, string service)
		{
			try
			{
				Response.ContentType = $"application/x-{service}-request";

				switch (service)
				{
					default:
						Response.ContentType = "text/plain";
						return StatusCode(403, "Requested service is not supported");
				}
			}
			catch(RepositoryNotFoundException)
			{
				return MakeError("Repository not found", repoName);
			}
			catch(Exception e)
			{
				return MakeError(e, repoName);
			}
		}

		[HttpGet("{repoName}/files")]
		[Produces("application/json")]
		public IActionResult GetFiles(string repoName)
		{
			try
			{
				return Json(
					_fileService.GetFiles(repoName)
				);
			}
			catch(RepositoryNotFoundException)
			{
				return MakeError("Repository not found", repoName);
			}
			catch(Exception e)
			{
				return MakeError(e, repoName);
			}
		}

		[HttpGet("{repoName}/files/{*filePath}")]
		[Produces("application/json")]
		public IActionResult GetFile(string repoName, string filePath)
		{
			try
			{
				return Content(
					_fileService
						.GetFileContents(repoName, filePath.Replace('/', Path.DirectorySeparatorChar))
					);
			}
			catch (RepositoryNotFoundException)
			{
				return MakeError("Repository not found", repoName);
			}
			catch (Exception e)
			{
				return MakeError(e, repoName);
			}
		}
    }
}