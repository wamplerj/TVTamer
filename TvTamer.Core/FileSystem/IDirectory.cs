using System.Collections.Generic;

namespace TvTamer.Core.FileSystem
{
    public interface IDirectory
    {
        string Path { get; }

        void Delete(bool recursive = false);
        IEnumerable<string> EnumerateFiles(string searchPattern, bool recursive = false);
        bool Exists();
        string ToString();
    }
}