# GitServer
A git smart http server implementation made with ASP.NET Core.

You can do all sorts of git related stuff with it right now, but the web backend still needs some work. Feel free to help out with pull requests.

## Settings
To set up the server, you need to change the appsettings.json. It is structured as follows:
```json
...
"ConnectionStrings": {
	"DefaultConnection": "{The connection string for the PostgreSQL database used for authentication with the server}"
},
"GitSettings": {
	"BasePath": "{Path which contains (or will contain) your repositories}",
	"GitPath": "{Path to your git executable (Or just 'git' if it's in your PATH variable)}"
},
"LogSettings": {
	"LogFile": "{Path to your log directory (must already exist)}"
},
"EmailSettings": {
	"ServerUri": "{Uri to your SMTP mailserver}",
	"ServerPort": "{SMTP server port}",
	"UseSSL": "{bool indicating whether or not to use SSL to connect to the mailserver}",
	"RequiresAuthentication": "{bool indicating whether or not the mailserver requires authentication}",
	"User": "{The username used to authenticate with the mailserver, can be ignored if RequiresAuthentication is false}",
	"Password": "{The password used to authenticate with the mailserver, can be ignored if RequiresAuthentication is false}"
}

```

## Static files
If you publish the project via Visual Studio or dotnet, you need to copy some files which will be served statically by the server like stylesheets and images.
So after you published the project to your desired output folder, you need to copy the /Styles and /Images folder into that folder.

Also, if you make changes to the stylesheets in the /Styles folder, you need to re-compile them with [SASS](https://sass-lang.com/) to /Styles/{name_without_extension}.min.css (Preferably minified, as the name suggests).

You can install the [Visual Studio Web Compiler Extension](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebCompiler) to do this for you automatically.