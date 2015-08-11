using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLog;
using NLog.Common;

namespace TvTamer.Core.FileSystem
{
    public class Directory : IDirectory
    {
        private readonly string _folder;
        private readonly Logger _logger = LogManager.GetLogger("log");

        public Directory(string folder)
        {
            _folder = folder;
        }

        public override string ToString()
        {
            return Path;
        }

        public string Path => System.IO.Path.GetDirectoryName(_folder);

        public void Delete(bool recursive = false)
        {
            try
            { 
                System.IO.Directory.Delete(_folder, recursive);
            }   
            catch (UnauthorizedAccessException auth)
            {
                _logger.ErrorException($"Access Denied.  Could not delete directory at {_folder}", auth);
            }
            catch (DirectoryNotFoundException dir)
            {
                _logger.ErrorException($"Directory Not Found.  Could not delete directory at {_folder}", dir);
            }
            catch (Exception ex)
            {
                _logger.ErrorException($"Could not delete directory at {_folder}", ex);
            }
        }


        public bool Exists()
        {
            return System.IO.Directory.Exists(_folder);
        }

        public IEnumerable<string> EnumerateFiles(string searchPattern, bool recursive = false)
        {
            var searchOptions = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return System.IO.Directory.EnumerateFiles(_folder, searchPattern, searchOptions);
        }

    }
}
