namespace TvTamer.Core.FileSystem
{
    public interface IFileSystemFactory
    {
        IFile GetFile(string filePath);
        IDirectory GetDirectory(string folderPath);
    }

    public class FileSystemFactory : IFileSystemFactory
    {

        public IFile GetFile(string filePath)
        {
            return new File(filePath);
        }

        public IDirectory GetDirectory(string folderPath)
        {
            return new Directory(folderPath);
        }

    }
}
