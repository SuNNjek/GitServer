using System;
using System.Collections.Generic;
using System.IO;
using GitServer.Models;
using GitServer.Security;
using GitServer.Services;
using GitServer.Settings;
using LibGit2Sharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GitServer.Controllers
{
	public class GitControllerBase : Controller
    {
		private IOptions<GitSettings> _gitOptions;
		private IOptions<LogSettings> _logOptions;
		private GitRepositoryService _repoService;
		private UserManager<GitServerUser> _userManager;
		private RoleManager<GitServerRole> _roleManager;

		protected GitSettings GitSettings => _gitOptions.Value;
		protected LogSettings LogSettings => _logOptions.Value;
		protected GitRepositoryService RepositoryService => _repoService;
		protected UserManager<GitServerUser> UserManager => _userManager;
		protected RoleManager<GitServerRole> RoleManager => _roleManager;

		protected GitControllerBase(
			IOptions<GitSettings> gitOptions,
			IOptions<LogSettings> logOptions,
			GitRepositoryService repoService,
			UserManager<GitServerUser> userManager,
			RoleManager<GitServerRole> roleManager
		)
		{
			_gitOptions = gitOptions;
			_logOptions = logOptions;
			_repoService = repoService;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		protected GitCommandResult GitCommand(string repoName, string service, bool advertiseRefs, bool endStreamWithNull = true)
		{
			return new GitCommandResult(_gitOptions.Value.GitPath, new GitCommandOptions(
				RepositoryService.GetRepository(repoName),
				service,
				advertiseRefs,
				UserManager,
				RoleManager,
				endStreamWithNull
			));
		}

		protected GitCommandResult GitUploadPack(string repoName) => GitCommand(repoName, "git-upload-pack", false, false);
		protected GitCommandResult GitReceivePack(string repoName) => GitCommand(repoName, "git-receive-pack", false);

		protected IActionResult TryGetResult(string repoName, Func<IActionResult> resFunc)
		{
			try
			{
				return resFunc();
			}
			catch(RepositoryNotFoundException)
			{
				return MakeError("Repository not found", repoName, 404);
			}
			catch(NotFoundException)
			{
				return MakeError("The requested file could not be found", repoName, 404);
			}
			catch(Exception e)
			{
				return MakeError(e, repoName);
			}
		}

		private IActionResult MakeError(string message, string repoName, int statusCode = 500)
		{
			ErrorModel model = new ErrorModel
			{
				StatusCode = statusCode,
				Message = message,
				Description = $"An error occured while accessing repository \"{repoName}\": {message}"
			};

			ViewResult viewResult = View("_Error", model);
			viewResult.StatusCode = statusCode;
			return viewResult;
		}

		private IActionResult MakeError(Exception error, string repoName, int statusCode = 500)
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
