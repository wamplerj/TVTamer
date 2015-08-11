namespace TvTamer.Core.FileSystem
{
    public interface IFile
    {
        string DirectoryName { get; }
        string Extension { get; }

        void Copy(string destinationFilename);
        void Delete();
        string ToString();
    }
}