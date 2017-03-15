using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GitServer.Controllers
{
	public class PackageListResult : ActionResult
	{
		private string _contentType;
		private List<string> _lines;

		public PackageListResult(string contentType, IEnumerable<string> lines)
		{
			_contentType = contentType;
			_lines = new List<string>(lines);
		}

		public PackageListResult(string contentType, string content)
			: this(contentType, content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
		{ }

		public override void ExecuteResult(ActionContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			HttpResponse resp = context.HttpContext.Response;

			resp.Headers.Add("Expires", "Fri, 01 Jan 1980 00:00:00 GMT");
			resp.Headers.Add("Pragma", "no-cache");
			resp.Headers.Add("Cache-Control", "no-cache, max-age=0, must-revalidate");

			resp.ContentType = _contentType;

			using (StreamWriter writer = new StreamWriter(resp.Body))
			{
				foreach(string line in _lines)
				{
					writer.WriteLine($"{line.Length:x4}{line}");
				}
				writer.WriteLine("0000");
			}
		}
	}
}
