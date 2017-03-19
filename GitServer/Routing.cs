using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;

namespace GitServer
{
	public static class Routing
    {
		public static void RegisterRoutes(IRouteBuilder routeBuilder)
		{
			routeBuilder.MapRoute(
				"GetFileView",
				"git/{repoName}/files/{*filePath}",
				new { controller = "Git", action = "GetFileView" },
				new { method = new HttpMethodRouteConstraint("GET") }
			);

			routeBuilder.MapRoute(
				"GetInfoRefs",
				"git/{repoName}.git/info/refs",
				new { controller = "Git", action = "GetInfoRefs" },
				new { method = new HttpMethodRouteConstraint("GET") }
			);

			routeBuilder.MapRoute(
				"ExecuteUploadPack",
				"git/{repoName}.git/git-upload-pack",
				new { controller = "Git", action = "ExecuteUploadPack" },
				new { method = new HttpMethodRouteConstraint("POST") }
			);

			routeBuilder.MapRoute(
				"ExecuteReceivePack",
				"git/{repoName}.git/git-receive-pack",
				new { controller = "Git", action = "ExecuteReceivePack" },
				new { method = new HttpMethodRouteConstraint("POST") }
			);
		}
    }
}
