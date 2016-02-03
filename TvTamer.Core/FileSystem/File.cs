using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

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

    public class File : IFile
    {
        private readonly string _filePath;
        private readonly FileInfo _fileInfo;
        private readonly Logger _logger = LogManager.GetLogger("log");

        public File(string filePath)
        {
            _filePath = filePath;
            _fileInfo = new FileInfo(_filePath);
        }

        public override string ToString()
        {
            return _fileInfo.FullName;
        }

        public string DirectoryName => _fileInfo.DirectoryName;
        public string Extension => _fileInfo.Extension;

        public void Delete()
        {
            try
            {
                _fileInfo.Delete();
            }
            catch (UnauthorizedAccessException auth)
            {
                _logger.ErrorException($"Access Denied.  Could not delete file at {_filePath}", auth);
            }
            catch (FileNotFoundException file)
            {
                _logger.ErrorException($"Directory Not Found.  Could not delete file at {_filePath}", file);
            }
            catch (Exception ex)
            {
                _logger.ErrorException($"Could not delete file at {_filePath}", ex);
            }
            
        }

        public void Copy(string destinationFilename)
        {
            try
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(destinationFilename));
                _fileInfo.CopyTo(destinationFilename);
            }
            catch (UnauthorizedAccessException auth)
            {
                _logger.ErrorException($"Access Denied.  Could not copy file to {destinationFilename}", auth);
                throw;
            }
            catch (FileNotFoundException file)
            {
                _logger.ErrorException($"Directory Not Found.  Could not copy file to {destinationFilename}", file);
                throw;
            }
            catch (Exception ex)
            {
                _logger.ErrorException($"Could not copy file to {destinationFilename}", ex);
                throw;
            }
        }
    }

    public class TvEpisodeFile : File
    {
        public TvEpisodeFile(string filePath) : base(filePath) {}


        public int EpisodeNumber { get; set; }
        public int Season { get; set; }
        public string SeriesName { get; set; }
        public string FileName { get; set; }
    }

}
