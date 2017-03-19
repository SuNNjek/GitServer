using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitServer.Settings;
using Microsoft.Extensions.Options;

namespace GitServer.Services
{
	public class GitCommandService : GitServiceBase
	{
		public GitCommandService(IOptions<GitSettings> settings) : base(settings)
		{ }


	}
}
