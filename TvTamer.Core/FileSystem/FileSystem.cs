using System.Threading;

namespace TvTamer.Core.FileSystem
{
    public interface IFileSystem
    {
        IFile GetFile(string filePath);
        IDirectory GetDirectory(string folderPath);
    }

    public class FileSystem : IFileSystem
    {

        public IFile GetFile(string filePath)
        {
            return new File(filePath);
        }

        public bool PathExists(string path)
        {
            var isFile = System.IO.File.Exists(path);
            return isFile || System.IO.Directory.Exists(path);
        }

        public IDirectory GetDirectory(string folderPath)
        {
            return new Directory(folderPath);
        }

    }
}
