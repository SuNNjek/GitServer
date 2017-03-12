using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using GitServer.Settings;
using System.IO;
using GitServer.Services;

namespace GitServer.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class GitController : Controller
    {
		private GitFileService _fileService;

		public GitController(GitFileService fileService)
		{
			_fileService = fileService;
		}

		[HttpGet("{repoName}/info/refs")]
		public IActionResult GetInfoRefs(string repoName)
		{
			return Json(_fileService.GetInfoRefs(repoName));
		}

		[HttpGet("{repoName}/files")]
		public IActionResult GetFiles(string repoName)
		{
			return Json(_fileService.GetFileTree(repoName));
		}
    }
}