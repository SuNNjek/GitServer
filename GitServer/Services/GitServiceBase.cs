using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitServer.Settings;
using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace GitServer.Services
{
    public abstract class GitServiceBase
    {
		private IOptions<GitSettings> _settings;
		protected GitSettings Settings => _settings.Value;

		protected GitServiceBase(IOptions<GitSettings> settings)
		{
			_settings = settings;
		}

		protected Repository GetRepository(string name)
			=> new Repository(Path.Combine(Settings.BasePath, name));
    }
}
