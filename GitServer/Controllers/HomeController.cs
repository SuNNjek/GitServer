using GitServer.Services;
using GitServer.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GitServer.Controllers
{
    public class HomeController : GitControllerBase
    {
        public HomeController(IOptions<GitSettings> gitOptions, IOptions<LogSettings> logOptions, GitRepositoryService repoService)
			: base(gitOptions, logOptions, repoService)
        { }

		public IActionResult Home()
		{
			return View();
		}
    }
}