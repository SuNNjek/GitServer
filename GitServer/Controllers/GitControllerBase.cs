using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitServer.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GitServer.Controllers
{
    public class GitControllerBase : Controller
    {
		private IOptions<LogSettings> _logOptions;

		protected GitControllerBase(IOptions<LogSettings> logOptions)
		{
			_logOptions = logOptions;
		}

		protected PackageListResult PackageList(string contentType, string content)
			=> new PackageListResult(contentType, content);

		protected PackageListResult PackageList(string contentType, IEnumerable<string> lines)
			=> new PackageListResult(contentType, lines);

		protected IActionResult MakeError(string message, string repoName)
		{
			return BadRequest(new
			{
				error = new
				{
					message = message,
					repository = repoName
				}
			});
		}

		protected IActionResult MakeError(Exception error, string repoName)
		{
			string logPath = Path.Combine(Directory.GetCurrentDirectory(), _logOptions.Value.LogFile);
			Directory.CreateDirectory(Path.GetDirectoryName(logPath));

			using (StreamWriter logWriter = System.IO.File.AppendText(logPath))
			{
				logWriter.WriteLine(error);
			}

			return MakeError(error.Message, repoName);
		}
	}
}
