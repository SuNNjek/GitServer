using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace GitServer.Helpers
{
	public class MinCssFileProvider : IFileProvider, IDisposable
	{
		private PhysicalFileProvider _fileProvider;

		public MinCssFileProvider(string root)
		{
			_fileProvider = new PhysicalFileProvider(root);
		}

		public void Dispose() => _fileProvider.Dispose();

		public IDirectoryContents GetDirectoryContents(string subpath)
			=> new MinCssDirContents(_fileProvider.GetDirectoryContents(subpath));

		public IFileInfo GetFileInfo(string subpath)
		{
			IFileInfo info = _fileProvider.GetFileInfo(subpath);

			if (!info.Name.EndsWith(".min.css"))
				return new NotFoundFileInfo(info.Name);

			return info;
		}

		public IChangeToken Watch(string filter) => _fileProvider.Watch($"*.min.css AND {filter}");

		private class MinCssDirContents : IDirectoryContents
		{
			private IDirectoryContents _contents;

			public MinCssDirContents(IDirectoryContents contents)
			{
				_contents = contents;
			}

			public bool Exists => _contents.Exists;

			public IEnumerator<IFileInfo> GetEnumerator() => _contents.Where(d => d.Name.EndsWith(".min.css")).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
