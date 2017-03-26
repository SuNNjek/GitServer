using GitServer.Security;
using GitServer.Services;
using GitServer.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GitServer.Controllers
{
	[Authorize(ActiveAuthenticationSchemes = "Basic")]
	public class GitController : GitControllerBase
    {
		public GitController(
			GitRepositoryService repoService,
			IOptions<GitSettings> gitOptions,
			IOptions<LogSettings> logOptions,
			UserManager<GitServerUser> userManager,
			RoleManager<GitServerRole> roleManager
		)
			: base(gitOptions, logOptions, repoService, userManager, roleManager)
		{ }

		public IActionResult ExecuteUploadPack(string repoName) => TryGetResult(repoName, () => GitUploadPack(repoName));

		public IActionResult ExecuteReceivePack(string repoName) => TryGetResult(repoName, () => GitReceivePack(repoName));

		public IActionResult GetInfoRefs(string repoName, string service) => TryGetResult(repoName, () => GitCommand(repoName, service, true));
    }
}