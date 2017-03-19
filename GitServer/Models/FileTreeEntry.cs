using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LibGit2Sharp;

namespace GitServer.Models
{
	public class FileTreeEntry : IEnumerable<FileTreeEntry>
	{
		private string _repoName, _path;
		private GitObject _object;
		private FileTreeEntry _parent = null;

		public string Path => _path.Replace(System.IO.Path.DirectorySeparatorChar, '/');
		public string Name => System.IO.Path.GetFileName(_path);

		public string Extension
		{
			get
			{
				if (!IsFile)
					throw new InvalidOperationException();

				return System.IO.Path.GetExtension(_path);
			}
		}

		public string RepoName => _repoName;

		public bool IsFile => _object is Blob;
		public bool IsDirectory => _object is Tree;

		public bool IsBinary => IsFile && ((Blob)_object).IsBinary;

		public string ContentString
		{
			get
			{
				if (!IsFile)
					throw new InvalidOperationException();

				return ((Blob)_object).GetContentText();
			}
		}

		public Stream ContentStream
		{
			get
			{
				if (!IsFile)
					throw new InvalidOperationException();

				return ((Blob)_object).GetContentStream();
			}
		}

		public FileTreeEntry(string repoName, string path, GitObject obj)
		{
			_repoName = repoName;
			_path = path;
			_object = obj;
		}

		public IEnumerator<FileTreeEntry> GetEnumerator()
		{
			if (!IsDirectory)
				yield break;

			if (_parent != null)
				yield return _parent;

			foreach (TreeEntry entry in (Tree)_object)
			{
				FileTreeEntry res = new FileTreeEntry(_repoName, entry.Path, entry.Target);
				res._parent = this;
				yield return res;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}