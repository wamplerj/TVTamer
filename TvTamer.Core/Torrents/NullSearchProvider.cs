using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvTamer.Core.Torrents
{

    public interface ISearchProvider
    {
        Torrent GetTorrent(string search);
        ISearchProvider NextSearchProvider { get; }
    }

    public class NullSearchProvider : ISearchProvider
    {

        public Torrent GetTorrent(string search)
        {
            return null;
        }

        public ISearchProvider NextSearchProvider { get; }
    }
}
