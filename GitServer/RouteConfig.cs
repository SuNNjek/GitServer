using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;

namespace GitServer
{
	public static class RouteConfig
    {
		public static void RegisterRoutes(IRouteBuilder routeBuilder)
		{
			routeBuilder.MapRoute(
				"Home",
				"",
				new { controller = "Home", action = "Home" },
				new { method = new HttpMethodRouteConstraint("GET") }
			);

			#region Routes for viewing the file tree
			routeBuilder.MapRoute(
				"RedirectGitLink",
				"git/{repoName}.git",
				new { controller = "FileView", action = "RedirectGitLink" },
				new { method = new HttpMethodRouteConstraint("GET") }
			);

			routeBuilder.MapRoute(
				"GetRepositoryHomeView",
				"git/{repoName}",
				new { controller = "FileView", action = "GetTreeView", id = "master", path = string.Empty },
				new { method = new HttpMethodRouteConstraint("GET") }
			);

			routeBuilder.MapRoute(
				"GetTreeView",
				"git/{repoName}/tree/{id}/{*path}",
				new { controller = "FileView", action = "GetTreeView" },
				new { method = new HttpMethodRouteConstraint("GET") }
			);

			routeBuilder.MapRoute(
				"GetBlobView",
				"git/{repoName}/blob/{id}/{*path}",
				new { controller = "FileView", action = "GetBlobView" },
				new { method = new HttpMethodRouteConstraint("GET") }
			);

			routeBuilder.MapRoute(
				"GetRawBlob",
				"git/{repoName}/raw/{id}/{*path}",
				new { controller = "FileView", action = "GetRawBlob" },
				new { method = new HttpMethodRouteConstraint("GET") }
			);
			#endregion

			#region Routes for the actual git server
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
			#endregion

			#region Routes for account management
			routeBuilder.MapRoute(
				"Logout",
				"users/logout",
				new { controller = "Account", action = "Logout" },
				new { method = new HttpMethodRouteConstraint("GET") }
			);

			routeBuilder.MapRoute(
				"Login",
				"users/login",
				new { controller = "Account", action = "Login" },
				new { method = new HttpMethodRouteConstraint("GET", "POST") }
			);

			routeBuilder.MapRoute(
				"Register",
				"users/register",
				new { controller = "Account", action = "Register" },
				new { method = new HttpMethodRouteConstraint("GET", "POST") }
			);

			routeBuilder.MapRoute(
				"ConfirmEmail",
				"users/confirmMail",
				new { controller = "Account", action = "ConfirmEmail" },
				new { method = new HttpMethodRouteConstraint("GET") }
			);
			#endregion
		}
    }
}
