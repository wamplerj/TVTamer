using System;

namespace TvTamer.Core.Torrents
{
    public class ThePirateBaySearchProvider : ISearchProvider
    {

        public ThePirateBaySearchProvider(ISearchProvider nextProvider)
        {
            if (nextProvider == null)
                throw new ArgumentNullException(nameof(nextProvider), "nextProvider Cannot be null");

            NextSearchProvider = nextProvider;
        }

        public Torrent GetTorrent(string search)
        {
            return NextSearchProvider.GetTorrent(search);
        }

        public ISearchProvider NextSearchProvider { get; }
    }
}
