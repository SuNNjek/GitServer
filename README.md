# GitServer
A git smart http server implementation made with ASP.NET Core.

You can do all sorts of git related stuff with it right now, but the web backend still needs some work. Feel free to help out with pull requests.

## Settings
To set up the server, you need to change the appsettings.json. It is structured as follows:
```json
...
"GitSettings": {
	"BasePath": "{Path which contains (or will contain) your repositories}",
	"GitPath": "{Path to your git executable (Or just 'git' if it's in your PATH variable)}"
},
"LogSettings": {
	"LogFile": "{Path to your log directory (must already exist)}"
}
```

Also, the stylesheets in the /Styles folder need to be compiled with SASS to {name_without_extension}.min.css
