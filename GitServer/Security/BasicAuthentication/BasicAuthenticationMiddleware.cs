using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GitServer.Security.BasicAuthentication
{
	public class BasicAuthenticationMiddleware : AuthenticationMiddleware<BasicAuthenticationOptions>
	{
		public BasicAuthenticationMiddleware(RequestDelegate next, BasicAuthenticationOptions options, ILoggerFactory loggerFactory, UrlEncoder encoder)
			: base(next, options, loggerFactory, encoder)
		{ }

		protected override AuthenticationHandler<BasicAuthenticationOptions> CreateHandler() => new BasicAuthenticationHandler();
	}
}
