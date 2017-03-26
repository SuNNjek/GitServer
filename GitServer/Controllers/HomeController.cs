using System.Linq;
using GitServer.Security;
using GitServer.Services;
using GitServer.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GitServer.Controllers
{
    public class HomeController : GitControllerBase
    {
        public HomeController(
			IOptions<GitSettings> gitOptions,
			IOptions<LogSettings> logOptions,
			GitRepositoryService repoService,
			UserManager<GitServerUser> userManager,
			RoleManager<GitServerRole> roleManager
		)
			: base(gitOptions, logOptions, repoService, userManager, roleManager)
        { }

		public IActionResult Home()
		{
			return View(RepositoryService.RepositoryDirectories.Select(d => d.Name));
		}
    }
}