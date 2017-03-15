using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GitServer.Attributes
{
	/// <summary>
	/// Identifies an action that requires authentication via the <see cref="GitAuthenticationService"/>.
	/// </summary>
	public class GitBasicAuthenticationAttribute : ActionFilterAttribute
    {
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			HttpRequest request = context.HttpContext.Request;
			if (!request.Headers.ContainsKey("Authorization"))
			{
				//No authorization header present
				Fail(context);
				return;
			}

			string authHeader = request.Headers["Authorization"];
			if(!authHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
			{
				//Wrong authorization scheme
				Fail(context);
				return;
			}

			string token = authHeader.Substring("Basic ".Length).Trim();
			string credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
			string[] credentials = credentialString.Split(':');
			if(!GitAuthenticationService.CheckCredentials(credentials[0], credentials[1]))
			{
				//Wrong username and password
				Fail(context);
				return;
			}

			base.OnActionExecuting(context);
		}
		
		private void Fail(ActionExecutingContext context)
		{
			context.HttpContext.Response.StatusCode = 401;
			context.HttpContext.Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"{context.HttpContext.Request.Host}\"");
		}
	}
}
