using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace GitServer.Security.BasicAuthentication
{
	public class BasicAuthenticationOptions : AuthenticationOptions, IOptions<BasicAuthenticationOptions>
	{
		private string _realm;

		public IServiceProvider ServiceProvider { get; set; }
		public BasicAuthenticationOptions Value => this;

		protected UserManager<GitServerUser> UserManager => ServiceProvider.GetRequiredService<UserManager<GitServerUser>>();
		protected RoleManager<GitServerRole> RoleManager => ServiceProvider.GetRequiredService<RoleManager<GitServerRole>>();

		public string Realm
		{
			get { return _realm; }
			set
			{
				if (value.Any(c => c < 32 || c >= 127))
					throw new ArgumentException("Realm must be ASCII string", nameof(value));

				_realm = value;
			}
		}

		public BasicAuthenticationOptions()
		{
			AuthenticationScheme = "Basic";
			AutomaticAuthenticate = true;
			AutomaticChallenge = true;
		}

		public async Task<ClaimsPrincipal> SignInAsync(string userName, string password)
		{
			using (IServiceScope scope = ServiceProvider.CreateScope())
			{
				GitServerUser user = await UserManager.FindByNameAsync(userName);
				//No user with the specified name found
				if (user == null)
					return null;

				//Wrong password supplied
				if(!(await UserManager.CheckPasswordAsync(user, password)))
					return null;

				//Create claims pricipal for user
				IOptions<IdentityOptions> identityOptions = ServiceProvider.GetRequiredService<IOptions<IdentityOptions>>();
				UserClaimsPrincipalFactory<GitServerUser, GitServerRole> principalFactory
					= new UserClaimsPrincipalFactory<GitServerUser, GitServerRole>(UserManager, RoleManager, identityOptions);

				return await principalFactory.CreateAsync(user);
			}
		}
	}
}
