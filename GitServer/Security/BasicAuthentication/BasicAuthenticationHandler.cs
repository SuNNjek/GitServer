using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Net.Http.Headers;

namespace GitServer.Security.BasicAuthentication
{
	public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
	{
		protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!Request.Headers.ContainsKey("Authorization"))
				return AuthenticateResult.Skip();

			string authHeader = Request.Headers["Authorization"];
			if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
				return AuthenticateResult.Skip();

			string token = authHeader.Substring("Basic ".Length).Trim();
			string credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
			string[] credentials = credentialString.Split(':');

			if (credentials.Length != 2)
				return AuthenticateResult.Fail("More than two strings seperated by colons found");

			ClaimsPrincipal principal = await Options.SignInAsync(credentials[0], credentials[1]);

			if(principal != null)
			{
				AuthenticationTicket ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Options.AuthenticationScheme);
				return AuthenticateResult.Success(ticket);
			}

			return AuthenticateResult.Fail("Wrong credentials supplied");
		}

		protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
		{
			Response.StatusCode = 401;

			string headerValue = $"{Options.AuthenticationScheme} realm=\"{Options.Realm}\"";
			Response.Headers.Append(HeaderNames.WWWAuthenticate, headerValue);

			return Task.FromResult(true);
		}

		protected override Task<bool> HandleForbiddenAsync(ChallengeContext context)
		{
			Response.StatusCode = 403;
			return Task.FromResult(true);
		}

		protected override Task HandleSignInAsync(SignInContext context)
		{
			throw new NotSupportedException();
		}

		protected override Task HandleSignOutAsync(SignOutContext context)
		{
			throw new NotSupportedException();
		}
	}
}
