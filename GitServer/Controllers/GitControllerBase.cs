using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitServer.Services;
using GitServer.Settings;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GitServer.Controllers
{
    public class GitControllerBase : Controller
    {
		private IOptions<GitSettings> _gitOptions;
		private IOptions<LogSettings> _logOptions;
		private GitRepositoryService _repoService;

		protected GitSettings GitSettings => _gitOptions.Value;
		protected LogSettings LogSettings => _logOptions.Value;
		protected GitRepositoryService RepositoryService => _repoService;

		protected GitControllerBase(IOptions<GitSettings> gitOptions, IOptions<LogSettings> logOptions, GitRepositoryService repoService)
		{
			_gitOptions = gitOptions;
			_logOptions = logOptions;
			_repoService = repoService;
		}

		protected PackageListResult PackageList(string contentType, string content)
			=> new PackageListResult(contentType, content);

		protected PackageListResult PackageList(string contentType, IEnumerable<string> lines)
			=> new PackageListResult(contentType, lines);

		protected GitCommandResult GitCommand(string repoName, string service, bool advertiseRefs, bool endStreamWithNull = true)
		{
			return new GitCommandResult(_gitOptions.Value.GitPath, new GitCommandOptions(
				RepositoryService.GetRepository(repoName),
				service,
				advertiseRefs,
				endStreamWithNull
			));
		}

		protected IActionResult TryGetResult(string repoName, Func<IActionResult> resFunc)
		{
			try
			{
				return resFunc();
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

		private IActionResult MakeError(string message, string repoName, int statusCode = 400)
		{
			return StatusCode(statusCode, new
			{
				error = new
				{
					message = message,
					repository = repoName
				}
			});
		}

		private IActionResult MakeError(Exception error, string repoName, int statusCode = 400)
		{
			string logPath = Path.Combine(Directory.GetCurrentDirectory(), _logOptions.Value.LogFile);
			Directory.CreateDirectory(Path.GetDirectoryName(logPath));

			using (StreamWriter logWriter = System.IO.File.AppendText(logPath))
			{
				logWriter.WriteLine(error);
			}

			return MakeError(error.Message, repoName, statusCode);
		}
	}
}
