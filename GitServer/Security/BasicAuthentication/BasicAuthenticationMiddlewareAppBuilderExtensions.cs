using Microsoft.AspNetCore.Builder;

namespace GitServer.Security.BasicAuthentication
{
	public static class BasicAuthenticationMiddlewareAppBuilderExtensions
	{
		public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder builder, string realm)
		{			
			return builder.UseMiddleware<BasicAuthenticationMiddleware>(new BasicAuthenticationOptions()
			{
				Realm = realm,
				ServiceProvider = builder.ApplicationServices
			});
		}
	}
}
